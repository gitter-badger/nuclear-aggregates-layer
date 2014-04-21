using System.Globalization;

namespace DoubleGis.Erm.Platform.Common.Utils.Resources
{
    public static class ResourceGroupManager
    {
        public static void SetCulture(CultureInfo cultureInfo)
        {
            // TODO {all, 11.04.2014}: фикс в рамках бага. В будущем стоит подумать о более элегантном методе просталения культуры ресурсникам в Backend сервисах
            BL.Resources.Server.Properties.Resources.Culture = cultureInfo;
            BLCore.Resources.Server.Properties.BLResources.Culture = cultureInfo;
            BLCore.Resources.Server.Properties.MetadataResources.Culture = cultureInfo;
            BLCore.Resources.Server.Properties.EnumResources.Culture = cultureInfo;
            BLFlex.Resources.Server.Properties.BLResources.Culture = cultureInfo;
        }
    }
}
