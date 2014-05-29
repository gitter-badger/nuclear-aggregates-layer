﻿using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.DomainEntityObtainers
{
    public class CzechBranchOfficeOrganizationUnitObtainer : BranchOfficeOrganizationUnitObtainerBase<CzechBranchOfficeOrganizationUnitDomainEntityDto>, ICzechAdapted
    {
        public CzechBranchOfficeOrganizationUnitObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<CzechBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Czech.Assign.Entity();
        }
    }
}