using System;
using Sitecore.Shell.Framework.Commands;
using System.Linq;
using System.Web;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore;

namespace Foundation.ImageMetadataGenerator.Commands
{
    public class GenerateMetadataForImages : Command
    {
        public override void Execute(CommandContext context)
        {
            Context.ClientPage.Start(this, "Run", context.Parameters);
        }

        protected static void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack) return;

            var urlString = new UrlString(UIUtil.GetUri("control:GenerateMetadataForImages"));
            SheerResponse.ShowModalDialog(urlString.ToString(), "800", "300", "", true);
            args.WaitForPostBack();
        }
    }
}