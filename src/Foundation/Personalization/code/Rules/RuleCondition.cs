using Sitecore.Data;
using Sitecore.Rules;

namespace Foundation.Personalization.Rules
{
    /// <summary>Defines the rule condition class.</summary>
    /// <typeparam name="T">The rule context.</typeparam>
    public abstract class RuleCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the unique identifier of the condition in the rule.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>Gets or sets the id of condition item.</summary>
        internal virtual ID ConditionItemId { get; set; }

        /// <summary>Executes the specified rule context.</summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <param name="stack">The stack.</param>
        public abstract void Evaluate(T ruleContext, RuleStack stack);

        /// <summary>Checks if the RuleCondition Can be Evaluated</summary>
        /// <param name="ruleContext">The rule context</param>
        /// <returns>
        /// A boolean value denoting if rule can be evaluated or not
        /// </returns>
        public virtual bool CanEvaluate(T ruleContext) => true;
    }
}