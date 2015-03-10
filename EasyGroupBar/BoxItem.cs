using System;

namespace EasyGroupBar
{
    /// <summary>
    /// 分类下项
    /// </summary>
    public class BoxItem
    {
        public string Name { get; set; }
        public object Tag { get; set; }
    }

    /// <summary>
    /// 点击分类下的项目事件数据
    /// </summary>
    public class ItemClickEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public object Tag { get; private set; }
        public ItemClickEventArgs(string name, object tag)
        {
            Name = name;
            Tag = tag;
        }
    }
}
