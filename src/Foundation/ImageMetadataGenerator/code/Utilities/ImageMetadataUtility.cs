using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Foundation.ImageMetadataGenerator.Utilities
{
    public static class ImageMetadataUtility
    {
        public static string GetBase64Encode(MediaItem mediaItem)
        {
            var stream = mediaItem.GetMediaStream();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return "data:" + mediaItem.MimeType + ";base64," + Convert.ToBase64String(bytes);
        }

        public static string GetMetadataFromVision(string imgUrl)
        {
            var address = GetBaseAddress();

            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(address, imgUrl).Result;
                response.EnsureSuccessStatusCode();

                var metadata = response.Content.ReadAsStringAsync().Result;
                return metadata;
            }
        }

        public static string GetBaseAddress()
        {
            return $"{Sitecore.Web.WebUtil.GetScheme()}://{Sitecore.Web.WebUtil.GetHostName()}/api/imagemetadatagenerator/getmetadata";
        }

        public static string GetValue(string part)
        {
            return part.Split(new[] { ": " }, 2, StringSplitOptions.None)[1];
        }

        public static bool SaveMetadata(Item item, string altText, string description, bool overwriteExisting = true)
        {
            item.Editing.BeginEdit();

            if (overwriteExisting || string.IsNullOrEmpty(item.Fields["Alt"]?.Value))
            {
                item.Fields["Alt"]?.SetValue(altText, true);
            }
            if (overwriteExisting || string.IsNullOrEmpty(item.Fields["Description"]?.Value))
            {
                item.Fields["Description"]?.SetValue(description, true);
            }

            item.Editing.EndEdit();

            return true;
        }

        public static IEnumerable<ID> GetImageTypeTemplateIDs()
        {
            return
                Sitecore.Configuration.Settings.GetSetting("Foundation.ImageMetadataGenerator.ImageTypeTemplateIDs")
                    .Split('|')
                    .Select(ID.Parse)
                    .ToList();
        }

        public static bool IsMediaLibraryItem(string path)
        {
            return path.StartsWith("/sitecore/media library", StringComparison.OrdinalIgnoreCase);
        }
    }
}