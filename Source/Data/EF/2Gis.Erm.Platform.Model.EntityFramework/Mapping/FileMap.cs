using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class FileMap : EntityTypeConfiguration<File>
    {
        public FileMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(1024);

            Property(t => t.ContentType)
                .IsRequired()
                .HasMaxLength(255);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Files", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.FileName).HasColumnName("FileName");
            Property(t => t.ContentType).HasColumnName("ContentType");
            Property(t => t.ContentLength).HasColumnName("ContentLength");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.DgppId).HasColumnName("DgppId");
        }
    }
}