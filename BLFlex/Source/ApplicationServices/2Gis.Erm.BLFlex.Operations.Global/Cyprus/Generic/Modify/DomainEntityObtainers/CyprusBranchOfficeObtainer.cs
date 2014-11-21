﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Cyprus;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.DomainEntityObtainers
{
    public class CyprusBranchOfficeObtainer : IBusinessModelEntityObtainer<BranchOffice>, IAggregateReadModel<BranchOffice>, ICyprusAdapted
    {
        private readonly IFinder _finder;

        public CyprusBranchOfficeObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BranchOffice ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CyprusBranchOfficeDomainEntityDto)domainEntityDto;
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(dto.Id)) 
                ?? new BranchOffice { IsActive = true };

            BranchOfficeFlexSpecs.BranchOffices.Cyprus.Assign.Entity().Assign(dto, branchOffice);

            return branchOffice;
        }
    }
}
