using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public class NullBranchOfficeOrganizationUnitObtainerFlex : IBusinessModelEntityObtainerFlex<BranchOfficeOrganizationUnit>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
    {
        public void CopyPartFields(BranchOfficeOrganizationUnit target, IDomainEntityDto dto)
        {
        }

        public IEntityPart[] GetEntityParts(BranchOfficeOrganizationUnit entity)
        {
            return new IEntityPart[0];
        }
    }
}
