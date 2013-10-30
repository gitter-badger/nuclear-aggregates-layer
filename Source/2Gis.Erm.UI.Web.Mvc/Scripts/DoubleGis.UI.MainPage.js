Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.MainPage = Ext.extend(Object, {
    constructor: function (settings)
    {
        this.Settings = settings;
        this.BuildPage();
    },

    BuildPage: function ()
    {
        var viewport = new Ext.Viewport({
            id: "MainPage",
            layout: "border",
            collapsible: false,
            defaults: {
                split: true,
                bodyStyle: "padding:0px"
            },
            items: [this.BuildWestPanel(), this.BuildContentPage(), this.BuildHeadPanel()]
        });
        Ext.get("ContentFrame").on("load", function () { this.Mask.hide(); }, this);
        this.Mask = new Ext.LoadMask(this.ContentPage.el);
        this.ExpandSection(Ext.getCmp("NavAreaToolbar").items.items[0]);
        Ext.getBody().on("keyup", function (e)
        {
            if (e.shiftKey)
            {
                if (e.keyCode == e.C)
                {
                    Ext.getCmp("AcrdContainer").collapse();
                }
                if (e.keyCode == e.E)
                {
                    Ext.getCmp("AcrdContainer").expand();
                }
            }
        });
    },

    BuildContentPage: function ()
    {
        this.ContentPage = new Ext.Panel({
            id: "StageContainer",
            title: "Stage",
            headerCfg: { cls: "nav-header-title" },
            region: "center",
            margins: "0 0 0 0",
            split: true,
            layout: "anchor",
            anchor: "100%, 100%",
            html: '<iframe style="width: 100%; height: 100%" id="ContentFrame"></iframe>'
        });
        return this.ContentPage;



    },

    BuildWestPanel: function ()
    {
        var accHeaders = [];
        Ext.each(this.Settings.Items, function (header, i)
        {
            var accItems = Ext.DoubleGis.Global.Helpers.NavBarHelper.BuildTree(header);
            accHeaders.push(new Ext.Panel({
                id: header.Id,
                title: header.LocalizedName,
                tbar: new Ext.Toolbar({ hidden: true, items: [] }),
                icon: header.Icon ? Ext.DoubleGis.Global.Helpers.GetEntityIconPath(header.Icon) : undefined,
                listeners: {
                    scope: this,
                    'beforeexpand': { fn: this.ExpandSection }
                },

                items: [{
                    xtype: "treepanel",
                    rootVisible: false,
                    lines: false,
                    enableDD: true,
                    border: false,
                    root: new Ext.tree.AsyncTreeNode({
                        children: accItems
                    }),
                    listeners: {
                        scope: this,
                        click: this.SelectNode
                    }
                }
            ]
            }));
        }, this);

        var acrdArea = new Ext.Panel({
            id: "NavAreaToolbar",
            //animate: true,
            layout: "MSOfficeAccordion",
            anchor: "100%, 100%",
            items: accHeaders,
            layoutConfig: {
                activeOnTop: true,
                collapseFirst: true,
                hideCollapseTool: true,
                titleCollapse: true
                //,animate: true
            }
        });

        return new Ext.Panel(
        {
            id: "AcrdContainer",
            region: "west",
            margins: "0 0 0 0",
            split: true,
            width: 280,
            collapsible: true,
            title: "",
            headerCfg: { cls: "nav-header-title" },
            layout: 'anchor',
            tbar: new Ext.Toolbar({ hidden: true, items: [] }),
            items: [acrdArea]
        }
        );
    },
    BuildHeadPanel: function ()
    {
        /*return new Ext.ux.Ribbon({
        region: 'north',
        height: 115,
        maxHeight: 115,
        minHeight: 115,
        activeTab: 0,
        items: this.BuildRibbonTabs()
        });*/


        return new Ext.Panel({
            id: "HeadPanel",
            height: 56,
            maxHeight: 56,
            minHeight: 56,
            region: "north",
            margins: "0 0 0 0",
            contentEl: "MasterHead"
        });

    },

    BuildRibbonTabs: function ()
    {
        var ribbonTabs = [];
        Ext.each(this.Settings.Items, function (tab, tabNum)
        {
            ribbonTabs.push(new Object({
                title: tab.LocalizedName,
                ribbon: this.BuildRibbonGroups(tab),
                cfg: {
                    defaults: {
                        height: 90
                    }

                }
            }));
        }, this);
        return ribbonTabs;
    },

    BuildRibbonGroups: function (tab)
    {
        var ribbonGroups = [];
        var orphansLength = 0;
        var lrgScale = 0;
        //Собираем сироток
        Ext.each(tab.Items, function (group, groupNum)
        {
            if (group.Items.length == 0)
            {
                orphansLength++;
            }
        });

        if (orphansLength)
        {
            lrgScale = (orphansLength % 3) - 1;

            ribbonGroups.push(new Object({
                title: tab.LocalizedName,
                items: [],
                cfg: {
                    columns: ((orphansLength - lrgScale) / 3) + lrgScale,
                    rows: 3,
                    defaults: {
                        allowDepress: true,
                        enableToggle: true,
                        toggleGroup: 'tg-ribbon'
                    }
                }
            }));

            Ext.each(tab.Items, function (group, groupNum)
            {
                if (group.Items.length == 0)
                {
                    if (group.Icon == null) { group.Icon = "en_ico_16_Default.gif"; }
                    ribbonGroups[0].items.push(new Object({
                        text: group.LocalizedName,
                        rowspan: groupNum <= lrgScale ? 3 : 1,
                        scale: groupNum <= lrgScale ? undefined : 'small',
                        icon: Ext.DoubleGis.Global.Helpers.GetEntityIconPath((groupNum <= lrgScale ? 'btn_lrg_Default.gif' : group.Icon)),
                        iconAlign: groupNum <= lrgScale ? 'top' : 'left',
                        menu: undefined,
                        requestUrl: group.RequestUrl,
                        listeners: {
                            click: this.SelectNode,
                            scope: this
                        }
                    }));
                }
            }, this);
        }

        Ext.each(tab.Items, function (group, groupNum)
        {
            if (group.Items.length)
            {

                lrgScale = (group.Items.length % 3) - 1;

                ribbonGroups.push(new Object({
                    title: group.LocalizedName,
                    items: this.BuildRibbonButtons(group),
                    cfg: {
                        columns: group.Items.length / 3,
                        rows: 3,
                        defaults: {
                            allowDepress: true,
                            enableToggle: true,
                            toggleGroup: 'tg-ribbon'
                        }
                    }
                }));
            }

        }, this);

        return ribbonGroups;
    },

    BuildRibbonButtons: function (group)
    {
        var groupButtons = [];
        var lrgScale = (group.Items.length % 3) - 1;

        Ext.each(group.Items, function (button, btnNum)
        {
            if (button.Icon == null) { button.Icon = "en_ico_16_Default.gif"; }
            groupButtons.push(new Object({
                text: button.LocalizedName,
                rowspan: btnNum <= lrgScale ? 3 : 1,
                scale: btnNum <= lrgScale ? undefined : 'small',
                icon: Ext.DoubleGis.Global.Helpers.GetEntityIconPath(btnNum <= lrgScale ? 'btn_lrg_Default.gif' : button.Icon),
                iconAlign: btnNum <= lrgScale ? 'top' : 'left',
                menu: undefined,
                requestUrl: button.RequestUrl,
                listeners: {
                    click: this.SelectNode,
                    scope: this
                }
            }));

        }, this);

        return groupButtons;
    },

    SelectNode: function (n)
    {

        if (Ext.isNull(n.leaf) || n.leaf === true)
        {
            Ext.getCmp("StageContainer").setTitle(n.text);
            this.Mask.show();
            ContentFrame.location.href = n.attributes ? n.attributes.requestUrl : n.requestUrl;
        }
        else
        {
            n.expanded ? n.collapse() : n.expand();
        }
    },

    ExpandSection: function (n)
    {
        //Ext.getCmp("AcrdContainer").setTitle(n.title);
        var treeEl = n.items.items[0];
        if (treeEl.selModel.getSelectedNode())
        {
            this.SelectNode(treeEl.selModel.getSelectedNode());
            return;
        }
        var firstLeaf = this.GetFirstLeaf(treeEl.root);
        firstLeaf.select();
        this.SelectNode(firstLeaf);
    },
    GetFirstLeaf: function (root)
    {
        return root.leaf ? root : this.GetFirstLeaf(root.childNodes[0]);
    }
});

