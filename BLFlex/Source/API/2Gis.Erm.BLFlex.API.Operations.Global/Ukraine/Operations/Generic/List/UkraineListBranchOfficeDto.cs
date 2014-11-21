using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Generic.List
{
    public sealed class UkraineListBranchOfficeDto : IOperationSpecificEntityDto, IUkraineAdapted
    {
        public long Id { get; set; }
        public long? ContributionTypeId { get; set; }
        public long? BargainTypeId { get; set; }
        public string Name { get; set; }
        public string Ipn { get; set; }
        public string Egrpou { get; set; }
        public string LegalAddress { get; set; }
        public string ContributionType { get; set; }
        public string BargainType { get; set; }
        public string Annotation { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
