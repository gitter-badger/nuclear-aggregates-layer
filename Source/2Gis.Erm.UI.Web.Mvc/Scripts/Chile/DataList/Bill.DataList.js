window.InitPage = function ()
{
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function ()
    {
        Ext.apply(this, {
            CustomizeToolbar: function ()
            {

                if (this.ParentState != "Active")
                {
                    this.Items.Grid.getTopToolbar().items.item("Create").disable();
                    this.Items.Grid.getTopToolbar().items.item("DeleteAll").disable();
                }
            },
            Create: function ()
            {
                var url = "/Bill/Create/?orderId=" + this.ParentId;
                var params = "dialogWidth:780px; dialogHeight:430px; status:yes; scroll:no;resizable:no;";
                window.showModalDialog(url, null, params);
                this.refresh();
            },
            DeleteAll: function ()
            {
                var url = "/Bill/DeleteAll/";
                var params = "dialogWidth:500px; dialogHeight:150px; status:yes; scroll:no;resizable:no;";
                var arguments = {
                    orderId: this.ParentId
                };
                window.showModalDialog(url, arguments, params);
                this.refresh();
            }
        });
    });
    this.on("afterbuild", function ()
    {
        this.CustomizeToolbar();
    });
    this.on("afterrebuild", function ()
    {
        this.CustomizeToolbar();
    });
};