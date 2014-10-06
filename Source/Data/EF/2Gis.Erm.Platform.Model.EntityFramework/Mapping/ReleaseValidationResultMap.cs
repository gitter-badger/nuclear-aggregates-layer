using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class ReleaseValidationResultMap : EntityTypeConfiguration<ReleaseValidationResult>
    {
        public ReleaseValidationResultMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.RuleCode)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.Message)
                .IsRequired();

            // Table & Column Mappings
            ToTable("ReleaseValidationResults", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReleaseInfoId).HasColumnName("ReleaseInfoId");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.IsBlocking).HasColumnName("IsBlocking");
            Property(t => t.RuleCode).HasColumnName("RuleCode");
            Property(t => t.Message).HasColumnName("Message");

            // Relationships
            HasRequired(t => t.ReleaseInfo)
                .WithMany(t => t.ReleaseValidationResults)
                .HasForeignKey(d => d.ReleaseInfoId);
        }
    }
}