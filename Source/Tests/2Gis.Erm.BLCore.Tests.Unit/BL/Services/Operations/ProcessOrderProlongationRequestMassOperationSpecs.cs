﻿using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.Common.Logging;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    public static class ProcessOrderProlongationRequestMassOperationSpecs
    {
        [Tags("BL")]
        [Subject(typeof(ProcessOrderProlongationRequestMassOperation))]
        private class When_process_method_called
        {
            private static IProcessOrderProlongationRequestMassOperation Operation;

            private static IOrderProcessingRequestService OrderProcessingRequestService;

            Establish context = () =>
            {
                OrderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();
                var basicOrderProlongationOperation = Mock.Of<IBasicOrderProlongationOperationLogic>();
                var logger = Mock.Of<ICommonLog>();

                Operation = new ProcessOrderProlongationRequestMassOperation(OrderProcessingRequestService,
                                                                             basicOrderProlongationOperation,
                                                                             logger);
            };

            Because of = () =>
                Operation.ProcessAll();

            It should_get_order_processing_requests = () =>
                Mock.Get(OrderProcessingRequestService).Verify(x => x.GetProlongationRequestsToProcess(), Times.Once);
        }
    }
}
