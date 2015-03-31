using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class CountryGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long IsoCode { get; set; }
        public bool IsDeleted { get; set; }

        // relations
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}