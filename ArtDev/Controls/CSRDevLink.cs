using Microsoft.SharePoint;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ArtDev.Controls
{
    public class CSRDevLink : UserControl
    {
        protected override void CreateChildControls()
        {
            var Urls = SPContext.Current.Web.AllProperties["CSR"] ?? null;
            if (Urls != null)
            {
                HtmlGenericControl script = new HtmlGenericControl("script");
                script.Attributes.Add("type", "text/javascript");
                script.Attributes.Add("src", (string)Urls);
                this.Controls.AddAt(0, script);
            }
        }
    }
}
