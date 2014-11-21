Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Territory = Ext.extend(Ext.DoubleGis.UI.Card, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Territory.superclass.constructor.call(this, config);
    },
    
    Activate: function () {
        var parameters = {
            Values: [this.form.Id.value],
            DoSpecialConfirmation: null
        };
        var url = "/GroupOperation/Activate/" + this.EntityName;
        var result = window.showModalDialog(url, parameters, "dialogWidth:500px; dialogHeight:203px; scroll:no;resizable:no;");
        if (result == true) {
            this.refresh();
        }
    }
});
