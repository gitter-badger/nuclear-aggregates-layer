using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ClientLinkMap : EntityConfig<ClientLink, ErmContainer>
    {
        public ClientLinkMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ClientLinks", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.MasterClientId).HasColumnName("MasterClientId");
            Property(t => t.ChildClientId).HasColumnName("ChildClientId");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");

            // Relationships
            HasRequired(t => t.ChildClient)
                .WithMany()
                .HasForeignKey(d => d.ChildClientId);
            HasRequired(t => t.MasterClient)
                .WithMany(t => t.ChildClientLinks)
                .HasForeignKey(d => d.MasterClientId);
        }
    }
}