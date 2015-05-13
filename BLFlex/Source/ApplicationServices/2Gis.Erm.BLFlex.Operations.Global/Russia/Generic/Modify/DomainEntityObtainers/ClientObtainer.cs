using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public class ClientObtainer : IBusinessModelEntityObtainer<Client>, IAggregateReadModel<Client>, IRussiaAdapted
    {
        private readonly IFinder _finder;

        public ClientObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Client ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ClientDomainEntityDto)domainEntityDto;

            var client = _finder.FindOne(Specs.Find.ById<Client>(dto.Id)) 
                ?? new Client { IsActive = true };

            client.Name = dto.Name;
            client.Comment = dto.Comment;
            client.MainAddress = dto.MainAddress;
            client.MainPhoneNumber = dto.MainPhoneNumber;
            client.AdditionalPhoneNumber1 = dto.AdditionalPhoneNumber1;
            client.AdditionalPhoneNumber2 = dto.AdditionalPhoneNumber2;
            client.Email = dto.Email;
            client.Fax = dto.Fax;
            client.Website = dto.Website;
            client.InformationSource = dto.InformationSource;
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