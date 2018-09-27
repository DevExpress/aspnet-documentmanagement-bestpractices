using DevExpress.Web.ASPxSpreadsheet;
using DocumentManagementDemo;
using System;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using DevExpress.Spreadsheet;

namespace DocumentManagementDemo
{
    public partial class Spreadsheet : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            SpreadsheetRibbonUtils.HideFileTab(spreadSheet);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Opening Document
            var documentID = Request.QueryString["id"];
            int itemID = -1;
            int.TryParse(documentID, out itemID);
            var item = Utils.CurrentDataProvider.GetDocumentById(itemID);

            var data = item.Content.Data;

            spreadSheet.Open(documentID + "&" + Guid.NewGuid().ToString(), () => data);

            // View Mode
            if (string.IsNullOrEmpty(Request.QueryString["edit"]))
            { // is view mode?
                spreadSheet.SettingsView.Mode = SpreadsheetViewMode.Reading;
                spreadSheet.ShowFormulaBar = false;
            }

        }
        // Saving Document
        protected void spreadSheet_Saving(object source, DevExpress.Web.Office.DocumentSavingEventArgs e)
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
                    spreadSheet.SaveCopy(Utils.GetSpreadsheetDocumentFormatByExtension(item.Name)));
        }
    }
    public static class SpreadsheetRibbonUtils
    {
        public static void HideFileTab(ASPxSpreadsheet spreadsheet)
        {
            spreadsheet.CreateDefaultRibbonTabs(true);
            spreadsheet.RibbonTabs.RemoveAt(0);
            SRFileCommonGroup gr = new SRFileCommonGroup();
            gr.Items.Add(new SRFileSaveCommand());
            gr.Items.Add(new SRFilePrintCommand());
            spreadsheet.RibbonTabs[0].Groups.Insert(0, gr);
            spreadsheet.ActiveTabIndex = 0;
        }
    }
}
