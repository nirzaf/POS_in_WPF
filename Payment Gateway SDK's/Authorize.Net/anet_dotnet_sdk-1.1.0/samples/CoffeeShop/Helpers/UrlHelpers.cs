using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc {
    public static class UrlHelpers {

        public static string AbsoluteActionLink(this UrlHelper helper, string action, string controller, object args) {
            var relUrl =  helper.Action(action, controller, args);
            return string.Format("{0}{1}", SiteRoot(helper.RequestContext.HttpContext), relUrl);
        }
        public static string AbsoluteActionLink(this UrlHelper helper, string action, string controller) {
            var relUrl = helper.Action(action, controller);
            return string.Format("{0}{1}", SiteRoot(helper.RequestContext.HttpContext), relUrl);
        }
        public static string SiteRoot(HttpContextBase context) {
            return SiteRoot(context, true);
        }

        public static string SiteRoot(HttpContextBase context, bool usePort) {
            var Port = context.Request.ServerVariables["SERVER_PORT"];
            if (usePort) {
                if (Port == null || Port == "80" || Port == "443")
                    Port = "";
                else
                    Port = ":" + Port;
            }
            var Protocol = context.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (Protocol == null || Protocol == "0")
                Protocol = "http://";
            else
                Protocol = "https://";

            var appPath = context.Request.ApplicationPath;
            if (appPath == "/")
                appPath = "";

            var sOut = Protocol + context.Request.ServerVariables["SERVER_NAME"] + Port + appPath;
            return sOut;

        }

        public static string SiteRoot(this UrlHelper url) {
            return SiteRoot(url.RequestContext.HttpContext);
        }


        public static string SiteRoot(this ViewPage pg) {
            return SiteRoot(pg.ViewContext.HttpContext);
        }

        public static string SiteRoot(this ViewUserControl pg) {
            var vpage = pg.Page as ViewPage;
            return SiteRoot(vpage.ViewContext.HttpContext);
        }

        public static string SiteRoot(this ViewMasterPage pg) {
            return SiteRoot(pg.ViewContext.HttpContext);
        }

    }
}
