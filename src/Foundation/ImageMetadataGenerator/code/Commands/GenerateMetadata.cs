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

            // If we want to enable an interface on any item within the Media Library node including the root...
            // if (context.Items.Length != 1 || !IsMediaLibraryItem(context.Items[0].Paths.FullPath))
            //    return CommandState.Hidden;

            var imgTypeTemplateIDs = ImageMetadataUtility.GetImageTypeTemplateIDs();

            if (context.Items.Length != 1 || !context.Items[0].Paths.IsMediaItem)
                return CommandState.Hidden;

            return
                !MediaManager.HasMediaContent(context.Items[0]) || !imgTypeTemplateIDs.Contains(context.Items[0].TemplateID)
                    ? CommandState.Disabled
                    : base.QueryState(context);

        }

        #region Private Methods

        //private static string GetBase64Encode(MediaItem mediaItem)
        //{
        //    var stream = mediaItem.GetMediaStream();
        //    var bytes = new byte[stream.Length];
        //    stream.Read(bytes, 0, bytes.Length);
        //    return "data:" + mediaItem.MimeType + ";base64," + Convert.ToBase64String(bytes);
        //}

        //private string GetMetadataFromVision(string imgUrl)
        //{
        //    var address = GetBaseAddress();

        //    using (var client = new HttpClient())
        //    {
        //        var response = client.PostAsJsonAsync(address, imgUrl).Result;
        //        response.EnsureSuccessStatusCode();

        //        var metadata = response.Content.ReadAsStringAsync().Result;
        //        return metadata;
        //    }
        //}

        //private string GetBaseAddress()
        //{
        //    return $"{Sitecore.Web.WebUtil.GetScheme()}://{Sitecore.Web.WebUtil.GetHostName()}/api/imagemetadatagenerator/getmetadata";
        //}

        //private static string GetValue(string part)
        //{
        //    return part.Split(new[] { ": " }, 2, StringSplitOptions.None)[1];
        //}

        //private bool SaveMetadata(Item item, string altText, string description)
        //{
        //    item.Editing.BeginEdit();
        //    item.Fields["Alt"]?.SetValue(altText, true);
        //    item.Fields["Description"]?.SetValue(description, true);
        //    item.Editing.EndEdit();
        //    return true;
        //}

        //private IEnumerable<ID> GetImageTypeTemplateIDs()
        //{
        //    return
        //        Sitecore.Configuration.Settings.GetSetting("Foundation.ImageMetadataGenerator.ImageTypeTemplateIDs")
        //        .Split('|')
        //        .Select(ID.Parse)
        //        .ToList();
        //}

        // See comment above in QueryState method
        //private bool IsMediaLibraryItem(string path)
        //{
        //    return path.StartsWith("/sitecore/media library", StringComparison.OrdinalIgnoreCase);
        //}

        #endregion
    }
}