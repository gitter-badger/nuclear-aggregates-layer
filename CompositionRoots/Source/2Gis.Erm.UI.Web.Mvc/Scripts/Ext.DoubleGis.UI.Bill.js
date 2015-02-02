window.InitPage = function () {
    Ext.apply(this, PrintLogic);

    Ext.apply(this, {
        PrintBill: function () {
            this.PrintWithoutProfileChoosing('PrintSingleBill', Ext.getDom('Id').value);
        }
    });
}