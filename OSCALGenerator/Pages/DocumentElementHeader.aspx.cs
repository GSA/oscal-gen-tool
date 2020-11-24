using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlDataProvider;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Configuration;

namespace OSCALGenerator.Pages
{
    public partial class DocumentHeaderElement : System.Web.UI.Page
    {
        private Color MainPanelColor;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            ErrorPanel.Visible = false;
            RemovePanel.Visible = false;
            
        }

        // protected void ElmentListBox_SelectedIndexChanged(object sender, EventArgs e)
        // {
        //    if( ElmentListBox.SelectedItem.Text == "Add")
        //     {
        //         this.Panel1.Visible = true;
        //     }
        //}

       

        void AddHeaderElement(string eltName, int eltTypeId, string eltTag, string eltDescription, int active)
        {
            string ConnString = ConfigurationManager.ConnectionStrings["OWT_DEVConnectionString"].ConnectionString;
            


            CSharpDAL dal = new CSharpDAL(ConnString);



            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[7];
            oParams[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            oParams[0].Value = 1;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = 1;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;
            oParams[3] = new SqlParameter("ElementTypeId", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;
            oParams[4] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[4].Value = eltTag;
            oParams[5] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 1024);
            oParams[5].Value = eltDescription;
            oParams[6] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[6].Value = active;



            //Execute Procedure With Parameters
            //Fill DataSet
            DataSet _ds = dal.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_Put]", CommandType.StoredProcedure, oParams);

            GridView1.DataBind();


        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            var eltName = EltNameTextbox.Text;
            var eltTypeIdString = DropDownList1.SelectedValue.ToString();
            var eltTag = EltTagTextbox.Text;
            var eltDesc = EltDescTextArea.InnerText;

            try
            {
                int eltTypeId = Convert.ToInt32(eltTypeIdString);


                AddHeaderElement(eltName, eltTypeId, eltTag, eltDesc, 1);
            }
            catch (Exception Ex)
            {
                ErrorPanel.Visible = true;
                ErrorTextBox.Text = string.Format(" Failed to save the entries to the database. Error  message :{0}", Ex.Message);
            }
            finally
            {
                MainPanel.BackColor = MainPanelColor;
                GridView1.Visible = true;

                AddButton.Visible = true;
                ElementListLabel.Visible = true;
                GridView1.DataBind();
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var temp = GridView1.SelectedRow.DataItem;
        }

        void UpdateHeaderElement(int deid, int uid, int doid, string eltName, int eltTypeId, string eltTag, string eltDescription, int active)
        {
            string ConnString = @"Server=146.20.79.16,1966; User Id=gsadev; Password=neBUla6853#1";


            CSharpDAL dal = new CSharpDAL(ConnString);



            //Create Parameters
            SqlParameter[] oParams = new SqlParameter[8];
            oParams[0] = new SqlParameter("UID", SqlDbType.Int, 10);
            oParams[0].Value = uid;
            oParams[1] = new SqlParameter("DOID", SqlDbType.Int, 10);
            oParams[1].Value = doid;
            oParams[2] = new SqlParameter("ElementName", SqlDbType.NVarChar, 255);
            oParams[2].Value = eltName;
            oParams[3] = new SqlParameter("ElementTypeId", SqlDbType.Int, 10);
            oParams[3].Value = eltTypeId;
            oParams[4] = new SqlParameter("ElementTag", SqlDbType.NVarChar, 255);
            oParams[4].Value = eltTag;
            oParams[5] = new SqlParameter("ElementDesc", SqlDbType.NVarChar, 1024);
            oParams[5].Value = eltDescription;
            oParams[6] = new SqlParameter("Active", SqlDbType.Int, 10);
            oParams[6].Value = active;
            oParams[7] = new SqlParameter("DEID", SqlDbType.Int, 10);
            oParams[7].Value = deid;



            //Execute Procedure With Parameters
            //Fill DataSet
            DataSet _ds = dal.FillDataset("[OWT_DEV].[dbo].[OWT_DocHeader_Put]", CommandType.StoredProcedure, oParams);

        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                var rows = GridView1.Rows;
                var colmns = GridView1.Columns;
                var es = rows[0];
                var deids = colmns[0];
                var keys = e.Keys;
                var oldVal = e.OldValues;
                var newVal = e.NewValues[0];
                var deidOb = int.Parse(e.OldValues[0].ToString());
                var doid = int.Parse(e.NewValues[0].ToString());
                var elementTypeID = int.Parse(e.NewValues[1].ToString());
                var elementName = (string)e.NewValues[2];
                var elementTag = (string)e.NewValues[3];
                var elementDesc = (string)e.NewValues[4];
                var active = int.Parse(e.NewValues[5].ToString());
                var uid = int.Parse(e.NewValues[7].ToString());

                UpdateHeaderElement((int)deidOb, uid, doid, elementName, elementTypeID, elementTag, elementDesc, active);
                GridView1.DataBind();
            }
            catch (Exception Ex)
            {
                MainPanel.BackColor = MainPanelColor;
                GridView1.Visible = true;
                ErrorPanel.Visible = true;
                ErrorTextBox.Text = string.Format("Update Failed. Error message:{0}", Ex.Message);
                GridView1.DataBind();
            }

        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            GridView1.Visible = false;
            // Panel1.Height = 200;
            MainPanelColor = MainPanel.BackColor;
            MainPanel.BackColor = Color.White;
            AddButton.Visible = false;
            ElementListLabel.Visible = false;

        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            RemovePanel.Visible = true;
            

        }

       

        protected void YesButton_Click(object sender, EventArgs e)
        {
            var index = GridView1.SelectedIndex;
            var selectedRowCells = GridView1.Rows[index].Cells;
            try
            {
                var deid = int.Parse(selectedRowCells[2].Text);
                var doid = int.Parse(selectedRowCells[3].Text);
                var elementTypeID = int.Parse( selectedRowCells[4].Text);
                var elementName = selectedRowCells[5].Text;
                var  elementTag = selectedRowCells[6].Text;
                var  elementDesc = selectedRowCells[7].Text;
                var active = 0;
                var uid = int.Parse(selectedRowCells[12].Text);


                UpdateHeaderElement(deid, uid, doid, elementName, elementTypeID, elementTag, elementDesc, active);
                GridView1.SelectedIndex = -1;
                GridView1.DataBind();
            }
            catch(Exception Ex)
            {
                ErrorPanel.Visible = true;
                ErrorTextBox.Text = string.Format("Deletion Failed. Error message:{0}", Ex.Message);
            }
        }

        protected void NoButton_Click(object sender, EventArgs e)
        {
            GridView1.SelectedIndex = -1;
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            MainPanel.BackColor = MainPanelColor;
            GridView1.Visible = true;

            AddButton.Visible = true;
            ElementListLabel.Visible = true;
            GridView1.DataBind();
        }
    }


    public class ElementHeader
    {
        public string Name { get; set; }
        public ElementTypeId TypeId { get; set; }
        public string Tag { get; set; }
        public string Desc { get; set; }

        public int Active { get; set; }

        public string Detail { get; set; }

        public ElementHeader( string name, ElementTypeId typeId, string tag, string desc, string detail, int active)
        {
            Name = name;
            TypeId = typeId;
            Tag = tag;
            Desc = desc;
            Active = active;
            Detail = detail;
        }

    }

    public enum ElementTypeId
    {
        String =1,
        Int = 2,
        XML = 5,

    }
}