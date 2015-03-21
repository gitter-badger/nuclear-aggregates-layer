using System;
using System.Linq;

using Nest;

using Newtonsoft.Json.Linq;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class ErmBulkDescriptor : BulkDescriptor
    {
        public ErmBulkDescriptor Update<T>(Func<BulkUpdateDescriptor<T, T>, BulkUpdateDescriptor<T, T>> bulkUpdateSelector, UpdateType updateType)
            where T : class
        {
            var operations = ((IBulkRequest)this).Operations;
            var newOperation = (IBulkUpdateOperation<T, T>)bulkUpdateSelector(new ErmBulkUpdateDescriptor<T, T> { UpdateType = updateType });

            switch (updateType)
            {
                case UpdateType.UpdateOverride:
                {
                    operations.Add(newOperation);
                    break;
                }

                case UpdateType.UpdateMerge:
                {
                    var existingOperation = (IBulkUpdateOperation<T, T>)operations.SingleOrDefault(x => string.Equals(x.Id, newOperation.Id, StringComparison.OrdinalIgnoreCase) && x.ClrType == newOperation.ClrType);
                    if (existingOperation != null)
                    {
                        var existingDoc = JObject.FromObject(existingOperation.Doc);
                        var newDoc = JObject.FromObject(newOperation.Doc);
                        existingDoc.Merge(newDoc);
                        existingOperation.Doc = existingDoc.ToObject<T>();
                    }
                    else
                    {
                        operations.Add(newOperation);
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException("updateType");
            }

            return this;
        }
    }
}