window.InitPage = function ()
{
    if (this.isSearchForm)
    {
        this.on("afterbuild", function (cmp)
        {
            var cnt = Ext.getCmp('gridPane');
            var tp = this.grid;
            tp.anchor = "100%, 40%";
            delete tp.anchorSpec;
            var adsBag = cnt.add(new Ext.ux.AdvertisementBagPanel(
            {
                id: 'adsElemBag',
                autoScroll: true,
                title: Ext.LocalizedResources.AdvertisementElements,
                anchor: "100%, 60%"
            }));
            cnt.doLayout();
            this.grid.on('rowclick', function (grid, rowIndex, evt) { this.store.reload({ params: { id: grid.getSelectionModel().selections.items[0].data.Id} }); }, adsBag);
            this.grid.getStore().on('load', function () { this.store.removeAll(); }, adsBag);
        });
    }
    else
    {
        this.on("afterbuild", function (cmp)
        {
            var cnt = Ext.getCmp('DataList');
            var tp = this.Items.Grid;
            tp.anchor = "100%, 40%";
            delete tp.anchorSpec;
            var adsBag = cnt.add(new Ext.ux.AdvertisementBagPanel(
            {
                id: 'adsElemBag',
                autoScroll: true,
                title: Ext.LocalizedResources.AdvertisementElements,
                anchor: "100%, 60%"
            }));
            cnt.doLayout();
            tp.on('rowclick', function (grid, rowIndex, evt) { this.store.reload({ params: { id: grid.getSelectionModel().selections.items[0].data.Id} }); }, adsBag);
            tp.getStore().on('load', function () { this.store.removeAll(); }, adsBag);
        });
        this.on("beforerebuild", function (cmp)
        {
            Ext.getCmp('adsElemBag').destroy();
        });
        this.on("afterrebuild", function (cmp)
        {
            var cnt = Ext.getCmp('DataList');
            var tp = this.Items.Grid;
            tp.anchor = "100%, 40%";
            delete tp.anchorSpec;
            var adsBag = cnt.add(new Ext.ux.AdvertisementBagPanel(
            {
                id: 'adsElemBag',
                autoScroll: true,
                title: Ext.LocalizedResources.AdvertisementElements,
                anchor: "100%, 60%"
            }));
            cnt.doLayout();
            tp.on('rowclick', function (grid, rowIndex, evt) { this.store.reload({ params: { id: grid.getSelectionModel().selections.items[0].data.Id} }); }, adsBag);
            tp.getStore().on('load', function () { this.store.removeAll(); }, adsBag);
        });
    }
};
