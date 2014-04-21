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
    public class UkraineLegalPersonProfileObtainerFlex : IBusinessModelEntityObtainerFlex<LegalPersonProfile>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<UkraineLegalPersonProfilePart> _legalPersonProfilerPartConverter;

        public UkraineLegalPersonProfileObtainerFlex(IFinder finder,
                                                     IBusinessEntityPropertiesConverter<UkraineLegalPersonProfilePart> legalPersonProfilerPartConverter)
        {
            _finder = finder;
            _legalPersonProfilerPartConverter = legalPersonProfilerPartConverter;
        }

        public void CopyPartFields(LegalPersonProfile target, IDomainEntityDto dto)
        {
            var source = (UkraineLegalPersonProfileDomainEntityDto)dto;
            var legalPersonProfilePart = target.UkrainePart();

            legalPersonProfilePart.Mfo = source.Mfo;
        }

        public IEntityPart[] GetEntityParts(LegalPersonProfile entity)
        {
            var legalPersonProfilePart = entity.IsNew()
                                       ? new UkraineLegalPersonProfilePart()
                                       : _finder.SingleOrDefault(entity.Id, _legalPersonProfilerPartConverter);

            return new IEntityPart[] { legalPersonProfilePart };
        }
    }
}