using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ReferenceItemMap : EntityConfig<ReferenceItem, ErmContainer>
    {
        public ReferenceItemMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            ToTable("ReferenceItems", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ReferenceId).HasColumnName("ReferenceId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");

            // Relationships
            HasRequired(t => t.Reference)
                .WithMany(t => t.ReferenceItems)
                .HasForeignKey(d => d.ReferenceId);
        }
    }
}