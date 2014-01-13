using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13681, "Исправление значения OwnerCode для профилей юрлиц, чьи кураторы были деактивированы")]
    public class Migration13681 : TransactedMigration
    {
        private const string SelectQuery = "select LegalPersonProfiles.Id as LegalPersonProfileId, LegalPersons.OwnerCode as LegalPersonOwnerCode " +
                                           "from Billing.LegalPersonProfiles " +
                                           "inner join Billing.LegalPersons on LegalPersons.Id = LegalPersonProfiles.LegalPersonId " +
                                           "where LegalPersonProfiles.OwnerCode in (select Id from Security.Users where IsActive <> 1 or IsDeleted <> 0); ";

        private const string TemporaryTableTemplate = "DECLARE @temp TABLE (Id bigint, OwnerCode bigint); " +
                                                      "insert into @temp " +
                                                      "values {0}; ";

        private const string UpdateStatement = "UPDATE Billing.LegalPersonProfiles " +
                                               "SET OwnerCode = T.OwnerCode " +
                                               "FROM @temp T inner join Billing.LegalPersonProfiles on LegalPersonProfiles.Id = T.Id; ";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var results = new List<Tuple<long, long>>();
            using (var reader = context.Connection.ExecuteReader(SelectQuery))
            {
                while (reader.Read())
                {
                    var legalPersonProfileId = reader.GetInt64(0);
                    var actualOwnerCode = reader.GetInt64(1);
                    results.Add(new Tuple<long, long>(legalPersonProfileId, actualOwnerCode));
                }
            }

            if (results.Any())
            {
                // Размер пакета обусловлен настркойками SQL-сервера, не позволяющего в одной инструкции INSERT использовать более 1000 записей.
                const int BatchSize = 1000;
                for (var skip = 0;; skip += BatchSize)
                {
                    var slice = results.Skip(skip).Take(BatchSize).ToArray();
                    if (!slice.Any())
                    {
                        break;
                    }

                    var temporaryTableDefinition = string.Format(TemporaryTableTemplate, string.Join(", ", slice.Select(tuple => string.Format("({0}, {1})", tuple.Item1, tuple.Item2))));
                    context.Connection.ExecuteNonQuery(temporaryTableDefinition + UpdateStatement);
                }
            }
        }
    }
}