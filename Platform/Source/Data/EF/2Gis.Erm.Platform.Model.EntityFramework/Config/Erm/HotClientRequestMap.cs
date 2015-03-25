using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class HotClientRequestMap : EntityConfig<HotClientRequest, ErmContainer>
    {
        public HotClientRequestMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SourceCode)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.UserCode)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.ContactName)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.ContactPhone)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("HotClientRequests", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SourceCode).HasColumnName("SourceCode");
            Property(t => t.UserCode).HasColumnName("UserCode");
            Property(t => t.UserName).HasColumnName("UserName");
            Property(t => t.CreationDate).HasColumnName("CreationDate");
            Property(t => t.ContactName).HasColumnName("ContactName");
            Property(t => t.ContactPhone).HasColumnName("ContactPhone");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.CardCode).HasColumnName("CardCode");
            Property(t => t.BranchCode).HasColumnName("BranchCode");
            Property(t => t.TaskId).HasColumnName("TaskId");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
        }
    }
}