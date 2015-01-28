
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            ExportAccountDetailsToServiceBus: function () {
                this.ShowDialogWindow("/Account/ExportToServiceBusDialog", "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ", false);
            },
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            }
        });
    });
};
