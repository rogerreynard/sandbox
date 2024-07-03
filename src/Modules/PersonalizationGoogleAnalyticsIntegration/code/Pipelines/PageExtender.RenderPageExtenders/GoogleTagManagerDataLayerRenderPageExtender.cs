using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.PersonalizationGoogleAnalyticsIntegration.Models;
using Sitecore.PersonalizationGoogleAnalyticsIntegration.Pipelines.RequestEnd;
using Newtonsoft.Json;

namespace Sitecore.PersonalizationGoogleAnalyticsIntegration.Pipelines.PageExtender.RenderPageExtenders
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
            var models = (Dictionary<string, string>)HttpContext.Current.Items["Personalization"];

            if (models != null && models.Any())
            {
                RenderPersonalizationPartial(writer, models.Select(m => JsonConvert.DeserializeObject<PersonalizedImpressionDataModel>(m.Value)).ToList());
            }

            HttpContext.Current.Items["Personalization"] = new Dictionary<string, string>();

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
    }
}