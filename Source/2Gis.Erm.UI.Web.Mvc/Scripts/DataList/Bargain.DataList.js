
window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {
            CloseBargains: function () {
                this.ShowDialogWindow("/Bargain/CloseBargains", 'dialogHeight:300px; dialogWidth:650px; status:yes; scroll:no; resizable:no; ', false);
            }
        });
    });
};
