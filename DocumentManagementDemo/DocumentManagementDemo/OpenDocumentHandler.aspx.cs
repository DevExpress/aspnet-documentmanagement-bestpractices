using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentManagementDemo
{
    public partial class OpenDocumentHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                long id = long.Parse(Request.QueryString["id"]);
                var docType = Utils.GetDocumentTypeById(id);
                if (docType == Utils.RtfDocType)
                    Response.Redirect("RichEdit.aspx?id=" + id.ToString());
                else if (docType == Utils.SheetDocType)
                    Response.Redirect("Spreadsheet.aspx?id=" + id.ToString());
                else
                    Utils.DownloadDocumentById(this, id);
            }
        }
    }
}