using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class BusinessOperationService :
        IEntity
    {
        public int Descriptor { get; set; }
        public int Operation { get; set; }
        public int Service { get; set; }
    }
}