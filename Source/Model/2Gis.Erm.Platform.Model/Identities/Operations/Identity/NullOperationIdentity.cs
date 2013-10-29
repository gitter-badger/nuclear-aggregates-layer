namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public sealed class NullOperationIdentity : OperationIdentityBase<NullOperationIdentity>
    {
        public override int Id
        {
            get
            {
                return 0;
            }
        }

        public override string Description
        {
            get
            {
                return "Operation stub";
            }
        }
    }
}
