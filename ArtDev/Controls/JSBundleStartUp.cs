using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ArtDev.Controls
{
    public class JSBundleStartUp : UserControl
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
            string hash = GetMD5Hash(this.JSBundleFile);
            string mode = (string)SPContext.Current.Web.AllProperties["DEV"] ?? null;
            string filename = this.JSBundleFile.Substring(this.JSBundleFile.LastIndexOf('/') + 1);
            string file = mode == "true" ? "http://localhost:3000/public/" + filename : this.JSBundleFile + "?hash=" + hash;

            ClientScriptManager csm = this.Page.ClientScript;
            //Render the function invocation. 
            string funcCall = "<script src='" + file + "' language='javascript'></script>";

            if (!csm.IsStartupScriptRegistered("JSScript_" + filename))
            {
                csm.RegisterStartupScript(this.GetType(), "JSScript_" + filename, funcCall);
            }
        }

    }
    
}
