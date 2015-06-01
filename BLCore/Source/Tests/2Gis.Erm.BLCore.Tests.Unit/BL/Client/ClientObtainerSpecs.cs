using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Readings;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Client
{
    // 2+: BL\Source\Tests\2Gis.Erm.BLCore.Tests.Unit\BL\Client\ClientObtainerSpecs.cs
    public class ClientObtainerSpecs
    {
        [Tags("BL")]
        [Tags("ClientObtainer")]
        [Subject(typeof(ClientObtainer))]
        public abstract class MockContext
        {
            private Establish context = () =>
                {
                    ClientDto = new ClientDomainEntityDto
                        {
                            Id = 0,
                            MainFirmRef = new EntityReference
                                {
                                    Id = 1
                                },
                            TerritoryRef = new EntityReference
                                {
                                    Id = 1
                                },
                            OwnerRef = new EntityReference
                                {
                                    Id = 1
                                }
                        };

                ClientObtainer = new ClientObtainer(Mock.Of<IFinder>());
            };

            protected static Platform.Model.Entities.Erm.Client Client { get; set; }
            protected static ClientDomainEntityDto ClientDto { get; private set; }
            protected static IBusinessModelEntityObtainer<Platform.Model.Entities.Erm.Client> ClientObtainer { get; set; }
        }

        // Клиент является рекламным агентством. Obtainer должен установить это значение
        private class When_client_dto_is_advertising_agency : MockContext
        {
            private Establish context = () =>
                {
                    ClientDto.IsAdvertisingAgency = true;
                };

            private Because of =
                () =>
                Client = ClientObtainer.ObtainBusinessModelEntity(ClientDto);

            private It should_be_an_advertising_agency_in_entity = () => Client.IsAdvertisingAgency.Should().BeTrue();
        }

        // Клиент не является рекламным агентством. Obtainer должен установить это значение
        private class When_client_dto_is_not_advertising_agency : MockContext
        {
            private Establish context = () =>
            {
                ClientDto.IsAdvertisingAgency = false;
            };

            private Because of =
                () =>
                Client = ClientObtainer.ObtainBusinessModelEntity(ClientDto);

            private It should_be_not_an_advertising_agency_in_entity = () => Client.IsAdvertisingAgency.Should().BeFalse();
        }
    }
}
