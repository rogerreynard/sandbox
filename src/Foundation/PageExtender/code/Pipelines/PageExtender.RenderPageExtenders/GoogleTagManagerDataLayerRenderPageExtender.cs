using Foundation.PageExtender.Models;
using Foundation.PageExtender.Pipelines.RequestEnd;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Analytics.Rules.Conditions;
using Sitecore.Rules.Conditions;

namespace Foundation.PageExtender.Pipelines.PageExtender.RenderPageExtenders
{
    public class GoogleTagManagerDataLayerRenderPageExtender : BasePageExtenderProcessor
    {
        public override void DoProcess(RenderPageExtendersArgs args)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
            if (args.IsRendered)
            {
                return;
            }
            args.IsRendered = Render(args.Writer);
        }
        private class PersonalizationController : Controller
        {
        }

        private bool Render(TextWriter writer)
        {
            var appliedPersonalizations = new List<PersonalizedImpressionDataModel>();
            IList<PersonalizationRuleData> exposedRules = null;

            if (Tracker.Current != null && Tracker.Enabled && Tracker.Current.CurrentPage != null && Tracker.Current.CurrentPage.Personalization != null)
            {
                var personalization = Tracker.Current.CurrentPage.Personalization;
                exposedRules = personalization.ExposedRules;
            }

            if (exposedRules != null)
            {
                if (exposedRules.Any())
                {
                    if (Context.Device != null)
                    {
                        var renderingReferences = Context.Item.Visualization.GetRenderings(Context.Device, true);
                        if (renderingReferences != null)
                        {
                            foreach (var renderingReference in renderingReferences)
                            {
                                if (renderingReference.Settings.Rules?.Rules == null) continue;

                                var renderingReferenceRules = renderingReference.Settings.Rules.Rules.ToList();

                                if (!renderingReferenceRules.Any()) continue;

                                var model = new PersonalizedImpressionDataModel
                                {
                                    UserName = Context.GetUserName(),
                                    RenderingName = renderingReference.RenderingItem?.DisplayName ?? string.Empty,
                                    RenderingPath = renderingReference.RenderingItem?.InnerItem?.Paths.FullPath.Replace("/sitecore/layout/Renderings", string.Empty) ?? string.Empty,
                                    RenderingID = renderingReference.RenderingID.ToShortID().ToString()
                                };

                                if (renderingReferenceRules.Any())
                                {
                                    var appliedRule =
                                    (
                                        from rules in renderingReferenceRules
                                        join exp in exposedRules
                                            on rules.UniqueId equals exp.RuleId
                                        select rules
                                    ).FirstOrDefault();

                                    var dataSourcePath = GetDataSourcePath(appliedRule);

                                    if (appliedRule != null)
                                    {
                                        model.DataSourcePath = dataSourcePath.Replace("/sitecore/content", string.Empty);
                                        model.DataSourceID = Context.Database.GetItem(dataSourcePath)?.ID.ToShortID().ToString();
                                        model.RuleName = appliedRule.Name;
                                        model.RenderingState = GetRenderingState(appliedRule);
                                        model.CardList = GetHasPatternConditionModelList(appliedRule.Condition);
                                    }
                                }

                                appliedPersonalizations.Add(model);
                            }
                        }
                    }
                }
            }

            RenderPersonalizationPartial(writer, appliedPersonalizations);

            return true;
        }

        private void RenderPersonalizationPartial(TextWriter writer, List<PersonalizedImpressionDataModel> appliedPersonalizations)
        {
            // via view - more maintainable
            var partialName = RazorView;
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var controllerContext = new ControllerContext(new RequestContext(httpContext, new RouteData
            {
                Values =
                {
                    {
                        "Controller",
                        "PersonalizationController"
                    }
                }
            }), new PersonalizationController());

            var personalizationModel = new PersonalizedComponentImpressionModel
            {
                PersonalizedImpressionData = appliedPersonalizations
            };

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialName).View;
            var viewContext = new ViewContext(controllerContext, view, new ViewDataDictionary { Model = personalizationModel },
                new TempDataDictionary(), writer);
            view.Render(viewContext, writer);
        }

        private static string GetRenderingState(Rule<ConditionalRenderingsRuleContext> rule)
        {
            return rule?.Actions != null &&
                   rule.Actions.Any(act => act is HideRenderingAction<ConditionalRenderingsRuleContext>)
                ? "hide"
                : "show";
        }

        private static string GetDataSourcePath(Rule<ConditionalRenderingsRuleContext> rule)
        {
            var dataSource = rule?.Actions?.OfType<SetDataSourceAction<ConditionalRenderingsRuleContext>>().FirstOrDefault()?.DataSource ?? string.Empty;
            return dataSource.Contains("local:") ? Context.Item.Paths.FullPath + dataSource.Replace("local:", ""): dataSource;
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