using Sitecore.Pipelines.HttpRequest;
using Sitecore.Diagnostics;
using System;
using System.Web;
using System.Linq;
using Sitecore.Web;
using System.Collections.Generic;
using Sitecore.RedirectionManager.RedirectionObject;
using CsvHelper;
using System.IO;
using Sitecore.RedirectionManager.Infrastructure.Caching;

namespace Sitecore.RedirectionManager.Infrastructure.Pipeline
{
    public class RedirectProcessor
    {
        public bool Enabled { get; set; }

        public string FileName { get; set; }

        public bool IsRedirect { get; set; }

        public void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.Context == null)
            {
                return;
            }

            if (HttpContext.Current == null && Context.Site == null && Context.Database == null)
            {
                return;
            }

            if (IsContentEditor())
            {
                return;
            }
            
            if (!MainUtil.GetBool(Sitecore.Context.Site.Properties["applyRedirection"], false))
            {
                return;
            }

            string requestedUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            string rawPath = Context.RawUrl;
            int question = rawPath.IndexOf("?", StringComparison.InvariantCultureIgnoreCase);

            if (question > -1)
            {
                rawPath = rawPath.Substring(0, question);
            }
            
            string redirectUrl = null;

            var allRedirectUrls = GetAllRedirectObjects();

            var redirectUrlObject = allRedirectUrls.FirstOrDefault(x => !string.IsNullOrEmpty(x.AcientUrl) && requestedUrl.Contains(x.AcientUrl));

            if (redirectUrlObject == null)
            {
                return;
            }
            
            redirectUrl = redirectUrlObject.NewUrl;

            if (HttpContext.Current.Request.HttpMethod != "POST")
            {
                Log.Info($"{this} : redirect from {requestedUrl} to {redirectUrl}", this);

                if (this.Enabled)
                {
                    return;
                }
                HttpContext.Current.Response.AddHeader("Location", redirectUrl);

                if (IsRedirect)
                {
                    HttpContext.Current.Response.StatusCode = 301;
                    HttpContext.Current.Response.Status = "301 Moved Permanently";
                }
                else
                {
                    HttpContext.Current.Response.StatusCode = 302;
                }

                WebUtil.Redirect(redirectUrl, false);
                args.AbortPipeline();
            }
            else
            {
                Log.Warn($"{this} should redirect from {requestedUrl} to {redirectUrl} but HTTP method is {HttpContext.Current.Request.HttpMethod}", this);
            }
        }
        
        private IEnumerable<RedirectUrlObject> GetAllRedirectObjects()
        {
            if (CachingManager.Instance.GetItem("allRedirectUrls", false) is IEnumerable<RedirectUrlObject> allRedirectUrls)
            {
                return allRedirectUrls;
            }

            var fullPath = HttpContext.Current.Server.MapPath("/App_Data/RedirectData/SEObocchiotti.it.csv");
            var csv = new CsvReader(File.OpenText(fullPath));
            csv.Configuration.RegisterClassMap(new RedirectionUrlMapObject());
            allRedirectUrls = csv.GetRecords<RedirectUrlObject>();

            CachingManager.Instance.AddItem("allRedirectUrls", allRedirectUrls, DateTimeOffset.UtcNow.AddMinutes(5));
            return allRedirectUrls;
        }

        private bool IsContentEditor()
        {
            if (Context.Site != null && !string.IsNullOrEmpty(Context.Site.Name))
            {
                return
                  Context.Site.Name == "shell" ||
                  Context.Site.Name == "login" ||
                  Context.Site.Name == "admin" ||
                  Context.Site.Name == "service" ||
                  Context.Site.Name == "service" ||
                  Context.Site.Name == "modules_website" ||
                  Context.Site.Name == "scheduler" ||
                  Context.Site.Name == "system" ||
                  Context.Site.Name == "publisher" ||
                  Context.Site.Name == "modules_shell";
            }

            return false;
        }
    }
}
