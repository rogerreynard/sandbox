using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework.Commands;
using System;
using System.Linq;
using Foundation.ImageMetadataGenerator.Utilities;

namespace Foundation.ImageMetadataGenerator.Commands
{
    [Serializable]
    public class GenerateMetadata : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            if (context.Items.Length != 1)
                return;
            var item = context.Items[0];

            var mediaItem = (MediaItem)item;
            var imgUrl = ImageMetadataUtility.GetBase64Encode(mediaItem);

            var metadata = ImageMetadataUtility.GetMetadataFromVision(imgUrl);
            var parts = metadata.Replace("\"", "'").Split(new[] { "\n\n" }, StringSplitOptions.None);
            var altText = ImageMetadataUtility.GetValue(parts[0]);
            var description = ImageMetadataUtility.GetValue(parts[1]);

            ImageMetadataUtility.SaveMetadata(item, altText, description);

            Log.Audit(this, "Generated Metadata for attachment to media item: {0},{1}", item.ID.ToString(), item.Paths.FullPath);
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            var imgTypeTemplateIDs = ImageMetadataUtility.GetImageTypeTemplateIDs();

            if (context.Items.Length != 1 || !context.Items[0].Paths.IsMediaItem)
                return CommandState.Hidden;

            return
                !MediaManager.HasMediaContent(context.Items[0]) || !imgTypeTemplateIDs.Contains(context.Items[0].TemplateID)
                    ? CommandState.Disabled
                    : base.QueryState(context);

        }
    }
}