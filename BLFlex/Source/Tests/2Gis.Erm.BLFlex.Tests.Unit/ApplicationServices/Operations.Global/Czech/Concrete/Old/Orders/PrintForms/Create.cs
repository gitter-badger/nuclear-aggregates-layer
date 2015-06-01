using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Moq;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    static class Create
    {
        const long LegalPersonId = 1;
        const long LegalPersonProfileId = 2;

        public static IFinder FinderForPrintForms()
        {
            var legalPersonProfile = new LegalPersonProfile { Id = LegalPersonProfileId };
            var legalPerson = new LegalPerson { Id = LegalPersonId, LegalPersonProfiles = new[] { legalPersonProfile } };
            var branchOffice = new BranchOffice();
            var branchOfficeOrganizationUnit = new BranchOfficeOrganizationUnit { BranchOffice = branchOffice };
            var order = new Order
                {
                    LegalPerson = legalPerson,
                    LegalPersonId = legalPerson.Id,
                    LegalPersonProfileId = legalPersonProfile.Id,
                    BranchOfficeOrganizationUnit = branchOfficeOrganizationUnit,
                };

            var finder = Mock.Of<IFinder>();

            Mock.Get(finder)
                .Setup(f => f.Find(It.IsAny<FindSpecification<Order>>()))
                .Returns(new QueryableSequence<Order>(new[] { order }.AsQueryable()));

            Mock.Get(finder)
                .Setup(f => f.Find(It.IsAny<FindSpecification<BranchOfficeOrganizationUnit>>()).One())
                .Returns(branchOfficeOrganizationUnit);

            Mock.Get(finder)
                .Setup(f => f.Find(It.IsAny<FindSpecification<LegalPerson>>()).One())
                .Returns(legalPerson);

            Mock.Get(finder)
                .Setup(f => f.Find(It.IsAny<FindSpecification<LegalPersonProfile>>()).One())
                .Returns(legalPersonProfile);

            Mock.Get(finder)
                .Setup(f => f.Find(It.IsAny<FindSpecification<BranchOffice>>()).One())
                .Returns(branchOffice);

            return finder;
        }
    }
}