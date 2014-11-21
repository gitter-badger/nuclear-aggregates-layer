using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List
{
    public sealed class EmiratesListBranchOfficeDto : IOperationSpecificEntityDto, IEmiratesAdapted
    {
        public long Id { get; set; }
        public long? ContributionTypeId { get; set; }
        public long? BargainTypeId { get; set; }
        public string Name { get; set; }
        public string CommercialLicense { get; set; }
        public string LegalAddress { get; set; }
        public string ContributionType { get; set; }
        public string BargainType { get; set; }
        public string Annotation { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
