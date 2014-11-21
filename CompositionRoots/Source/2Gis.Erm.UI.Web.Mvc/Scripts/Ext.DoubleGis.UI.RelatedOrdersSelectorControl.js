Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.RelatedOrdersSelectorControl = Ext.extend(Ext.util.Observable, {
    Config: {
        RelatedOrderControlElement: null,
        GridHeight: -1,
        GridWidth: -1
    },
    RelatedOrderFields: [
    { name: 'Id', type: 'string' },
    { name: 'IsOrderSelected', type: 'bool' },
    { name: 'Number', type: 'string' },
    { name: 'SourceOrganizationUnit', type: 'string'},
    { name: 'DestinationOrganizationUnit', type: 'string' },
    { name: 'BeginDistributionDate', type: 'date' },
    { name: 'EndDistributionDate', type: 'date' } ],
    RelatedOrderRecord: null,
    GridColumnModel: null,
    RelatedOrderGrid: null,
    
    constructor: function (config) {
        Ext.apply(this.Config, config);
        
        this.RelatedOrderRecord = Ext.data.Record.create(this.RelatedOrderFields);
        this.RelatedOrdersStore = new Ext.data.ArrayStore({
            idProperty: 'Id',  
            autoDestroy: true,
            storeId: 'relatedOrdersStore',
            fields: this.RelatedOrderFields
        });
        var checkColumn = new Ext.grid.CheckColumn({
            header: Ext.LocalizedResources.RelatedOrdersIsSelected,
            dataIndex: 'IsOrderSelected',
            id: 'check',
            readOnly: false,
            fixed: true,
            width: 55
        });
        this.GridColumnModel = new Ext.grid.ColumnModel({
            columns: [
                {
                    header: 'Id',
                    dataIndex: 'Id',
                    hidden: true 
                },
                checkColumn,
                {
                    header: Ext.LocalizedResources.RelatedOrdersNumber,
                    dataIndex: 'Number',
                    fixed: false,
                    resizable: true,
                    width: 120
                },
                {
                    header: Ext.LocalizedResources.RelatedOrdersSource,
                    dataIndex: 'SourceOrganizationUnit'
                },
                {
                    header: Ext.LocalizedResources.RelatedOrdersDestination,
                    dataIndex: 'DestinationOrganizationUnit'
                },
                {
                    header: Ext.LocalizedResources.BeginDistributionDate,
                    dataIndex: 'BeginDistributionDate',
                    xtype: 'datecolumn',
                    format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern
                },
                {
                    header: Ext.LocalizedResources.EndDistributionDate,
                    dataIndex: 'EndDistributionDate',
                    xtype: 'datecolumn',
                    format: Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern
                }],
            defaults: {
                sortable: false,
                menuDisabled: true,
                readOnly: true,
                resizable: false
            }
        });
        this.RelatedOrderGrid = new Ext.grid.GridPanel({
            store: this.RelatedOrdersStore,
            renderTo: this.Config.RelatedOrderControlElement,
            height: this.Config.GridHeight,
            width: this.Config.GridWidth,
            plugins: [checkColumn],
            colModel: this.GridColumnModel,
            enableColumnHide: false,
            viewConfig: {
                markDirty: false,
                //headersDisabled: true,
                autoHeight: false,
                emptyText: Ext.LocalizedResources.RelatedOrdersNoOrders,
                deferEmptyText: false,
                forceFit: true
            }
        });
    }, 
    GetOrders: function () {
        var orders = [];
        this.RelatedOrdersStore.each(function (record)
        {
            var order = record.data;
            if (order.IsOrderSelected)
            {
                orders.push(order.Id);
            }
        }, this);
        return orders;
    },
    SetRelatedOrders: function (relatedOrders) {
        this.RelatedOrdersStore.removeAll();
        Ext.each(relatedOrders, function (item, index) {
            var record = new this.RelatedOrderRecord({
                Id: item.Id,
                IsOrderSelected: false,
                Number: item.Number,
                SourceOrganizationUnit: item.SourceOrganizationUnit,
                DestinationOrganizationUnit: item.DestinationOrganizationUnit,
                BeginDistributionDate: item.BeginDistributionDate,
                EndDistributionDate: item.EndDistributionDate});
            this.RelatedOrdersStore.add(record);
        }, this);
    }
});
