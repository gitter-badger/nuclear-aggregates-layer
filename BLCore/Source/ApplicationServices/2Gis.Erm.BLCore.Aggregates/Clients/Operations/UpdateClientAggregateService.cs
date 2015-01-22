using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class UpdateClientAggregateService : IAggregateRootRepository<Client>, IUpdateAggregateRepository<Client>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Client> _legalPersonSecureRepository;
        private readonly IFirmReadModel _firmReadModel;

        public UpdateClientAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Client> legalPersonSecureRepository,
            IFirmReadModel firmReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _firmReadModel = firmReadModel;
        }

        public int Update(Client entity)
        {
            if (entity.MainFirmId != null)
            {
                if (!_firmReadModel.DoesFirmBelongToClient(entity.MainFirmId.Value, entity.Id))
                {
                    throw new NotificationException(BLResources.EditClientMainFirmDoesNotExists);
                }
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, LegalPerson>())
            {
                _legalPersonSecureRepository.Update(entity);
                operationScope.Updated<Client>(entity.Id);

                var count = _legalPersonSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}