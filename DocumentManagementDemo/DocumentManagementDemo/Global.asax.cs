using System;

namespace DocumentManagementDemo
{
    public class Global_asax : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            Utils.ClearDocumentPreviews();
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            Utils.DisposeCurrentDataProvider();
        }
    }
}