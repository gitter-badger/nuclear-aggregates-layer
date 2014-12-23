using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class FirmMap : EntityConfig<Firm, ErmContainer>
    {
        public FirmMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

            Property(t => t.Comment)
                .HasMaxLength(2048);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Firms", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.PromisingScore).HasColumnName("PromisingScore");
            Property(t => t.UsingOtherMedia).HasColumnName("UsingOtherMedia");
            Property(t => t.ProductType).HasColumnName("ProductType");
            Property(t => t.MarketType).HasColumnName("MarketType");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.TerritoryId).HasColumnName("TerritoryId");
            Property(t => t.ClientId).HasColumnName("ClientId");
            Property(t => t.ClosedForAscertainment).HasColumnName("ClosedForAscertainment");
            Property(t => t.LastQualifyTime).HasColumnName("LastQualifyTime");
            Property(t => t.LastDisqualifyTime).HasColumnName("LastDisqualifyTime");
            Property(t => t.Information).HasColumnName("Information");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.BudgetType).HasColumnName("BudgetType");
            Property(t => t.Geolocation).HasColumnName("Geolocation");
            Property(t => t.InCityBranchesAmount).HasColumnName("InCityBranchesAmount");
            Property(t => t.OutCityBranchesAmount).HasColumnName("OutCityBranchesAmount");
            Property(t => t.StaffAmount).HasColumnName("StaffAmount");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.Client)
                .WithMany(t => t.Firms)
                .HasForeignKey(d => d.ClientId);
            HasRequired(t => t.OrganizationUnit)
                .WithMany(t => t.Firms)
                .HasForeignKey(d => d.OrganizationUnitId);
            HasRequired(t => t.Territory)
                .WithMany(t => t.Firms)
                .HasForeignKey(d => d.TerritoryId);
        }
    }
}