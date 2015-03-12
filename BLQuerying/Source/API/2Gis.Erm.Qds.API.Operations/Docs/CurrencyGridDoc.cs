using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class CurrencyGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long IsoCode { get; set; }
        public string Symbol { get; set; }
        public bool IsBase { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}