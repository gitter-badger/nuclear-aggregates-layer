window.InitPage = function () {
    if (this.isSearchForm)
        return;

    this.on("beforebuild", function () {
        Ext.apply(this, {  
            Edit: function () {
                if (!window.Ext.isNullOrDefault(this.AppendedEntity)) {
                    return;
                }        

                if (this.fireEvent("beforeedit", this) === false) {
                    return;
                }

                if (this.Items.Grid.getSelectionModel().selections.items.length == 0) return;
                var val = this.Items.Grid.getSelectionModel().selections.items[0].data.CategoryId;
                var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);

                var queryString = "";
                if (this.ParentType) {
                    queryString += (queryString ? "&" : "?") + "pType=" + this.ParentType;
                }
                if (this.ParentId) {
                    queryString += (queryString ? "&" : "?") + "pId=" + this.ParentId;
                }
                if (this.currentSettings.ReadOnly) {
                    queryString += (queryString ? "&" : "?") + "ReadOnly=" + this.currentSettings.ReadOnly;
                }

                var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl('Category', val, queryString);
                window.open(sUrl, "_blank", params);
            },
        });
    });
    
    this.on("afterbuild", function () {
        this.Items.Grid.addListener("RowDblClick", this.Edit, this);
    });
};