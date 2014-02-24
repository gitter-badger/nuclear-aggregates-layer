namespace DoubleGis.Erm.Qds.Docs
{
    public interface IAuthDoc : IDoc
    {
        DocumentAuthorization Auth { get; set; }
    }
}