using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportCityPhoneZoneIdentity : OperationIdentityBase<ImportCityPhoneZoneIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCityPhoneZoneIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowPhoneZones.CityPhoneZone"; }
        }
    }
}