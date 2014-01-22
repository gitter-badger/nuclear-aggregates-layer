using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists.Configuration
{
    public class DataLists
    {
        public static readonly DataListStructure Orders =
            DataListStructure.Config
                .DisplayNameLocalized(() => ErmConfigLocalization.DListAllOrders)
                .SortDescending()
                .DefaultFilter("IsActive==true&&IsDeleted==false")
                .Operation.EntitySpecific<AssignIdentity>(EntityName.Order)
                .Operation.EntitySpecific<DeleteIdentity>(EntityName.Order)
                .Operation.NonCoupled<SetInspectorIdentity>()
                .DataFields.Attach(
                    DataFieldStructure.Config
                        .Property<ListOrderDto, Order>(dto => dto.Id, order => order.Id)
                        .LocalizedName(() => MetadataResources.FirmName)
                        .MainAttribute()
                        .DisableSorting(),
                    DataFieldStructure.Config
                        .PropertyReference<ListOrderDto, Order, Firm>(dto => dto.FirmName, it => it.Firm.Name, dto => dto.FirmId, it => it.FirmId)
                        .LocalizedName(() => MetadataResources.FirmName)
                        .DisableSorting());


        public static readonly DataListStructure ActiveOrders =
            DataListStructure.Config
                .BasedOn(Orders)
                .Operation.EntitySpecific<DeactivateIdentity>(EntityName.Order)
                .DisplayNameLocalized(() => ErmConfigLocalization.DListActiveOrders)
                .DefaultFilter("IsActive==true&&IsDeleted==false")
                .DataFields.Attach(
                    DataFieldStructure.Config
                        .Property<Order>(x => x.Number, "it.Number")
                        .LocalizedName(() => ErmConfigLocalization.DFieldNumber)
                        .MainAttribute()
                        .DisableSorting());
    }
}