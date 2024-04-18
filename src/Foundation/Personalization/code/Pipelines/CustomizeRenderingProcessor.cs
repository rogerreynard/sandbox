using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Presentation;
using System.Xml.Linq;
using Foundation.Personalization.Models;
using Sitecore.Personalization.Mvc.Pipelines.Response.CustomizeRendering;

namespace Foundation.Personalization.Pipelines
{
    public abstract class CustomizeRenderingProcessor : MvcPipelineProcessor<CustomizeRenderingArgs>
    {
        protected PersonalizedImpressionDataModel PersonalizedComponentEventModel { get; set; }

        public static RenderingReference GetRenderingReference(
          Rendering rendering,
          Language language,
          Database database)
        {
            Assert.IsNotNull(rendering, nameof(rendering));
            Assert.IsNotNull(language, nameof(language));
            Assert.IsNotNull(database, nameof(database));
            var property = rendering.Properties["RenderingXml"];
            return string.IsNullOrEmpty(property) ? null : new RenderingReference(XElement.Parse(property).ToXmlNode(), language, database);
        }

        protected virtual void ApplyChanges(Rendering rendering, RenderingReference reference)
        {
            Assert.ArgumentNotNull(rendering, nameof(rendering));
            Assert.ArgumentNotNull(reference, nameof(reference));
            TransferRenderingItem(rendering, reference);
            TransferDataSource(rendering, reference);
        }

        private static void TransferDataSource(Rendering rendering, RenderingReference reference)
        {
            Assert.ArgumentNotNull(rendering, nameof(rendering));
            Assert.ArgumentNotNull(reference, nameof(reference));
            var dataSource = reference.Settings.DataSource;
            if (string.IsNullOrEmpty(dataSource) || dataSource.Equals(rendering.DataSource))
                return;
            rendering.DataSource = dataSource;
            var itemByDataSource = GetItemByDataSource(dataSource, reference.Database);
            if (itemByDataSource == null)
                return;
            rendering.Item = itemByDataSource;
        }

        private static void TransferRenderingItem(Rendering rendering, RenderingReference reference)
        {
            Assert.ArgumentNotNull(rendering, nameof(rendering));
            Assert.ArgumentNotNull(reference, nameof(reference));
            if (reference.RenderingItem == null)
                return;
            rendering.RenderingItem = reference.RenderingItem;
        }

        private static Item GetItemByDataSource(string dataSource, Database database)
        {
            if (ID.TryParse(dataSource, out var result))
                return database.GetItem(result);
            var uri = DataUri.Parse(dataSource);
            return uri != null ? database.GetItem(uri) : database.GetItem(dataSource);
        }
    }
}
