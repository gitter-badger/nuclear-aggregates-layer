using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class DenormalizedClientLinkMap : EntityConfig<DenormalizedClientLink, ErmContainer>
    {
        public DenormalizedClientLinkMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("DenormalizedClientLinks", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.MasterClientId).HasColumnName("MasterClientId");
            Property(t => t.ChildClientId).HasColumnName("ChildClientId");
            Property(t => t.IsLinkedDirectly).HasColumnName("IsLinkedDirectly");
            Property(t => t.GraphKey).HasColumnName("GraphKey");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");

            // Relationships
            HasRequired(t => t.ChildClient)
                .WithMany(t => t.DenormalizedLinksForClientAsChild)
                .HasForeignKey(d => d.ChildClientId);
            HasRequired(t => t.MasterClient)
                .WithMany(t => t.DenormalizedLinksForClientAsMaster)
                .HasForeignKey(d => d.MasterClientId);
        }
    }
}