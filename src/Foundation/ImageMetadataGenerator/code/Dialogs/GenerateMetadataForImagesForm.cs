using System;
using Foundation.ImageMetadataGenerator.Utilities;
using Sitecore;
using Sitecore.Data;
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
        protected DataContext MediaDataContext;
        protected TreeviewEx MediaTreeView;
        protected Border Preview;
        protected Literal SelectedItem;
        protected Literal SelectedItemPath;
        protected Checkbox OverwriteExisting;

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, nameof(message));
            Item obj = null;
            if (message.Arguments.Count > 0 && ID.IsID(message.Arguments["id"]))
            {
                var dataView = MediaTreeView.GetDataView();
                if (dataView != null)
                    obj = dataView.GetItem(message.Arguments["id"]);
            }
            if (obj == null)
                obj = MediaTreeView.GetSelectionItem(MediaDataContext.Language, Sitecore.Data.Version.Latest);
            Dispatcher.Dispatch(message, obj);
            if (message.Name == "item:load")
                MediaTreeView.Refresh(obj.Parent);
            base.HandleMessage(message);
        }

       /// <summary>Raises the load event.</summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs" /> instance containing the event data.
        /// </param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client postback,
        /// or if it is being loaded and accessed for the first time.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, nameof(e));
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;

            MediaDataContext.GetFromQueryString();

            if (Context.Item == null) return;

            var selectedItem = Context.Item;

            UpdatePreview(selectedItem);
        }

        ///// <summary>Called when the new has folder.</summary>
        ///// <param name="message">The message.</param>
        //[HandleMessage("medialink:newfolder")]
        //protected void OnNewFolder(Message message)
        //{
        //    Assert.ArgumentNotNull(message, nameof(message));
        //    var folder = MediaDataContext.GetFolder();
        //    if (folder == null)
        //        return;
        //    Items.NewFolder(folder);
        //}

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
            //else
            //{
            //    var mediaPath = selectionItem.Paths.MediaPath;
            //    base.OnOK(sender, args);
            //}
        }

        ///// <summary>Called when this instance has open.</summary>
        //protected void OnOpen()
        //{
        //    var selectionItem = MediaTreeview.GetSelectionItem();
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