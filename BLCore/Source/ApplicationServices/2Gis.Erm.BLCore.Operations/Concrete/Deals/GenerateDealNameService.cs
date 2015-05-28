using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Deals
{
    public class GenerateDealNameService : IGenerateDealNameService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IFinder _finder;

        public GenerateDealNameService(
            IOperationScopeFactory operationScopeFactory,
            // FIXME {d.ivanov, 05.12.2013}: Read-model
            IFinder finder)
        {
            _operationScopeFactory = operationScopeFactory;
            _finder = finder;
        }

        public string GenerateDealName(string clientName, string mainFirmName)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GenerateDealNameIdentity>())
            {
                var dealName = string.Format("{0} - {1}", clientName, mainFirmName);

                scope.Complete();

                return dealName;
            }
        }

        public string GenerateDealName(long clientId)
        {
            var item = _finder.FindObsolete(Specs.Find.ById<Client>(clientId))
                             .Select(x => new { ClientName = x.Name, MainFirmName = x.Firm.Name })
                             .Single();

            return GenerateDealName(item.ClientName, item.MainFirmName);
        }
    }
}
