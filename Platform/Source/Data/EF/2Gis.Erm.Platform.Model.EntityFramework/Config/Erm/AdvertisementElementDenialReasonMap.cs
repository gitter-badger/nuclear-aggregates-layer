using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdvertisementElementDenialReasonMap : EntityConfig<AdvertisementElementDenialReason, ErmContainer>
    {
        public AdvertisementElementDenialReasonMap()
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
            ToTable("AdvertisementElementDenialReasons", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AdvertisementElementId).HasColumnName("AdvertisementElementId");
            Property(t => t.DenialReasonId).HasColumnName("DenialReasonId");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.AdvertisementElement)
                .WithMany(t => t.AdvertisementElementDenialReasons)
                .HasForeignKey(d => d.AdvertisementElementId);
            HasRequired(t => t.DenialReason)
                .WithMany(t => t.AdvertisementElementDenialReasons)
                .HasForeignKey(d => d.DenialReasonId);
        }
    }
}