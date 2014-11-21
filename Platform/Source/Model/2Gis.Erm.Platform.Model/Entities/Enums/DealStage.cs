namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum DealStage
    {
        None = 0,

        // -- stages syncronized with dynamics crm

        // Сбор информации
        CollectInformation = 1,

        // Проведение презентации
        HoldingProductPresentation = 3,

        // Согласование КП
        MatchAndSendProposition = 5,

        // -- non-dynamics crm stages

        // Сформирован Заказ
        OrderFormed = 7,

        // Заказ одобрен в Выпуск
        OrderApprovedForRelease = 8,

        // Сервис
        Service = 9,
    }
}