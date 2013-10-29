namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // объект привязки
    public enum PositionBindingObjectType
    {
        None = 0,

        // фирма
        Firm = 9,

        // рубрика (единственная, множественная, множественная со звёздочкой)
        CategorySingle = 33,
        CategoryMultiple = 34,
        CategoryMultipleAsterix = 1,

        // адрес фирмы (единственный, множественный)
        AddressSingle = 6,
        AddressMultiple = 35,

        // рубрика адреса (единственная, множественная, первого уровня единственная, первого уровня множественная)
        AddressCategorySingle = 7,
        AddressCategoryMultiple = 8,
        AddressFirstLevelCategorySingle = 36,
        AddressFirstLevelCategoryMultiple = 37,

        // тематика
        ThemeMultiple = 40
    }
}