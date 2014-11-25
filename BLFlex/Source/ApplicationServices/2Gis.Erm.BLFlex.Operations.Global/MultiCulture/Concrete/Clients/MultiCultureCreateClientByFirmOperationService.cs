using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Clients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Clients
{
    public sealed class MultiCultureCreateClientByFirmOperationService : ICreateClientByFirmOperationService,
                                                                         IRussiaAdapted,
                                                                         ICyprusAdapted,
                                                                         IChileAdapted,
                                                                         ICzechAdapted,
                                                                         IUkraineAdapted,
                                                                         IKazakhstanAdapted
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICreateClientAggregateService _createClientAggregateService;
        private readonly IFirmRepository _firmRepository;

        public MultiCultureCreateClientByFirmOperationService(IOperationScopeFactory scopeFactory,
                                                              ICreateClientAggregateService createClientAggregateService,
                                                              IFirmRepository firmRepository)
        {
            _scopeFactory = scopeFactory;
            _createClientAggregateService = createClientAggregateService;
            _firmRepository = firmRepository;
        }

        public Client CreateByFirm(Firm firm, long ownerCode)
        {
            if (!firm.LastQualifyTime.HasValue)
            {
                throw new InvalidOperationException(BLResources.FirmHasNotLastQualifyTime);
            }

            if (firm.ClientId.HasValue)
            {
                throw new InvalidOperationException(BLResources.ConnotCreateClientForFirmSinceItAlreadyExists);
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<CreateClientByFirmIdentity>())
            {
                var client = new Client
                {
                    MainFirmId = firm.Id,
                    Name = firm.Name,
                    TerritoryId = firm.TerritoryId,
                    InformationSource = InformationSource.SalesDepartment,
                    OwnerCode = ownerCode,
                    LastQualifyTime = firm.LastQualifyTime.Value,
                    IsActive = true
                };

                FirmAddress mainFirmAddress;
                _createClientAggregateService.Create(client, out mainFirmAddress);
                operationScope.Added<Client>(client.Id);

                _firmRepository.SetFirmClient(firm, client.Id);
                operationScope.Updated<Firm>(firm.Id)
                              .Complete();

                return client;
            }
        }
    }
}