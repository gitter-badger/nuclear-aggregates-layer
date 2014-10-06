using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class ExportFlowOrders_InvoiceMap : EntityTypeConfiguration<ExportFlowOrdersInvoice>
    {
        public ExportFlowOrders_InvoiceMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ExportFlowOrders_Invoice", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Date).HasColumnName("Date");
        }
    }
}