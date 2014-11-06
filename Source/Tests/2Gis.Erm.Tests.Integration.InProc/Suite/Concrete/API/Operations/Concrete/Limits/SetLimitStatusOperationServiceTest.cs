﻿using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Limits
{
    public class SetLimitStatusOperationServiceTest : UseModelEntityTestBase<Limit>
    {
        private readonly ISetLimitStatusOperationService _setLimitStatusOperationService;

        public SetLimitStatusOperationServiceTest(IAppropriateEntityProvider<Limit> appropriateEntityProvider, ISetLimitStatusOperationService setLimitStatusOperationService)
            : base(appropriateEntityProvider)
        {
            _setLimitStatusOperationService = setLimitStatusOperationService;
        }

        protected override FindSpecification<Limit> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec && new FindSpecification<Limit>(l => l.Status != (int)LimitStatus.Opened
                    && !l.Account.Orders
                        .Any(o => o.BeginDistributionDate <= l.StartPeriodDate && o.EndDistributionDateFact >= l.EndPeriodDate
                        && (o.WorkflowStepId == (int)OrderState.Approved || o.WorkflowStepId == (int)OrderState.OnTermination || o.WorkflowStepId == (int)OrderState.Archive)));
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Limit modelEntity)
        {
            _setLimitStatusOperationService.SetStatus(modelEntity.Id, LimitStatus.Opened, modelEntity.ReplicationCode);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}