using System;
using System.Linq;

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
    public class ChileBranchOfficeOrganizationUnitObtainerFlex : IBusinessModelEntityObtainerFlex<BranchOfficeOrganizationUnit>, IChileAdapted
    {
        private readonly IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> _partPropertiesConverter;
        private readonly IFinder _finder;

        public ChileBranchOfficeOrganizationUnitObtainerFlex(
            IFinder finder,
            IBusinessEntityPropertiesConverter<ChileBranchOfficeOrganizationUnitPart> partPropertiesConverter)
        {
            _partPropertiesConverter = partPropertiesConverter;
            _finder = finder;
        }

        public IEntityPart[] GetEntityParts(BranchOfficeOrganizationUnit entity)
        {
             var part =  entity.IsNew()
                       ? new ChileBranchOfficeOrganizationUnitPart()
                       : _finder.SingleOrDefault(entity.Id, _partPropertiesConverter);

            return new IEntityPart[] { part };
            }

        public void CopyPartFields(BranchOfficeOrganizationUnit target, IDomainEntityDto dto)
        {
            var source = (ChileBranchOfficeOrganizationUnitDomainEntityDto)dto;
            var entityPart = target.ChilePart();

            entityPart.RepresentativeRut = source.RepresentativeRut; // RUT представителя
        }
    }
}
