Ext.onReady(function () {
    if (Ext.getDom("Notifications").innerHTML.trim() == "OK") {
        window.returnValue = 'OK';
        window.close();
        return;
    } else if (Ext.getDom("Notifications").innerHTML.trim() != "") {
        Ext.getDom("Notifications").style.display = "block";
    }

    Ext.get("Cancel").on("click", function () { window.returnValue = 'CANCELED'; window.close(); });
    Ext.get("OK").on("click", function () {
        if (Ext.DoubleGis.FormValidator.validate(window.EntityForm)) {
            Ext.getDom("OK").disabled = "disabled";
            Ext.getDom("Cancel").disabled = "disabled";
            EntityForm.submit();
        }
    });
});