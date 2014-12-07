using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class FirmContactMap : EntityConfig<FirmContact, ErmContainer>
    {
        public FirmContactMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Contact)
                .IsRequired()
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("FirmContacts", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.FirmAddressId).HasColumnName("FirmAddressId");
            Property(t => t.ContactType).HasColumnName("ContactType");
            Property(t => t.Contact).HasColumnName("Contact");
            Property(t => t.SortingPosition).HasColumnName("SortingPosition");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.CardId).HasColumnName("CardId");

            // Relationships
            HasOptional(t => t.FirmAddress)
                .WithMany(t => t.FirmContacts)
                .HasForeignKey(d => d.FirmAddressId);
            HasOptional(t => t.DepCard)
                .WithMany(t => t.FirmContacts)
                .HasForeignKey(d => d.CardId);
        }
    }
}