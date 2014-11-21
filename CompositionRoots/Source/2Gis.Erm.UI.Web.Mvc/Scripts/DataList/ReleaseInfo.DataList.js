window.InitPage = function ()
{
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function()
    {
        Ext.apply(this, {
            ExecRelease: function()
            {
                var params = "dialogWidth:500px; dialogHeight:220px; status:no; resizable:no; scroll:no; ";
                window.showModalDialog("/ReleaseInfo/ReleaseDialog", null, params);
                this.refresh();
            },
            RevertRelease: function()
            {
                var params = "dialogWidth:500px; dialogHeight:260px; status:no; resizable:no; scroll:no; ";
                window.showModalDialog("/ReleaseInfo/ReleaseRevertDialog", null, params);
                this.refresh();
            }
        });
    });
};