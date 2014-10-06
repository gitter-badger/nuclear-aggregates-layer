using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class ActionsHistoryDetailMap : EntityTypeConfiguration<ActionsHistoryDetail>
    {
        public ActionsHistoryDetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.PropertyName)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("ActionsHistoryDetails", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ActionsHistoryId).HasColumnName("ActionsHistoryId");
            Property(t => t.PropertyName).HasColumnName("PropertyName");
            Property(t => t.OriginalValue).HasColumnName("OriginalValue");
            Property(t => t.ModifiedValue).HasColumnName("ModifiedValue");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.ActionsHistory)
                .WithMany(t => t.ActionsHistoryDetails)
                .HasForeignKey(d => d.ActionsHistoryId);
        }
    }
}