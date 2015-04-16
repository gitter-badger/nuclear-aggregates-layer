namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition
{
    public class GetSymmetricDeniedPositionIdentity : OperationIdentityBase<GetSymmetricDeniedPositionIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetSymmetricDeniedPositionIdentity; }
        }

        public override string Description
        {
            get { return "Получение симметричной запрещенной позиции"; }
        }
    }
}
