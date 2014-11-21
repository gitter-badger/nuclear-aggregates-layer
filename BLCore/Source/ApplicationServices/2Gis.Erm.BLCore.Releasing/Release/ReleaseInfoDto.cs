using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public class ReleaseInfoDto
    {
        public ReleaseInfo Release { get; set; }
        public string OrganizationUnitName { get; set; }
        public TimePeriod Period { get; set; }
    }
}