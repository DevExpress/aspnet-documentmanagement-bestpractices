using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentManagementDemo
{
    public partial class Document : System.Web.UI.MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var currentDocumentId = Request.QueryString["id"];
                int id = -1;
                if (int.TryParse(currentDocumentId, out id))
                {
                    DocumentTitle.InnerText = Utils.GetDocumentNameById(id);

                    var queryParams = HttpUtility.ParseQueryString(Request.Url.Query);
                    if (string.IsNullOrEmpty(queryParams["edit"]))
                    {
                        queryParams.Add("edit", "true");
                        DocumentEditLink.NavigateUrl = Request.Url.AbsolutePath + "?" + queryParams.ToString();
                        DocumentEditLink.Visible = true;
                    }
                    else
                    {
                        queryParams.Remove("edit");
                        DocumentBackLink.NavigateUrl = Request.Url.AbsolutePath + "?" + queryParams.ToString();
                        DocumentEditLink.Visible = false;
                    }
                }
            }
        }
    }
}