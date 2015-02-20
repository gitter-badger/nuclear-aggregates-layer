namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    /// <summary>
    /// Перечисление групп проверок заказов
    /// </summary>
    public enum OrderValidationRuleGroup
    {
        /// <summary>
        /// Общая группа. Предназначена для проверок, не попавших в остальные группы
        /// </summary>
        Generic = 1,

        /// <summary>
        /// Группа проверок на рекламные материалы
        /// </summary>
        AdvertisementMaterialsValidation = 2,

        /// <summary>
        /// Группа для проверки на сопутствующие и запрещенные позиции
        /// </summary>
        ADPositionsValidation = 3,

        /// <summary>
        /// Группа для проверки на количество рекламы
        /// </summary>
        AdvertisementAmountValidation = 4,

        /// <summary>
        /// Группа для проверок по моделям продаж
        /// </summary>
        SalesModelValidation = 5
    }
}
