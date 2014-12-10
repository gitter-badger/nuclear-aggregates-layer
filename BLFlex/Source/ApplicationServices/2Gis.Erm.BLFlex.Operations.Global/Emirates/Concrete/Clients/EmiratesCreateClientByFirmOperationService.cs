using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Clients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Clients
{
    public sealed class EmiratesCreateClientByFirmOperationService : ICreateClientByFirmOperationService, IEmiratesAdapted
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICreateClientAggregateService _createClientAggregateService;
        private readonly IUpdateAggregateRepository<Client> _updateClientRepository;
        private readonly IFirmRepository _firmRepository;

        public EmiratesCreateClientByFirmOperationService(IOperationScopeFactory scopeFactory,
                                                          ICreateClientAggregateService createClientAggregateService,
                                                          IUpdateAggregateRepository<Client> updateClientRepository,
                                                          IFirmRepository firmRepository)
        {
            _scopeFactory = scopeFactory;
            _createClientAggregateService = createClientAggregateService;
            _updateClientRepository = updateClientRepository;
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
                        IsActive = true,
                        Parts = new[] { new EmiratesClientPart() }
                    };

                FirmAddress mainFirmAddress;
                _createClientAggregateService.Create(client, out mainFirmAddress);
                operationScope.Added<Client>(client.Id);

                _firmRepository.SetFirmClient(firm, client.Id);
                operationScope.Updated<Firm>(firm.Id);

                if (mainFirmAddress != null)
                {
                    var addressPoBox = mainFirmAddress.Within<EmiratesFirmAddressPart>().GetPropertyValue(part => part.PoBox);
                    client.Within<EmiratesClientPart>().SetPropertyValue(part => part.PoBox, addressPoBox);

                    _updateClientRepository.Update(client);
                    operationScope.Updated<Client>(client.Id);
                }

                operationScope.Complete();
                return client;
            }
        }
    }
}