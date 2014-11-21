namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // enum is syncronized with 1C, do not change enum values
    [UndefinedEnumValue(-1)]
    public enum DocumentsDeliveryMethod
    {
        Undefined = -1,
        DeliveryByManager = 0,
        PostOnly = 1,
        DeliveryByClient = 2,
        ByEmail = 3,
        ByCourier = 4
    }
}