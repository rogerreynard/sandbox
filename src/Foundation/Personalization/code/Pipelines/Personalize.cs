using System;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.ConditionalRenderings;
using System.Collections.Generic;
using System.Linq;
using Foundation.Personalization.Models;
using Sitecore;
using Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering;
using Sitecore.Personalization.Pipelines.GetRenderingRules;
using Foundation.Personalization.Helpers;
using Newtonsoft.Json;
using Sitecore.Rules;

namespace Foundation.Personalization.Pipelines
{
    public class Personalize : Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering.Personalize
    {
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
            var renderingsRuleContext =
                new ConditionalRenderingsRuleContext(new List<RenderingReference> { renderingReference }, renderingReference)
                {
                    Item = obj,
                    Parameters = { ["mvc.rendering"] = args.Rendering }
                };

            RunRules(ruleList, renderingsRuleContext);
            ApplyActions(args, renderingsRuleContext);
            args.IsCustomized = true;

            var renderingPath = renderingReference.RenderingItem.InnerItem.Paths.FullPath;
            var dataSource = renderingReference.Settings.DataSource;

            var personalizedComponentViewModel = new PersonalizedImpressionDataModel
            {
                ComponentName = args.Rendering.RenderingItem.Name,
                RenderingPath = renderingPath.Substring(renderingPath.ToLower().LastIndexOf("renderings", StringComparison.Ordinal) + "renderings".Length),
                RenderingID = args.Rendering.RenderingItem.ID.ToShortID().ToString(),
                DatasourcePath = dataSource.Substring(dataSource.ToLower().LastIndexOf("content", StringComparison.Ordinal) + "content".Length),
                DatasourceID = args.Rendering.Item.ID.ToShortID().ToString()
            };

            var rulesList = new List<RuleDataModel>();
            var order = 1;
            foreach (var rule in ruleList.Rules)
            {
                var ruleDataModel = new RuleDataModel
                {
                    RuleName = rule.Name,
                    RuleOrder = order.ToString(),
                    ActionState = GetActionState(rule),
                };

                order++;

                rulesList.Add(ruleDataModel);
            }

            personalizedComponentViewModel.Rules = rulesList;

            StringifyModel(personalizedComponentViewModel);
        }

        private static void StringifyModel(PersonalizedImpressionDataModel model)
        {
            var keySuffix = model.ComponentName + model.DatasourceID;

            var script = JsonConvert.SerializeObject(model);

            InlineScriptHelper.AddScript("Personalization_" + keySuffix, script);

            Log.Info($"Personalization: {model.ComponentName} - {model.RenderingPath} - {model.DatasourcePath} - {model.DatasourceID}", typeof(Personalize));
        }

        private static string GetActionState(Rule<ConditionalRenderingsRuleContext> rule)
        {
            return rule.Actions != null &&
                   rule.Actions.Any() &&
                   rule.Actions[0].GetType().Name.ToLower().Contains("hide")
                ? "hide"
                : "show";
        }
    }
}
