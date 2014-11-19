using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public class DeleteClientLinkAggregateService : IDeleteLegalPersonDealService
    {
        private readonly IRepository<LegalPersonDeal> _repository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteClientLinkAggregateService(
            IRepository<LegalPersonDeal> repository,
            IOperationScopeFactory operationScopeFactory)
        {
            _repository = repository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Delete(LegalPersonDeal link, bool isLinkLastOne)
        {
            if (link.IsMain && !isLinkLastOne)
            {
                throw new OperationException<LegalPersonDeal, DeleteIdentity>(BLResources.CannotDeleteMainLegalPersonDeal);
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonDeal>())
            {
                _repository.Delete(link);
                _repository.Save();
                scope.Deleted(link).Complete();
            }
        }
    }
}
