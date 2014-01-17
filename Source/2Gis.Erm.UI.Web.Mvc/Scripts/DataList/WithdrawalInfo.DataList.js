﻿window.InitPage = function ()
{
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function()
    {
        Ext.apply(this, {
            ExecWithdrawing: function ()
            {
                var params = "dialogWidth:500px; dialogHeight:295px; status:no; resizable:no; scroll:no; ";
                window.showModalDialog("/WithdrawalInfo/WithdrawalDialog", null, params);
                this.refresh();
            },
            RevertWithdrawing: function ()
            {
                var params = "dialogWidth:500px; dialogHeight:345px; status:no; resizable:no; scroll:no; ";
                window.showModalDialog("/WithdrawalInfo/WithdrawalRevertDialog", null, params);
                this.refresh();
            }
        });
    });
};