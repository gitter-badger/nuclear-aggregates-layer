using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts
{
    [DataContract(Namespace =  ServiceNamespaces.BasicOperations.CheckForDebts201303)]
    public class CheckForDebtsResult
    {
        [DataMember]
        public bool DebtsExist { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    public interface ICheckEntityForDebtsService : IOperation<CheckForDebtsIdentity>
    {
        CheckForDebtsResult CheckForDebts(long entityId); 
    }
}