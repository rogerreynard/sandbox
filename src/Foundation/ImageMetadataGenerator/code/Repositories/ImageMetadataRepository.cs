using System;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Foundation.ImageMetadataGenerator.Repositories
{
    using Models;
    public class ImageMetadataRepository : IImageMetadataGeneratorRepository
    {
        private readonly ID _settingsId = new ID("{5D4DEFBD-2E66-48FA-A6AF-99A6F3C06A6D}");

        public IChatGptVisionApiSettingsItem GetSettings()
        {
            var item = GetSettingsItem();

            return new ChatGptVisionApiSettings(item);
        }

        private Item GetSettingsItem()
        {
            var masterDb = Database.GetDatabase("master");

            return
                masterDb.GetItem(_settingsId) ?? throw new Exception(
                        $"Could not find the Image Metadata Generator Settings item: ID={_settingsId}, Path=/sitecore/system/Modules/Image Metadata Generator/Settings");
        }
    }
}