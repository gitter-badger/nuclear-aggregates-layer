using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class PboChangeDescriptor : IChangeDescriptor
    {
        public PboChangeDescriptor(PerformedBusinessOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            Operation = operation;
        }

        public PerformedBusinessOperation Operation { get; private set; }
    }
}