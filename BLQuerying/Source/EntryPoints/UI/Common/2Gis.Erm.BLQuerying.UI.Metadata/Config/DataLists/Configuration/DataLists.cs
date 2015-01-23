using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.DataLists.Configuration
{
    public class DataLists
    {
        public static readonly DataListMetadata Orders =
            DataListMetadata.Config
                .DisplayNameLocalized(() => ErmConfigLocalization.DListAllOrders)
                .SortDescending()
                .DefaultFilter("IsActive==true&&IsDeleted==false")
                .Operation.SpecificFor<AssignIdentity, Order>()
                .Operation.SpecificFor<DeleteIdentity, Order>()
                .Operation.NonCoupled<SetInspectorIdentity>()
                .DataFields.Attach(
                    DataFieldMetadata.Config
                        .Property<ListOrderDto, Order>(dto => dto.Id, order => order.Id)
                        .LocalizedName(() => MetadataResources.FirmName)
                        .MainAttribute()
                        .DisableSorting(),
                    DataFieldMetadata.Config
                        .PropertyReference<ListOrderDto, Order, Firm>(dto => dto.FirmName, it => it.Firm.Name, dto => dto.FirmId, it => it.FirmId)
                        .LocalizedName(() => MetadataResources.FirmName)
                        .DisableSorting());


        public static readonly DataListMetadata ActiveOrders =
            DataListMetadata.Config
                .BasedOn(Orders)
                .Operation.SpecificFor<DeactivateIdentity, Order>()
                .DisplayNameLocalized(() => ErmConfigLocalization.DListActiveOrders)
                .DefaultFilter("IsActive==true&&IsDeleted==false")
                .DataFields.Attach(
                    DataFieldMetadata.Config
                        .Property<Order>(x => x.Number, "it.Number")
                        .LocalizedName(() => ErmConfigLocalization.DFieldNumber)
                        .MainAttribute()
                        .DisableSorting());
    }
}