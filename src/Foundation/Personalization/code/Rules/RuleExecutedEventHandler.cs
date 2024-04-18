using Sitecore.Rules;

namespace Foundation.Personalization.Rules
{
    /// <summary>The rule executed event handler.</summary>
    /// <typeparam name="T">The type of rule context.</typeparam>
    /// <param name="ruleList">The rule set.</param>
    /// <param name="ruleContext">The rule context.</param>
    /// <param name="rule">The current rule.</param>
    public delegate void RuleExecutedEventHandler<T>(
        RuleList<T> ruleList,
        T ruleContext,
        Rule<T> rule)
        where T : RuleContext;
}