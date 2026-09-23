using System.Collections;
using System.Windows.Forms;

namespace SymbolDB
{
    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    ///
    //---- MUST TAG THE LV COLUMNS as to type of SORT
    // listviewX.ListView.Columns[ColumnToSort].Tag = "Text";
    //  "Integer"
    // "Numeric"
    // "Text" ,,,, default
    //
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        public CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            if (listviewX.ListView.Columns[ColumnToSort].Tag == null)
            {
                listviewX.ListView.Columns[ColumnToSort].Tag = "Text";
            }

            if (listviewX.ListView.Columns[ColumnToSort].Tag.ToString() == "Integer")
            {
                int int1 = int.Parse(listviewX.SubItems[ColumnToSort].Text);
                int int2 = int.Parse(listviewY.SubItems[ColumnToSort].Text);

                if (Order == SortOrder.Ascending)
                {
                    return int1.CompareTo(int2);
                }
                else
                {
                    return int2.CompareTo(int1);
                }
            } else if (listviewX.ListView.Columns[ColumnToSort].Tag.ToString() == "Numeric")
            {
                float fl1 = float.Parse(listviewX.SubItems[ColumnToSort].Text);
                float fl2 = float.Parse(listviewY.SubItems[ColumnToSort].Text);

                if (Order == SortOrder.Ascending)
                {
                    return fl1.CompareTo(fl2);
                }
                else
                {
                    return fl2.CompareTo(fl1);
                }
            }
            else
            {
                // Compare the two items
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            }
           


            // Calculate correct return value based on object comparison

            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }
}