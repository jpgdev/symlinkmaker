using System;
using System.Collections.Generic;

namespace SymlinkMaker.Core
{
    public abstract class PredicateLocator<TEvaluatorCondition, TValue> : ILocator<TEvaluatorCondition, TValue>
    {
        private readonly Dictionary<Func<TEvaluatorCondition, bool>, TValue> _dependencies = 
            new Dictionary<Func<TEvaluatorCondition, bool>, TValue>();

        /// <summary>
        /// Gets the dependencies from an evaluator function.
        /// </summary>
        /// <value>The dependencies.</value>
        protected Dictionary<Func<TEvaluatorCondition, bool>, TValue> Dependencies
        {
            get  { return _dependencies; }
        }

        /// <summary>
        /// Register the specified evaluator to an implemented dependency.
        /// </summary>
        /// <param name="evaluator">Evaluator function to get the right implementation.</param>
        /// <param name="dependencyImplementation">Dependency implementation.</param>
        public virtual void Register(Func<TEvaluatorCondition, bool> evaluator, TValue dependencyImplementation)
        {
            _dependencies.Add(evaluator, dependencyImplementation);
        }

        /// <summary>
        /// Get the dependency implementation from the evaluator value.
        /// </summary>
        /// <param name="evaluatorValue">The value to run against the evaluator.</param>
        public abstract TValue Get (TEvaluatorCondition evaluatorValue);
    }
}
