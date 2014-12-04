using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class LocalMessageMap : EntityConfig<LocalMessage, ErmContainer>
    {
        public LocalMessageMap()
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
            ToTable("LocalMessages", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.EventDate).HasColumnName("EventDate");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.ProcessResult).HasColumnName("ProcessResult");
            Property(t => t.MessageTypeId).HasColumnName("MessageTypeId");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.ProcessingTime).HasColumnName("ProcessingTime");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.OrganizationUnit)
                .WithMany(t => t.LocalMessages)
                .HasForeignKey(d => d.OrganizationUnitId);
            HasRequired(t => t.File)
                .WithMany(t => t.LocalMessages)
                .HasForeignKey(d => d.FileId);
            HasRequired(t => t.MessageType)
                .WithMany(t => t.LocalMessages)
                .HasForeignKey(d => d.MessageTypeId);
        }
    }
}