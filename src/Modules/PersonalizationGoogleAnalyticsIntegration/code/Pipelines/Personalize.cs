using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.PersonalizationGoogleAnalyticsIntegration.Models;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Analytics.Rules.Conditions;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering;
using Sitecore.Personalization.Pipelines.GetRenderingRules;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using Sitecore.Rules.Actions;

namespace Sitecore.PersonalizationGoogleAnalyticsIntegration.Pipelines
{
    public class Personalize : Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering.Personalize
    {
        private readonly List<RuleAction<ConditionalRenderingsRuleContext>> _rulesApplied = new List<RuleAction<ConditionalRenderingsRuleContext>>();

        protected override void Evaluate(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var obj = args.PageContext.Item;
            if (obj == null || args.PageContext != null && args.PageContext.RequestContext.HttpContext.Request.QueryString["sc_haspersonalize"] == "0")
                return;
            var renderingReference = GetRenderingReference(args.Rendering, Context.Language, args.PageContext.Database);
            var args1 = new GetRenderingRulesArgs(obj, renderingReference);
            GetRenderingRulesPipeline.Run(args1);

            var ruleList = args1.RuleList;
            if (ruleList == null || ruleList.Count == 0)
                return;

            SubscribeToExecutedEvent(ruleList);

            var renderingsRuleContext =
                new ConditionalRenderingsRuleContext(new List<RenderingReference> { renderingReference }, renderingReference)
                {
                    Item = obj,
                    Parameters = { ["mvc.rendering"] = args.Rendering }
                };

            RunRules(ruleList, renderingsRuleContext);
            ApplyActions(args, renderingsRuleContext);
            args.IsCustomized = true;

            var appliedRule = ruleList.Rules.FirstOrDefault(rul => _rulesApplied.Any(r => new ID(r.UniqueId) == rul.UniqueId));
            if (appliedRule == null) return;

            var renderingPath = renderingReference.RenderingItem.InnerItem.Paths.FullPath;

            var model = new PersonalizedImpressionDataModel
            {
                UserName = Context.GetUserName(),
                RenderingName = args.Rendering.RenderingItem.Name,
                RenderingPath = renderingPath.Substring(renderingPath.ToLower().LastIndexOf("renderings", StringComparison.Ordinal) + "renderings".Length),
                RenderingID = args.Rendering.RenderingItem.ID.ToShortID().ToString(),
            };

            var dataSourcePath = args.Rendering.DataSource;

            dataSourcePath = GetDataSourcePath(dataSourcePath, appliedRule);

            model.DataSourcePath = dataSourcePath.Replace("/sitecore/content", string.Empty);
            model.DataSourceID = Context.Database.GetItem(dataSourcePath)?.ID.ToShortID().ToString();
            model.RuleName = appliedRule.Name;
            model.RenderingState = GetRenderingState(appliedRule);
            model.CardList = GetHasPatternConditionModelList(appliedRule.Condition);

            AddModelToSession("Rule_" + renderingReference.RenderingID.ToShortID(), model);
        }

        public void SubscribeToExecutedEvent(RuleList<ConditionalRenderingsRuleContext> ruleList)
        {
            ruleList.Applied += RuleList_Applied;
        }

        private void RuleList_Applied(RuleList<ConditionalRenderingsRuleContext> ruleList, ConditionalRenderingsRuleContext ruleContext, RuleAction<ConditionalRenderingsRuleContext> action)
        {
            _rulesApplied.Add(action);
        }

        private static void AddModelToSession(string key, PersonalizedImpressionDataModel model)
        {
            if (HttpContext.Current.Items["Personalization"] == null)
                HttpContext.Current.Items["Personalization"] = new Dictionary<string, string>();

            var dictionary = (Dictionary<string, string>)HttpContext.Current.Items["Personalization"] ?? new Dictionary<string, string>();

            if(!dictionary.ContainsKey(key))
                dictionary.Add(key, JsonConvert.SerializeObject(model));

            HttpContext.Current.Items["Personalization"] = dictionary;
        }

        private static string GetRenderingState(Rule<ConditionalRenderingsRuleContext> rule)
        {
            return rule?.Actions != null &&
                   rule.Actions.Any(act => act is HideRenderingAction<ConditionalRenderingsRuleContext>)
                ? "hide"
                : "show";
        }

        private static string GetDataSourcePath(string path, Rule<ConditionalRenderingsRuleContext> rule)
        {
            var dataSource = rule?.Actions?.OfType<SetDataSourceAction<ConditionalRenderingsRuleContext>>().FirstOrDefault()?.DataSource ?? path;
            return dataSource.Contains("local:") ? Context.Item.Paths.FullPath + dataSource.Replace("local:", "") : dataSource;
        }

        private static List<HasPatternConditionModel> GetHasPatternConditionModelList(RuleCondition<ConditionalRenderingsRuleContext> ruleCondition)
        {
            var list = new List<HasPatternConditionModel>();
            switch (ruleCondition)
            {
                case HasPatternCondition<ConditionalRenderingsRuleContext> condition:
                    list.Add(new HasPatternConditionModel
                    {
                        PatternName = condition.PatternName,
                        ProfileName = condition.ProfileName
                    });
                    break;

                case UnaryCondition<ConditionalRenderingsRuleContext> condition:
                    list.AddRange(GetHasPatternConditionModelList(condition.Operand));
                    break;

                case BinaryCondition<ConditionalRenderingsRuleContext> condition:
                    list.AddRange(GetHasPatternConditionModelList(condition.LeftOperand));
                    list.AddRange(GetHasPatternConditionModelList(condition.RightOperand));
                    break;
            }
            return list;
        }
    }
}
