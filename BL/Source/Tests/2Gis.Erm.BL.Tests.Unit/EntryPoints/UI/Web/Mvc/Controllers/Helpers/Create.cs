using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Moq;

namespace DoubleGis.Erm.BL.Tests.Unit.EntryPoints.UI.Web.Mvc.Controllers.Helpers
{
    static class Create
    {
        const long OrderId = 1;
        const long LegalPersonId = 2;
        const long LegalPersonProfileId1 = 3;
        const long LegalPersonProfileId2 = 3;
        const long InvalidLegalPersonProfileId = 666;

        public static Order Order(bool isProfileSpecified, int profileCount)
        {
            var profiles = new [] { LegalPersonProfileId1, LegalPersonProfileId2 }
                .Take(profileCount)
                .Select(LegalPersonProfile)
                .ToArray();

            var profileId = profiles.Any() ? profiles.First().Id : InvalidLegalPersonProfileId;

            return new Order
                {
                    Id = OrderId,
                    LegalPersonId = LegalPersonId,
                    LegalPerson = new LegalPerson
                        {
                            Id = LegalPersonId,
                            LegalPersonProfiles = profiles,
                        },
                    LegalPersonProfileId = isProfileSpecified ? (long?)profileId : null
                };
        }

        private static LegalPersonProfile LegalPersonProfile(long id)
        {
            return new LegalPersonProfile { Id = id };
        }

        public static IOrderReadModel OrderReadModel(Order order)
        {
            var mock = Mock.Of<IOrderReadModel>();
            Mock.Get(mock)
                .Setup(model => model.GetOrderSecure(Moq.It.IsAny<long>()))
                .Returns(order);
            return mock;
        }

        public static ILegalPersonReadModel LegalPersonReadModel(Order order)
        {
            var mock = Mock.Of<ILegalPersonReadModel>();
            Mock.Get(mock)
                .Setup(model => model.GetLegalPersonProfileIds(Moq.It.IsAny<long>()))
                .Returns(order.LegalPerson.LegalPersonProfiles.Select(p => p.Id).ToArray());
            return mock;
        }

        public static ISecurityServiceEntityAccess SecurityServiceEntityAccess()
        {
            var mock = Mock.Of<ISecurityServiceEntityAccess>();
            return mock;
        }
    }
}