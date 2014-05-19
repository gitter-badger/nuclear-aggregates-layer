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
    public class ChileLegalPersonObtainerFlex : IBusinessModelEntityObtainerFlex<LegalPerson>, IChileAdapted
    {
        private readonly IBusinessEntityPropertiesConverter<ChileLegalPersonPart> _partPropertiesConverter;
        private readonly IFinder _finder;

        public ChileLegalPersonObtainerFlex(
            IFinder finder,
            IBusinessEntityPropertiesConverter<ChileLegalPersonPart> partPropertiesConverter)
        {
            _partPropertiesConverter = partPropertiesConverter;
            _finder = finder;
        }

        public IEntityPart[] GetEntityParts(LegalPerson entity)
        {
            var part = entity.IsNew()
                           ? new ChileLegalPersonPart()
                           : _finder.SingleOrDefault(entity.Id, _partPropertiesConverter);

            return new IEntityPart[] { part };
        }

        public void CopyPartFields(LegalPerson target, IDomainEntityDto dto)
        {
            var source = (ChileLegalPersonDomainEntityDto)dto;
            var entityPart = target.ChilePart();

            entityPart.OperationsKind = source.OperationsKind;
            entityPart.CommuneId = source.CommuneRef.Id.Value;
        }
    }
}