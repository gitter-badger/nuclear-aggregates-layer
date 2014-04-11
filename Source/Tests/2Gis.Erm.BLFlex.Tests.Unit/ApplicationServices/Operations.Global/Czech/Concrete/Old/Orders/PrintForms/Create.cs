using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Moq;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    static class Create
    {
        public static IFinder FinderForPrintForms()
        {
            var order = new Order
                {
                    LegalPerson = new LegalPerson { LegalPersonProfiles = new[] { new LegalPersonProfile { Id = 0 } } },
                    BranchOfficeOrganizationUnit = new BranchOfficeOrganizationUnit { BranchOffice = new BranchOffice() },
                };

            var finder = Mock.Of<IFinder>();
            Mock.Get(finder)
                .Setup(f => f.Find(Moq.It.IsAny<IFindSpecification<Order>>()))
                .Returns(new[] { order }.AsQueryable());

            return finder;
        }
    }
}