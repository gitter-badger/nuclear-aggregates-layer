using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Bills
{
    public static class BillSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Bill> ByNumbers(IEnumerable<string> numbers)
            {
                return new FindSpecification<Bill>(x => numbers.Contains(x.Number));
            }
        }
    }
}
