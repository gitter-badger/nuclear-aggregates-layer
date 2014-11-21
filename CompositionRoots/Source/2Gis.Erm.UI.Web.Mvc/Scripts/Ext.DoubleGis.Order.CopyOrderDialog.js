Ext.ns('Ext.DoubleGis.Order');

Ext.DoubleGis.Order.CopyOrderDialog = Ext.extend(Ext.Window, {
    orderId: null,
    target: null,
    isTerminationAllowed: true,
    initialWindow: null,
    progressWindow: null,
    successWindow: null,

    initComponent: function ()
    {
        Ext.DoubleGis.Order.CopyOrderDialog.superclass.initComponent.apply(this, arguments);

        var self = this;

        var initialStatePanel = new Ext.Panel({
            layout: 'vbox',
            border: false,
            layoutConfig: {
                align: 'stretch',
                defaultMargins: {
                    top: 5,
                    right: 5,
                    bottom: 0,
                    left: 5
                }
            },

            items: [
                    new Ext.Button({
                        text: Ext.LocalizedResources.CopyCurrentOrder,
                        scale: 'large',
                        iconCls: 'copyOrderButton',
                        cls: 'x-btn-text-icon',
                        icon: 'images/Actions-edit-copy-icon.png',
                        handler: function ()
                        {
                            self.beginCopying(false);
                        }
                    }),
                    new Ext.Button({
                        text: Ext.LocalizedResources.CopyOrderWithTermination,
                        scale: 'large',
                        iconCls: 'copyOrderWithTerminationButton',
                        cls: 'x-btn-text-icon',
                        disabled: !this.isTerminationAllowed,
                        handler: function ()
                        {
                            self.beginCopying(true);
                        }
                    })
                   ]
        });

        this.initialWindow = new Ext.Window({
            applyTo: this.target,
            layout: 'fit',
            width: 270,
            height: 160,
            closeAction: 'close',
            plain: true,
            title: Ext.LocalizedResources.OrderCopying,
            items: initialStatePanel,
            buttons: [{
                text: Ext.LocalizedResources.Cancel,
                handler: function ()
                {
                    self.initialWindow.hide();
                }
            }]
        });

    },

    show: function ()
    {
        this.initialWindow.show();
    },

    beginCopying: function (withTermination)
    {
        var self = this;

        this.initialWindow.hide();
        this.progressWindow = Ext.MessageBox.wait(Ext.LocalizedResources.OrderCopyingInProgress, Ext.LocalizedResources.OrderCopying, { animate: false });

        Ext.Ajax.request({
            timeout: 1200000,
            method: 'POST',
            url: '/Order/CopyOrder/',
            params: { orderId: this.orderId, isTechnicalTermination: withTermination },
            success: function (xhr)
            {
                self.progressWindow.hide();
                var response = Ext.decode(xhr.responseText);
                if (response.ErrorMessage) {
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: response.ErrorMessage,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                } else {
                    self.onCopyingCompleted(self, response);
                }
            },
            failure: function (xhr)
            {
                self.progressWindow.hide();
                Ext.Msg.show({
                    title: Ext.LocalizedResources.Error,
                    msg: xhr.responseText || Ext.LocalizedResources.OrderCopyingFailure,
                    buttons: Ext.Msg.OK,
                    icon: Ext.MessageBox.ERROR
                });
            }
        });
    },

    onCopyingCompleted: function (self, response)
    {
        self.progressWindow.hide();
        
        if (response.Messages != 0) {
            var upgradeResultWindow = new Ext.DoubleGis.UI.Order.UpgradeResultWindow({ renderTarget: self.target });
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
            title: Ext.LocalizedResources.OrderCopyingCompleted,
            msg: Ext.LocalizedResources.OrderCopyingSuccess,
            buttons: Ext.Msg.OK,
            icon: Ext.MessageBox.INFO,
            entityName: 'Order',
            entityId: response.OrderId,
            entityDescription: response.OrderNumber
        });
    }
});