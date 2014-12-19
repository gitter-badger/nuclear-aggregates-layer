using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AppointmentReferenceMap : EntityConfig<AppointmentReference, ErmContainer>
    {
        public AppointmentReferenceMap()
        {
            // Primary Key
            HasKey(t => new { t.AppointmentId, t.Reference, t.ReferencedType, t.ReferencedObjectId });

            // Properties
            Property(t => t.AppointmentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Reference)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedType)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedObjectId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("AppointmentReferences", "Activity");
            Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            Property(t => t.Reference).HasColumnName("Reference");
            Property(t => t.ReferencedType).HasColumnName("ReferencedType");
            Property(t => t.ReferencedObjectId).HasColumnName("ReferencedObjectId");

            // Relationships
            HasRequired(t => t.AppointmentBase)
                .WithMany(t => t.AppointmentReferences)
                .HasForeignKey(d => d.AppointmentId);
        }
    }
}