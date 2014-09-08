using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Firms
{
    public class CreateBlankFirmsOperationService : ICreateBlankFirmsOperationService
    {
        private readonly IBulkCreateFirmAggregateService _bulkCreateFirmsAggregateService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public CreateBlankFirmsOperationService(IBulkCreateFirmAggregateService bulkCreateFirmsAggregateService,
                                                IFirmReadModel firmReadModel,
                                                IIntegrationLocalizationSettings integrationLocalizationSettings,
                                                IOperationScopeFactory scopeFactory,
                                                ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _bulkCreateFirmsAggregateService = bulkCreateFirmsAggregateService;
            _firmReadModel = firmReadModel;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _scopeFactory = scopeFactory;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public void CreateBlankFirms(IEnumerable<BlankFirmDto> firmDtos)
        {
            var organizationUnitDgppIds = firmDtos.Select(x => x.BranchCode).ToArray();
            var regionalTerritories = _firmReadModel.GetRegionalTerritoriesByBranchCodes(organizationUnitDgppIds,
                                                                                         _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord);

            var organizationUnitsWithoutRegionalTerritories = organizationUnitDgppIds.Except(regionalTerritories.Keys)
                                                                                     .ToArray();

            if (organizationUnitsWithoutRegionalTerritories.Any())
            {
                throw new OrganizationUnitsRegionalTerritoryNotFoundException(
                    string.Format("Cannot find either organization unit with DgppId in ({0}) or its regional territory",
                                  string.Join(", ", organizationUnitsWithoutRegionalTerritories)));
            }

            var reservaUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;

            var firmsToCreate = firmDtos.Select(dto => new { Dto = dto, Territory = regionalTerritories[dto.BranchCode] })
                                        .Select(x => new Firm
                                            {
                                                Id = x.Dto.FirmId,

                                                // COMMENT {all, 17.06.2014}: текущая логика повторена, но, возможно, название стоит вынести в конфиг
                                                Name = string.Format("Пустая фирма #{0}", x.Dto.FirmId),
                                                ReplicationCode = Guid.NewGuid(),
                                                TerritoryId = x.Territory.TerritoryId,
                                                ClosedForAscertainment = true,
                                                OwnerCode = reservaUserCode,
                                                IsActive = false,
                                                OrganizationUnitId = x.Territory.OrganizationUnitId
                                            });

            using (var scope = _scopeFactory.CreateNonCoupled<CreateBlankFirmsIdentity>())
            {
                _bulkCreateFirmsAggregateService.Create(firmsToCreate.ToArray());
                scope.Complete();
            }
        }
    }
}