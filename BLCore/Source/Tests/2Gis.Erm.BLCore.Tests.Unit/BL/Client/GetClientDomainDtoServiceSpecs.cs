using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Security.API.UserContext;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Client
{
    // 2+: BL\Source\Tests\2Gis.Erm.BLCore.Tests.Unit\BL\Client\GetClientDtoServiceServiceSpecs.cs
    public class GetClientDtoServiceServiceSpecs
    {
        [Tags("BL")]
        [Tags("GetDomainEntityDtoService")]
        [Subject(typeof(GetClientDtoService))]
        public abstract class FinderMockContext
        {
            private Establish context = () =>
                {
                    Client = new Platform.Model.Entities.Erm.Client
                        {
                            InformationSource = InformationSource.DoubleGis,
                            Firm = new Firm
                                {
                                    Name = "test"
                                },
                            Territory = new Territory
                                {
                                    Name = "test"
                                },
                        };

                    FinderMock = new Mock<ISecureFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<Expression<Func<Platform.Model.Entities.Erm.Client, bool>>>()))
                              .Returns(new[] { Client }.AsQueryable());

                    var userContext = Mock.Of<IUserContext>(x => x.Identity == new NullUserIdentity());
                    
                    GetDtoService = new GetClientDtoService(userContext, FinderMock.Object);
                };

            protected static Mock<ISecureFinder> FinderMock { get; private set; }
            protected static Platform.Model.Entities.Erm.Client Client { get; set; }
            protected static ClientDomainEntityDto ClientDto { get; set; }
            protected static IGetDomainEntityDtoService<Platform.Model.Entities.Erm.Client> GetDtoService { get; set; }
        }

        // Клиент является рекламным агентством. Полученный Dto должен содержать эту информацию
        private class When_client_entity_is_advertising_agency : FinderMockContext
        {
            private Establish context = () =>
                {
                    Client.IsAdvertisingAgency = true;
                };

            private Because of =
                () =>
                ClientDto = (ClientDomainEntityDto)GetDtoService.GetDomainEntityDto(1, false, null, EntityName.None, string.Empty);

            private It should_be_an_advertising_agency_in_dto = () => ClientDto.IsAdvertisingAgency.Should().BeTrue();
        }

        // Клиент не является рекламным агентством. Полученный Dto должен содержать эту информацию
        private class When_client_entity_is_not_advertising_agency : FinderMockContext
        {
            private Establish context = () =>
                {
                    Client.IsAdvertisingAgency = false;
                };

            private Because of =
                () =>
                ClientDto = (ClientDomainEntityDto)GetDtoService.GetDomainEntityDto(1, false, null, EntityName.None, string.Empty);

            private It should_be_not_an_advertising_agency_in_dto = () => ClientDto.IsAdvertisingAgency.Should().BeFalse();
        }
    }
}
