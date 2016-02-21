using System;
using System.Diagnostics.Contracts;

namespace FunSharp
{
    /// <summary>
    /// Implementation of the State monad. A <see cref="State{TState, TResult}"/> is a function from an 
    /// initial to a pair containing a result and a new state value.
    /// </summary>
    /// <typeparam name="TState">The type of state.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public class State<TState, TResult>
    {
        private readonly Func<TState, StateResult<TState, TResult>> f;

        /// <summary>Creates a new instance from a delegate.</summary>
        /// <param name="f">The delegate containing the stateful computation.</param>
        public State(Func<TState, StateResult<TState, TResult>> f)
        {
            this.f = f;
        }

        /// <summary>Runs this stateful computation.</summary>
        /// <param name="state">The initial state.</param>
        /// <returns>The result of this computation.</returns>
        public StateResult<TState, TResult> Run(TState state)
        {
            return this.f(state);
        }

        /// <summary>Runs this stateful computation and returns the result, discarding the final state.</summary>
        /// <param name="state">The initial state.</param>
        /// <returns>The result of this computation.</returns>
        public TResult RunResult(TState state)
        {
            return this.f(state).Result;
        }

        /// <summary>Runs this stateful computation and returns the final state, discarding the result.</summary>
        /// <param name="state">The initial state.</param>
        /// <returns>The final state.</returns>
        public TState RunState(TState state)
        {
            return this.f(state).State;
        }

        /// <summaryTransforms the result of this computation with the given transform function.</summary>
        /// <typeparam name="TOut">The transformed result type.</typeparam>
        /// <param name="mapFunc">Transform function for the result of this computation.</param>
        /// <returns>A stateful computation which transforms the result of this computation with <paramref name="mapFunc"/>.</returns>
        public State<TState, TOut> Select<TOut>(Func<TResult, TOut> mapFunc)
        {
            Contract.Requires(mapFunc != null);

            return new State<TState, TOut>(s =>
            {
                var thisResult = this.f(s);
                return new StateResult<TState, TOut>(s, mapFunc(thisResult.Result));
            });
        }

        /// <summary>Transforms both the state and result of this computation with the given transform function.</summary>
        /// <typeparam name="TOut">The updated result type.</typeparam>
        /// <param name="mapFunc">Function to transform the result of this computation.</param>
        /// <returns>A stateful computation which transforms the output of this computation.</returns>
        public State<TState, TOut> BiSelect<TOut>(Func<StateResult<TState, TResult>, StateResult<TState, TOut>> mapFunc)
        {
            return new State<TState, TOut>(s =>
            {
                return mapFunc(this.f(s));
            });
        }

        /// <summary>Combines this stateful computation with another obtained from the result of this.</summary>
        /// <typeparam name="TOut">Result type from <paramref name="selector"/></typeparam>
        /// <param name="bindFunc">Function to construct a new stateful computation from the result of this one.</param>
        /// <returns>
        /// A stateful computation which runs this computation and the one returned from <paramref name="bindFunc"/>.
        /// </returns>
        public State<TState, TOut> SelectMany<TOut>(Func<TResult, State<TState, TOut>> bindFunc)
        {
            return SelectMany(bindFunc, (_, r) => r);
        }

        /// <summary>Combines this stateful computation with another obtained from the result of this.</summary>
        /// <typeparam name="TInter">Result type for the </typeparam>
        /// <typeparam name="TOut">Result type from <paramref name="selector"/></typeparam>
        /// <param name="bindFunc">Function to construct a new stateful computation from the result of this one.</param>
        /// <param name="selector">Function to combine the results of this computation and the one returned from <paramref name="bindFunc"/>.</param>
        /// <returns>
        /// A stateful computation which runs this computation and the one returned from <paramref name="bindFunc"/> 
        /// before combining the results with <paramref name="selector"/>.
        /// </returns>
        public State<TState, TOut> SelectMany<TInter, TOut>(Func<TResult, State<TState, TInter>> bindFunc, Func<TResult, TInter, TOut> selector)
        {
            return new State<TState, TOut>(initialState =>
            {
                var thisResult = this.f(initialState);
                var nextState = bindFunc(thisResult.Result);
                var nextResult = nextState.Run(thisResult.State);
                var result = selector(thisResult.Result, nextResult.Result);
                return new StateResult<TState, TOut>(nextResult.State, result);
            });
        }
    }

