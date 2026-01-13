using System;

namespace iTunesSyncer.DataDiffListUtility
{
    public class DataDiffListItem<T>
    {
        public T Item1 { get; set; }
        public T Item2 { get; set; }

        public T NotNullItem
        {
            get
            {
                if (Item1 is not null)
                    return Item1;
                if (Item2 is not null)
                    return Item2;

                throw new NullReferenceException();
            }
        }

        public DataDiffListItem(T item1, T item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public DataDiffListItem(DataDiffListItem<T> origin)
        {
            Item1 = origin.Item1;
            Item2 = origin.Item2;
        }
    }
}
