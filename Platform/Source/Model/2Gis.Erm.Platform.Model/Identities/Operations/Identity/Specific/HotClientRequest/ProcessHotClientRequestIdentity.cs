using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class ProcessHotClientRequestIdentity : OperationIdentityBase<ProcessHotClientRequestIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ProcessHotClientRequestIdentity; }
        }

        public override string Description
        {
            get { return "Создает задачу на запрос по горячему клиенту."; }
        }
    }
}