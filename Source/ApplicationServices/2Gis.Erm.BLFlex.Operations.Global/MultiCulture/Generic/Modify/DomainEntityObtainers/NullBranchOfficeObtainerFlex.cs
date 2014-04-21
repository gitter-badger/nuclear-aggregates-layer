using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public class NullBranchOfficeObtainerFlex : IBusinessModelEntityObtainerFlex<BranchOffice>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IChileAdapted
    {
        public void CopyPartFields(BranchOffice target, IDomainEntityDto dto)
        {
        }

        public IEntityPart[] GetEntityParts(BranchOffice entity)
        {
            return new IEntityPart[0];
        }
    }
}