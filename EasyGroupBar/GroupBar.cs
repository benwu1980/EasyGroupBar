using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EasyGroupBar
{
    [ToolboxBitmap(typeof(GroupBar), "Resources.GroupPanel.bmp")] //Resources表示目录,其中GroupPanel.bmp是"嵌入式资源"
    public partial class GroupBar : UserControl
    {
        public event ItemClickEventHandler ItemClick;

        // 定义一些颜色
        private Color _backIDE;
        private Color _backColor;
        private Color _backDark;
        private Color _backDarkDark;
        private Color _backLight;
        private Color _backLightLight;

        private BarCategoryCollection _barCategories;

        #region Properties

        // Mouse control
        private int _pressedIndex = -1;					// Index of the presses panel (but not shown)
        private int _pressedIndexRightButton = -1;		// Index of the presses panel (but not shown) for the right button
        private bool _mouseOnButton = false;			// Indicates if the mouse is over the pressed panel


        private int _lastSelectIndex = -1; // 最后显示Bar索引编号

        private bool _mouseButtonJustReleased = false;	// 是否阻上mousemove事件
        private int _hotTrackIndex = -1;				//跟踪鼠标移动到的bar索引编号

        private bool _selectionJustChanged = false;		// Used to set the focus to the control just after the panel is changed with the mouse


        private bool _bitmapHotSpot = true; // 是否要显示 上或下的箭对
        private Bitmap _downBitmap = null;  //bar 上面的下移图标
        private Bitmap _upBitmap = null;    //bar 上面的上移图标

        private int _bitmapX0;
        private int _bitmapX1;
        private int _bitmapY0;

        private Color _transparentColor = Color.Magenta; //图片透明的颜色
        private int _selectedIndex = 0;
        private int _tabHeight = 20;
        private Color _colorLeft;
        private Color _colorRight;

        private int _labelMaxLenght = 32767;
        private ImageList _imageList;

        private Color _hotTrackColor = SystemColors.HotTrack;


        private bool _hotTrack = false;
        /// <summary>
        /// 鼠标移到bar上是否需要变颜色
        /// </summary>
        public bool HotTrack
        {
            get { return _hotTrack; }
            set { _hotTrack = value; }
        }
        #endregion

        public GroupBar()
        {
            _barCategories = new BarCategoryCollection();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            _colorLeft = Color.FromArgb(50, 0, 153, 204);
            _colorRight = SystemColors.ControlLight;

            _backColor = this.BackColor;
            _backLight = ControlPaint.Light(_backColor);
            _backLightLight = ControlPaint.LightLight(_backColor);
            _backDark = ControlPaint.Dark(_backColor);
            _backDarkDark = ControlPaint.DarkDark(_backColor);

            BorderStyle = BorderStyle.FixedSingle;

            try
            {
                System.Reflection.Assembly assembly = this.GetType().Assembly;
                using (var imgStream = assembly.GetManifestResourceStream("EasyGroupBar.Resources.up.bmp"))
                {
                    _upBitmap = (Bitmap)Image.FromStream(imgStream);
                }


                using (var downImgStream = assembly.GetManifestResourceStream("EasyGroupBar.Resources.down.bmp"))
                {
                    _downBitmap = (Bitmap)Image.FromStream(downImgStream);
                }

                _upBitmap.MakeTransparent(_transparentColor);
                _downBitmap.MakeTransparent(_transparentColor);
            }
            catch (Exception)
            {

            }
        }

        public void AddBarCategory(BarCategory bar, BoxItem[] items)
        {
            bar.Visible = false;
            this.Controls.Add(bar);

            bar.AddItems(items);
            _barCategories.Add(bar);

            bar.ItemClick += new ItemClickEventHandler(bar_ItemClick);
        }

        void bar_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(this, e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;

                Brush brushText = null;
                Brush brushLight = new SolidBrush(_backLight);
                Brush brushDark = new SolidBrush(_backDark);
                Pen penLight = new Pen(brushLight, 1);
                Pen penDark = new Pen(brushDark, 1);

                // 测试文本尺寸
                int textHeight = (int)g.MeasureString("bg", this.Font).Height;
                int textOffset = (_tabHeight - textHeight) / 2;

                int hOffset;
                int vOffset;
                if (this.BorderStyle == BorderStyle.None)
                {
                    hOffset = -1;
                    vOffset = -1;
                }
                else if (this.BorderStyle == BorderStyle.FixedSingle)
                {
                    hOffset = 0;
                    vOffset = 0;
                }
                else
                {
                    // ControlPaint.DrawBorder3D(g, 0, 0, this.Width - 1, this.Height - 1, Border3DStyle.Raised);

                    hOffset = 1;
                    vOffset = 1;
                }

                int index = -1;
                int top = vOffset + 1;
                int oldTop;

                //要显示的Bar Items 控件的位置
                int panelTop = 0;
                int panelBottom = 0;

                int fitWidth;
                int textX2;

                if (_bitmapHotSpot)
                    textX2 = this.Width - hOffset - 2 - _downBitmap.Width * 2;
                else
                    textX2 = this.Width - hOffset - 2;


                foreach (var barPage in _barCategories)
                {
                    index++;

                    ///背景
                    Rectangle rect = new Rectangle(hOffset + 1, top, this.Width - hOffset * 2 - 1, _tabHeight);
                    using (var brush = new LinearGradientBrush(rect, _colorLeft, _colorRight, 0, false))
                    {
                        g.FillRectangle(brush, rect);
                    }

                    //计算此bar的rect
                    barPage.LeftTop = new Point(hOffset + 1, top);
                    barPage.RightBottom = new Point(this.Width - hOffset - 2, top + _tabHeight - 1);

                    //写文字
                    if (_hotTrack)
                    {
                        brushText = _hotTrackIndex == index
                            ? new SolidBrush(_hotTrackColor)
                            : new SolidBrush(this.ForeColor);
                    }
                    else
                    {
                        brushText = new SolidBrush(this.ForeColor);
                    }

                    int textX = 0;
                    fitWidth = (int)g.MeasureString(barPage.Text, this.Font).Width;
                    if (fitWidth > textX2 - textX + 8)
                        g.DrawString(TrimText(barPage.Text, textX2 - textX - 5, g, this.Font), this.Font, brushText, textX, top + textOffset);
                    else
                        g.DrawString(barPage.Text, this.Font, brushText, textX, top + textOffset);


                    if (_bitmapHotSpot) //计算指示图片位置
                    {
                        _bitmapX0 = this.Width - _downBitmap.Width - 8;
                        _bitmapX1 = _bitmapX0 + _downBitmap.Width;
                        _bitmapY0 = (_tabHeight - _downBitmap.Height) / 2 + top;
                    }

                    oldTop = top;
                    if (index == _selectedIndex)
                    {
                        panelTop = top + _tabHeight;
                        top = (top + _tabHeight - 1) + (this.Height - vOffset * 2) - (index + 1) * _tabHeight - (_barCategories.Count - 1 - index) * _tabHeight - 1;
                        panelBottom = top;

                        // 检查垂直空间
                        if (top < oldTop + _tabHeight * 2)
                        {
                            panelBottom = this.Height - vOffset;
                            break;
                        }
                    }
                    else
                    {
                        // 画上图片
                        if (_bitmapHotSpot)
                        {
                            if (index < _selectedIndex)
                                g.DrawImageUnscaled(_downBitmap, _bitmapX0, _bitmapY0);
                            else
                                g.DrawImageUnscaled(_upBitmap, _bitmapX0, _bitmapY0);
                        }
                        top += _tabHeight;

                    }	// if (index == _selectedIndex)
                }

                try	 //防止设计时出错
                {
                    if (_lastSelectIndex != _selectedIndex)
                    {
                        _lastSelectIndex = _selectedIndex;
                        foreach (var tabPage in _barCategories)
                            tabPage.Visible = false;
                        _barCategories[_selectedIndex].Left = hOffset + 1;
                        _barCategories[_selectedIndex].Width = this.Width - hOffset - 2;
                        _barCategories[_selectedIndex].Top = panelTop;
                        _barCategories[_selectedIndex].Height = panelBottom - panelTop;
                        _barCategories[_selectedIndex].Visible = true;
                        _barCategories[_selectedIndex].TabIndex = 100;
                    }
                    if (_selectionJustChanged)
                    {
                        _selectionJustChanged = false;
                        _barCategories[_selectedIndex].SetControlFocus();
                    }
                }
                catch { }
            }
            catch (Exception)
            {

            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _lastSelectIndex = -1;
            Invalidate();
        }


        #region 鼠标事件

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var barPage in _barCategories)
            {
                if (barPage.HitTest(e.X, e.Y))
                {
                    _pressedIndexRightButton = _barCategories.IndexOf(barPage);

                    if (e.Button == MouseButtons.Right)
                    {
                        _mouseOnButton = true;
                        Invalidate();
                    }
                    else
                    {
                        if (_selectedIndex != _pressedIndexRightButton)
                        {
                            _pressedIndex = _pressedIndexRightButton;
                            _mouseOnButton = true;
                            Invalidate();
                        }
                    }
                    break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_mouseButtonJustReleased)
            {
                _mouseButtonJustReleased = false;
                return;
            }

            bool found = false;
            bool invalidate = false;

            // Detect if the mouse is over a tabPage
            _hotTrackIndex = -1;
            foreach (BarCategory tabPage in _barCategories)
            {
                if (tabPage.HitTest(e.X, e.Y))
                {
                    _hotTrackIndex = _barCategories.IndexOf(tabPage);
                    invalidate = true;
                    if (_hotTrackIndex != _selectedIndex)
                        found = true;
                    break;
                }
            }
            if (found)
                this.Cursor = Cursors.Hand;
            else
                this.Cursor = Cursors.Default;

            if (_pressedIndex != -1)
            {
                bool onButton = _barCategories[_pressedIndex].HitTest(e.X, e.Y);
                if (_mouseOnButton != onButton)
                {
                    _mouseOnButton = onButton;
                    invalidate = true;
                }
            }

            // Refresh the control
            if (invalidate)
                Invalidate();
        }



        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            bool hitTest;

            BarCategory tabPageMouseUp = null;

            // Get the tabPage at the mouse coordinates
            _mouseButtonJustReleased = true;
            foreach (BarCategory tabPage in _barCategories)
            {
                if (e.Button == MouseButtons.Right)
                    hitTest = tabPage.HitTest(e.X, e.Y);
                else
                    hitTest = tabPage.HitTest(e.X, e.Y);
                if (hitTest)
                {
                    if (_pressedIndexRightButton == _barCategories.IndexOf(tabPage))
                        tabPageMouseUp = tabPage;

                    if (_pressedIndex == _barCategories.IndexOf(tabPage))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            _selectionJustChanged = true;
                            _selectedIndex = _pressedIndex;
                        }

                        break;
                    }
                }
            }

            if (_selectedIndex == _pressedIndex)
            {
                _barCategories[_selectedIndex].SetControlFocus();
            }

            // Refresh the control
            _pressedIndex = -1;
            _pressedIndexRightButton = -1;
            _mouseOnButton = false;
            Invalidate();
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            // Reset the cursor
            this.Cursor = Cursors.Default;

            // Reset the hot track index
            _hotTrackIndex = -1;

            // Refresh the control
            Invalidate();
        }

        #endregion

        /// <summary>
        /// 截取文字长度,并在尾部加上...
        /// </summary>
        /// <param name="text">要载取的文字</param>
        /// <param name="width"></param>
        /// <param name="g"></param>
        /// <param name="font">字体</param>
        /// <returns></returns>
        private string TrimText(string text, int width, Graphics g, Font font)
        {
            int lenght = text.Length;
            try
            {
                while ((int)g.MeasureString(text, font).Width > width)
                {
                    lenght--;
                    text = text.Substring(0, lenght);
                }
                return text + "...";
            }
            catch
            {
                return "...";
            }
        }
    }
}
