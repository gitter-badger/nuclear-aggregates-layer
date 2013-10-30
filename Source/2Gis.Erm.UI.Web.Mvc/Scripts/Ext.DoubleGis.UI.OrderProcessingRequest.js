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
                            method: 'POST',
                            url: '/Order/ProlongateOrderForOrderProcessingRequest',
                            params: { orderProcessingRequestId: this.form.Id.value },
                            success: function (xhr) {
                                progressWindow.hide();
                                var response = Ext.decode(xhr.responseText);

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
                            },
                            failure: function () {
                                progressWindow.hide();
                                Ext.Msg.show({
                                    title: Ext.LocalizedResources.Error,
                                    msg: Ext.LocalizedResources.ApplicationError,
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
                }
            });
   
};
