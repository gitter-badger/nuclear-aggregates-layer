using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ClientMap : EntityConfig<Client, ErmContainer>
    {
        public ClientMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .HasMaxLength(250);

            Property(t => t.MainPhoneNumber)
                .HasMaxLength(64);

            Property(t => t.AdditionalPhoneNumber1)
                .HasMaxLength(64);

            Property(t => t.AdditionalPhoneNumber2)
                .HasMaxLength(64);

            Property(t => t.Fax)
                .HasMaxLength(50);

            Property(t => t.Email)
                .HasMaxLength(100);

            Property(t => t.Website)
                .HasMaxLength(200);

            Property(t => t.MainAddress)
                .HasMaxLength(250);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Clients", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.MainPhoneNumber).HasColumnName("MainPhoneNumber");
            Property(t => t.AdditionalPhoneNumber1).HasColumnName("AdditionalPhoneNumber1");
            Property(t => t.AdditionalPhoneNumber2).HasColumnName("AdditionalPhoneNumber2");
            Property(t => t.Fax).HasColumnName("Fax");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.Website).HasColumnName("Website");
            Property(t => t.MainFirmId).HasColumnName("MainFirmId");
            Property(t => t.MainAddress).HasColumnName("MainAddress");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.IsAdvertisingAgency).HasColumnName("IsAdvertisingAgency");
            Property(t => t.TerritoryId).HasColumnName("TerritoryId");
            Property(t => t.InformationSource).HasColumnName("InformationSource");
            Property(t => t.PromisingValue).HasColumnName("PromisingValue");
            Property(t => t.LastQualifyTime).HasColumnName("LastQualifyTime");
            Property(t => t.LastDisqualifyTime).HasColumnName("LastDisqualifyTime");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.Firm)
                .WithMany(t => t.Clients)
                .HasForeignKey(d => d.MainFirmId);
            HasRequired(t => t.Territory)
                .WithMany(t => t.Clients)
                .HasForeignKey(d => d.TerritoryId);
        }
    }
}