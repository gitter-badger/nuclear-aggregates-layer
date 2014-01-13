﻿using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Remote
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.OrderValidation.OrderValidation201303)]
    public interface IOrderValidationApplicationRestService
    {
        [OperationContract(Name = "ValidateSingleOrderRest")]
        [WebGet(UriTemplate = "Single/{orderId}", ResponseFormat = WebMessageFormat.Json)]
        ValidateOrdersResult ValidateSingleOrder(string orderId); 

        [OperationContract(Name = "ValidateSingleOrderStateTransferRest")]
        [WebGet(UriTemplate = "SingleStateTransfer/{orderId}/{newOrderState}", ResponseFormat = WebMessageFormat.Json)]
        ValidateOrdersResult ValidateOnStateChangeSingleOrder(string orderId, string newOrderState); 
    }
}