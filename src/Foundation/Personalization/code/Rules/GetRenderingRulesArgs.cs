using Sitecore.Diagnostics;
using System;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Pipelines;
using Sitecore.Rules.ConditionalRenderings;
using System.Runtime.Serialization;

namespace Foundation.Personalization.Rules
{
    [Serializable]
    public class GetRenderingRulesArgs : PipelineArgs
    {
        public GetRenderingRulesArgs(Item item, RenderingReference renderingReference)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNull(renderingReference, nameof(renderingReference));
            Item = item;
            RenderingReference = renderingReference;
        }

        protected GetRenderingRulesArgs(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Assert.ArgumentNotNull(info, nameof(info));
            var obj = info.GetValue("GetRenderingRulesArgs.Item", typeof(Item)) as Item;
            Assert.IsNotNull(obj, "item != null");
            var renderingReference = info.GetValue("GetRenderingRulesArgs.RenderingReference", typeof(RenderingReference)) as RenderingReference;
            Assert.IsNotNull(renderingReference, "renderingReference != null");
            Item = obj;
            RenderingReference = renderingReference;
        }

        public Item Item { get; protected set; }

        public RenderingReference RenderingReference { get; protected set; }

        public Sitecore.Rules.RuleList<ConditionalRenderingsRuleContext> RuleList { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Assert.ArgumentNotNull(info, nameof(info));
            base.GetObjectData(info, context);
            info.AddValue("GetRenderingRulesArgs.Item", Item);
            info.AddValue("GetRenderingRulesArgs.RenderingReference", RenderingReference);
        }
    }
}