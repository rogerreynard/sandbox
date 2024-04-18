using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Personalization.Mvc.Presentation;
using Sitecore.Personalization.Pipelines.RenderingRuleEvaluated;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using System.Collections.Generic;
using System.Linq;
using Foundation.Personalization.Models;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering;
using Sitecore.Personalization.Pipelines.GetRenderingRules;
using System.Xml.Linq;
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using Foundation.Personalization.Helpers;
using Newtonsoft.Json;
//using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Personalization.Shell.Applications.PageModes.Personalization;
using Sitecore.Rules.Actions;
using Sitecore.Web;

namespace Foundation.Personalization.Pipelines
{
    public class Personalize : CustomizeRenderingProcessor
    {
        public override void Process(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            if (args.IsCustomized)
                return;
            Evaluate(args);
        }

        protected virtual void ApplyActions(
          CustomizeRenderingArgs args,
          ConditionalRenderingsRuleContext context)  
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(context, nameof(context));
            var reference = context.References.Find(r => r.UniqueId == context.Reference.UniqueId);
            if (reference == null)
                args.Renderer = new EmptyRenderer();
            else
                ApplyChanges(args.Rendering, reference);
        }

        protected void Evaluate(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var obj = args.PageContext.Item;
            if (obj == null || args.PageContext?.RequestContext.HttpContext != null && args.PageContext.RequestContext.HttpContext.Request.QueryString["sc_haspersonalize"] == "0")
                return;
            var renderingReference = GetRenderingReference(args.Rendering, Context.Language, args.PageContext?.Database);
            var args1 = new GetRenderingRulesArgs(obj, renderingReference);
            GetRenderingRulesPipeline.Run(args1);
            var ruleList = args1.RuleList;
            if (ruleList == null || ruleList.Count == 0)
                return;

            var renderingsRuleContext = new ConditionalRenderingsRuleContext(new List<RenderingReference>()
            {
                renderingReference
            }, renderingReference)
            {
                Item = obj,
                Parameters =
                {
                    ["mvc.rendering"] = args.Rendering
                }
            };

            RunRules(ruleList, renderingsRuleContext);
            ApplyActions(args, renderingsRuleContext);

            Rule<ConditionalRenderingsRuleContext> rule1 = (Rule<ConditionalRenderingsRuleContext>)null;
            if (!string.IsNullOrEmpty(args1.RenderingReference.UniqueId))
            {
                ID activeRuleId = this.GetRuleId(ID.Parse(args1.RenderingReference.UniqueId), args1.RenderingReference, args1.Item);
                if (!ID.IsNullOrEmpty(activeRuleId))
                    rule1 = args1.RuleList.Rules.FirstOrDefault<Rule<ConditionalRenderingsRuleContext>>((Func<Rule<ConditionalRenderingsRuleContext>, bool>)(r => r.UniqueId == activeRuleId));
            }
            if (rule1 == null)
                rule1 = args1.RuleList.Rules.FirstOrDefault<Rule<ConditionalRenderingsRuleContext>>((Func<Rule<ConditionalRenderingsRuleContext>, bool>)(r => r.UniqueId == new ID("{00000000-0000-0000-0000-000000000000}")));
            if (rule1 == null)
                return;

            var personalizedComponentViewModelList = new List<PersonalizedImpressionDataModel>();

            var order = 1;
            foreach (var rule in ruleList.Rules)
            {
                var state = rule.Actions != null &&
                                  rule.Actions.Any() &&
                                  rule.Actions[0].GetType().Name.ToLower().Contains("hide")
                                    ? "hide"
                                    : "show";

                var personalizedComponentViewModel = new PersonalizedImpressionDataModel
                {
                    ComponentName = args.Rendering.RenderingItem.Name,
                    ConditionName = rule.Name,
                    ConditionOrder = order.ToString(),
                    ConditionState = state,
                    PersonalizedRendering = renderingReference.RenderingItem.InnerItem.Paths.FullPath,
                    PersonalizedContent = renderingReference.Settings.DataSource,
                    PersonalizedContentId = args.Rendering.RenderingItem.ID.ToString()
                };

                order++;
                
                personalizedComponentViewModelList.Add(personalizedComponentViewModel);
            }

            RenderPersonalizationScript(personalizedComponentViewModelList);

            args.IsCustomized = true;
        }

        private static void RenderPersonalizationScript(List<PersonalizedImpressionDataModel> models)
        {
            var keySuffix = models.FirstOrDefault() != null ? models.FirstOrDefault()?.ComponentName + models.FirstOrDefault()?.PersonalizedContentId : "";

            var script = HttpUtility.HtmlEncode(GeneratePersonalizationScript(models));

            InlineScriptHelper.AddScript("Personalization_" + keySuffix, script);
        }

        private static string GeneratePersonalizationScript(List<PersonalizedImpressionDataModel> models)
        {
            var sb = new StringBuilder();
            //sb.AppendLine("<script>");
            //sb.AppendLine("window.dataLayer = window.dataLayer || [];");

            foreach (var model in models)
            {
                var wrapper = new PersonalizedComponentImpressionModel
                {
                    PersonalizedImpressionData = new List<PersonalizedImpressionDataModel> { model }
                };
                sb.AppendLine($"window.dataLayer.push({JsonConvert.SerializeObject(wrapper)});");
            }
            //sb.AppendLine("</script>");

            return sb.ToString();
        }

        protected virtual void RunRules(
          RuleList<ConditionalRenderingsRuleContext> rules,
          ConditionalRenderingsRuleContext context)
        {
            Assert.ArgumentNotNull(rules, nameof(rules));
            Assert.ArgumentNotNull(context, nameof(context));
            if (!RenderingRuleEvaluatedPipeline.IsEmpty())
                rules.Evaluated += RulesEvaluatedHandler;
            rules.RunFirstMatching(context);
        }

        private static void RulesEvaluatedHandler(
          RuleList<ConditionalRenderingsRuleContext> ruleList,
          ConditionalRenderingsRuleContext ruleContext,
          Rule<ConditionalRenderingsRuleContext> rule)
        {
            RenderingRuleEvaluatedPipeline.Run(new RenderingRuleEvaluatedArgs(ruleList, ruleContext, rule));
        }


        protected virtual ID GetRuleId(ID renderingUId, RenderingReference rendering, Item host) => WebEditUtil.GetPersistedRuleId(renderingUId, host);
    }
}
