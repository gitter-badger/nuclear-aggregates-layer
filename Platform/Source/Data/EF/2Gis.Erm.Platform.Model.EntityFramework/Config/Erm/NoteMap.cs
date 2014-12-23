using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class NoteMap : EntityConfig<Note, ErmContainer>
    {
        public NoteMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Title)
                .HasMaxLength(256);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Notes", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ParentId).HasColumnName("ParentId");
            Property(t => t.ParentType).HasColumnName("ParentType");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.Text).HasColumnName("Text");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.File)
                .WithMany(t => t.Notes)
                .HasForeignKey(d => d.FileId);
        }
    }
}