using System;
using System.Collections.Generic;

namespace EasyGroupBar
{
    /// <summary>
    /// 分类集合
    /// </summary>
    public class BarCategoryCollection : IEnumerable<BarCategory>
    {
        public event EventHandler<ChangeEventArgs> Removed;
        public event EventHandler<ChangeEventArgs> Added;
        public event EventHandler Clearing;

        public List<BarCategory> innerList = new List<BarCategory>();

        public void Add(BarCategory item)
        {
            innerList.Add(item);
            if (Added != null)
            {
                Added(this, new ChangeEventArgs(innerList.Count - 1, item));
            }
        }

        public void Clear()
        {
            innerList.Clear();
            if (Clearing != null)
            {
                Clearing(this, EventArgs.Empty);
            }
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public void Remove(BarCategory item)
        {
            int index = innerList.IndexOf(item);
            if (index >= 0)
            {
                innerList.RemoveAt(index);
                if (Removed != null)
                {
                    Removed(this, new ChangeEventArgs(index, item));
                }
            }
        }

        public IEnumerator<BarCategory> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public BarCategory this[int index]
        {
            get { return (innerList[index] as BarCategory); }
        }

        internal int IndexOf(BarCategory barPage)
        {
            return innerList.IndexOf(barPage);
        }
    }

    public class ChangeEventArgs : EventArgs
    {
        public int Index { get; private set; }
        public BarCategory Item { get; private set; }
        public ChangeEventArgs(int index, BarCategory item)
        {
            this.Index = index;
            this.Item = item;
        }
    }

}
