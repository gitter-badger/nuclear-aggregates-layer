using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail
{
    public class GetDebitsInfoInitialForExportIdentity : OperationIdentityBase<GetDebitsInfoInitialForExportIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetDebitsInfoInitialForExportIdentity; }
        }

        public override string Description
        {
            get { return "Получение данных для выгрузки списаний"; }
        }
    }
}