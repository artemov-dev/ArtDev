using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ArtDev.Controls
{
    public class JSBundleChunk : UserControl
    {
        public string JSBundleFile { get; set; }
        private string GetMD5Hash(string filename)
        {
            string hive = SPUtility.GetGenericSetupPath(string.Empty) + "TEMPLATE\\"; 
            using (var md5 = MD5.Create())
            {
                filename = filename.Replace("/15/", "/").Replace("/14/", "/").Replace("/12/", "/").Replace("/_layouts/", "/layouts/");
                try
                {
                    using (var stream = File.OpenRead(hive + filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
                catch (Exception e) { return ""; }
            }
        }

        protected override void CreateChildControls()
        {          
            string mode = (string)SPContext.Current.Web.AllProperties["DEV"] ?? null;
            if (mode != "true")
            {
                string file = this.JSBundleFile;
                string hash = GetMD5Hash(this.JSBundleFile);

                HtmlGenericControl script = new HtmlGenericControl("script");
                script.Attributes.Add("type", "text/javascript");
                script.Attributes.Add("src", file + "?hash=" + hash);
                this.Controls.AddAt(0, script);
            }
        }
    }
}
