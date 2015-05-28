using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify.DomainEntityObtainers
{
    public class EmiratesClientObtainer : IBusinessModelEntityObtainer<Client>, IAggregateReadModel<Client>, IEmiratesAdapted
    {
        private readonly IFinder _finder;

        public EmiratesClientObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Client ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (EmiratesClientDomainEntityDto)domainEntityDto;

            var client = _finder.Find(Specs.Find.ById<Client>(dto.Id)).One()
                ?? new Client { IsActive = true, Parts = new[] { new EmiratesClientPart() } };

            ClientFlexSpecs.Clients.Emirates.Assign.Entity().Assign(dto, client);

            return client;
        }
    }
}