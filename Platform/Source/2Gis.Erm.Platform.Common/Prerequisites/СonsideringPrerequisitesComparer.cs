using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Common.Prerequisites
{
    public sealed class ÑonsideringPrerequisitesComparer<TItem> : IComparer<TItem>
        where TItem : class
    {
        public int Compare(TItem x, TItem y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return -1;
            }

            var xType = x as Type ?? x.GetType();
            var yType = y as Type ?? y.GetType();

            var prerequsites = (PrerequisitesAttribute[])xType.GetCustomAttributes(typeof(PrerequisitesAttribute), false);
            return prerequsites.Any(t => t.Prerequisites.Contains(yType)) ? 1 : 0;
        }
    }
}