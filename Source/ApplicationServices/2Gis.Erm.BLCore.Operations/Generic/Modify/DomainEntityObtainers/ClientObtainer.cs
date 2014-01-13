using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    // 2+: BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations\Generic\Modify\DomainEntityObtainers\ClientObtainer.cs
    public class ClientObtainer : IBusinessModelEntityObtainer<Client>, IAggregateReadModel<Client>
    {
        private readonly IFinder _finder;

        public ClientObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Client ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ClientDomainEntityDto)domainEntityDto;

            var client = dto.Id == 0
                             ? new Client { IsActive = true }
                             : _finder.Find(Specs.Find.ById<Client>(dto.Id)).Single();

            client.Name = dto.Name;
            client.Comment = dto.Comment;
            client.MainAddress = dto.MainAddress;
            client.MainPhoneNumber = dto.MainPhoneNumber;
            client.AdditionalPhoneNumber1 = dto.AdditionalPhoneNumber1;
            client.AdditionalPhoneNumber2 = dto.AdditionalPhoneNumber2;
            client.Email = dto.Email;
            client.Fax = dto.Fax;
            client.Website = dto.Website;
            client.InformationSource = (int)dto.InformationSource;
            client.LastQualifyTime = dto.LastQualifyTime;
            client.LastDisqualifyTime = dto.LastDisqualifyTime;
            client.MainFirmId = dto.MainFirmRef.Id;
            client.IsAdvertisingAgency = dto.IsAdvertisingAgency;

            client.TerritoryId = dto.TerritoryRef.Id.Value;
            client.OwnerCode = dto.OwnerRef.Id.Value;

            client.Timestamp = dto.Timestamp;

            return client;
        }
    }
}