    public static class State
    {
        /// <summary>Creates a stateful computation which ignores the current state and returns the given result.</summary>
        /// <typeparam name="TState">The state type for the returned computation.</typeparam>
        /// <typeparam name="TResult">The result type for the computation.</typeparam>
        /// <param name="result">The result to return from the created computation..</param>
        /// <returns>A stateful computation which ignores the state and returns <paramref name="result"/> as its result.</returns>
        public static State<TState, TResult> FromResult<TState, TResult>(TResult result)
        {
            return new State<TState, TResult>(s => new StateResult<TState, TResult>(s, result));
        }

        /// <summary>Returns a computation which returns the current state as its result.</summary>
        /// <typeparam name="TState">The state type of the computation.</typeparam>
        /// <returns>A computation which returns the current state as its result.</returns>
        public static State<TState, TState> Get<TState>()
        {
            return new State<TState, TState>(s => new StateResult<TState, TState>(s, s));
        }

        /// <summary>Returns a computation which updates the current state.</summary>
        /// <typeparam name="TState">The state type for the computation.</typeparam>
        /// <param name="state">The new value for the state.</param>
        /// <returns>A computation which updates the current state and returns an uninteresting value.</returns>
        public static State<TState, Unit> Put<TState>(TState state)
        {
            return new State<TState, Unit>(_ => new StateResult<TState, Unit>(state, Unit.Instance));
        }

        /// <summary>Returns a stateful computation which modifies the state with the given transform function.</summary>
        /// <typeparam name="TState">The state type of the computation.</typeparam>
        /// <param name="modifyFunc">The transform function for the state.</param>
        /// <returns>A stateful computation which transforms the current state withe <paramref name="modifyFunc"/> and sets the result as the new state.</returns>
        public static State<TState, Unit> Modify<TState>(Func<TState, TState> modifyFunc)
        {
            return from s in Get<TState>()
                   from _ in Put(modifyFunc(s))
                   select Unit.Instance;
        }

        /// <summary>Lifts function application over state.</summary>
        /// <typeparam name="TState">The state type for the computation.</typeparam>
        /// <typeparam name="TInter">The parameter type of the function result.</typeparam>
        /// <typeparam name="TResult">The result type of the returned computation.</typeparam>
        /// <param name="fState">A stateful computation with a function result.</param>
        /// <param name="valState">A stateful computation </param>
        /// <returns>
        /// A stateful computation which first runs <paramref name="fState"/> followed by <paramref name="valState"/> 
        /// with the updated state, before applying the resulting function with the value.
        /// </returns>
        public static State<TState, TResult> Ap<TState, TInter, TResult>(this State<TState, Func<TInter, TResult>> fState, State<TState, TInter> valState)
        {
            return from f in fState
                   from v in valState
                   select f(v);
        }

        /// <summary>Flattens a nested stateful computation.</summary>
        /// <typeparam name="TState">The state type of the computations.</typeparam>
        /// <typeparam name="TResult">The result type of the inner computation.</typeparam>
        /// <param name="nested">A stateful computation which returns as its result another stateful computation.</param>
        /// <returns>A stateful computation which runs the outer computation followed by the inner one, threading the state between them.</returns>
        public static State<TState, TResult> Join<TState, TResult>(this State<TState, State<TState, TResult>> nested)
        {
            return from s in nested
                   from r in s
                   select r;
        }
    }

    /// <summary>Pair containing the output result and state of a stateful computation.</summary>
    /// <typeparam name="TState">The type of state.</typeparam>
    /// <typeparam name="TResult">The type of result.</typeparam>
    public struct StateResult<TState, TResult>
    {
        /// <summary>Creates a new instance.</summary>
        /// <param name="state">The output state.</param>
        /// <param name="result">The result.</param>
        public StateResult(TState state, TResult result)
            : this()
        {
            this.State = state;
            this.Result = result;
        }

        /// <summary>Gets the state.</summary>
        public TState State { get; private set; }

        /// <summary>Gets the result.</summary>
        public TResult Result { get; private set; }
    }
}
