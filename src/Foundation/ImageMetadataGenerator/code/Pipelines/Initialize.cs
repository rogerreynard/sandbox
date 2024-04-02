using System.Web.Http;
using Sitecore.Pipelines;
using Sitecore.Diagnostics;

namespace Foundation.ImageMetadataGenerator.Pipelines
{
    public class Initialize
    {
        public void Process(PipelineArgs args)
        {
            Log.Info("ImageMetadataGenerator: Running Initialize.Process method", this);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}