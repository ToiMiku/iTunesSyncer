using System;
using System.Collections.Generic;
using System.Linq;

namespace iTunesSyncer.DataDiffListUtility
{
    public class DataDiffListGenerator<T>
    {
        public static List<DataDiffListItem<T>> GenerateWithoutOrder(IList<T> list1, IList<T> list2, Comparison<T> comparison)
        {
            var resultList = new List<DataDiffListItem<T>>(Math.Max(list1.Count, list2.Count));

            var workList2 = new List<T>(list2);

            foreach (var item1 in list1)
            {
                var item2 = workList2.FirstOrDefault(x => comparison(item1, x) == 0);
                if (item2 is not null)
                {
                    resultList.Add(new DataDiffListItem<T>(item1, item2));
                    workList2.Remove(item2);
                }
                else
                {
                    resultList.Add(new DataDiffListItem<T>(item1, default));
                }
            }

            foreach (var item2 in workList2)
            {
                resultList.Add(new DataDiffListItem<T>(default, item2));
            }

            return resultList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static List<DataDiffListItem<T>> Generate(IList<T> list1, IList<T> list2, Comparison<T> comparison)
        {
            var LCSAlgorithm = new DynamicProgramming<T>();
            var lcaList = LCSAlgorithm.Execute(list1, list2, comparison);

            var resultList = ShortestEditScript(lcaList, list1, list2);

            return resultList;
        }

        private static List<DataDiffListItem<T>> ShortestEditScript(
            List<DataDiffListItem<T>> lcsList, IList<T> list1, IList<T> list2)
        {
            var resultList = new List<DataDiffListItem<T>>(Math.Max(list1.Count, list2.Count));

            var indexLCS = 0;
            var index1 = 0;
            var index2 = 0;

            while (lcsList.Count > indexLCS && list1.Count > index1 && list2.Count > index2)
            {
                var dataDiff = lcsList[indexLCS];
                var item1 = list1[index1];
                var item2 = list2[index2];

                if (ReferenceEquals(dataDiff.Item1, item1) && ReferenceEquals(dataDiff.Item2, item2))
                {
                    resultList.Add(dataDiff);

                    indexLCS++;
                    index1++;
                    index2++;
                }
                else if (!ReferenceEquals(dataDiff.Item1, item1))
                {
                    resultList.Add(new DataDiffListItem<T>(item1, default));

                    index1++;
                }
                else// if (!ReferenceEquals(dataDiff.Item2, item2))
                {
                    resultList.Add(new DataDiffListItem<T>(default, item2));

                    index2++;
                }
            }

            if (lcsList.Count != indexLCS)
                throw new Exception("LCSを追加しきれていない！");

            // 追加しきれていない分を追加
            while (list1.Count > index1)
            {
                var item1 = list1[index1];
                resultList.Add(new DataDiffListItem<T>(item1, default));
                index1++;
            }
            while (list2.Count > index2)
            {
                var item2 = list2[index2];
                resultList.Add(new DataDiffListItem<T>(default, item2));
                index2++;
            }

            return resultList;
        }
    }
}
