using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class CurrencyRateMap : EntityConfig<CurrencyRate, ErmContainer>
    {
        public CurrencyRateMap()
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
            ToTable("CurrencyRates", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            Property(t => t.BaseCurrencyId).HasColumnName("BaseCurrencyId");
            Property(t => t.Rate).HasColumnName("Rate");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Currency)
                .WithMany(t => t.CurrencyRates)
                .HasForeignKey(d => d.CurrencyId);
            HasRequired(t => t.BaseCurrency)
                .WithMany(t => t.BaseurrencyRates)
                .HasForeignKey(d => d.BaseCurrencyId);
        }
    }
}