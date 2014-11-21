window.InitPage = function () {
    Ext.apply(this, PrintLogic);

    Ext.apply(this,
    {
        PrintBargain: function () {
            var entityId = {
                bargainId: Ext.getDom('Id').value
            };
            var callback = function (profileId) {
                this.PrintWithoutProfileChoosing('PrintBargain', entityId.bargainId, profileId);
            };

            this.ChooseProfile(entityId, callback);
        },

        PrintNewSalesModelBargain: function () {
            var entityId = {
                bargainId: Ext.getDom('Id').value
            };
            var callback = function (profileId) {
                this.PrintWithoutProfileChoosing('PrintNewSalesModelBargain', entityId.bargainId, profileId);
            };

            this.ChooseProfile(entityId, callback);
        },

        PrintBargainProlongationAgreement: function () {
            var entityId = {
                bargainId: Ext.getDom('Id').value
            };
            var callback = function (profileId) {
                this.PrintWithoutProfileChoosing('PrintBargainProlongationAgreement', entityId.bargainId, profileId);
            };

            this.ChooseProfile(entityId, callback);
        }
    });

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