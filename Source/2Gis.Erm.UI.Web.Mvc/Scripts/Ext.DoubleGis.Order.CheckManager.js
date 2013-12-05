Ext.ns("Ext.DoubleGis.UI.Order");
Ext.DoubleGis.UI.Order.CheckManager = Ext.extend(Ext.util.Observable,
function ()
{
    var //Private members
        self,
        renderTarget,
        orderId,
        checkResultWindow,
        upgradeResultWindow,
        resultWindowActionHandler,
        orderValidationServiceUrl;

    var //Private methods
        ensureResultWindowInitialized = function ()
        {
            if (checkResultWindow == null)
            {
                checkResultWindow = new Ext.DoubleGis.UI.Order.CheckResultWindow({ renderTarget: renderTarget });
                checkResultWindow.on("action", onResultWindowAction);
            }
        },
        showUpgradeResult = function (messages) {
            if (upgradeResultWindow == null) {
                upgradeResultWindow = new Ext.DoubleGis.UI.Order.UpgradeResultWindow({ renderTarget: renderTarget });
                upgradeResultWindow.on("action", function (actionType) {
                    switch (actionType) {
                        case "close":
                            self.fireEvent("repairOutdatedOrderPositionsCompleted");
                            break;
                    }
                });
            }

            upgradeResultWindow.showResult(messages);
        },
        onResultWindowAction = function (actionType)
        {
            resultWindowActionHandler(actionType);
        };

    return {
        constructor: function (config)
        {
            self = this;

            this.addEvents("validationCompleted");
            this.addEvents("validationError");

            renderTarget = config.renderTarget;
            orderId = config.orderId;
            orderValidationServiceUrl = config.orderValidationServiceUrl;
        },

        performManualCheck: function ()
        {
            var messageBox = Ext.MessageBox.wait(Ext.LocalizedResources.CheckInProgressPleaseStandBy, Ext.LocalizedResources.OrderCheck, { animate: false });
            var jsonStore = new Ext.data.JsonStore({
                autoLoad: true,
                root: 'Messages',
                fields: ['MessageText', 'OrderId', 'OrderNumber', 'RuleCode', 'Type'],
                proxy: new Ext.data.ScriptTagProxy({
                    url: orderValidationServiceUrl + '/Single/' + orderId,
                    timeout: 60000
                }),
                listeners: {
                    load: function (store, records, options) {
                        messageBox.hide();

                        var messages = [];
                        Ext.each(records, function (rec) {
                            messages.push(rec.json);
                        });
                        resultWindowActionHandler = function (actionType)
                        {
                            switch (actionType)
                            {
                                case "refresh":
                                    self.performManualCheck();
                                    break;
                                case "repairOutdatedOrderPositions":
                                    self.repairOutdatedOrderPositionsWithPrompt();
                                    break;
                            }
                        };
                        ensureResultWindowInitialized();
                        checkResultWindow.showResult(messages, false);
                    },
                    exception: function (proxy, type, action, o, response) {
                        messageBox.hide();
                        var message = response == null
                            ? Ext.LocalizedResources.OrderCheckFailed
                            : Ext.LocalizedResources.OrderCheckFailed + ": " + (response.responseText || response.statusText);

                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: message,
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    }
                }
            });
        },
        
        repairOutdatedOrderPositionsWithPrompt: function () {
            Ext.Msg.show({
                title: Ext.LocalizedResources.OrderCheck,
                msg: Ext.LocalizedResources.DiscountResetWarning,
                icon: Ext.Msg.WARNING,
                buttons: Ext.Msg.YESNO,
                fn: function(btn) {
                    if (btn != 'yes')
                        return;
                    self.repairOutdatedOrderPositions();
                }
            });
        },

        repairOutdatedOrderPositions: function () {
            var messageBox = Ext.MessageBox.wait(Ext.LocalizedResources.RepairInProgressPleaseStandBy, Ext.LocalizedResources.OrderCheck, { animate: false });
            Ext.Ajax.request({
                method: 'POST',
                timeout: 1200000,
                url: '/Order/RepairOutdatedOrderPositions/',
                params: { orderId: orderId },
                success: function (xhr) {
                    messageBox.hide();
                    var response = Ext.decode(xhr.responseText);
                    if (response.Messages == 0) {
                        self.fireEvent("repairOutdatedOrderPositionsCompleted");
                    } else {
                        showUpgradeResult(response.Messages);
                    }
                },
                failure: function () {
                    messageBox.hide();
                    Ext.Msg.show({
                        title: Ext.LocalizedResources.Error,
                        msg: Ext.LocalizedResources.OrderRepairFailed,
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                }
            });
        },
        
        validateStateChangeAsync: function (newState)
        {
            var messageBox = Ext.MessageBox.wait(Ext.LocalizedResources.CheckInProgressPleaseStandBy, Ext.LocalizedResources.OrderCheck, { animate: false });
            var jsonStore = new Ext.data.JsonStore({
                autoLoad: true,
                root: 'Messages',
                fields: ['MessageText', 'OrderId', 'OrderNumber', 'RuleCode', 'Type'],
                proxy: new Ext.data.ScriptTagProxy({
                    url: orderValidationServiceUrl + '/SingleStateTransfer/' + orderId + '/' + newState,
                    timeout: 1200000
                }),
                listeners: {
                    load: function (store, records, options) {
                        messageBox.hide();

                        var messages = [];
                        Ext.each(records, function (rec) {
                            messages.push(rec.json);
                        });

                        if (Ext.isEmpty(messages)) {
                            self.fireEvent("validationCompleted", true);
                            return;
                        }

                        resultWindowActionHandler = function (actionType) {
                            switch (actionType) {
                                case "refresh":
                                    self.validateStateChangeAsync(newState);
                                    break;
                                case "repairOutdatedOrderPositions":
                                    self.repairOutdatedOrderPositionsWithPrompt();
                                    break;
                                case "proceed":
                                    self.fireEvent("validationCompleted", true);
                                    break;
                                case "close":
                                    self.fireEvent("validationCompleted", false);
                                    break;
                            }
                        };
                        
                        ensureResultWindowInitialized();
                        checkResultWindow.showResult(messages, true);
                    },
                    exception: function (proxy, type, action, o, response) {
                        messageBox.hide();
                        var message = response == null
                            ? Ext.LocalizedResources.OrderCheckFailed
                            : Ext.LocalizedResources.OrderCheckFailed + ": " + (response.responseText || response.statusText);

                        Ext.Msg.show({
                            title: Ext.LocalizedResources.Error,
                            msg: message,
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    }
                }
            });
        }
    };

} ());