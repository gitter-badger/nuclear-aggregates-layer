using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportFirmAddressService : IImportFirmAddressService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ICreateAggregateRepository<FirmAddress> _createFirmAddressService;
        private readonly IUpdateAggregateRepository<FirmAddress> _updateFirmAddressService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly ICreateAggregateRepository<Firm> _createFirmService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public ImportFirmAddressService(IOperationScopeFactory operationScopeFactory,
                                        ICreateAggregateRepository<FirmAddress> createFirmAddressService,
                                        IUpdateAggregateRepository<FirmAddress> updateFirmAddressService,
                                        IFirmReadModel firmReadModel,
                                        IIntegrationLocalizationSettings integrationLocalizationSettings,
                                        ICreateAggregateRepository<Firm> createFirmService,
                                        ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _operationScopeFactory = operationScopeFactory;
            _createFirmAddressService = createFirmAddressService;
            _updateFirmAddressService = updateFirmAddressService;
            _firmReadModel = firmReadModel;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _createFirmService = createFirmService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public void Import(IEnumerable<ImportFirmAddressDto> dtos)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<ImportFirmAddressIdentity>())
            {
                var addressesToImport = dtos.ToDictionary(x => x.FirmAddress.Id);

                var ids = addressesToImport.Keys;
                var existingAddresses = _firmReadModel.GetFirmAddresses(ids);

                var idsToUpdate = addressesToImport.Keys.Intersect(existingAddresses.Keys).ToArray();
                var idsToInsert = addressesToImport.Keys.Except(existingAddresses.Keys).ToArray();

                // Создаем пустые фирмы, если нужно
                CreateBlankFirms(addressesToImport.Values);

                if (idsToUpdate.Any())
                {
                    foreach (var id in idsToUpdate)
                    {
                        var addressToUpdate = addressesToImport[id].FirmAddress;
                        var existingAddress = existingAddresses[id];

                        addressToUpdate.ReplicationCode = existingAddress.ReplicationCode;
                        addressToUpdate.CreatedBy = existingAddress.CreatedBy;
                        addressToUpdate.CreatedOn = existingAddress.CreatedOn;
                        addressToUpdate.Timestamp = existingAddress.Timestamp;

                        addressToUpdate.Within().SyncPropertyValue(x => x.Id, existingAddress);
                        addressToUpdate.Within().SyncPropertyValue(x => x.EntityId, existingAddress);
                        addressToUpdate.Within().SyncPropertyValue(x => x.Timestamp, existingAddress);

                        _updateFirmAddressService.Update(addressToUpdate);

                        scope.Updated<FirmAddress>(addressToUpdate.Id);
                    }
                }

                if (idsToInsert.Any())
                {
                    foreach (var addressToInsert in idsToInsert.Select(id => addressesToImport[id].FirmAddress))
                    {
                        _createFirmAddressService.Create(addressToInsert);
                        scope.Added<FirmAddress>(addressToInsert.Id);
                    }
                }

                scope.Complete();
            }
        }

        private void CreateBlankFirms(IEnumerable<ImportFirmAddressDto> addresses)
        {
            var regionalTerritoryName = _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord;
            var addressesByFirmCode = addresses.GroupBy(x => x.FirmAddress.FirmId).ToDictionary(x => x.Key);
            var firmIds = addressesByFirmCode.Select(x => x.Key).ToArray();
            var existingFirms = _firmReadModel.GetFirms(firmIds);

            var firmIdsToInsert = firmIds.Except(existingFirms.Keys).ToArray();
            if (!firmIdsToInsert.Any())
            {
                return;
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Firm>())
            {
                var branchCodesByFirmCodes = addressesByFirmCode.Where(x => firmIdsToInsert.Contains(x.Key))
                                                                .ToDictionary(x => x.Key, y => y.Value.Select(z => z.BranchCode).Distinct());

                var organizationUnitDgppIds = branchCodesByFirmCodes.SelectMany(x => x.Value).Distinct();

                var regionalTerritories = _firmReadModel.GetRegionalTerritoriesByBranchCodes(organizationUnitDgppIds, regionalTerritoryName);

                var organizationUnitsWithoutRegionalTerritories =
                    organizationUnitDgppIds.Except(regionalTerritories.Select(x => x.OrganizationUnitDgppId)).ToArray();

                if (organizationUnitsWithoutRegionalTerritories.Any())
                {
                    throw new OrganizationUnitsRegionalTerritoryNotFoundException(
                        string.Format("Cannot find either organization unit with DgppId in ({0}) or its regional territory",
                                      string.Join(", ", organizationUnitsWithoutRegionalTerritories.Select(x => x.ToString()))));
                }

                foreach (var firmIdToInsert in firmIdsToInsert)
                {
                    var regionalTerritory = regionalTerritories.First(x => x.OrganizationUnitDgppId == branchCodesByFirmCodes[firmIdToInsert].First());

                    var firm = new Firm
                        {
                            Id = firmIdToInsert,

                            // COMMENT {all, 17.06.2014}: текущая логика повторена, но, возможно, название стоит вынести в конфиг
                            Name = string.Format("Пустая фирма #{0}", firmIdToInsert),
                            ReplicationCode = Guid.NewGuid(),
                            TerritoryId = regionalTerritory.TerritoryId,
                            ClosedForAscertainment = true,
                            OwnerCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code,
                            IsActive = false,
                            OrganizationUnitId = regionalTerritory.OrganizationUnitId,
                        };

                    _createFirmService.Create(firm);
                    scope.Added<Firm>(firm.Id);
                }

                scope.Complete();
            }
        }
    }
}