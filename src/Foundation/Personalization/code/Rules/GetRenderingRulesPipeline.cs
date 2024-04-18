using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Foundation.Personalization.Rules
{
    public static class GetRenderingRulesPipeline
    {
        public const string PipelineName = "getRenderingRules";

        public static void Run(GetRenderingRulesArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            CorePipeline pipeline = CorePipelineFactory.GetPipeline("getRenderingRules", string.Empty);
            if (pipeline == null)
            {
                Log.Error(string.Format("Cannot find pipeline with name '{0}'.", (object)"getRenderingRules"), typeof(GetRenderingRulesPipeline));
                args.AbortPipeline();
            }
            else
                pipeline.Run((PipelineArgs)args);
        }
    }
}