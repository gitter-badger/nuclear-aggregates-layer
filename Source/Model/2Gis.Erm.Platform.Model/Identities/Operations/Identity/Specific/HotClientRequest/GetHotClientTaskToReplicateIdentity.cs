namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class GetHotClientTaskToReplicateIdentity : OperationIdentityBase<GetHotClientTaskToReplicateIdentity>
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetHotClientTaskToReplicateIdentity; }
        }

        public override string Description
        {
            get { return "Создание dto для задачи типа 'горячий клиент' в MSCRM с целью дальнейшей репликации"; }
        }
    }
}