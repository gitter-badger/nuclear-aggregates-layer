using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing
{
    public interface IMassProcessor
    {
        Type[] GetAssignableTypes();

        void ProcessTypes(IEnumerable<Type> types, bool firstRun);

        void AfterProcessTypes(bool firstRun);
    }
}