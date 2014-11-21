﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10725, "Отключаем автоинкремент в таблицах RolePrivilege, UserRole")]
    public class Migration10725 : TransactedMigration
    {
        private static readonly SchemaQualifiedObjectName[] Tables = 
        {
           ErmTableNames.RolePrivileges,
           ErmTableNames.UserRoles,
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