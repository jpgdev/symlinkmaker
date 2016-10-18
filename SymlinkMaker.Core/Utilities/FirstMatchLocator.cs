using System.Linq;

namespace SymlinkMaker.Core
{    
    public class FirstMatchLocator<EvaluatorConditionType, DependencyType> : PredicateLocator<EvaluatorConditionType, DependencyType>
    {       
        /// <summary>
        /// Get the dependency implementation from the evaluator value. The first that returns true from the value.
        /// </summary>
        /// <param name="evaluatorValue">The value to run against the evaluator.</param>
        public override DependencyType Get(EvaluatorConditionType evaluatorValue)
        {
            return Dependencies.First(x => x.Key(evaluatorValue)).Value;
        }
    }
}