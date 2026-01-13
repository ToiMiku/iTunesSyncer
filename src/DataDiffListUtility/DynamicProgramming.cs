using System;
using System.Collections.Generic;

namespace iTunesSyncer.DataDiffListUtility
{
    public class DynamicProgramming<T> : LCSAlgorithm<T>
    {

        public List<DataDiffListItem<T>> Execute(IList<T> list1, IList<T> list2, Comparison<T> comparison)
        {
            int index1, index2;

            // LCSを計算するための動的計画法
            var dp = new DPArray(list1.Count, list2.Count);
            for (index1 = 0; index1 < list1.Count; index1++)
            {
                for (index2 = 0; index2 < list2.Count; index2++)
                {
                    if (comparison(list1[index1], list2[index2]) == 0)
                    {
                        dp[index1, index2] = dp[index1 - 1, index2 - 1] + 1;
                    }
                    else
                    {
                        dp[index1, index2] = Math.Max(dp[index1 - 1, index2], dp[index1, index2 - 1]);
                    }
                }
            }

            // LCSの復元
            var resultList = new List<DataDiffListItem<T>>(dp[list1.Count - 1, list2.Count - 1]);
            index1 = list1.Count - 1;
            index2 = list2.Count - 1;
            while (index1 >= 0 && index2 >= 0)
            {
                if (comparison(list1[index1], list2[index2]) == 0)
                {
                    resultList.Insert(0, new DataDiffListItem<T>(list1[index1], list2[index2]));
                    index1--;
                    index2--;
                }
                else if (dp[index1 - 1, index2] >= dp[index1, index2 - 1])
                {
                    index1--;
                }
                else
                {
                    index2--;
                }
            }

            return resultList;
        }

        private class DPArray
        {
            private int[,] _dp;

            public DPArray(int xNum, int yNum)
            {
                _dp = new int[xNum, yNum];
            }

            public int this[int x, int y]
            {
                get
                {
                    if (x < -1 || y < -1)
                        throw new IndexOutOfRangeException();

                    if (x == -1 || y == -1)
                        return 0;

                    return _dp[x, y];
                }
                set
                {
                    _dp[x, y] = value;
                }
            }
        }
    }
}
