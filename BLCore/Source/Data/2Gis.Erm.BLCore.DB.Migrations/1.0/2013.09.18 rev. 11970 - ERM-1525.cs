using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11970, "Дополнительный фикс ERM-1525")]
    public class Migration11970 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // 'Корректор', 'РГ Москва' отбираем все права
            context.Database.ExecuteNonQuery(@"
            DELETE RP
            FROM Security.RolePrivileges RP
            INNER JOIN Security.Roles R ON R.Id = RP.RoleId
            WHERE
            R.Name IN ('Корректор', 'РГ Москва')
            AND PrivilegeId IN
            (
            -- создать действие
            78068824150115072,
            -- читать действие
            78067531851497728,
            -- изменить действие
            78067927718298112,
            -- удалить действие
            78068947043222528
            )");

            // 'Настройщик системы', 'Специалист отдела производства', 'Читатель' оставляем только права на чтение
            context.Database.ExecuteNonQuery(@"
            DELETE RP
            FROM Security.RolePrivileges RP
            INNER JOIN Security.Roles R ON R.Id = RP.RoleId
            WHERE
            R.Name IN ('Настройщик системы', 'Специалист отдела производства', 'Читатель')
            AND PrivilegeId IN
            (
            -- создать действие
            78068824150115072,
            -- читать действие
            NULL,
            -- изменить действие
            78067927718298112,
            -- удалить действие
            78068947043222528
            )");

            // 'Системный администратор' даём все права
            context.Database.ExecuteNonQuery(@"
            UPDATE RP
            SET Mask = 16
            FROM Security.RolePrivileges RP
            INNER JOIN Security.Roles R ON R.Id = RP.RoleId
            WHERE
            R.Name IN ('Системный администратор')
            AND PrivilegeId IN
            (
            -- создать действие
            78068824150115072,
            -- читать действие
            78067531851497728,
            -- изменить действие
            78067927718298112,
            -- удалить действие
            78068947043222528
            )");
        }
    }
}
