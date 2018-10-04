using DocumentManagementDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxRichEdit;
using System.IO;
using DevExpress.Web.Office;

namespace DocumentManagementDemo
{
    public partial class RichEdit : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            RichEditRibbonUtils.HideFileTab(richEdit);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Opening Document
            var documentID = Request.QueryString["id"];
            int itemID = -1;
            int.TryParse(documentID, out itemID);
            var item = Utils.CurrentDataProvider.GetDocumentById(itemID);
            var data = item.Content.Data;
            var documentFormat = GetDocumentFormat(data, item.Name);
            // View Mode emulation
            if (string.IsNullOrEmpty(Request.QueryString["edit"]))
            { // is view mode?
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(data, 0, data.Length);
                    ms.Position = 0;

                    string documentPreviewFilesFolder = Utils.GetDocumentPreviewFilesFolderById(itemID);
                    string documentPreviewFileName = documentPreviewFilesFolder + @"\preview";
                    string html;
                    if (Directory.Exists(documentPreviewFilesFolder) && File.Exists(documentPreviewFileName))
                        html = File.ReadAllText(documentPreviewFileName);
                    else
                    {
                        ASPxHtmlEditor htmlEditor = new ASPxHtmlEditor();
                        htmlEditor.Import(GetHtmlEditorImportFormat(documentFormat), ms, true, documentPreviewFilesFolder);
                        html = htmlEditor.Html;
                        if (!Directory.Exists(documentPreviewFilesFolder))
                            Directory.CreateDirectory(documentPreviewFilesFolder);
                        File.WriteAllText(documentPreviewFileName, html);
                    }
                    viewer.InnerHtml = html;
                }

                viewerWrapper.Visible = true;
                richEdit.Visible = false;
            }
            else
            {
                richEdit.Open(documentID + "&" + Guid.NewGuid().ToString(), () => data);
                viewerWrapper.Visible = false;
            }
        }
        private DocumentFormat GetDocumentFormat(byte[] data, string fileName)
        {
            using (var ms = new MemoryStream(data))
            {
                var documentFormat = new RichEditDocumentServer().GetService<IFormatDetectorService>().DetectFormat(ms);
                if (documentFormat.Equals(DocumentFormat.Undefined))
                    documentFormat = Utils.GetRichEditDocumentFormatByExtension(fileName);
                return documentFormat;
            }
        }

        protected HtmlEditorImportFormat GetHtmlEditorImportFormat(DocumentFormat documentFormat)
        {
            if (documentFormat.Equals(DocumentFormat.OpenXml))
                return HtmlEditorImportFormat.Docx;
            else if (documentFormat.Equals(DocumentFormat.Doc))
                return HtmlEditorImportFormat.Doc;
            else if (documentFormat.Equals(DocumentFormat.Rtf))
                return HtmlEditorImportFormat.Rtf;
            else if (documentFormat.Equals(DocumentFormat.OpenDocument))
                return HtmlEditorImportFormat.Odt;
            else if (documentFormat.Equals(DocumentFormat.Mht))
                return HtmlEditorImportFormat.Mht;
            else
                return HtmlEditorImportFormat.Txt;
        }

        // Saving Document
        protected void richEdit_Saving(object source, DocumentSavingEventArgs e)
        {
            e.Handled = true;
            if (!Request.IsLocal) // Disable modifying for online demo
                return;

            string[] idStrs = e.DocumentID.Split('&');
            string currentDocumentId = idStrs[0];
            long id = long.Parse(currentDocumentId);

            var item = Utils.CurrentDataProvider.GetDocumentById(id);
            if (item != null)
                Utils.CurrentDataProvider.SetDocumentContent(id,
                    richEdit.SaveCopy(Utils.GetRichEditDocumentFormatByExtension(item.Name)));
        }

    }
    public static class RichEditRibbonUtils
    {
        public static void HideFileTab(ASPxRichEdit richEdit)
        {
            richEdit.CreateDefaultRibbonTabs(true);
            richEdit.RibbonTabs.RemoveAt(0);
            RERFileCommonGroup gr = new RERFileCommonGroup();
            gr.Items.Add(new RERSaveCommand());
            gr.Items.Add(new RERPrintCommand());
            richEdit.RibbonTabs[0].Groups.Insert(0, gr);
            richEdit.ActiveTabIndex = 0;
        }
    }
}