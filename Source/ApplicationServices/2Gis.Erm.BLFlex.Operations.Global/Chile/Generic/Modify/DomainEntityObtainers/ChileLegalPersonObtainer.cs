using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, IChileAdapted
    {
        private readonly IDynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance> _legalPersonPartPropertiesConverter;
        private readonly IFinder _finder;

        public ChileLegalPersonObtainer(
            IDynamicEntityPropertiesConverter<LegalPersonPart, BusinessEntityInstance, BusinessEntityPropertyInstance> legalPersonPartPropertiesConverter, 
            IFinder finder)
        {
            _legalPersonPartPropertiesConverter = legalPersonPartPropertiesConverter;
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ChileLegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(dto.Id)).SingleOrDefault()
                                     ?? new LegalPerson { IsActive = true };

            CopyFields(dto, legalPerson);
            
            var legalPersonPart = legalPerson.IsNew()
                ? new LegalPersonPart()
                : _finder.SingleOrDefault(dto.Id, _legalPersonPartPropertiesConverter.ConvertFromDynamicEntityInstance);

            CopyPartFields(dto, legalPersonPart);

            legalPerson.Parts = new[] { legalPersonPart };
            return legalPerson;
        }

        private static void CopyFields(ChileLegalPersonDomainEntityDto dto, LegalPerson legalPerson)
        {
            legalPerson.LegalPersonTypeEnum = (int)dto.LegalPersonTypeEnum;
            legalPerson.LegalName = dto.LegalName;
            legalPerson.LegalAddress = dto.LegalAddress;
            legalPerson.Inn = dto.Rut;
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment;
            legalPerson.Timestamp = dto.Timestamp;
            legalPerson.CardNumber = dto.CardNumber;
        }

        private static void CopyPartFields(ChileLegalPersonDomainEntityDto dto, LegalPersonPart legalPersonPart)
        {
            legalPersonPart.OperationsKind = dto.OperationsKind;
            legalPersonPart.CommuneId = dto.CommuneRef.Id.Value;
        }
    }
}
