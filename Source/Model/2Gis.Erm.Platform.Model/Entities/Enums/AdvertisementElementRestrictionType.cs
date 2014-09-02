namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum AdvertisementElementRestrictionType
    {
        Text = 1,
        Article = 2,
        Image = 3,
        FasComment = 4,
        Date = 5,
    }

    // Тип обязан своим существованием невозможностью разделить значение Text в AdvertisementElementRestrictionType
    public enum AdvertisementElementRestrictionActualType
    {
        File = 2,
        Image = 3,
        FasComment = 4,
        Date = 5,

        PlainText = 100,
        FormattedText = 101,
        Link = 102,
    }
}