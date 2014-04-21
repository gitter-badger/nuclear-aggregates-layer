using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public class NullLegalPersonObtainerFlex : IBusinessModelEntityObtainerFlex<LegalPerson>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public void CopyPartFields(LegalPerson target, IDomainEntityDto dto)
        {
        }

        public IEntityPart[] GetEntityParts(LegalPerson entity)
        {
            return new IEntityPart[0];
        }
    }
}