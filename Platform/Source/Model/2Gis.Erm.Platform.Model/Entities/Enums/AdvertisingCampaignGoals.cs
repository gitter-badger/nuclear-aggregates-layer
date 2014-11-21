using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    [Flags]
    public enum AdvertisingCampaignGoals
    {
        Undefined = 0,

        // Увеличение продаж
        IncreaseSales = 1,

        // Привлечение ЦА на сайт клиента
        AttractAudienceToSite = 2,

        // Увеличение количества звонков
        IncreasePhoneCalls = 4,

        // Увеличение узнаваемости ТМ
        IncreaseBrandAwareness = 8
    }
}
