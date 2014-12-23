using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ReleaseInfoMap : EntityConfig<ReleaseInfo, ErmContainer>
    {
        public ReleaseInfoMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("ReleaseInfos", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.FinishDate).HasColumnName("FinishDate");
            Property(t => t.PeriodStartDate).HasColumnName("PeriodStartDate");
            Property(t => t.PeriodEndDate).HasColumnName("PeriodEndDate");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.IsBeta).HasColumnName("IsBeta");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.OrganizationUnit)
                .WithMany(t => t.ReleaseInfos)
                .HasForeignKey(d => d.OrganizationUnitId);
            HasOptional(t => t.File)
                .WithMany(t => t.ReleaseInfos)
                .HasForeignKey(d => d.FileId);
        }
    }
}