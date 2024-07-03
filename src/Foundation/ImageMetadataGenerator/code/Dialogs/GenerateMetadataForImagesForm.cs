using System;
using System.Linq;
using Foundation.ImageMetadataGenerator.Models;
using Foundation.ImageMetadataGenerator.Repositories;
using Foundation.ImageMetadataGenerator.Utilities;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;

namespace Foundation.ImageMetadataGenerator.Dialogs
{
    public class GenerateMetadataForImagesForm : DialogForm
    {
        private IImageMetadataRepository _repo;
        private IChatGptVisionApiSettingsItem _settings;

        protected DataContext MediaDataContext;
        protected TreeviewEx MediaTreeView;
        protected Border Preview;
        protected Literal SelectedItem;
        protected Literal SelectedItemPath;
        protected Checkbox OverwriteExisting;
        protected Combobox Scope;

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        //public override void HandleMessage(Message message)
        //{
        //    var obj = (Item)null;
        //    if (message.Arguments.Count > 0 && ID.IsID(message.Arguments["id"]))
        //    {
        //        var dataView = MediaTreeView.GetDataView();
        //        if (dataView != null)
        //            obj = dataView.GetItem(message.Arguments["id"]);
        //    }
        //    if (obj == null)
        //        obj = MediaTreeView.GetSelectionItem(MediaDataContext.Language, Sitecore.Data.Version.Latest);
        //    Dispatcher.Dispatch(message, obj);
        //}

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);

            _repo = new ImageMetadataRepository();
            _settings = _repo.GetSettings();

            if (Context.ClientPage.IsEvent)
                return;

            MediaDataContext.GetFromQueryString();

            var selectedItem = Context.Item;

            UpdatePreview(selectedItem);
        }

        /// <summary>Handles a click on the OK button.</summary>
        /// <param name="sender">
        /// </param>
        /// <param name="args">
        /// </param>
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
        /// </remarks>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull(args, nameof(args));
            var selectionItem = MediaTreeView.GetSelectionItem();
            if (selectionItem == null)
            {
                Context.ClientPage.ClientResponse.Alert("Select a media item.");
            }
            else
            {
                var overwriteExisting = OverwriteExisting.Checked;
                var scope = Scope.Value;
                var templateIds = ImageMetadataUtility.GetImageTypeTemplateIDs().ToList();
                var predicate = "[@@templateid='" + string.Join("' or @@templateid='", templateIds) + "']";

                var query = selectionItem.Paths.FullPath;
                if (scope.Contains("Children"))
                {
                    query += "/*";
                }

                if (scope.Contains("Descendants"))
                {
                    query += "//*";
                }

                query += predicate;

                var items = Sitecore.Configuration.Factory.GetDatabase("master").SelectItems(query).ToList();
                if (scope.Contains("Root") && templateIds.Contains(selectionItem.TemplateID))
                {
                    items.Add(selectionItem);
                }

                foreach (var item in items)
                {
                    var mediaItem = (MediaItem)item;
                    if((mediaItem.Alt != string.Empty || mediaItem.Description != string.Empty) && !overwriteExisting)
                        continue;

                    var imgUrl = ImageMetadataUtility.GetBase64Encode(mediaItem);

                    var metadata = ImageMetadataUtility.GetMetadataFromVision(imgUrl);
                    var parts = metadata.Replace("\"", "'").Split(new[] { "\n\n" }, StringSplitOptions.None);
                    var altText = ImageMetadataUtility.GetValue(parts[0]);
                    var description = ImageMetadataUtility.GetValue(parts[1]);

                    ImageMetadataUtility.SaveMetadata(item, altText, description);
                }
            }
        }

        //[ProcessorMethod]
        //protected void OnOpen()
        //{
        //    var selectionItem = MediaTreeView.GetSelectionItem();
        //    if (selectionItem == null || !selectionItem.HasChildren)
        //        return;
        //    MediaDataContext.SetFolder(selectionItem.Uri);
        //}

        /// <summary>Selects the tree node.</summary>
        [ProcessorMethod]
        protected void SelectTreeNode()
        {
            var selectionItem = MediaTreeView.GetSelectionItem();
            if (selectionItem == null)
                return;

            UpdatePreview(selectionItem);
        }

        /// <summary>Updates the preview.</summary>
        /// <param name="item">The item.</param>
        private void UpdatePreview(Item item)
        {
            Assert.ArgumentNotNull(item, nameof(item));

            if (!ImageMetadataUtility.IsMediaLibraryItem(item.Paths.FullPath))
            {
                item = Context.Database.GetItem("/sitecore/Media Library");
            }

            SelectedItem.Text = item.DisplayName;
            SelectedItemPath.Text = item.Paths.FullPath;
            OverwriteExisting.Checked = _settings.DefaultOverwriteExistingField.Checked;
            Scope.Value = _settings.ScopeField.TargetItem.Name;

            var thumbnailOptions = MediaUrlBuilderOptions.GetThumbnailOptions(item);
            thumbnailOptions.UseDefaultIcon = true;
            thumbnailOptions.Width = 96;
            thumbnailOptions.Height = 96;
            thumbnailOptions.Language = item.Language;
            thumbnailOptions.AllowStretch = false;
            Preview.InnerHtml = "<img src=\"" + MediaManager.GetMediaUrl(item, thumbnailOptions) + "\" width=\"96\" height=\"96\" border=\"0\" alt=\"\" />";
        }

    }
}