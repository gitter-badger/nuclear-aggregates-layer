window.InitPage = function () {
    this.renderHeader = false;
    Ext.apply(this,
        {
            checkDirty: function () {
                if (this.form.Id.value == 0) {
                    Ext.Msg.alert('', Ext.LocalizedResources.CardIsNewAlert);
                    return false;
                }
                if (this.isDirty) {
                    Ext.Msg.alert('', Ext.LocalizedResources.CardIsDirtyAlert);
                    return false;
                }
                return true;
            },
            CreateOrder: function () {
                if (!this.checkDirty()) return;
                var renewedOrder = Ext.getCmp('RenewedOrder').getValue();
                var self = this;
                if (!renewedOrder) {
                    var progressWindow = Ext.MessageBox.wait(Ext.LocalizedResources.OrderCreationIsInProgress, Ext.LocalizedResources.OrderCreation);

                    Ext.Ajax.request({
                        timeout: 1200000,
                        method: 'POST',
                        url: '/Order/ProlongateOrderForOrderProcessingRequest',
                        params: { orderProcessingRequestId: this.form.Id.value },
                        success: function (xhr) {
                            progressWindow.hide();
                            var response = Ext.decode(xhr.responseText);
                            self.onCreationCompleted(self, response);

                        },
                        failure: function (xhr) {
                            progressWindow.hide();

                            Ext.Msg.show({
                                title: Ext.LocalizedResources.Error,
                                msg: xhr.responseText.length > 0
                                    ? xhr.responseText
                                    : Ext.LocalizedResources.ApplicationError,
                                buttons: Ext.Msg.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        }
                    });
                }
            },
            CancelOrderProcessingRequest: function () {
                if (!this.checkDirty()) return;
                Ext.Ajax.request({
                    method: 'POST',
                    url: '/OrderProcessingRequest/CancelOrderProcessingRequest',
                    params: { orderProcessingRequestId: this.form.Id.value },
                    success: function (xhr) {
                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Info,
                            msg: Ext.LocalizedResources.OrderProcessingRequestCancelled,
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.INFO,
                            fn: function () {
                                location.reload();
                            }
                        });
                    },
                    failure: function () {
                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: Ext.LocalizedResources.ApplicationError,
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    }
                });
            },
            onCreationCompleted: function (self, response) {
                if (response.Messages != 0) {
                    
                    var renderTarget = document.createElement('div');
                    renderTarget.style.visibility = 'hidden';
                    document.body.appendChild(renderTarget);

                    var upgradeResultWindow = new Ext.DoubleGis.UI.Order.UpgradeResultWindow({ renderTarget: renderTarget });
                    upgradeResultWindow.on("action", function (actionType) {
                        switch (actionType) {
                            case "close":
                                self.showOrderLink(self, response);
                                break;
                        }
                    });

                    upgradeResultWindow.showResult(response.Messages);
                } else {
                    self.showOrderLink(self, response);
                }
            },
            showOrderLink: function (self, response) {
                Ext.DoubleGis.Global.Helpers.ShowEntityLink({
                    title: Ext.LocalizedResources.OrderCreation,
                    msg: Ext.LocalizedResources.OrderIsCreated,
                    buttons: Ext.Msg.OK,
                    icon: Ext.MessageBox.INFO,
                    entityName: 'Order',
                    entityId: response.OrderId,
                    entityDescription: response.OrderNumber,
                    fn: function () { self.refresh(true); }
                });
            }
        });

};
