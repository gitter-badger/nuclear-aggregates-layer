using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PositionMap : EntityConfig<Position, ErmContainer>
    {
        public PositionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Positions", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.ExportCode).HasColumnName("ExportCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.IsControlledByAmount).HasColumnName("IsControlledByAmount");
            Property(t => t.IsComposite).HasColumnName("IsComposite");
            Property(t => t.RestrictChildPositionPlatforms).HasColumnName("RestrictChildPositionPlatforms");
            Property(t => t.CalculationMethodEnum).HasColumnName("CalculationMethodEnum");
            Property(t => t.BindingObjectTypeEnum).HasColumnName("BindingObjectTypeEnum");
            Property(t => t.SalesModel).HasColumnName("SalesModel");
            Property(t => t.PositionsGroup).HasColumnName("PositionsGroup");
            Property(t => t.PlatformId).HasColumnName("PlatformId");
            Property(t => t.CategoryId).HasColumnName("CategoryId");
            Property(t => t.AdvertisementTemplateId).HasColumnName("AdvertisementTemplateId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.AdvertisementTemplate)
                .WithMany(t => t.Positions)
                .HasForeignKey(d => d.AdvertisementTemplateId);
            HasRequired(t => t.Platform)
                .WithMany(t => t.Positions)
                .HasForeignKey(d => d.PlatformId);
            HasRequired(t => t.PositionCategory)
                .WithMany(t => t.Positions)
                .HasForeignKey(d => d.CategoryId);
        }
    }
}