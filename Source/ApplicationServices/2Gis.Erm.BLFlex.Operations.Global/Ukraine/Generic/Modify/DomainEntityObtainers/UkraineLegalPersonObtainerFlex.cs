using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineLegalPersonObtainerFlex : IBusinessModelEntityObtainerFlex<LegalPerson>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<UkraineLegalPersonPart> _legalPersonPartConverter;

        public UkraineLegalPersonObtainerFlex(IFinder finder, IBusinessEntityPropertiesConverter<UkraineLegalPersonPart> legalPersonPartConverter)
        {
            _finder = finder;
            _legalPersonPartConverter = legalPersonPartConverter;
        }

        public void CopyPartFields(LegalPerson target, IDomainEntityDto dto)
        {
            var source = (UkraineLegalPersonDomainEntityDto)dto;
            var legalPersonProfilePart = target.UkrainePart();

            legalPersonProfilePart.Egrpou = source.Egrpou;
            legalPersonProfilePart.TaxationType = source.TaxationType;
        }

        public IEntityPart[] GetEntityParts(LegalPerson entity)
        {
            var legalPersonProfilePart = entity.IsNew()
                                       ? new UkraineLegalPersonPart()
                                       : _finder.SingleOrDefault(entity.Id, _legalPersonPartConverter);

            return new IEntityPart[] { legalPersonProfilePart };
        }
    }
}