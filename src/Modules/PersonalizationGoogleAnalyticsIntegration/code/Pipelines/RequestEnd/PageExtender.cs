using Sitecore.PersonalizationGoogleAnalyticsIntegration.ResponseFilter;
using Sitecore;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Request.RequestEnd;

namespace Sitecore.PersonalizationGoogleAnalyticsIntegration.Pipelines.RequestEnd
{
    public class PageExtender : RequestEndProcessor
    {
        public override void Process(RequestEndArgs args)
        {
            if (Context.Site == null) return;

            var pageContext = args.PageContext;

            if (pageContext?.Item == null) return;

            if (!Context.PageMode.IsNormal || Context.PageMode.IsExperienceEditor || Context.PageMode.IsExperienceEditorEditing ||
                Context.PageMode.IsPreview || Context.PageMode.IsProfiling)
                return;

            var requestContext = pageContext.RequestContext;
            var filter = requestContext.HttpContext.Response.Filter;
            if (filter == null) return;

            var pageExtenderResponseFilter = new PageExtenderResponseFilter(filter);
            if (pageExtenderResponseFilter.ExtendersHtml.IsWhiteSpaceOrNull()) return;

            requestContext.HttpContext.Response.Filter = pageExtenderResponseFilter;
        }
    }
}