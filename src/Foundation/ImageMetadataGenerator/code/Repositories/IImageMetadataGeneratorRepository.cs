namespace Foundation.ImageMetadataGenerator.Repositories
{
    using Models;

    public interface IImageMetadataGeneratorRepository
    {
        IChatGptVisionApiSettingsItem GetSettings();
    }
}
