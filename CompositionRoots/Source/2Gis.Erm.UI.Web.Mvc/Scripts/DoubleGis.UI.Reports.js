Ext.ns("Ext.DoubleGis.UI");

Ext.DoubleGis.UI.Reports = Ext.extend(Object, {
    constructor: function (settings) {
        this.settings = settings || {};
        this.buildPage();
    },

    buildPage: function () {
        this.treeLoader = new Ext.tree.TreeLoader({
            dataUrl: '/Report/Tree',
            requestMethod: 'GET',
            listeners: {
                load: this.foo,
                scope: this
            }
        }),

        this.rootNode = new Ext.tree.AsyncTreeNode({
            expanded: true
        });

        this.viewport = new Ext.Viewport({
            id: 'ReportsPage',
            layout: 'border',
            collapsible: false,
            defaults: {
                split: true,
                bodyStyle: 'padding:0px'
            },
            items: [{
                region: 'west',
                collapsible: true,
                title: window.Ext.LocalizedResources.Reports,
                xtype: 'treepanel',
                width: '38%',
                autoScroll: true,
                split: true,
                loader: this.treeLoader,
                root: this.rootNode,
                rootVisible: false,
                listeners: { click: this.onMenuItemClick, scope: this }

            }, {
                id: 'ContentFrameHolder',
                xtype: 'panel',
                region: 'center',
                html: '<iframe id="ContentFrame" width="100%" height=100%></iframe>'
            }]
        });

        Ext.get('ContentFrame').on('load', function () { if (this.mask) { this.mask.hide(); }; }, this);

        this.mask = new Ext.LoadMask('ContentFrameHolder');
    },

    foo: function (_, node) {
        if (node && node.firstChild) {
            this.loadReportPage(node.firstChild.attributes.id);
        }
    },

    onMenuItemClick: function (node) {
        this.loadReportPage(node.id);
    },

    loadReportPage: function (id) {
        this.mask.show();
        window.ContentFrame.location = '/Report/Edit/' + id;
    }
});

Ext.onReady(function () {
    window.MainPage = new Ext.DoubleGis.UI.Reports();
});