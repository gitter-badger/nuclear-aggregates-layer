Ext.onReady(function () {
    var form = Ext.select('form').first();

    Ext.get("Cancel").on("click", function () {
        window.returnValue = null;
        window.close();
    });

    Ext.get("OK").on("click", function () {
        if (!Ext.DoubleGis.FormValidator.validate(form)) {
            return;
        }

        window.returnValue = {
            legalPerson: Ext.getCmp('LegalPerson').getValue().id,
            legalPersonProfile: Ext.getCmp('LegalPersonProfile').getValue().id
        };
        window.close();
    });
})