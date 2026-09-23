using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class dialogHelp : Form
    {
        GlobalVars gv;
        public dialogHelp()
        {
            InitializeComponent();
        }
        public dialogHelp(GlobalVars g, string WindowTitle)
        {
            InitializeComponent();
            gv = g;
            

        }
    
        private void CreateLvInitParms(GlobalVars gv)
        {
    
            lvHelp.View = View.Details;
            lvHelp.Scrollable = true;
            // Allow the user to edit item text.
            lvHelp.LabelEdit = false;
            // Allow the user to rearrange columns.
            lvHelp.AllowColumnReorder = true;
            // Display check boxes.
            lvHelp.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            lvHelp.FullRowSelect = true;
            // Display grid lines.
            lvHelp.GridLines = true;
            // Sort the items in the list in ascending order.
            lvHelp.Sorting = SortOrder.None;

            Rectangle wndBounds = this.Bounds;

           // listViewLoc = lvInitParms.Location;
            Boolean test = true;
            // Create columns for the items and subitems.
            lvHelp.Columns.Add("parm", 200, HorizontalAlignment.Center);
            lvHelp.Columns.Add("value", 1000, HorizontalAlignment.Left);

            lvHelp.Columns[0].Tag = "String";
            lvHelp.Columns[1].Tag = "String";
        }
        public Boolean insertRowLvParms(string pname, string pvalue)
        {
            if (pname == null)
                pname = "null";
            if (pvalue == null)
                pvalue = "null";
            //-- NEW ITEM
            ListViewItem item1 = new ListViewItem(pname, 0);
            // Place a check mark next to the item.
            //-- SUB-ITEMS
            item1.SubItems.Add(pvalue);            //counter

            //++rowCount;
            lvHelp.Items.Add(item1);
            return true;
        }
     
    }
}
