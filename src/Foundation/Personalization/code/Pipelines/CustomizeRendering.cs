using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;
using Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering;
using System;

namespace Foundation.Personalization.Pipelines
{
    public class CustomizeRendering : GetRendererProcessor
    {
        public override void Process(GetRendererArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (args.Result != null || args.Rendering == null || string.IsNullOrEmpty(args.Rendering["RenderingXml"]))
                return;
            CustomizeRenderingArgs args1 = new CustomizeRenderingArgs(args.Rendering);
            args.Result = PipelineService.Get().RunPipeline<CustomizeRenderingArgs, Renderer>("mvc.customizeRendering", args1, (Func<CustomizeRenderingArgs, Renderer>)(pipelineArgs => pipelineArgs.Renderer));
        }
    }
}