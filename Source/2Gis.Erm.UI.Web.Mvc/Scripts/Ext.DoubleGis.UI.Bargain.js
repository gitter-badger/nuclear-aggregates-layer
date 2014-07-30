window.InitPage = function () {
    window.Card.on('beforepost', function () { window.returnValue = true; });

    this.on("afterbuild", function () {
        if (window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {
            this.Items.TabPanel.add(
                {
                    xtype: "actionshistorytab",
                    pCardInfo:
                    {
                        pTypeId: this.Settings.EntityId,
                        pId: window.Ext.getDom("ViewConfig_Id").value,
                        pTypeName: Ext.get("ViewConfig_EntityName").dom.value
                    }
                });
        }
    });
}