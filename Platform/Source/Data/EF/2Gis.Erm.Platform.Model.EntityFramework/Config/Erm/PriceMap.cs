using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PriceMap : EntityConfig<Price, ErmContainer>
    {
        public PriceMap()
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
            ToTable("Prices", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.PublishDate).HasColumnName("PublishDate");
            Property(t => t.BeginDate).HasColumnName("BeginDate");
            Property(t => t.IsPublished).HasColumnName("IsPublished");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Currency)
                .WithMany(t => t.Prices)
                .HasForeignKey(d => d.CurrencyId);
            HasRequired(t => t.OrganizationUnit)
                .WithMany(t => t.Prices)
                .HasForeignKey(d => d.OrganizationUnitId);
        }
    }
}