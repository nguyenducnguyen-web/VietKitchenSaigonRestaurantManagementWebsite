using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebDatDoAnOnline
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            System.Web.UI.ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new System.Web.UI.ScriptResourceDefinition
            {
                Path = "~/Mau/js/jquery-3.4.1.min.js",
                DebugPath = "~/Mau/js/jquery-3.4.1.min.js",
                CdnPath = "https://code.jquery.com/jquery-3.4.1.min.js",
                CdnDebugPath = "https://code.jquery.com/jquery-3.4.1.js"
            });
        }
        void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            if (exc is HttpException && ((HttpException)exc).GetHttpCode() == 404)
            {
                Response.Redirect("~/Error404.aspx");
            }
        }
    }
}