using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OSCALGenerator.CustomControls
{
    public partial class Parameters : System.Web.UI.UserControl
    {
        public List<string> Titles;
        protected void Page_Load(object sender, EventArgs e)
        {
            Titles = new List<string> { "parA", "parB", "parC" };
            foreach (var title in Titles)
            {
                var titleLabel = new Label();
                titleLabel.Height = 20;
                titleLabel.Width = 100;
                titleLabel.Text = title;
                var para = new TextBox();
                para.Height = 20;
                para.Width = 100;

                this.Controls.Add(titleLabel);
                this.Controls.Add(para);
            }
        }
    }
}