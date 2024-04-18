using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Collections.Generic;
using Sitecore.Configuration;

namespace Foundation.Personalization.Rules
{
    public class RuleList<T> : Sitecore.Rules.RuleList<T> where T : RuleContext
    {
        private readonly List<Rule<T>> _rules = new List<Rule<T>>();

        /// <summary>Occurs when an action has been applied.</summary>
        public new event RuleActionEventHandler<T> Applied;

        /// <summary>Occurs when evaluated.</summary>
        public new event RuleConditionEventHandler<T> Evaluated;

        /// <summary>Occurs when evaluating.</summary>
        public new event RuleConditionEventHandler<T> Evaluating;

        /// <summary>Occurs when rule has been successfully executed.</summary>
        public new event RuleExecutedEventHandler<T> Executed;

        /// <summary>Gets the rules.</summary>
        /// <value>The rules.</value>
        public new IEnumerable<Rule<T>> Rules => _rules;

        /// <summary>Executes this rule set.</summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <param name="stopOnFirstMatching">if set to <c>true</c> the rules execution stops when any condition evaluates to true.</param>
        /// <param name="executedRulesCount">The number of rules, whose actions has been executed.</param>
        protected new void Run(T ruleContext, bool stopOnFirstMatching, out int executedRulesCount)
        {
            Assert.ArgumentNotNull((object)ruleContext, nameof(ruleContext));
            executedRulesCount = 0;
            if (this.Count == 0)
                return;
            using (new LongRunningOperationWatcher(Settings.Profiling.RenderFieldThreshold, "Long running rule set: {0}", new string[1]
            {
                this.Name ?? string.Empty
            }))
            {
                foreach (Rule<T> rule in this.Rules)
                {
                    if (rule.Condition == null || !rule.Condition.CanEvaluate(ruleContext))
                    {
                        Log.Debug(
                            $"Evaluation of rule skipped. Rule item ID: {(rule.RuleItemId != (ID)null ? (object)rule.RuleItemId.ToString() : (object)"Unknown")}", (object)this);
                    }
                    else
                    {
                        RuleStack stack = new RuleStack();
                        try
                        {
                            RuleList<T>.RaiseConditionEventHandler(this.Evaluating, this, ruleContext, rule);
                            rule.Condition.Evaluate(ruleContext, stack);
                            RuleList<T>.RaiseConditionEventHandler(this.Evaluated, this, ruleContext, rule);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("Evaluation of condition failed. Rule item ID: {0}, condition item ID: {1}", rule.RuleItemId != (ID)null ? (object)rule.RuleItemId.ToString() : (object)"Unknown", rule.Condition.ConditionItemId != (ID)null ? (object)rule.Condition.ConditionItemId.ToString() : (object)"Unknown"), ex, (object)this);
                            ruleContext.Abort();
                        }
                        if (ruleContext.IsAborted)
                            break;
                        if (stack.Count != 0)
                        {
                            if (!(bool)stack.Pop() || ruleContext.SkipRule)
                            {
                                ruleContext.SkipRule = false;
                            }
                            else
                            {
                                foreach (RuleAction<T> action in rule.Actions)
                                {
                                    try
                                    {
                                        action.Apply(ruleContext);
                                        RuleList<T>.RaiseActionEventHandler(this.Applied, this, ruleContext, action);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(string.Format("Execution of action failed. Rule item ID: {0}, action item ID: {1}", rule.RuleItemId != (ID)null ? (object)rule.RuleItemId.ToString() : (object)"Unknown", action.ActionItemId != (ID)null ? (object)action.ActionItemId.ToString() : (object)"Unknown"), ex, (object)this);
                                        ruleContext.Abort();
                                    }
                                    if (ruleContext.IsAborted)
                                        return;
                                }
                                ++executedRulesCount;
                                RaiseExecutedEventHandler(Executed, this, ruleContext, rule);
                                if (stopOnFirstMatching)
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Raises the event handler.</summary>
        /// <param name="eventHandler">The evaluating.</param>
        /// <param name="rules">The rules.</param>
        /// <param name="context">The context.</param>
        /// <param name="action">The current rule.</param>
        private static void RaiseActionEventHandler(
          RuleActionEventHandler<T> eventHandler,
          RuleList<T> rules,
          T context,
          RuleAction<T> action)
        {
            Assert.ArgumentNotNull((object)rules, nameof(rules));
            Assert.ArgumentNotNull((object)context, nameof(context));
            Assert.ArgumentNotNull((object)action, nameof(action));
            if (eventHandler == null)
                return;
            eventHandler(rules, context, action);
        }

        /// <summary>Raises the event handler.</summary>
        /// <param name="eventHandler">The evaluating.</param>
        /// <param name="rules">The rules.</param>
        /// <param name="context">The context.</param>
        /// <param name="rule">The current rule.</param>
        private static void RaiseConditionEventHandler(
          RuleConditionEventHandler<T> eventHandler,
          RuleList<T> rules,
          T context,
          Rule<T> rule)
        {
            Assert.ArgumentNotNull((object)rules, nameof(rules));
            Assert.ArgumentNotNull((object)context, nameof(context));
            Assert.ArgumentNotNull((object)rule, nameof(rule));
            if (eventHandler == null)
                return;
            eventHandler(rules, context, rule);
        }

        /// <summary>Raises the event handler.</summary>
        /// <param name="eventHandler">The event handler to execute.</param>
        /// <param name="rules">The rules.</param>
        /// <param name="context">The context.</param>
        /// <param name="rule">The current rule.</param>
        private static void RaiseExecutedEventHandler(
          RuleExecutedEventHandler<T> eventHandler,
          RuleList<T> rules,
          T context,
          Rule<T> rule)
        {
            Assert.ArgumentNotNull((object)rules, nameof(rules));
            Assert.ArgumentNotNull((object)context, nameof(context));
            Assert.ArgumentNotNull((object)rule, nameof(rule));
            if (eventHandler == null)
                return;
            eventHandler(rules, context, rule);
        }
    }
}