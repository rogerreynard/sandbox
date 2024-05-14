namespace Foundation.ImageMetadataGenerator.Repositories
{
    using Models;

    public interface IImageMetadataRepository
    {
        IChatGptVisionApiSettingsItem GetSettings();
    }
}
