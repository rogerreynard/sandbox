using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Foundation.Personalization.Helpers
{
    public static class InlineScriptHelper
    {
        public static void Initialize()
        {
            if (HttpContext.Current.Items["Personalization"] == null)
                HttpContext.Current.Items["Personalization"] = new Dictionary<string, string>();
        }

        public static void AddScript(string key, string script)
        {
            Initialize();
            ((Dictionary<string, string>)HttpContext.Current.Items["Personalization"])?.Add(key, script);
        }

        public static HtmlString RenderScripts()
        {
            Initialize();

            if ((Dictionary<string, string>)HttpContext.Current.Items["Personalization"] == null || !((Dictionary<string, string>)HttpContext.Current.Items["Personalization"]).Any())
                return new HtmlString(string.Empty);

            var sb = new StringBuilder();
            sb.AppendLine("<script>");
            var scripts = (Dictionary<string, string>)HttpContext.Current.Items["Personalization"];

            //sb.AppendLine("window.onload = function () {");
            //sb.AppendLine("window.dataLayer = window.dataLayer || [];");
            //foreach (var script in scripts)
            //{
            //    sb.AppendLine(HttpUtility.HtmlDecode(script.Value));
            //}
            //sb.AppendLine("};");


            // New way to dispatch event

            sb.AppendLine("const event = new Event('personalized_component_impression')");
            sb.AppendLine("window.addEventListener('personalized_component_impression', (e) => {");
            sb.AppendLine("window.dataLayer = window.dataLayer || [];");
            foreach (var script in scripts)
            {
                sb.AppendLine(HttpUtility.HtmlDecode(script.Value));
            }
            sb.AppendLine("}, false,);");
            sb.AppendLine("window.dispatchEvent(event);");



            // Old way to dispatch event - if this is used, it should be used with the code starting at ln 35 and ending at ln 41 

            //sb.AppendLine("var event;");
            //sb.AppendLine("if(document.createEvent){");
            //sb.AppendLine("event = document.createEvent('HTMLEvents');");
            //sb.AppendLine("event.initEvent('personalized_component_impression', true, true);");
            //sb.AppendLine("event.eventName = 'personalized_component_impression';");
            //sb.AppendLine("document.dispatchEvent(event);");
            //sb.AppendLine("} else {");
            //sb.AppendLine("event = document.createEventObject();");
            //sb.AppendLine("event.eventName = 'personalized_component_impression';");
            //sb.AppendLine("event.eventType = 'personalized_component_impression';");
            //sb.AppendLine("document.fireEvent('on' + event.eventType, event);");
            //sb.AppendLine("}");

            sb.AppendLine("</script>");

            return new HtmlString(sb.ToString());
        }
    }
}