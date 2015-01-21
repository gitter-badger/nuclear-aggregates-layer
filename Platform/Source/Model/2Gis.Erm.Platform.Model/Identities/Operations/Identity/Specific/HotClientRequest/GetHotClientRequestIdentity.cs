using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class GetHotClientRequestIdentity : OperationIdentityBase<GetHotClientRequestIdentity>
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetHotClientRequestIdentity; }
        }

        public override string Description
        {
            get { return "Создание dto для задачи типа 'горячий клиент'."; }
        }
    }
}