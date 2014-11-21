namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class SpecifyAdditionalServicesIdentity : OperationIdentityBase<SpecifyAdditionalServicesIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SpecifyFirmAddressAdditionalServicesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Указать дополнительные услуги";
            }
        }
    }
}