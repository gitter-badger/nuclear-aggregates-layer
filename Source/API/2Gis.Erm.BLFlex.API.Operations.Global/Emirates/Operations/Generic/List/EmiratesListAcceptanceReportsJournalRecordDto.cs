using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List
{
    public sealed class EmiratesListAcceptanceReportsJournalRecordDto : IEmiratesAdapted, IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public long AuthorId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public int DocumentsAmount { get; set; }
        public string OrganizationUnitName { get; set; }
        public string AuthorName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}