namespace DoubleGis.Erm.Qds.Docs
{
    public class UserDoc : IAuthDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DocumentAuthorization Auth { get; set; }
    }
}