using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;

namespace DocumentManagementDemo
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            // Must be Page_Init!
            fileManager.CustomFileSystemProvider = new DocumentsFileSystemProvider(Utils.CurrentDataProvider);
            fileManager.StylesDetailsView.CommandColumn.Width = Unit.Pixel(42);

            // Disable modifying for online demo
            if (!Request.IsLocal)
            {
                fileManager.SettingsUpload.Enabled = false;
                // fileManager.SettingsEditing.AllowDownload = false;
                fileManager.SettingsEditing.AllowCopy = false;
                fileManager.SettingsEditing.AllowCreate = false;
                fileManager.SettingsEditing.AllowDelete = false;
                fileManager.SettingsEditing.AllowMove = false;
                fileManager.SettingsEditing.AllowRename = false;
            }
        }
        // Filtering
        protected void fileManager_CustomCallback(object sender, CallbackEventArgsBase e)
        {
            fileManager.FilterBoxText = string.Empty; // It's optional
            if (string.IsNullOrEmpty(e.Parameter))
            {
                fileManager.FileListCustomFilter = string.Empty;
                fileManager.FileListCustomFilterBreadcrumbsText = string.Empty;
            }
            else
            {
                string[] filterArgs = e.Parameter.Split('|');
                fileManager.FileListCustomFilter = filterArgs[0];
                fileManager.FileListCustomFilterBreadcrumbsText = filterArgs[1];
            }
        }
        // Appearance customization
        protected void fileManager_CustomThumbnail(object source, FileManagerThumbnailCreateEventArgs e)
        {
            var file = e.Item as FileManagerFile;
            if (file == null)
                return;

            string url = GetThumbnailUrl(file.Extension);

            if (!string.IsNullOrEmpty(url))
                e.ThumbnailImage.Url = ResolveUrl(url);
        }

        private string GetThumbnailUrl(string extension)
        {
            string fileName = string.Empty;
            var iconFolder = "~/Images/";
            switch (extension)
            {
                case ".txt":
                case ".rtf":
                case ".odt":
                case ".doc":
                case ".docx":
                    fileName = "word.svg";
                    break;
                case ".xls":
                case ".xlsx":
                case ".ods":
                    fileName = "excel.svg";
                    break;
                case ".pdf":
                    fileName = "pdf.svg";
                    break;
            }
            if (!string.IsNullOrEmpty(fileName))
                return iconFolder + fileName;
            return null;

        }

        // Documents Opening 
        protected string GetItemUrl(FileManagerItem item)
        {
            if (item is FileManagerFolder)
                return "javascript:openCurrentFolder();";
            else
                return "OpenDocumentHandler.aspx?id=" + item.Id;
        }
        // ShowPath for Custom Filtering
        protected void fileManager_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e)
        {
            e.Properties["cpHasFilter"] = !string.IsNullOrEmpty(fileManager.FileListCustomFilter) ||
                !string.IsNullOrEmpty(fileManager.FilterBoxText);
        }
    }
}