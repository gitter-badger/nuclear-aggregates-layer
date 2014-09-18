Ext.onReady(function () {
    Ext.get("Cancel").on("click", function () { window.close(); });
    Ext.get("OK").on("click", function () {
        if (Ext.DoubleGis.FormValidator.validate(EntityForm)) {
            Ext.getDom("OK").disabled = "disabled";
            Ext.getDom("Cancel").disabled = "disabled";
            EntityForm.submit();
        }
    });
});