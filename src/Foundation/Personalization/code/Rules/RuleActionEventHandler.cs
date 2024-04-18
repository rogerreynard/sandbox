using Sitecore.Rules;

namespace Foundation.Personalization.Rules
{
    /// <summary>The rule action event handler.</summary>
    /// <typeparam name="T">The type of rule context.</typeparam>
    /// <param name="ruleList">The rule set.</param>
    /// <param name="ruleContext">The rule context.</param>
    /// <param name="action">The current action.</param>
    public delegate void RuleActionEventHandler<T>(
        RuleList<T> ruleList,
        T ruleContext,
        RuleAction<T> action)
        where T : RuleContext;
}