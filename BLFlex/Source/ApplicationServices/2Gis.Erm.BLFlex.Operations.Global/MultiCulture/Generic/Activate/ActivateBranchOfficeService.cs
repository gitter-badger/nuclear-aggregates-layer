using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Activate
{
    public class ActivateBranchOfficeService : IActivateGenericEntityService<BranchOffice>, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted,
                                               IUkraineAdapted
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public ActivateBranchOfficeService(IOperationScopeFactory operationScopeFactory,
                                           IBranchOfficeReadModel branchOfficeReadModel,
                                           IBranchOfficeRepository branchOfficeRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeReadModel = branchOfficeReadModel;
            _branchOfficeRepository = branchOfficeRepository;
        }

        public int Activate(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, BranchOffice>())
            {
                var restoringBranchOffice = _branchOfficeReadModel.GetBranchOffice(entityId);
                if (restoringBranchOffice.IsActive)
                {
                    throw new ActiveEntityActivationException(typeof(BranchOffice), restoringBranchOffice.Name);
                }

                var activateAggregateRepository = (IActivateAggregateRepository<BranchOffice>)_branchOfficeRepository;
                var result = activateAggregateRepository.Activate(entityId);

                scope.Updated<BranchOffice>(entityId)
                     .Complete();

                return result;
            }
        }
    }
}