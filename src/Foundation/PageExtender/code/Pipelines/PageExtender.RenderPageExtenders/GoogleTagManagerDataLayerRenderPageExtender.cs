using Foundation.PageExtender.Models;
using Foundation.PageExtender.Pipelines.RequestEnd;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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
            args.IsRendered = this.Render(args.Writer);
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

                                var renderingPath = renderingReference.RenderingItem.InnerItem.Paths.FullPath;
                                var dataSource = renderingReference.Settings.DataSource;

                                var model = new PersonalizedImpressionDataModel
                                {
                                    RenderingName = renderingReference.RenderingItem.DisplayName,
                                    RenderingPath = renderingPath.Substring(renderingPath.ToLower().LastIndexOf("renderings", StringComparison.Ordinal) + "renderings".Length),
                                    RenderingID = renderingReference.RenderingID.ToShortID().ToString(),
                                    DatasourcePath = dataSource.Substring(dataSource.ToLower().LastIndexOf("content", StringComparison.Ordinal) + "content".Length),
                                    DatasourceID = Context.Database.GetItem(dataSource)?.ID.ToShortID().ToString()
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

                                    if (appliedRule != null)
                                    {
                                        model.RuleName = appliedRule.Name;
                                        model.RenderingState = GetRenderingState(appliedRule);
                                    }
                                }

                                appliedPersonalizations.Add(model);
                            }
                        }
                    }
                }
            }

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
            var viewContext = new ViewContext(controllerContext, view, new ViewDataDictionary { Model = personalizationModel }, new TempDataDictionary(), writer);
            view.Render(viewContext, writer);

            return true;
        }

        private static string GetRenderingState(Rule<ConditionalRenderingsRuleContext> rule)
        {
            return rule?.Actions != null &&
                   rule.Actions.Any() &&
                   rule.Actions[0].GetType().Name.ToLower().Contains("hide")
                ? "hide"
                : "show";
        }
    }

}