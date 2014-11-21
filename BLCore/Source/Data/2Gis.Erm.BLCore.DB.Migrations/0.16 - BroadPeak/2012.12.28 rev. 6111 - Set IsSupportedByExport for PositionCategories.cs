using System.Text;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6111, "Заполнение колонки 'поддерживается экспортом' в таблице PositionCategories")]
    public class Migration6111 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var names = new[]
            {
                "Приоритет в рубрике",
                "Рекламный модуль под окном справочника",
                "Рекламная статья",
                "Логотип на карте",
                "Комментарий к фирме",
                "Комментарий к адресу",
                "Баннер в рубрике",
                "Рекламный модуль на стартовой заставке",
                "Рекламный модуль на финальной заставке",
                "Рекламный модуль в окне карты",
                "Рекламная ссылка",
                "Бюджет",
                "Объявления в рубрике",
                "Абонентская плата в API/Mobile",
                "Тематические подборки",
                "Подключение баннера к рубрике",
                "Дополнительный макет для баннера",
                "Микрокомментарий в транспорте",
                "Абонентская плата в Мобильной версии",
                "Выгодные покупки с 2ГИС",
                "Дополнительный текст для микрокомментария",
                "Подключение микрокомментария к дополнительной рубрике",
                "Маркеры на карте",
                "Объявление в рубрике(Объявление под списком выдачи)",
                "Микрокомментарий в рубрике(Микрокомментарий к свёрнутой карточке в справочнике)",
                "Реклама в печати",
                "Дополнительная рубрика",
                "Нанесение объектов на карту",
                "Собственная рекламная рубрика",
                "Ссылка на Фламп для рекламодателей",
                "Самореклама только для ПК",
                "Бонусная информация",
                "Абонентская плата Mobile",
                "Контекстный модуль под окном справочника",
                "Баннер в рубрике Выгодные покупки с 2ГИС",
                "Старт в он-лайн версии",
                "Абонентская плата в API/Mobile (Продукты 2ГИС)"
            };

            var queryBuilder = new StringBuilder("UPDATE [Billing].[PositionCategories] SET IsSupportedByExport = 1 WHERE Name in (");
            foreach (var name in names)
            {
                queryBuilder.AppendFormat("'{0}',", name);
            }

            queryBuilder.Remove(queryBuilder.Length - 1, 1);
            queryBuilder.Append(")");

            context.Connection.ExecuteNonQuery(queryBuilder.ToString());
        }
    }
}
