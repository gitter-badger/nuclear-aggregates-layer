using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Prerequisites;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class IntegrationTestsComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return -1;
            }

            var prerequsites = (PrerequisitesAttribute[])x.GetCustomAttributes(typeof(PrerequisitesAttribute), false);
            return prerequsites.Any(t => t.Prerequisites.Contains(y)) ? 1 : 0;
        }
    }
}