namespace DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse
{
    public interface IPublicService 
    {
        Response Handle(Request request);
    }
}