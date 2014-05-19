﻿using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
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
    public sealed class UkraineBranchOfficeObtainerFlex : IBusinessModelEntityObtainerFlex<BranchOffice>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly IBusinessEntityPropertiesConverter<UkraineBranchOfficePart> _branchOfficePartConverter;

        public UkraineBranchOfficeObtainerFlex(IFinder finder,
            IBusinessEntityPropertiesConverter<UkraineBranchOfficePart> branchOfficePartConverter)
        {
            _finder = finder;
            _branchOfficePartConverter = branchOfficePartConverter;
        }

        public void CopyPartFields(BranchOffice target, IDomainEntityDto dto)
        {
            var source = (UkraineBranchOfficeDomainEntityDto)dto;
            var branchOfficePart = target.UkrainePart();

            branchOfficePart.Ipn = source.Ipn;
        }

        public IEntityPart[] GetEntityParts(BranchOffice entity)
        {
            var branchOfficePart = entity.IsNew()
                                       ? new UkraineBranchOfficePart()
                                       : _finder.SingleOrDefault(entity.Id, _branchOfficePartConverter);

            return new IEntityPart[] { branchOfficePart };
        }
    }
}
