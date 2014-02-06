using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Client
{
    // 2+: BL\Source\Tests\2Gis.Erm.BLCore.Tests.Unit\BL\Client\CyprusClientVewModelSpecs.cs
    public class CyprusClientVewModelSpecs
    {
        [Tags("BL")]
        [Tags("CyprusClientViewModel")]
        [Subject(typeof(ClientViewModel))]
        public abstract class LoadDomainEntityDtoContext
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
            };

            protected static ClientViewModel ViewModel { get; set; }
            protected static ClientDomainEntityDto ClientDto { get; private set; }
        }

        [Tags("BL")]
        [Tags("CyprusClientViewModel")]
        [Subject(typeof(ClientViewModel))]
        public abstract class TransformToDomainEntityDtoContext
        {
            private Establish context = () =>
            {
                ViewModel = new ClientViewModel
                {
                    Id = 0,
                    MainFirm = new LookupField
                    {
                        Key = 1
                    },
                    Territory = new LookupField
                    {
                        Key = 1
                    },
                    Owner = new LookupField
                    {
                        Key = 1
                    }
                };
            };

            protected static ClientViewModel ViewModel { get; private set; }
            protected static ClientDomainEntityDto ClientDto { get; set; }
        }

        // Проверяем загрузку моделью из дто информации о том, является ли клиент рекламным агентством. Клиент является рекламным агентством.
        private class When_cyprus_adapted_client_dto_is_an_advertising_agency : LoadDomainEntityDtoContext
        {
            private Establish context = () =>
            {
                ViewModel = new ClientViewModel();
                ClientDto.IsAdvertisingAgency = true;
            };

            private Because of =
                () =>
                ViewModel.LoadDomainEntityDto(ClientDto);

            private It should_be_an_advertising_agency_in_viewmodel = () => ViewModel.IsAdvertisingAgency.Should().BeTrue();
        }

        // Проверяем загрузку моделью из дто информации о том, является ли клиент рекламным агентством. Клиент не является рекламным агентством.
        private class When_cyprus_adapted_client_dto_is_not_an_advertising_agency : LoadDomainEntityDtoContext
        {
            private Establish context = () =>
            {
                ViewModel = new ClientViewModel();
                ClientDto.IsAdvertisingAgency = false;
            };

            private Because of =
                () =>
                ViewModel.LoadDomainEntityDto(ClientDto);

            private It should_be_not_an_advertising_agency_in_viewmodel = () => ViewModel.IsAdvertisingAgency.Should().BeFalse();
        }

        // Проверяем верно ли модель передает информации о том, является ли клиент рекламным агентством. Клиент является рекламным агентством.
        private class When_client_viewmodel_is_an_advertising_agency : TransformToDomainEntityDtoContext
        {
            private Establish context = () =>
            {
                ViewModel.IsAdvertisingAgency = true;
            };

            private Because of =
                () =>
                ClientDto = (ClientDomainEntityDto)ViewModel.TransformToDomainEntityDto();

            private It should_be_an_advertising_agency_in_dto = () => ClientDto.IsAdvertisingAgency.Should().BeTrue();
        }

        // Проверяем верно ли модель передает информации о том, является ли клиент рекламным агентством. Клиент не является рекламным агентством.
        private class When_client_viewmodel_is_not_an_advertising_agency : TransformToDomainEntityDtoContext
        {
            private Establish context = () =>
            {
                ViewModel.IsAdvertisingAgency = false;
            };

            private Because of =
                () =>
                ClientDto = (ClientDomainEntityDto)ViewModel.TransformToDomainEntityDto();

            private It should_be_not_an_advertising_agency_in_dto = () => ClientDto.IsAdvertisingAgency.Should().BeFalse();
        }
    }
}
