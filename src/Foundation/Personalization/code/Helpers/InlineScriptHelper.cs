using Foundation.Personalization.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Foundation.Personalization.Helpers
{
    public static class InlineScriptHelper
    {
        private static void Initialize(bool reinitialize = false)
        {
            if (reinitialize || HttpContext.Current.Items["Personalization"] == null)
                HttpContext.Current.Items["Personalization"] = new Dictionary<string, string>();
        }

        public static void AddScript(string key, string script)
        {
            Initialize();
            ((Dictionary<string, string>)HttpContext.Current.Items["Personalization"])?.Add(key, script);
        }

        public static HtmlString RenderScripts()
        {
            if ((Dictionary<string, string>)HttpContext.Current.Items["Personalization"] == null || !((Dictionary<string, string>)HttpContext.Current.Items["Personalization"]).Any())
                return new HtmlString(string.Empty);

            var models = (Dictionary<string, string>)HttpContext.Current.Items["Personalization"];

            var wrapper = new PersonalizedComponentImpressionModel
            {
                PersonalizedImpressionData = models.Select(m => JsonConvert.DeserializeObject<PersonalizedImpressionDataModel>(m.Value)).ToList()
            };

            var sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("window.dataLayer = window.dataLayer || [];");
            sb.AppendLine("window.dataLayer.push(");
            sb.AppendLine(JsonConvert.SerializeObject(wrapper));
            sb.AppendLine(");");
            sb.AppendLine("</script>");

            Initialize(true);

            return new HtmlString(sb.ToString());
        }
    }
}