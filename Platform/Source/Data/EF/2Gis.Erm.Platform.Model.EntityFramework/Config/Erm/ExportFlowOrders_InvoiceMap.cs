using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ExportFlowOrders_InvoiceMap : EntityConfig<ExportFlowOrdersInvoice, ErmContainer>
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