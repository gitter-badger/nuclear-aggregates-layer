using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportFirmsDuringImportCardsForErmOperationService : IImportFirmsDuringImportCardsForErmOperationService
    {
        private readonly IBulkCreateFirmAggregateService _bulkCreateFirmAggregateService;
        private readonly IBulkUpdateFirmAggregateService _bulkUpdateFirmAggregateService;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IDetachClientFromFirmAggregateService _detachClientFromFirmAggregateService;
        private readonly IDetachDealFromFirmAggregateService _detachDealFromFirmAggregateService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public ImportFirmsDuringImportCardsForErmOperationService(IBulkCreateFirmAggregateService bulkCreateFirmAggregateService,
                                                                  IBulkUpdateFirmAggregateService bulkUpdateFirmAggregateService,
                                                                  IClientReadModel clientReadModel,
                                                                  IDealReadModel dealReadModel,
                                                                  IDetachClientFromFirmAggregateService detachClientFromFirmAggregateService,
                                                                  IDetachDealFromFirmAggregateService detachDealFromFirmAggregateService,
                                                                  IFirmReadModel firmReadModel,
                                                                  IIntegrationLocalizationSettings integrationLocalizationSettings,
                                                                  IOrganizationUnitReadModel organizationUnitReadModel,
                                                                  IOperationScopeFactory scopeFactory,
                                                                  ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _bulkCreateFirmAggregateService = bulkCreateFirmAggregateService;
            _bulkUpdateFirmAggregateService = bulkUpdateFirmAggregateService;
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _detachClientFromFirmAggregateService = detachClientFromFirmAggregateService;
            _detachDealFromFirmAggregateService = detachDealFromFirmAggregateService;
            _firmReadModel = firmReadModel;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _organizationUnitReadModel = organizationUnitReadModel;
            _scopeFactory = scopeFactory;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public void Import(IReadOnlyCollection<FirmForErmDto> firms)
        {
            var reserveUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;

            var firmsToImport = firms.ToDictionary(x => x.Code);

            var existingFirms = _firmReadModel.GetFirms(firmsToImport.Keys);

            var firmIdsToCreate = firmsToImport.Keys.Except(existingFirms.Keys).ToArray();
            var firmIdsToUpdate = existingFirms.Keys.Intersect(firmsToImport.Keys).ToArray();

            var branchCodes = firmsToImport.Values.Select(x => x.BranchCode).Distinct().ToArray();
            var organizationInitIdsByBranchCodes = _organizationUnitReadModel.GetOrganizationUnitIdsByDgppIds(branchCodes);

            using (var scope = _scopeFactory.CreateNonCoupled<ImportFirmsDuringImportCardsForErmIdentity>())
            {
                if (firmIdsToCreate.Any())
                {
                    var regionalTerritories = _firmReadModel.GetRegionalTerritoriesByBranchCodes(branchCodes,
                                                                                                 _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord);
                    var firmsToCreate = firmIdsToCreate.Select(id => firmsToImport[id])
                                                       .Select(dto => new Firm
                                                           {
                                                               Id = dto.Code,
                                                               OrganizationUnitId = organizationInitIdsByBranchCodes[dto.BranchCode],
                                                               Name = dto.Name,
                                                               IsActive = dto.IsActive,
                                                               ClosedForAscertainment = dto.ClosedForAscertainment,
                                                               TerritoryId = regionalTerritories[dto.BranchCode].TerritoryId,
                                                               OwnerCode = reserveUserCode
                                                           })
                                                       .ToArray();

                    _bulkCreateFirmAggregateService.Create(firmsToCreate);
                }

                if (firmIdsToUpdate.Any())
                {
                    var firmsToUpdate = new List<Firm>(firmIdsToUpdate.Length);
                    foreach (var id in firmIdsToUpdate)
                    {
                        var firm = existingFirms[id];
                        var dto = firmsToImport[id];

                        firm.OrganizationUnitId = organizationInitIdsByBranchCodes[dto.BranchCode];
                        firm.Name = dto.Name;
                        firm.IsActive = dto.IsActive;
                        firm.ClosedForAscertainment = dto.ClosedForAscertainment;

                        firmsToUpdate.Add(firm);
                    }

                    _bulkUpdateFirmAggregateService.Update(firmsToUpdate);

                    var inactiveFirmIds = firmsToUpdate.Where(x => !x.IsActive || x.ClosedForAscertainment).Select(x => x.Id).ToArray();
                    if (inactiveFirmIds.Any())
                    {
                        var clientsToDetach = _clientReadModel.GetClientsByMainFirmIds(inactiveFirmIds);
                        var dealsToDetach = _dealReadModel.GetDealsByMainFirmIds(inactiveFirmIds);

                        _detachClientFromFirmAggregateService.Detach(clientsToDetach);
                        _detachDealFromFirmAggregateService.Detach(dealsToDetach);
                    }
                }

                scope.Complete();
            }
        }
    }
}