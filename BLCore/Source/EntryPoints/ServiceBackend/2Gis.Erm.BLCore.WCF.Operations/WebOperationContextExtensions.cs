using System;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    public static class WebOperationContextExtensions
    {
         public static void SetBinaryResponseProperties(this WebOperationContext webOperationContext, StreamResponse streamResponse)
         {
             webOperationContext.OutgoingResponse.Headers.Add("Content-Disposition",
                                                              string.Format("attachment; filename={0}", Uri.EscapeDataString(streamResponse.FileName)));
             webOperationContext.OutgoingResponse.ContentType = streamResponse.ContentType;
             webOperationContext.OutgoingResponse.ContentLength = streamResponse.Stream.Length;
         }
    }
}