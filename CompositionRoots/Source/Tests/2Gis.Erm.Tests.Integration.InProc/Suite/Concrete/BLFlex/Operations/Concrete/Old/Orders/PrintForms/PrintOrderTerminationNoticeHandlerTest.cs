﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.BLFlex.Operations.Concrete.Old.Orders.PrintForms
{
    public class PrintOrderTerminationNoticeHandlerTest : UseModelEntityHandlerTestBase<Order, PrintOrderTerminationNoticeRequest, StreamResponse>
    {
        public PrintOrderTerminationNoticeHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Order> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Order> ModelEntitySpec
        {
            get
            {
                var allowedStates = new[] { (int)OrderState.OnTermination, (int)OrderState.Archive };
                return base.ModelEntitySpec &&
                       new FindSpecification<Order>(o => o.IsTerminated && allowedStates.Contains(o.WorkflowStepId) && o.LegalPersonProfileId != null);
            }
        }

        protected override bool TryCreateRequest(Order modelEntity, out PrintOrderTerminationNoticeRequest request)
        {
            request = new PrintOrderTerminationNoticeRequest
                {
                    OrderId = modelEntity.Id,
                };

            return true;
        }

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}