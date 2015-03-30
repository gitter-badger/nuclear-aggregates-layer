using System;
using System.Linq;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class ErmMultiGetDescriptor : MultiGetDescriptor
    {
        public ErmMultiGetDescriptor GetDistinct<T>(Func<MultiGetOperationDescriptor<T>, MultiGetOperationDescriptor<T>> getSelector) where T : class
        {
            var descriptor = (IMultiGetOperation)getSelector(new MultiGetOperationDescriptor<T>());

            var documentType = typeof(T);
            var operations = ((IMultiGetRequest)this).GetOperations;
            var idExists = operations.Any(x => String.Equals(x.Id, descriptor.Id, StringComparison.OrdinalIgnoreCase) && x.ClrType == documentType);
            if (!idExists)
            {
                operations.Add(descriptor);
            }

            return this;
        }
    }
}