using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zxcvbn
{
    static class LinqExtensions
    {
        /// <summary>
        /// Used to group elements by a key function, but only where elements are adjacent
        /// </summary>
        public static IEnumerable<AdjacentGrouping<TKey, TSource>> GroupAdjacent<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var prevKey = default(TKey);
            var prevStartIndex = 0;
            var prevInit = false;
            var itemsList = new List<TSource>();

            var i = 0;
            foreach (var item in source)
            {
                var key = keySelector(item);
                if (prevInit)
                {
                    if (!prevKey.Equals(key))
                    {
                        yield return new AdjacentGrouping<TKey, TSource>(key, itemsList, prevStartIndex, i - 1);

                        prevKey = key;
                        itemsList = new List<TSource>();
                        itemsList.Add(item);
                        prevStartIndex = i;
                    }
                    else
                    {
                        itemsList.Add(item);
                    }
                }
                else
                {
                    prevKey = key;
                    itemsList.Add(item);
                    prevInit = true;
                }

                i++;
            }

            if (prevInit) yield return new AdjacentGrouping<TKey, TSource>(prevKey, itemsList, prevStartIndex, i - 1); ;
        }

        /// <summary>
        /// A single grouping from the GroupAdjacent function, includes start and end indexes for the grouping in addition to standard IGrouping bits
        /// </summary>
        public class AdjacentGrouping<TKey, TElement> :  IGrouping<TKey, TElement>, IEnumerable<TElement>
        {
            public TKey Key
            {
                get;
                private set;
            }

            public int StartIndex
            {
                get;
                private set;
            }

            public int EndIndex
            {
                get;
                private set;
            }

            private IEnumerable<TElement> m_groupItems;

            public AdjacentGrouping(TKey key, IEnumerable<TElement> groupItems, int startIndex, int endIndex)
            {
                this.Key = key;
                this.StartIndex = startIndex;
                this.EndIndex = endIndex;
                m_groupItems = groupItems;
            }

            IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
            {
                return m_groupItems.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return m_groupItems.GetEnumerator();
            }
        }
    }
}
