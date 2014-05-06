using System;
using System.Threading.Tasks;

namespace FunSharp.Tasks
{
    /// <summary>Extension methods for tasks.</summary>
    public static class TaskExtensions
    {
        /// <summary>Maps the result of the given task with the given delegate if it has a result.</summary>
        /// <typeparam name="T">Result type of the task.</typeparam>
        /// <typeparam name="TResult">Result type of the map function.</typeparam>
        /// <param name="task">The task to map.</param>
        /// <param name="selector">Delegate to map the result of <paramref name="task"/> if one exists.</param>
        /// <returns>A task containing the transformed result of <paramref name="task"/>.</returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> selector)
        {
            T result = await task;
            return selector(result);
        }

        /// <summary>Monadic bind function for tasks.</summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <typeparam name="U">Result type of the secondary task.</typeparam>
        /// <param name="task">The first task.</param>
        /// <param name="bindFunc">Function to create the successor task from the result of the first.</param>
        /// <returns>The task returned from <paramref name="bindFunc"/> if <paramref name="task"/> succeeded.</returns>
        public static Task<U> SelectMany<T, U>(this Task<T> task, Func<T, Task<U>> bindFunc)
        {
            return SelectMany<T, U, U>(task, bindFunc, (_, r) => r);
        }

        /// <summary>Monadic bind function with a selector for the intermediate and output results.</summary>
        /// <typeparam name="T">Result type of the input task.</typeparam>
        /// <typeparam name="U">Result type of the created task.</typeparam>
        /// <typeparam name="R">Result type of the selector function.</typeparam>
        /// <param name="task">The first task.</param>
        /// <param name="bindFunc">Function to create the subsequent task after the result of <paramref name="task"/> is available.</param>
        /// <param name="selector">Selector function to combine the results from <paramref name="task"/> and the result returned from <paramref name="bindFunc"/>.</param>
        /// <returns>Returns a task containing the results of the two tasks combined with the given selector function.</returns>
        public static async Task<R> SelectMany<T, U, R>(this Task<T> task, Func<T, Task<U>> bindFunc, Func<T, U, R> selector)
        {
            T firstResult = await task;
            U secondResult = await bindFunc(firstResult);
            return selector(firstResult, secondResult);
        }

        /// <summary>Filters the result of a task with a given predicate.</summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The task to filter.</param>
        /// <param name="predicate">Predicate to filter the result of <paramref name="task"/> if it exists.</param>
        /// <returns>A task containing the result from <paramref name="task"/> if it exists and satisfies <paramref name="predicate"/>.</returns>
        public static async Task<T> Where<T>(this Task<T> task, Func<T, bool> predicate)
        {
            T result = await task;
            if (predicate(result)) return result;
            else throw new NoSuchElementException();
        }

        /// <summary>Converts the given task into one which succeeds with a <see cref="ITry{T}"/> result.</summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The task to convert the result for.</param>
        /// <returns>
        /// A task which succeeds with a Try result. If <paramref name="task"/> succeeds this Try is a Success
        /// containing the result. If the task fails or is canceled, the Try is a Failure containing the exception.
        /// </returns>
        public static async Task<ITry<T>> TryResult<T>(this Task<T> task)
        {
            try
            {
                T result = await task;
                return Try.Success(result);
            }
            catch (Exception ex)
            {
                return Try.Failed<T>(ex);
            }
        }

        /// <summary>Applies the given partial recovery function if a task fails.</summary>
        /// <typeparam name="T">Result type of the task.</typeparam>
        /// <param name="task">The task to try recover.</param>
        /// <param name="recoverFunc">
        /// Function to try recover if <paramref name="task"/> failed. If the recovery is successful this delegate should
        /// return Some containing the recovery result, otherwise None.
        /// </param>
        /// <returns>
        /// The result of <paramref name="task"/> if it was successful. Otherwise the result from <paramref name="recoverFunc"/> if 
        /// it returns Some, or a failed task containing the exception from <paramref name="task"/> if the recovery was unsuccessful.
        /// </returns>
        public static async Task<T> Recover<T>(this Task<T> task, Func<Exception, Maybe<T>> recoverFunc)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                var maybeResult = recoverFunc(ex);
                return maybeResult.GetOr(() => { throw ex; });
            }
        }

        /// <summary>Lifts a void task into one which returns <see cref="Unit"/>.</summary>
        /// <param name="t">The task to lift.</param>
        /// <returns>A task which represents the computation for <paramref name="t"/> and returns the <see cref="Unit"/> value.</returns>
        public static async Task<Unit> Lift(this Task t)
        {
            await t;
            return Unit.Instance;
        }
    }
}
