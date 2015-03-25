using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ActionsHistoryDetailMap : EntityConfig<ActionsHistoryDetail, ErmContainer>
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