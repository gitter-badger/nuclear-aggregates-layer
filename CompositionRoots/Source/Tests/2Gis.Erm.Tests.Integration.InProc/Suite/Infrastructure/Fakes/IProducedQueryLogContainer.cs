using System.Collections.Generic;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes
{
    public interface IProducedQueryLogContainer
    {
        IEnumerable<string> Queries { get; }
        void Reset();
    }
}