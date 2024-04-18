using Sitecore.Data;
using Sitecore.Rules;

namespace Foundation.Personalization.Rules
{
    /// <summary>Defines the rule action class.</summary>
    /// <typeparam name="T">The rule context type.</typeparam>
    public abstract class RuleAction<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the unique identifier of the condition in the rule.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>Gets or sets the id of action item.</summary>
        internal virtual ID ActionItemId { get; set; }

        /// <summary>Executes the specified rule context.</summary>
        /// <param name="ruleContext">The rule context.</param>
        public abstract void Apply(T ruleContext);
    }
}