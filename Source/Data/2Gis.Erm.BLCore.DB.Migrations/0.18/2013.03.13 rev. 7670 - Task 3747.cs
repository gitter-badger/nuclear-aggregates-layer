using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7670, "Включаем доп. услуги для фирм")]
    public sealed class Migration7670 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            ClearTables(context);

            DropForeignKey(context);
            DropIdColumn(context);
            CreateIdColumn(context);
            CreateForeignKey(context);

            CreateDecriptionColumn(context);
            FillTable(context);
        }

        private static void ClearTables(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
TRUNCATE TABLE BusinessDirectory.FirmAddressServices
DELETE FROM Integration.AdditionalFirmServices
");
        }

        private static void FillTable(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
INSERT INTO Integration.AdditionalFirmServices (ServiceCode, Description) VALUES
('flamp', 'Cсылка на Фламп'),
('payment_type', 'Способ оплаты'),

('avg_price', 'Средний чек (Общепит)'),
('wifi', 'Наличие Wi-Fi (Общепит)'),
('business_lunch', 'Наличие бизнес-ланча (Общепит)'),
('zon', 'z-o-n.ru - Заказать столик (Общепит)'),
('bestreserve', 'gotable.ru - Заказать столик (Общепит)'),
('tomesto', 'tomesto.ru - Заказать столик (Общепит)'),

('hotel_class', 'Количество звёзд (Гостиницы)'),
('rooms_count', 'Количествово номеров (Гостиницы)'),
('price', 'Стоимость номера (Гостиницы)'),
('hotel_internet', 'Наличие интернета (Гостиницы)'),
('room_internet', 'Наличие интернета в номере (Гостиницы)'),
('smoking', 'Курение запрещено (Гостиницы)'),
('dist_to_city_center', 'Расстояние до центра города (Гостиницы)'),
('booking', 'booking.com - Забронировать (Гостиницы)'),

('tire_storage', 'Хранение шин (Шиномонтаж)'),
('tire_mobile', 'Выездной шиномонтаж (Шиномонтаж)'),
('tire_change_price', 'Стоимость смены резины на все колеса (Шиномонтаж)'),
('tire_prerecord', 'Предварительная запись (Шиномонтаж)'),
('tire_rolling_disk', 'Прокатка дисков (Шиномонтаж)'),
('tire_cargo', 'Грузовой шиномонтаж (Шиномонтаж)'),
('tire_studding', 'Ошиповка (Шиномонтаж)'),
('tire_repair_lateral_cuts', 'Ремонт боковых порезов (Шиномонтаж)'),
('tire_moto', 'Шиномонтаж мототехники (Шиномонтаж)'),
('tire_retreading', 'Восстановление протектора (Шиномонтаж)'),
('tire_passenger', 'Шиномонтаж легковой (Шиномонтаж)'),

('carwash_chemical', 'Химчистка (Автомойки)'),
('carwash_automatic', 'Автоматическая мойка (Автомойки)'),
('carwash_hand', 'Ручная мойка (Автомойки)'),
('carwash_truck', 'Мойка грузовых автомобилей (Автомойки)'),
('carwash_cafe', 'Зал ожидания / кафе (Автомойки)'),
('carwash_wifi', 'Наличие Wi-Fi (Автомойки)'),

('timeliner', 'timeliner.ru - Записаться'),
('gbooking', 'gbooking.ru - Записаться'),
('sonline', 'sonline.su - Записаться (Салоны красоты)'),
('moresalonov', 'moresalonov.ru - Записаться (Салоны красоты)'),
('leverans', 'leverans.ru - Оформить заказ (Доставка еды)'),
('mangorin', 'mangorin.ru - Оформить заказ (Доставка еды)'),
('rambler_kassa', 'kassa.rambler.ru - Купить билеты (Кинотеатры)'),
('taximaster', 'taximaster.ru - Заказать такси (Такси)'),
('tariff_master', 'tffm.ru - Купить билеты (Авиабилеты)'),
('vsetreningi', 'vsetreningi.ru - Выбрать тренинг (Тренинги)'),
('all_evak', 'all-evak.ru - Вызвать эвакуатор (Эвакуаторы)'),
('med_room', 'med-room.com - Записаться (Медицина)')
");
        }

        private static void CreateForeignKey(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmAddressServices", ErmSchemas.BusinessDirectory];
            var foreignKey = table.ForeignKeys["FK_FirmAddressServices_AdditionalFirmServices"];

            if (foreignKey != null)
            {
                return;
            }

            foreignKey = new ForeignKey(table, "FK_FirmAddressServices_AdditionalFirmServices");
            foreignKey.Columns.Add(new ForeignKeyColumn(foreignKey, "ServiceId", "Id"));
            foreignKey.ReferencedTable = "AdditionalFirmServices";
            foreignKey.ReferencedTableSchema = ErmSchemas.Integration;

            foreignKey.Create();
        }

        private static void CreateIdColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["AdditionalFirmServices", ErmSchemas.Integration];
            var column = table.Columns["Id"];

            if (column != null)
            {
                return;
            }

            column = new Column(table, "Id", DataType.Int) { Identity = true };
            column.Create();

            var pkIndex = new Index(table, "PK_AdditionalFirmServices");
            pkIndex.IndexedColumns.Add(new IndexedColumn(pkIndex, "Id"));
            pkIndex.IndexKeyType = IndexKeyType.DriPrimaryKey;
            pkIndex.Create();
        }

        private static void DropForeignKey(IMigrationContext context)
        {
            var table = context.Database.Tables["FirmAddressServices", ErmSchemas.BusinessDirectory];
            var foreignKey = table.ForeignKeys["FK_FirmAddressServices_AdditionalFirmServices"];

            if (foreignKey == null)
            {
                return;
            }

            foreignKey.Drop();
        }

        private static void DropIdColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["AdditionalFirmServices", ErmSchemas.Integration];
            var column = table.Columns["Id"];

            if (column == null)
            {
                return;
            }

            var pkIndex = table.Indexes["PK_AdditionalFirmServices"];
            if (pkIndex == null)
            {
                return;
            }

            pkIndex.Drop();

            column.Drop();
        }

        private static void CreateDecriptionColumn(IMigrationContext context)
        {
            var table = context.Database.Tables["AdditionalFirmServices", ErmSchemas.Integration];

            var column = table.Columns["Description"];
            if (column != null)
            {
                return;
            }

            column = new Column(table, "Description", DataType.NVarChar(200)) { Nullable = true };
            column.Create();
        }
    }
}
