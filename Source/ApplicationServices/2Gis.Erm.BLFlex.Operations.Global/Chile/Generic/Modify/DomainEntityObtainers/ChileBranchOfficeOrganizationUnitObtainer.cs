using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileBranchOfficeOrganizationUnitObtainer : 
        DynamicBranchOfficeOrganizationUnitObtainerBase<ChileBranchOfficeOrganizationUnitDomainEntityDto, ChileBranchOfficeOrganizationUnitPart>,
        IChileAdapted
    {
        public ChileBranchOfficeOrganizationUnitObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override BranchOfficeOrganizationUnit CreateEntity()
        {
            return new BranchOfficeOrganizationUnit { IsActive = true, Parts = new[] { new ChileBranchOfficeOrganizationUnitPart() } };
        }

        protected override IAssignSpecification<ChileBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Chile.Assign.Entity();
        }
    }
}
