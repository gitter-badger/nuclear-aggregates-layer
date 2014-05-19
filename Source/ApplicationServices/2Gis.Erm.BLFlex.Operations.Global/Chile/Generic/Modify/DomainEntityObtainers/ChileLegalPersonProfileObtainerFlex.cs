using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileLegalPersonProfileObtainerFlex : IBusinessModelEntityObtainerFlex<LegalPersonProfile>, IChileAdapted
    {
        private readonly IBusinessEntityPropertiesConverter<ChileLegalPersonProfilePart> _partPropertiesConverter;
        private readonly IFinder _finder;

        public ChileLegalPersonProfileObtainerFlex(
            IFinder finder,
            IBusinessEntityPropertiesConverter<ChileLegalPersonProfilePart> partPropertiesConverter)
        {
            _partPropertiesConverter = partPropertiesConverter;
            _finder = finder;
        }

        public IEntityPart[] GetEntityParts(LegalPersonProfile entity)
        {
             var part =  entity.IsNew()
                       ? new ChileLegalPersonProfilePart()
                       : _finder.SingleOrDefault(entity.Id, _partPropertiesConverter);

            return new IEntityPart[] { part };
        }

        public void CopyPartFields(LegalPersonProfile target, IDomainEntityDto dto)
        {
            var source = (ChileLegalPersonProfileDomainEntityDto)dto;
            var entityPart = target.ChilePart();

            entityPart.AccountType = source.AccountType;
            entityPart.BankId = source.BankRef.Id;
            entityPart.RepresentativeRut = source.RepresentativeRut;
            entityPart.RepresentativeAuthorityDocumentIssuedBy = source.RepresentativeDocumentIssuedBy;
            entityPart.RepresentativeAuthorityDocumentIssuedOn = source.RepresentativeDocumentIssuedOn;
        }
    }
}
