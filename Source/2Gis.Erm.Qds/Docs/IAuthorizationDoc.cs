namespace DoubleGis.Erm.Qds.Docs
{
    public interface IAuthorizationDoc : IDoc
    {
        DocumentAuthorization Authorization { get; set; }
    }
}