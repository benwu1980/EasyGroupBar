using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EasyGroupBar
{
    /// <summary>
    /// 表示分类的控件
    /// </summary>
    [ToolboxItem(false)]
    public class BarCategory : Panel
    {
        public event ItemClickEventHandler ItemClick;

        /// <summary>
        /// Bar标题在父控件上的左上角位置
        /// </summary>
        public Point LeftTop { get; set; }

        /// <summary>
        /// Bar标题在父控件上的右下角位置
        /// </summary>
        public Point RightBottom { get; set; }

        private ItemList itemList;
        public BarCategory()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.FixedHeight, false);
            SetStyle(ControlStyles.FixedWidth, false);

            itemList = new ItemList();
            itemList.Dock = DockStyle.Fill;
            itemList.Width = 0;

            itemList.ItemClick += new ItemClickEventHandler(itemList_ItemClick);

            this.Controls.Add(itemList);
        }

        void itemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(this, e);
            }
        }

        public BarCategory(string text)
            : this()
        {
            Text = text;
        }

        public void AddItems(BoxItem[] item)
        {
            itemList.AddItems(item);
        }

        internal void ReleaseControl()
        {
            if (itemList != null)
            {
                itemList.Visible = false;
                itemList = null;
            }
        }

        /// <summary>
        /// Sets the focus to the control
        /// </summary>
        internal void SetControlFocus()
        {
            if (itemList != null)
                itemList.Focus();
        }

        internal bool HitTest(int x, int y)
        {
            if (x >= LeftTop.X && x <= RightBottom.X &&
                y >= LeftTop.Y && y <= RightBottom.Y)
            {
                return true;
            }
            return false;
        }

    }
}
