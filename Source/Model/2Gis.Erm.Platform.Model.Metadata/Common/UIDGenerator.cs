using System.Threading;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public static class UIDGenerator
    {
        private static int uidCounter = 0;
        public static int Next 
        {
            get
            {
                return Interlocked.Increment(ref uidCounter);
            }
        }
    }
}
