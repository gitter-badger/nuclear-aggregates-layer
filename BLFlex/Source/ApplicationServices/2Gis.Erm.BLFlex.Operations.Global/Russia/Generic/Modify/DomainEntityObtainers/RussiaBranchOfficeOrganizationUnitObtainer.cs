﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public class RussiaBranchOfficeOrganizationUnitObtainer : IBusinessModelEntityObtainer<BranchOfficeOrganizationUnit>, IAggregateReadModel<BranchOffice>, IRussiaAdapted
    {
        private readonly IFinder _finder;

        public RussiaBranchOfficeOrganizationUnitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOfficeOrganizationUnit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (RussiaBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(dto.Id)) 
                ?? new BranchOfficeOrganizationUnit { IsActive = true };

            BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Russia.Assign.Entity().Assign(dto, branchOfficeOrganizationUnit);

            return branchOfficeOrganizationUnit;
        }
    }
}