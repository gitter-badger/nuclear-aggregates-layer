Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.ContactRelationController = Ext.extend(Ext.util.Observable, {
    suppressClientUpdate: false,
    clientField: "ClientId",
    clientComp: "Client",
    contactField: null,
    contactComp: null,
    invoker: null,
    constructor: function (config) {
        Ext.DoubleGis.UI.ContactRelationController.superclass.constructor.call(this, config);

        config = config || {};
        this.contactField = config.contactField;
        this.contactComp = config.contactComp;

        Ext.getCmp(this.clientComp).on("change", this.onClientChanged, this);
        Ext.getCmp(this.contactComp).on("afterselect", this.onContactChanged, this);
       
    },
    onClientChanged: function () {
        if (Ext.fly(this.clientComp).getValue()) {
            this.suppressClientUpdate = true;

            if (this.invoker !== "Contact") {
                Ext.getCmp(this.contactComp).forceGetData();
            }

            this.suppressClientUpdate = false;
            this.invoker = null;
        }
    },
    onContactChanged: function () {
        if (this.suppressClientUpdate) return;
        if (!Ext.fly(this.clientField).getValue() && Ext.fly(this.contactField).getValue()) {
            this.invoker = "Contact";
            Ext.getCmp(this.clientComp).forceGetData({ extendedInfo: "ContactId={" + this.contactField + "}" });
        }
    }
});
