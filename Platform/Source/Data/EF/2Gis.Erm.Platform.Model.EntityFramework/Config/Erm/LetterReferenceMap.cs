using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class LetterReferenceMap : EntityConfig<LetterReference, ErmContainer>
    {
        public LetterReferenceMap()
        {
            // Primary Key
            HasKey(t => new { t.LetterId, t.Reference, t.ReferencedType, t.ReferencedObjectId });

            // Properties
            Property(t => t.LetterId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Reference)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedType)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedObjectId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("LetterReferences", "Activity");
            Property(t => t.LetterId).HasColumnName("LetterId");
            Property(t => t.Reference).HasColumnName("Reference");
            Property(t => t.ReferencedType).HasColumnName("ReferencedType");
            Property(t => t.ReferencedObjectId).HasColumnName("ReferencedObjectId");

            // Relationships
            HasRequired(t => t.LetterBase)
                .WithMany(t => t.LetterReferences)
                .HasForeignKey(d => d.LetterId);
        }
    }
}