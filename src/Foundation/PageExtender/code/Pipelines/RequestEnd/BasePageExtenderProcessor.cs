using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Sitecore;
using Sitecore.Xml;

namespace Foundation.PageExtender.Pipelines.RequestEnd
{
    public abstract class BasePageExtenderProcessor
    {
        protected string RazorView = string.Empty;
        protected Collection<PageExtenderSiteFilter> SiteFilters { get; }

        protected BasePageExtenderProcessor()
        {
            SiteFilters = new Collection<PageExtenderSiteFilter>();
        }

        public void AddFilter(XmlNode node)
        {
            if (node == null)
                return;

            var key = XmlUtil.GetAttribute("filterKey", node);
            var value = XmlUtil.GetAttribute("filterValue", node);
            var razorViewAttribute = XmlUtil.GetAttribute("razorViewName", node);

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value) || string.IsNullOrEmpty(razorViewAttribute))
                return;

            SiteFilters.Add(new PageExtenderSiteFilter { Key = key, Value = value, RazorViewFilename = razorViewAttribute });
        }

        public void Process(RenderPageExtendersArgs args)
        {
            if (args == null || Context.Site == null) return;

            var filter = SiteFilters.FirstOrDefault(f => string.Equals(Context.Site.Properties[f.Key], f.Value) && !string.IsNullOrEmpty(f.RazorViewFilename));

            if (filter == null) return;

            RazorView = filter.RazorViewFilename;
            DoProcess(args);
        }

        public abstract void DoProcess(RenderPageExtendersArgs args);
    }
}