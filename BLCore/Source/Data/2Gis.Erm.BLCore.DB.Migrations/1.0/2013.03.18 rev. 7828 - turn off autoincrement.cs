using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7828, "Удаление Identity")]
    public sealed class Migration7828 : TransactedMigration
    {
        private static readonly SchemaQualifiedObjectName[] Tables = 
        {
            ErmTableNames.AdvertisementElementTemplates,
            ErmTableNames.AdvertisementTemplates,
            ErmTableNames.BargainTypes,
            ErmTableNames.BranchOfficeOrganizationUnits,
            ErmTableNames.BranchOffices,
            ErmTableNames.Countries,
            ErmTableNames.ContributionTypes,
            ErmTableNames.Currencies,
            ErmTableNames.OperationTypes,
            ErmTableNames.OrderValidationRuleGroupDetails,
            ErmTableNames.OrderValidationRuleGroups,
            ErmTableNames.OrganizationUnits,
            ErmTableNames.Platforms,
            ErmTableNames.PositionCategories,
            ErmTableNames.CategoryGroups,
            ErmTableNames.Territories,
            ErmTableNames.Departments,
            ErmTableNames.Privileges,
            ErmTableNames.Roles,
            ErmTableNames.Users,
            ErmTableNames.MessageTypes,
            ErmTableNames.TimeZones,
            ErmTableNames.Positions,
            ErmTableNames.Themes,
            ErmTableNames.UserProfiles,
        };

        protected override void ApplyOverride(IMigrationContext context)
        {
            var tables = Tables.Select(name => context.Database.GetTable(name))
                               .Where(table => table.Columns["Id"].Identity)
                               .ToArray();

            foreach (var table in tables)
            {
                try
                {
                    EntityCopyHelper.RemoveIdentity(context.Database, table);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("While processing {0}", table.Name), e);
                }
            }
        }
    }
}
