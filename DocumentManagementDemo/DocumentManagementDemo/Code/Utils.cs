using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.Web;
using DevExpress.Web.Internal;

namespace DocumentManagementDemo
{
    public static class Utils
    {
        const string
            DataProviderKey = "04FC2F63-54D8-4925-B404-6EAC8827476C";

        public static DataProvider CurrentDataProvider
        {
            get
            {
                if (HttpContext.Current.Items[DataProviderKey] == null)
                    HttpContext.Current.Items[DataProviderKey] = new DataProvider();
                return (DataProvider)HttpContext.Current.Items[DataProviderKey];
            }
        }

        public static void DisposeCurrentDataProvider()
        {
            if (HttpContext.Current.Items.Contains(DataProviderKey))
            {
                DataProvider dataProvider = (DataProvider)HttpContext.Current.Items[DataProviderKey];
                dataProvider.Dispose();
                HttpContext.Current.Items.Remove(DataProviderKey);
            }
        }

        // Doc Types
        public static readonly string RtfDocType = "Rtf";
        public static readonly string SheetDocType = "Sheet";

        public static DevExpress.XtraRichEdit.DocumentFormat GetRichEditDocumentFormatByExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".rtf":
                    return DevExpress.XtraRichEdit.DocumentFormat.Rtf;
                case ".doc":
                    return DevExpress.XtraRichEdit.DocumentFormat.Doc;
                case ".docx":
                    return DevExpress.XtraRichEdit.DocumentFormat.OpenXml;
                case ".txt":
                    return DevExpress.XtraRichEdit.DocumentFormat.PlainText;
                default:
                    return DevExpress.XtraRichEdit.DocumentFormat.Undefined;
            }
        }

        public static DevExpress.Spreadsheet.DocumentFormat GetSpreadsheetDocumentFormatByExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".xls":
                    return DevExpress.Spreadsheet.DocumentFormat.Xls;
                case ".xlsx":
                    return DevExpress.Spreadsheet.DocumentFormat.Xlsx;
                default:
                    return DevExpress.Spreadsheet.DocumentFormat.Undefined;
            }
        }

        public static string GetDocumentTypeById(long id)
        {
            var item = CurrentDataProvider.GetDocumentById(id);
            if (item != null)
            {
                if (!GetRichEditDocumentFormatByExtension(item.Name).Equals(DevExpress.XtraRichEdit.DocumentFormat.Undefined))
                    return RtfDocType;
                if (!GetSpreadsheetDocumentFormatByExtension(item.Name).Equals(DevExpress.Spreadsheet.DocumentFormat.Undefined))
                    return SheetDocType;
            }
            return string.Empty;
        }

        public static void DownloadDocumentById(System.Web.UI.Page page, long id)
        {
            var item = CurrentDataProvider.GetDocumentById(id);
            if (item != null)
            {
                var stream = new MemoryStream(item.Content.Data.ToArray());
                string fileName = Path.GetFileNameWithoutExtension(item.Name);
                string fileExt = Path.GetExtension(item.Name);
                fileExt = fileExt.Remove(0, 1);
                HttpUtils.WriteFileToResponse(page, stream, fileName, true, fileExt, HttpUtils.GetContentType(fileExt), true);
            }
        }
        public static string GetDocumentNameById(long id)
        {
            var item = CurrentDataProvider.GetDocumentById(id);
            if (item != null)
                return item.Name;
            return string.Empty;
        }

        // RichText utils

        public static readonly string PreviewsFolder = @"~\DocumentPreviews";

        public static string GetDocumentPreviewFilesFolderById(long id)
        {
            return HttpContext.Current.Server.MapPath(PreviewsFolder + @"\" + id.ToString());
        }
        public static void ClearDocumentPreviewFilesById(long id)
        {
            string directory = GetDocumentPreviewFilesFolderById(id);
            try
            {
                foreach (var file in new DirectoryInfo(directory).GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }
                }
            }
            catch { }
        }
        public static void ClearDocumentPreviews()
        {
            try
            {
                foreach (var directoryInfo in new DirectoryInfo(HttpContext.Current.Server.MapPath(PreviewsFolder)).GetDirectories())
                    directoryInfo.Delete(true);
            }
            catch { }
        }
    }
}