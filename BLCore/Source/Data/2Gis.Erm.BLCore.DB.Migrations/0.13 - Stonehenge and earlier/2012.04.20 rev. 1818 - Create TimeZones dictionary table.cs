using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1818, "Создание таблицы справочника для TimeZone. " +
                           "Перевод всех таблиц использующих timezoneid в виде строк на использование справочника.")]
    // ReSharper disable InconsistentNaming
    public class Migration_1818 : TransactedMigration
    // ReSharper restore InconsistentNaming
    {
        protected override void RevertOverride(IMigrationContext context)
        {
            throw new NotImplementedException();
        }

        private const String CheckIsMigrationAlreadyAppliedTemplate = "select count(*) from {0}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.Tables.Contains(ErmTableNames.TimeZones.Name, ErmTableNames.TimeZones.Schema))
            {
                var result = context.Connection.ExecuteScalar(String.Format(CheckIsMigrationAlreadyAppliedTemplate, (ErmTableNames.TimeZones)));
                if (result != null)
                {
                    var count = Convert.ToInt32(result);
                    if (count > 0)
                    {
                        // таблица справочник TimeZone уже есть, она заполнена (т.е. не является результатом deploy ERM dbproj) => ничего не делаем
                        return;
                    }
                }
            }
            else
            {
                CreateTimeZonesTable(context);
            }

            FillTimeZonesTable(context);
            ConvertOrganizationUnitsTimeZoneIdColumn(context);
            ConvertUserProfilesTimeZoneInfoIdColumn(context);
        }

        private const String TimeZonesIdColumnName = "Id";
        private const String TimeZonesTimeZoneIdColumnName = "TimeZoneId";
        private const String TimeZonesDescriptionColumnName = "Description";
        private const String TimeZonesCreatedByColumnName = "CreatedBy";
        private const String TimeZonesCreatedOnColumnName = "CreatedOn";
        private const String TimeZonesModifiedByColumnName = "ModifiedBy";
        private const String TimeZonesModifiedOnColumnName = "ModifiedOn";
        private const String TimeZonesTimestampColumnName = "Timestamp";

        private void CreateTimeZonesTable(IMigrationContext context)
        {
            var timeZonesTable = new Table(context.Database, ErmTableNames.TimeZones.Name, ErmTableNames.TimeZones.Schema);
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesIdColumnName, DataType.Int) { Nullable = false });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesTimeZoneIdColumnName, DataType.NVarChar(1024)) { Nullable = false });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesDescriptionColumnName, DataType.NVarChar(1024)) { Nullable = true });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesCreatedByColumnName, DataType.Int) { Nullable = false });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesCreatedOnColumnName, DataType.DateTime2(2)) { Nullable = false });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesModifiedByColumnName, DataType.Int) { Nullable = true });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesModifiedOnColumnName, DataType.DateTime2(2)) { Nullable = true });
            timeZonesTable.Columns.Add(new Column(timeZonesTable, TimeZonesTimestampColumnName, DataType.Timestamp) { Nullable = false });

            timeZonesTable.Create();
            var primaryKey = new Index(timeZonesTable, "PK_" + ErmTableNames.TimeZones.Name + "_" + TimeZonesIdColumnName);
            var primaryKeyIndexColumn = new IndexedColumn(primaryKey, TimeZonesIdColumnName);
            primaryKey.IndexedColumns.Add(primaryKeyIndexColumn);
            primaryKey.IndexKeyType = IndexKeyType.DriPrimaryKey;
            primaryKey.Create();
            var uniqueKey = new Index(timeZonesTable, "UQ_" + ErmTableNames.TimeZones.Name + "_" + TimeZonesTimeZoneIdColumnName);
            var uniqueKeyIndexColumn = new IndexedColumn(uniqueKey, TimeZonesTimeZoneIdColumnName);
            uniqueKey.IndexedColumns.Add(uniqueKeyIndexColumn);
            uniqueKey.IndexKeyType = IndexKeyType.DriUniqueKey;
            uniqueKey.Create();
        }

        private const String TimeZonesDesrciptionsInsertTemplate = 
            "INSERT INTO {0} " +
            "(Id, TimeZoneId, Description, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn) " +
            "VALUES ({1}, '{2}', '{3}', 1, '{4}', NULL, NULL)\n";

        #region TimeZonesList
        // список timezone которые возвращает .NET через TimeZoneInfo.GetSystemTimeZones() зависит от версии ОС где исполняется код, набора установленных обновлений и т.д.
        // чтобы избежать проблем вида английская винда, без новых обновлений и т.д. - например, нет Kaliningrad Standard Time
        // список timezone слит из windows 7 x64 rus с обновлениями до конца апреля 2012
        private readonly List<Tuple<string, string, int>> _systemIndependentTimeZonesList = 
            new List<Tuple<string, string, int>> {
                    new Tuple<string, string, int>("Dateline Standard Time","(UTC-12:00) Линия перемены дат", -(12*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("UTC-11","(UTC-11:00) Время в формате UTC -11", -(11*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Samoa Standard Time","(UTC-11:00) Самоа", -(11*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Hawaiian Standard Time","(UTC-10:00) Гавайи", -(10*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Alaskan Standard Time","(UTC-09:00) Аляска", -(09*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Pacific Standard Time (Mexico)","(UTC-08:00) Нижняя Калифорния", -(08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Pacific Standard Time","(UTC-08:00) Тихоокеанское время (США и Канада)", -(08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("US Mountain Standard Time","(UTC-07:00) Аризона", -(07*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Mountain Standard Time","(UTC-07:00) Горное время (США и Канада)", -(07*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Mountain Standard Time (Mexico)","(UTC-07:00) Ла-Пас, Мазатлан, Чихуахуа", -(07*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central Standard Time (Mexico)","(UTC-06:00) Гвадалахара, Мехико, Монтеррей", -(06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Canada Central Standard Time","(UTC-06:00) Саскачеван", -(06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central America Standard Time","(UTC-06:00) Центральная Америка", -(06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central Standard Time","(UTC-06:00) Центральное время (США и Канада)", -(06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("SA Pacific Standard Time","(UTC-05:00) Богота, Кито, Лима", -(05*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Eastern Standard Time","(UTC-05:00) Восточное время (США и Канада)", -(05*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("US Eastern Standard Time","(UTC-05:00) Индиана (восток)", -(05*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Venezuela Standard Time","(UTC-04:30) Каракас", -(04*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("Paraguay Standard Time","(UTC-04:00) Асунсьон", -(04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Atlantic Standard Time","(UTC-04:00) Атлантическое время (Канада)", -(04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("SA Western Standard Time","(UTC-04:00) Джорджтаун, Ла-Пас, Манаус, Сан-Хуан", -(04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central Brazilian Standard Time","(UTC-04:00) Куяба", -(04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Pacific SA Standard Time","(UTC-04:00) Сантьяго", -(04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Newfoundland Standard Time","(UTC-03:30) Ньюфаундленд", -(03*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("E. South America Standard Time","(UTC-03:00) Бразилия", -(03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Argentina Standard Time","(UTC-03:00) Буэнос-Айрес", -(03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Greenland Standard Time","(UTC-03:00) Гренландия", -(03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("SA Eastern Standard Time","(UTC-03:00) Кайенна, Форталеза", -(03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Montevideo Standard Time","(UTC-03:00) Монтевидео", -(03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("UTC-02","(UTC-02:00) Время в формате UTC -02", -(02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Mid-Atlantic Standard Time","(UTC-02:00) Среднеатлантическое время", -(02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Azores Standard Time","(UTC-01:00) Азорские о-ва", -(01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Cape Verde Standard Time","(UTC-01:00) О-ва Зеленого Мыса", -(01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("UTC","(UTC) Время в формате UTC", (00*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("GMT Standard Time","(UTC) Дублин, Лиссабон, Лондон, Эдинбург", (00*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Morocco Standard Time","(UTC) Касабланка", (00*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Greenwich Standard Time","(UTC) Монровия, Рейкьявик", (00*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("W. Europe Standard Time","(UTC+01:00) Амстердам, Берлин, Берн, Вена, Рим, Стокгольм", (01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central Europe Standard Time","(UTC+01:00) Белград, Братислава, Будапешт, Любляна, Прага", (01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Romance Standard Time","(UTC+01:00) Брюссель, Копенгаген, Мадрид, Париж", (01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central European Standard Time","(UTC+01:00) Варшава, Загреб, Сараево, Скопье", (01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Namibia Standard Time","(UTC+01:00) Виндхук", (01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("W. Central Africa Standard Time","(UTC+01:00) Западная Центральная Африка", (01*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Jordan Standard Time","(UTC+02:00) Амман", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("GTB Standard Time","(UTC+02:00) Афины, Бухарест", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Middle East Standard Time","(UTC+02:00) Бейрут", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("FLE Standard Time","(UTC+02:00) Вильнюс, Киев, Рига, София, Таллин, Хельсинки", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Syria Standard Time","(UTC+02:00) Дамаск", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Israel Standard Time","(UTC+02:00) Иерусалим", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Egypt Standard Time","(UTC+02:00) Каир", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("E. Europe Standard Time","(UTC+02:00) Минск", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Turkey Standard Time","(UTC+02:00) Стамбул", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("South Africa Standard Time","(UTC+02:00) Хараре, Претория", (02*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Arabic Standard Time","(UTC+03:00) Багдад", (03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Kaliningrad Standard Time","(UTC+03:00) Калининград", (03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Arab Standard Time","(UTC+03:00) Кувейт, Эр-Рияд", (03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("E. Africa Standard Time","(UTC+03:00) Найроби", (03*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Iran Standard Time","(UTC+03:30) Тегеран", (03*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("Arabian Standard Time","(UTC+04:00) Абу-Даби, Мускат", (04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Azerbaijan Standard Time","(UTC+04:00) Баку", (04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Russian Standard Time","(UTC+04:00) Волгоград, Москва, Санкт-Петербург", (04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Caucasus Standard Time","(UTC+04:00) Ереван", (04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Mauritius Standard Time","(UTC+04:00) Порт-Луи", (04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Georgian Standard Time","(UTC+04:00) Тбилиси", (04*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Afghanistan Standard Time","(UTC+04:30) Кабул", (04*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("Pakistan Standard Time","(UTC+05:00) Исламабад, Карачи", (05*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("West Asia Standard Time","(UTC+05:00) Ташкент", (05*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("India Standard Time","(UTC+05:30) Колката, Мумбаи, Нью-Дели, Ченнай", (05*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("Sri Lanka Standard Time","(UTC+05:30) Шри-Джаявардене-пура-Котте", (05*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("Nepal Standard Time","(UTC+05:45) Катманду", (05*3600 + 45*60 + 00)),
                    new Tuple<string, string, int>("Central Asia Standard Time","(UTC+06:00) Астана", (06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Bangladesh Standard Time","(UTC+06:00) Дакка", (06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Ekaterinburg Standard Time","(UTC+06:00) Екатеринбург", (06*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Myanmar Standard Time","(UTC+06:30) Янгон", (06*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("SE Asia Standard Time","(UTC+07:00) Бангкок, Джакарта, Ханой", (07*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("N. Central Asia Standard Time","(UTC+07:00) Новосибирск", (07*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("China Standard Time","(UTC+08:00) Гонконг, Пекин, Урумчи, Чунцин", (08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("North Asia Standard Time","(UTC+08:00) Красноярск", (08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Singapore Standard Time","(UTC+08:00) Куала-Лумпур, Сингапур", (08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("W. Australia Standard Time","(UTC+08:00) Перт", (08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Taipei Standard Time","(UTC+08:00) Тайбэй", (08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Ulaanbaatar Standard Time","(UTC+08:00) Улан-Батор", (08*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("North Asia East Standard Time","(UTC+09:00) Иркутск", (09*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Tokyo Standard Time","(UTC+09:00) Осака, Саппоро, Токио", (09*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Korea Standard Time","(UTC+09:00) Сеул", (09*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Cen. Australia Standard Time","(UTC+09:30) Аделаида", (09*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("AUS Central Standard Time","(UTC+09:30) Дарвин", (09*3600 + 30*60 + 00)),
                    new Tuple<string, string, int>("E. Australia Standard Time","(UTC+10:00) Брисбен", (10*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("West Pacific Standard Time","(UTC+10:00) Гуам, Порт-Морсби", (10*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("AUS Eastern Standard Time","(UTC+10:00) Канберра, Мельбурн, Сидней", (10*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Tasmania Standard Time","(UTC+10:00) Хобарт", (10*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Yakutsk Standard Time","(UTC+10:00) Якутск", (10*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Vladivostok Standard Time","(UTC+11:00) Владивосток", (11*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Central Pacific Standard Time","(UTC+11:00) Соломоновы о-ва, Нов. Каледония", (11*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("New Zealand Standard Time","(UTC+12:00) Веллингтон, Окленд", (12*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("UTC+12","(UTC+12:00) Время в формате UTC +12", (12*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Magadan Standard Time","(UTC+12:00) Магадан", (12*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Kamchatka Standard Time","(UTC+12:00) Петропавловск-Камчатский — устаревшее", (12*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Fiji Standard Time","(UTC+12:00) Фиджи", (12*3600 + 00*60 + 00)),
                    new Tuple<string, string, int>("Tonga Standard Time","(UTC+13:00) Нукуалофа", (13*3600 + 00*60 + 00)), };
        #endregion

        private void FillTimeZonesTable(IMigrationContext context)
        {
            var sb = new StringBuilder();
            var orderDescendingByTimeOffest = _systemIndependentTimeZonesList.OrderByDescending(x => x.Item3).ToArray();
            var currentTime = DateTime.Now.ToString("yyyyMMdd");
            for (int index = 0; index < orderDescendingByTimeOffest.Length; index++)
            {
                var timeZone = orderDescendingByTimeOffest[index];
                sb.AppendFormat(TimeZonesDesrciptionsInsertTemplate, ErmTableNames.TimeZones, index + 1, timeZone.Item1, timeZone.Item2, currentTime);
            }

            context.Connection.ExecuteNonQuery(sb.ToString());
        }

        private const String UpdateTimeZoneDictionaryColumnTemplate = @"
UPDATE {0}
SET {0}.{1} = tz.Id
FROM {0} t
inner join [Shared].[TimeZones] tz
on t.{2} = tz.TimeZoneId";

        private void ConvertOrganizationUnitsTimeZoneIdColumn(IMigrationContext context)
        {
            const String originalColumnName = "TimeZoneId";
            const String targetColumnName = "TimeZoneId";
            const string tempColumnName = originalColumnName + "Temp";
            var table = context.Database.Tables[ErmTableNames.OrganizationUnits.Name, ErmTableNames.OrganizationUnits.Schema];
            int? index = table.Columns.IndexOf(originalColumnName);
            if (!index.HasValue)
            {
                throw new InvalidOperationException("Can't find column: " + originalColumnName + " in table: " + ErmTableNames.OrganizationUnits);
            }

            table = EntityCopyHelper.CopyAndInsertColumns(context.Database, table,
                                                  new List<InsertedColumnDefinition> { new InsertedColumnDefinition(index.Value, 
                                                      t=>new Column(t, tempColumnName,DataType.Int){Nullable = true}) });

            context.Connection.ExecuteNonQuery( String.Format(UpdateTimeZoneDictionaryColumnTemplate, 
                                                ErmTableNames.OrganizationUnits,
                                                tempColumnName,
                                                originalColumnName));
            var originalColumn = table.Columns[originalColumnName];
            originalColumn.Drop();

            var tempColumn = table.Columns[tempColumnName];
            tempColumn.Rename(targetColumnName);

            var foreignKey = new ForeignKey(table, "FK_OrganizationUnit_TimeZone");
            var foreignKeyColumn = new ForeignKeyColumn(foreignKey, targetColumnName, "Id");
            foreignKey.Columns.Add(foreignKeyColumn);
            foreignKey.ReferencedTable = ErmTableNames.TimeZones.Name;
            foreignKey.ReferencedTableSchema = ErmTableNames.TimeZones.Schema;
            foreignKey.Create();

            tempColumn.Nullable = false;
            tempColumn.Alter();
        }

        private void ConvertUserProfilesTimeZoneInfoIdColumn(IMigrationContext context)
        {
            const String originalColumnName = "TimeZoneInfoId";
            const String targetColumnName = "TimeZoneId";
            const string tempColumnName = originalColumnName + "Temp";
            var table = context.Database.Tables[ErmTableNames.UserProfiles.Name, ErmTableNames.UserProfiles.Schema];
            int? index = table.Columns.IndexOf(originalColumnName);
            if (!index.HasValue)
            {
                throw new InvalidOperationException("Can't find column: " + originalColumnName + " in table: " + ErmTableNames.UserProfiles);
            }

            table = EntityCopyHelper.CopyAndInsertColumns(context.Database, table,
                                                  new List<InsertedColumnDefinition> { new InsertedColumnDefinition(index.Value, 
                                                      t=>new Column(t, tempColumnName,DataType.Int){Nullable = true}) });

            context.Connection.ExecuteNonQuery(String.Format(UpdateTimeZoneDictionaryColumnTemplate,
                                                ErmTableNames.UserProfiles,
                                                tempColumnName,
                                                originalColumnName));
            var originalColumn = table.Columns[originalColumnName];
            originalColumn.Drop();

            var tempColumn = table.Columns[tempColumnName];
            tempColumn.Rename(targetColumnName);

            var foreignKey = new ForeignKey(table, "FK_UserProfile_TimeZone");
            var foreignKeyColumn = new ForeignKeyColumn(foreignKey, targetColumnName, "Id");
            foreignKey.Columns.Add(foreignKeyColumn);
            foreignKey.ReferencedTable = ErmTableNames.TimeZones.Name;
            foreignKey.ReferencedTableSchema = ErmTableNames.TimeZones.Schema;
            foreignKey.Create();

            tempColumn.Nullable = false;
            tempColumn.Alter();
        }
    }
}
