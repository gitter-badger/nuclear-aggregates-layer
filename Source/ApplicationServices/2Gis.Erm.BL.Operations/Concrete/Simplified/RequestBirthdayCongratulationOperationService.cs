using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Contact;

namespace DoubleGis.Erm.BL.Operations.Concrete.Simplified
{
    public sealed class RequestBirthdayCongratulationOperationService : IRequestBirthdayCongratulationOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IClientReadModel _clientReadModel;
        private readonly IBirthdayCongratulationReadModel _birthdayCongratulationReadModel;
        private readonly ICreateBirthdayCongratulationService _createBirthdayCongratulationService;

        public RequestBirthdayCongratulationOperationService(IBirthdayCongratulationReadModel birthdayCongratulationReadModel,
                                                             ICreateBirthdayCongratulationService createBirthdayCongratulationService,
                                                             IOperationScopeFactory operationScopeFactory,
                                                             IClientReadModel clientReadModel)
        {
            _birthdayCongratulationReadModel = birthdayCongratulationReadModel;
            _createBirthdayCongratulationService = createBirthdayCongratulationService;
            _operationScopeFactory = operationScopeFactory;
            _clientReadModel = clientReadModel;
        }

        public void AddRequest(DateTime congratulationDate)
        {
            if (_birthdayCongratulationReadModel.IsTherePlannedCongratulation(congratulationDate))
            {
                throw new BirthdayCongratulationIsNotUniqueException("уже есть рассылка поздравлений за " + congratulationDate.ToShortDateString());
            }

            var contacts = _clientReadModel.GetContactEmailsByBirthDate(congratulationDate.Month, congratulationDate.Day);

            if (!contacts.Any())
            {
                throw new ThereAreNoContactsToCongratulateException(string.Format("для рассылки поздравлений за {0} нет контактов",
                                                                                  congratulationDate.ToShortDateString()));
            }

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<RequestBirthdayCongratulationsIdentity>())
            {
                var entity = new BirthdayCongratulation { CongratulationDate = congratulationDate };
                _createBirthdayCongratulationService.Create(entity);
                operationScope.Added<BirthdayCongratulation>(entity.Id);

                operationScope.Complete();
            }
        }
    }
}
