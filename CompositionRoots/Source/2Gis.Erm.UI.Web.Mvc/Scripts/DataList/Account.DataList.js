
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            ExportAccountDetailsTo1C: function () {
                this.ShowDialogWindow("/Account/ExportTo1CDialog", "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ", false);
            },
            ExportAccountDetailsToServiceBus: function () {
                this.ShowDialogWindow("/Account/ExportToServiceBusDialog", "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ", false);
            },
            Assign: function () {
                this.ShowDialogWindowForOneOrMoreEntities("/GroupOperation/Assign/" + this.EntityName, "dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ");
            }
        });
    });
};
