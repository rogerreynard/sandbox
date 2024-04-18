using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;
using System.Collections.Generic;

namespace Foundation.Personalization.Rules
{
    /// <summary>Defines the rule class.</summary>
    /// <typeparam name="T">The rule context.</typeparam>
    public class Rule<T> : Sitecore.Rules.Rule<T> where T : RuleContext
    {
        /// <summary>The actions.</summary>
        private List<RuleAction<T>> _actions = new List<RuleAction<T>>();
        /// <summary>The conditions.</summary>
        private RuleCondition<T> _condition;

        /// <summary>Initializes a new instance of the Rule class.</summary>
        public Rule() => Name = "[unknown]";

        /// <summary>Initializes a new instance of the Rule class.</summary>
        /// <param name="ruleCondition">The condition.</param>
        public Rule(RuleCondition<T> ruleCondition)
          : this()
        {
            Assert.ArgumentNotNull(ruleCondition, nameof(ruleCondition));
            _condition = ruleCondition;
        }

        ///// <summary>Initializes a new instance of the Rule class.</summary>
        ///// <param name="ruleCondition">The condition.</param>
        ///// <param name="ruleAction">The action.</param>
        //public Rule(RuleCondition<T> ruleCondition, RuleAction<T> ruleAction)
        //  : this()
        //{
        //    Assert.ArgumentNotNull(ruleCondition, nameof(ruleCondition));
        //    Assert.ArgumentNotNull(ruleAction, nameof(ruleAction));
        //    _condition = ruleCondition;
        //    _actions.Add(ruleAction);
        //}

        ///// <summary>Initializes a new instance of the Rule class.</summary>
        ///// <param name="ruleCondition">The condition.</param>
        ///// <param name="actions">The actions.</param>
        //public Rule(RuleCondition<T> ruleCondition, List<RuleAction<T>> actions)
        //  : this()
        //{
        //    Assert.ArgumentNotNull(ruleCondition, nameof(ruleCondition));
        //    Assert.ArgumentNotNull(actions, nameof(actions));
        //    _condition = ruleCondition;
        //    _actions.AddRange(actions);
        //}

        ///// <summary>Gets or sets the actions.</summary>
        ///// <value>The actions.</value>
        public new List<RuleAction<T>> Actions
        {
            get => _actions;
            set
            {
                Assert.ArgumentNotNull(value, nameof(value));
                _actions = value;
            }
        }

        /// <summary>Gets or sets the rule condition.</summary>
        /// <value>The rule condition.</value>
        public new RuleCondition<T> Condition
        {
            get => _condition;
            set => _condition = value;
        }

        ///// <summary>Gets or sets the rule's unique id.</summary>
        ///// <value>The unique id.</value>
        //public ID UniqueId { get; set; }

        ///// <summary>Gets or sets the name of the rule</summary>
        //public string Name { get; set; }

        /// <summary>Gets or sets the id of rule item.</summary>
        internal ID RuleItemId { get; set; }

        ///// <summary>Evaluates this instance.</summary>
        ///// <param name="ruleContext">The rule context.</param>
        ///// <returns><c>true</c>, if the condition is true, otherwise <c>false</c>.</returns>
        //public bool Evaluate(T ruleContext)
        //{
        //    var condition = Condition;
        //    if (condition == null)
        //        return false;
        //    var stack = new RuleStack();
        //    try
        //    {
        //        condition.Evaluate(ruleContext, stack);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(
        //            $"Evaluation of condition failed. Rule item ID: {(RuleItemId != (ID)null ? RuleItemId.ToString() : (object)"Unknown")}, condition item ID: {(Condition.ConditionItemId != (ID)null ? Condition.ConditionItemId.ToString() : (object)"Unknown")}", ex, this);
        //    }
        //    return stack.Count != 0 && (bool)stack.Pop();
        //}

        ///// <summary>Executes the specified rule context.</summary>
        ///// <param name="ruleContext">The rule context.</param>
        //public void Execute(T ruleContext)
        //{
        //    foreach (var action in Actions)
        //    {
        //        action.Apply(ruleContext);
        //        if (ruleContext.IsAborted)
        //            break;
        //    }
        //}

        ///// <summary>Sets the condition.</summary>
        ///// <param name="ruleCondition">The condition.</param>
        //public void SetCondition(RuleCondition<T> ruleCondition)
        //{
        //    Assert.ArgumentNotNull(ruleCondition, nameof(ruleCondition));
        //    _condition = ruleCondition;
        //}
    }
}