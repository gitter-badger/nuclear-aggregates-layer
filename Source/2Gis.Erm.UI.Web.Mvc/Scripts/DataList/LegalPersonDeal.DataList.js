window.InitPage = function() {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function() {
        Ext.apply(this, {
            AppendLegalPerson: function() {
                // Параметры pId, pType предназначены не для сервера, 
                // они будут прочитаны существующим механизмом в диалоге операции Append 
                // и использованы, если из него будет создана карточка юрлица.
                var parameters = { pId: this.ParentId, pType: this.ParentType };
                this.Append({ UrlParameters: parameters });
            },

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

                if (this.Items.Grid.getSelectionModel().selections.items.length != 1) return;

                var queryParameters = {};
                if (this.ParentType) {
                    queryParameters['pType'] = this.ParentType;
                }
                if (this.ParentId) {
                    queryParameters['pId'] = this.ParentId;
                }
                if (this.currentSettings.ReadOnly) {
                    queryParameters['ReadOnly'] = this.currentSettings.ReadOnly;
                }

                var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}",
                                            window.Ext.DoubleGis.Global.UISettings.ActualCardWidth,
                                            window.Ext.DoubleGis.Global.UISettings.ActualCardHeight,
                                            window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop,
                                            window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
                
                var legalPersonId = this.Items.Grid.getSelectionModel().selections.items[0].data.LegalPersonId;
                var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl('LegalPerson', legalPersonId, '?' + Ext.urlEncode(queryParameters));

                window.open(sUrl, "_blank", params);
            }
        });
    });

    this.on("afterbuild", function () {
        this.Items.Grid.addListener("RowDblClick", this.Edit, this);
    });
}