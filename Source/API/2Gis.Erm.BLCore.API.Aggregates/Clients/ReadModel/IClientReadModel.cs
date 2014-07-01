using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public interface IClientReadModel : IAggregateReadModel<Client>
    {
        Client GetClient(long clientId);
        string GetClientName(long clientId);
        HotClientRequest GetHotClientRequest(long hotClientRequestId);
    }
}