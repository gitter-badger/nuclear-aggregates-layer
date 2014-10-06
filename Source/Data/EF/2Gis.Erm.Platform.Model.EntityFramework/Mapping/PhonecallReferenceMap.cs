using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class PhonecallReferenceMap : EntityTypeConfiguration<PhonecallReference>
    {
        public PhonecallReferenceMap()
        {
            // Primary Key
            HasKey(t => new { t.PhonecallId, t.Reference, t.ReferencedType, t.ReferencedObjectId });

            // Properties
            Property(t => t.PhonecallId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Reference)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedType)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedObjectId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("PhonecallReferences", "Activity");
            Property(t => t.PhonecallId).HasColumnName("PhonecallId");
            Property(t => t.Reference).HasColumnName("Reference");
            Property(t => t.ReferencedType).HasColumnName("ReferencedType");
            Property(t => t.ReferencedObjectId).HasColumnName("ReferencedObjectId");

            // Relationships
            HasRequired(t => t.PhonecallBase)
                .WithMany(t => t.PhonecallReferences)
                .HasForeignKey(d => d.PhonecallId);
        }
    }
}