using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.ReadModel
{
    public class ClientReadModel : IClientReadModel
    {
        private readonly IFinder _finder;

        public ClientReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Client GetClient(long clientId)
        {
            return _finder.FindOne(Specs.Find.ById<Client>(clientId));
        }

        public string GetClientName(long clientId)
        {
            return _finder.Find(Specs.Find.ById<Client>(clientId)).Select(x => x.Name).Single();
        }
    }
}