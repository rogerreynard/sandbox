﻿using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Foundation.ImageMetadataGenerator.DependencyInjection
{
    using Controllers;
    using Repositories;
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IImageMetadataRepository, ImageMetadataRepository>();
            serviceCollection.AddTransient<ImageMetadataGeneratorController>();
        }
    }
}