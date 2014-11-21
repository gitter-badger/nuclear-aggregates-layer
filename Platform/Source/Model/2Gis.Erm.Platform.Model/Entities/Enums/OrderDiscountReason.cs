namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // причина сдидки
    public enum OrderDiscountReason
    {
        None = 0,

        // причина сдидки - рекламная акция (купил квартиру, получи кепку)
        AdvertisingCampaign = 1,

        // причина сдидки - торг (выторговал подешевле)
        Bargain = 2,

        // причина сдидки - другая
        Other = 3
    }
}