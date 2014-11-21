using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO
{
    public abstract class ImportHeaderDto
    {
        public Guid MessageId { get; set; }
        public DateTime CreateDate { get; set; }
        public int DgppId { get; set; }
        public long OrganizationUnitId { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class ImportTerritoriesHeaderDto : ImportHeaderDto
    {
        public DateTime ExportDate { get; set; }
    }

    public sealed class ImportTerritoryDto
    {
        public long DgppId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<long> Firms { get; set; }
    }
}