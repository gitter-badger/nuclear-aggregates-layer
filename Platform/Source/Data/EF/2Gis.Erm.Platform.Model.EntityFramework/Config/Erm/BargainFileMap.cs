using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BargainFileMap : EntityConfig<BargainFile, ErmContainer>
    {
        public BargainFileMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("BargainFiles", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.BargainId).HasColumnName("BargainId");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.FileKind).HasColumnName("FileKind");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Bargain)
                .WithMany(t => t.BargainFiles)
                .HasForeignKey(d => d.BargainId);
            HasRequired(t => t.File)
                .WithMany(t => t.BargainFiles)
                .HasForeignKey(d => d.FileId);
        }
    }
}