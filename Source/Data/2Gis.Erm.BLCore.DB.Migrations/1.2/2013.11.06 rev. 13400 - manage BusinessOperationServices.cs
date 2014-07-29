
using System;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13400, "Управление записями в BusinessOperationServices (чистка таблицы от операций версии 0.18)")]
    public sealed class Migration13400 : TransactedMigration
    {
        private enum Identity
        {
            Create = 30,
            Disqualify = 10,
            SetAsMainLegalPersonProfile = 21901,
            Merge = 25,
        }

        private enum Descriptor
        {
            Firm = 653315513,
            LegalPerson = 942141479,
            LegalPersonProfile = 936439653,
            DoubleClient = -566764810,
            DoubleLegalPersons = -394024283,
        }

        private enum Flow
        {
            FinancialDataClient = 11,
            FinancialDataLegalEntity = 3,
            OrdersOrder = 5,
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var cleaner = new Cleaner(context);
            
            cleaner.DeleteRecord(Identity.Create, Descriptor.Firm, Flow.FinancialDataClient);
            cleaner.DeleteRecord(Identity.Disqualify, Descriptor.Firm, Flow.FinancialDataClient);
            cleaner.DeleteRecord(Identity.SetAsMainLegalPersonProfile, Descriptor.LegalPerson, Flow.FinancialDataLegalEntity);
            cleaner.DeleteRecord(Identity.SetAsMainLegalPersonProfile, Descriptor.LegalPersonProfile, Flow.FinancialDataLegalEntity);
            cleaner.DeleteRecord(Identity.Merge, Descriptor.DoubleClient, Flow.FinancialDataLegalEntity);
            cleaner.DeleteRecord(Identity.Merge, Descriptor.DoubleClient, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.Merge, Descriptor.DoubleLegalPersons, Flow.FinancialDataClient);
            cleaner.DeleteRecord(Identity.Merge, Descriptor.DoubleLegalPersons, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.Merge, Descriptor.DoubleLegalPersons, Flow.FinancialDataLegalEntity);
        }

        private class Cleaner
        {
            private const string DeleteTemplate = @"delete from Shared.BusinessOperationServices where Operation = {0} and Descriptor = {1} and Service = {2}";
            private const string SelectTemplate = @"select count(*) from Shared.BusinessOperationServices where Operation = {0} and Descriptor = {1} and Service = {2}";
            private readonly IMigrationContext _context;

            public Cleaner(IMigrationContext context)
            {
                _context = context;
            }

            public void DeleteRecord(Identity identity, Descriptor descriptor, Flow flow)
            {
                var select = string.Format(SelectTemplate, (int)identity, (int)descriptor, (int)flow);
                var count = (int)_context.Connection.ExecuteScalar(select);

                if (count != 1)
                {
                    var message = string.Format("В таблице Shared.BusinessOperationServices ожидалась запись (Operation = {0}, Descriptor = {1}, Service = {2}), но её там не оказалось",
                        (int)identity,
                        (int)descriptor, 
                        (int)flow);

                    throw new Exception(message);
                }

                var statement = string.Format(DeleteTemplate, (int)identity, (int)descriptor, (int)flow);
                _context.Connection.ExecuteNonQuery(statement);
            }
        }
    }
}
