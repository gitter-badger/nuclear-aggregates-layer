using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public class ClientObtainer : IBusinessModelEntityObtainer<Client>, IAggregateReadModel<Client>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted, IKazakhstanAdapted
    {
        private readonly IFinder _finder;

        public ClientObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Client ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            // FIXME {y.baranihin, 04.06.2014}: Может быть тут должнен быть MultiCultureClientDomainEntityDto?
            // DONE {d.ivanov, 04.06.2014}: да
            var dto = (MultiCultureClientDomainEntityDto)domainEntityDto;

            var client = _finder.Find(Specs.Find.ById<Client>(dto.Id)).One()
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

            client.TerritoryId = dto.TerritoryRef.Id.Value;
            client.OwnerCode = dto.OwnerRef.Id.Value;

            client.Timestamp = dto.Timestamp;

            return client;
        }
    }
}