﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify.DomainEntityObtainers
{
    public sealed class EmiratesBranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>, IEmiratesAdapted
    {
        private readonly IFinder _finder;

        public EmiratesBranchOfficeOrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (EmiratesBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id)) 
                ?? new BranchOfficeOrganizationUnit { IsActive = true };

            BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Emirates.Assign.Entity().Assign(dto, branchOfficeOrganizationUnit);

            return branchOfficeOrganizationUnit;
        }
    }
}