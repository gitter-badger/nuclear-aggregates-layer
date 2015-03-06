Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.RegardingObjectController = Ext.extend(Ext.util.Observable, {
    suppressClientUpdate: false,
    invoker: null,
    constructor: function (config) {
        Ext.DoubleGis.UI.RegardingObjectController.superclass.constructor.call(this, config);

        Ext.getCmp("Client").on("change", this.onClientChanged, this);
        Ext.getCmp("Firm").on("afterselect", this.onFirmChanged, this);
        Ext.getCmp("Deal").on("afterselect", this.onDealChanged, this);
        
    },
    onClientChanged: function () {
        // Если контакт 1, то выставляем его.
        // Если фирма 1, то выбираем её.
        // Если сделка 1, то выбираем её.
        // при этом проверяем, чем было вызвано изменение поля Client

        if (Ext.fly("Client").getValue()) {
            this.suppressClientUpdate = true;

            if (this.invoker !== "Firm") {
                Ext.getCmp("Firm").forceGetData();
            }

            if (this.invoker !== "Deal") {
                Ext.getCmp("Deal").forceGetData();
            }

            this.suppressClientUpdate = false;
            this.invoker = null;
        }
    },
    onDealChanged: function () {
        if (!this.suppressClientUpdate && !Ext.fly("ClientId").getValue() && Ext.fly("DealId").getValue()) {
            // Заполнить поле "Клиент" если оно ещё не заполнено
            this.invoker = "Deal";
            var clientCmp = Ext.getCmp("Client");
            clientCmp.forceGetData({
                extendedInfo: "DealId={DealId}"
            });
        }
    },
    onFirmChanged: function () {
        if (!this.suppressClientUpdate && !Ext.fly("ClientId").getValue() && Ext.fly("FirmId").getValue()) {
            // Заполнить поле "Клиент" если оно ещё не заполнено
            this.invoker = "Firm";
            var clientCmp = Ext.getCmp("Client");
            clientCmp.forceGetData({
                extendedInfo: "FirmId={FirmId}"
            });
        }
    }
});
