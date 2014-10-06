using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class ExportFlowDeliveryData_LetterSendRequestMap : EntityTypeConfiguration<ExportFlowDeliveryData_LetterSendRequest>
    {
        public ExportFlowDeliveryData_LetterSendRequestMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ExportFlowDeliveryData_LetterSendRequest", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Date).HasColumnName("Date");
        }
    }
}