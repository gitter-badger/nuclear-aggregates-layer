window.InitPage = function() {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function() {
        Ext.apply(this, {
            MakeMain: function() {
                if (this.Items.Grid.getSelectionModel().selections.items.length != 1) {
                    window.Ext.MessageBox.show({
                        title: '',
                        msg: Ext.LocalizedResources.MustSelectOnlyOneObject,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });

                    return;
                }

                if (this.Items.Grid.getSelectionModel().selections.items[0].data.IsMain) {
                    window.Ext.MessageBox.show({
                        title: '',
                        msg: Ext.LocalizedResources.LegalPersonIsAlreadyMain,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });

                    return;
                }

                Ext.Ajax.request(
                {
                    timeout: 1200000,
                    url: "/Deal/SetMainLegalPerson",
                    method: "POST",
                    params: {
                        dealId: this.Items.Grid.getSelectionModel().selections.items[0].data.DealId,
                        legalPersonId: this.Items.Grid.getSelectionModel().selections.items[0].data.LegalPersonId
                    },
                    success: function() {
                        location.reload();
                    }
                });
            },

            Edit: function () {
                
                if (this.fireEvent("beforeedit", this) === false) {
                    return;
                }

                if (this.Items.Grid.getSelectionModel().selections.items.length == 0) return;

                var legalPersonId = this.Items.Grid.getSelectionModel().selections.items[0].data.LegalPersonId;
                var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl('LegalPerson', legalPersonId);
                if (this.ParentType) {
                    sUrl = Ext.urlAppend(sUrl, "pType=" + this.ParentType);
                }
                if (this.ParentId) {
                    sUrl = Ext.urlAppend(sUrl, "pId=" + this.ParentId);
                }
                if (this.currentSettings.ReadOnly) {
                    sUrl = Ext.urlAppend(sUrl, "ReadOnly=" + this.currentSettings.ReadOnly);
                }

                var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}",
                                            window.Ext.DoubleGis.Global.UISettings.ActualCardWidth,
                                            window.Ext.DoubleGis.Global.UISettings.ActualCardHeight,
                                            window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop,
                                            window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
                
                window.open(sUrl, "_blank", params);
            }
        });
    });

    this.on("afterbuild", function () {
        this.Items.Grid.addListener("RowDblClick", this.Edit, this);
    });
}