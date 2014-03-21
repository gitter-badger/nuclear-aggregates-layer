namespace DoubleGis.Erm.Qds.Docs
{
    public class UserDoc : IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DocumentAuthorization Authorization { get; set; }
    }
}