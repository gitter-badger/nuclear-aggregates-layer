using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BargainMap : EntityConfig<Bargain, ErmContainer>
    {
        public BargainMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Number)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Comment)
                .HasMaxLength(256);

            Property(t => t.DocumentsComment)
                .HasMaxLength(300);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Bargains", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Number).HasColumnName("Number");
            Property(t => t.BargainTypeId).HasColumnName("BargainTypeId");
            Property(t => t.CustomerLegalPersonId).HasColumnName("CustomerLegalPersonId");
            Property(t => t.ExecutorBranchOfficeId).HasColumnName("ExecutorBranchOfficeId");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.SignedOn).HasColumnName("SignedOn");
            Property(t => t.BargainEndDate).HasColumnName("BargainEndDate");
            Property(t => t.BargainKind).HasColumnName("BargainKind");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ClosedOn).HasColumnName("ClosedOn");
            Property(t => t.HasDocumentsDebt).HasColumnName("HasDocumentsDebt");
            Property(t => t.DocumentsComment).HasColumnName("DocumentsComment");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.BargainType)
                .WithMany(t => t.Bargains)
                .HasForeignKey(d => d.BargainTypeId);
            HasRequired(t => t.BranchOfficeOrganizationUnit)
                .WithMany(t => t.Bargains)
                .HasForeignKey(d => d.ExecutorBranchOfficeId);
            HasRequired(t => t.LegalPerson)
                .WithMany(t => t.Bargains)
                .HasForeignKey(d => d.CustomerLegalPersonId);
        }
    }
}