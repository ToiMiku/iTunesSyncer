using System;
using System.Collections.Generic;

namespace iTunesSyncer.DataDiffListUtility
{
    public interface LCSAlgorithm<T>
    {
        public List<DataDiffListItem<T>> Execute(IList<T> list1, IList<T> list2, Comparison<T> comparison);
    }
}
