using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class FirmAddressMap : EntityConfig<FirmAddress, ErmContainer>
    {
        public FirmAddressMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Address)
                .IsRequired()
                .HasMaxLength(4000);

            Property(t => t.PaymentMethods)
                .HasMaxLength(512);

            Property(t => t.WorkingTime)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("FirmAddresses", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.FirmId).HasColumnName("FirmId");
            Property(t => t.TerritoryId).HasColumnName("TerritoryId");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.ClosedForAscertainment).HasColumnName("ClosedForAscertainment");
            Property(t => t.IsLocatedOnTheMap).HasColumnName("IsLocatedOnTheMap");
            Property(t => t.SortingPosition).HasColumnName("SortingPosition");
            Property(t => t.PaymentMethods).HasColumnName("PaymentMethods");
            Property(t => t.WorkingTime).HasColumnName("WorkingTime");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.BuildingCode).HasColumnName("BuildingCode");
            Property(t => t.AddressCode).HasColumnName("AddressCode");
            Property(t => t.ReferencePoint).HasColumnName("ReferencePoint");

            // Relationships
            HasOptional(t => t.Building)
                .WithMany(t => t.FirmAddresses)
                .HasForeignKey(d => d.BuildingCode);
            HasRequired(t => t.Firm)
                .WithMany(t => t.FirmAddresses)
                .HasForeignKey(d => d.FirmId);
            HasOptional(t => t.Territory)
                .WithMany(t => t.FirmAddresses)
                .HasForeignKey(d => d.TerritoryId);
        }
    }
}