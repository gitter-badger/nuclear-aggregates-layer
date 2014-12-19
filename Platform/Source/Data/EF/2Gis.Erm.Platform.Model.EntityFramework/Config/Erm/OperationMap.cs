using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OperationMap : EntityConfig<Operation, ErmContainer>
    {
        public OperationMap()
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
            ToTable("Operations", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Guid).HasColumnName("Guid");
            Property(t => t.LogFileId).HasColumnName("LogFileId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.StartTime).HasColumnName("StartTime");
            Property(t => t.FinishTime).HasColumnName("FinishTime");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.Type).HasColumnName("Type");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.OrganizationUnit)
                .WithMany(t => t.Operations)
                .HasForeignKey(d => d.OrganizationUnitId);
            HasOptional(t => t.File)
                .WithMany(t => t.Operations)
                .HasForeignKey(d => d.LogFileId);
        }
    }
}