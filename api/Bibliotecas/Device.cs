using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace api.Bibliotecas
{
    public class Device
    {
        public static Boolean IsMobile()
        {
            var userAgent = HttpContext.Current.Request.UserAgent;

            if( userAgent.Contains("Mobile") )
                return true;
            else
                return false;

            /*//Console.WriteLine(HttpContext.Current.Request.UserHostAddress);
            var userBrowser = new HttpBrowserCapabilities { Capabilities = new Hashtable { { string.Empty, userAgent } } };
            //var factory = new BrowserCapabilitiesFactory();
            //factory.ConfigureBrowserCapabilities(new NameValueCollection(), userBrowser);

            System.Web.HttpBrowserCapabilities myBrowserCaps = userBrowser;
            if (((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).IsMobileDevice)
                return true;
            else
                return false;*/
        }
    }
}