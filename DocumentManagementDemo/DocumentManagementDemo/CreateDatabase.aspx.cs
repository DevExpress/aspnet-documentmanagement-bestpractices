using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocumentManagementDemo
{
    public partial class CreateDatabase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!Request.IsLocal)
                return;

            const string FilesVirtualPath = @"~\App_Data\Files";
            DocumentsDbPopulationHelper.Populate(Server.MapPath(FilesVirtualPath));

            Button1.Text = "The database has been created";
        }
    }
}