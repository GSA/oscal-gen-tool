using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OSCALGenerator
{
    public partial class SiteSAR : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Cache["username"] != null)
                Label1.Text = Cache["username"].ToString();
        }
    }
}