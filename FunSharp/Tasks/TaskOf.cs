using System;
using System.Threading.Tasks;

namespace FunSharp.Tasks
{
    /// <summary>Utility class for creating completed tasks.</summary>
    public static class TaskOf
    {
        /// <summary>Creates a failed task with the given result type.</summary>
        /// <typeparam name="TResult">The result type of the created task.</typeparam>
        /// <param name="ex">The exception to fail the created task with.</param>
        /// <returns>A completed task which fails with the given exception.</returns>
        public static Task<TResult> Failed<TResult>(Exception ex)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        /// <summary>Creates a failed task.</summary>
        /// <param name="ex">The exception to fail the created task with.</param>
        /// <returns>A completed task failed with the given exception.</returns>
        public static Task Failed(Exception ex)
        {
            return Failed<object>(ex);
        }

        /// <summary>Creates a canceled task with the given result type.</summary>
        /// <typeparam name="TResult">The result type of the canceled task.</typeparam>
        /// <returns>A canceled task.</returns>
        public static Task<TResult> Canceled<TResult>()
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetCanceled();
            return tcs.Task;
        }

        /// <summary>Creates a canceled task.</summary>
        /// <returns>A new canceled task.</returns>
        public static Task Canceled()
        {
            return Canceled<object>();
        }
    }
}
