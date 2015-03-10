using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EasyGroupBar;

namespace TestProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBar1.AddBarCategory(new BarCategory("文件"), new BoxItem[] {
                                                                                new BoxItem(){ Name="新建",Tag="New"},
                                                                                new BoxItem(){ Name="打开",Tag="New"},
                                                                                new BoxItem(){ Name="添加",Tag="New"},
                                                                                new BoxItem(){ Name="保存",Tag="New"},
                                                                                new BoxItem(){ Name="全部保存",Tag="New"},
                                                                                new BoxItem(){ Name="关闭",Tag="New"}
                                                                            });

            groupBar1.AddBarCategory(new BarCategory("编辑"), new BoxItem[] {
                                                                                new BoxItem(){ Name="aaa",Tag="New"},
                                                                                new BoxItem(){ Name="bbb",Tag="New"},
                                                                                new BoxItem(){ Name="ccc",Tag="New"},
                                                                                new BoxItem(){ Name="ddd",Tag="New"},
                                                                                new BoxItem(){ Name="eeeee",Tag="New"},
                                                                                new BoxItem(){ Name="fff",Tag="New"}
                                                                            });

            groupBar1.AddBarCategory(new BarCategory("视图"), new BoxItem[] {
                                                                                new BoxItem(){ Name="qqq",Tag="New"},
                                                                                new BoxItem(){ Name="wwww",Tag="New"},
                                                                                new BoxItem(){ Name="eeee",Tag="New"},
                                                                                new BoxItem(){ Name="rrrr",Tag="New"},
                                                                                new BoxItem(){ Name="tttt",Tag="New"},
                                                                                new BoxItem(){ Name="yyyy",Tag="New"}
                                                                            });

            groupBar1.AddBarCategory(new BarCategory("项目"), new BoxItem[] {
                                                                                new BoxItem(){ Name="aaaa",Tag="New"},
                                                                                new BoxItem(){ Name="ssss",Tag="New"},
                                                                                new BoxItem(){ Name="dddd",Tag="New"},
                                                                                new BoxItem(){ Name="ffff",Tag="New"},
                                                                                new BoxItem(){ Name="ggggg",Tag="New"},
                                                                                new BoxItem(){ Name="hhhh",Tag="New"}
                                                                            });

            groupBar1.ItemClick += new ItemClickEventHandler(groupBar1_ItemClick);
        }


        void groupBar1_ItemClick(object sender, ItemClickEventArgs e)
        {
            textBox1.Text = e.Name;
        }
    }
}
