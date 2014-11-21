Ext.ns('Ext.DoubleGis.UI');
if (!Ext.DoubleGis.UI.QuikFind) {
    Ext.DoubleGis.UI.QuikFind = {};
}

Ext.DoubleGis.UI.QuickFind = Ext.extend(Ext.util.Observable,
{
    constructor: function (name) {

        Ext.DoubleGis.UI.QuikFind[name] = this;

        this.addEvents('change');
        this.Name = name;
        this.SearchButton = document.getElementById(this.Name + "_Button");
        this.SearchInput = document.getElementById(this.Name + "_Text");

        this.SearchButton.src = Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfind.gif");
        this.SearchButton.onmouseout = function () { this.src = Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfind.gif"); };
        this.SearchButton.onmouseover = function () { this.src = Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfindhover.gif"); };
        this.SearchButton.onmousedown = function () { this.src = Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfinddown.gif"); };
        this.SearchButton.onmouseup = function () { this.src = Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/qfindhover.gif"); };

        Ext.fly(this.SearchButton.id).on("click", function () { this.fireEvent('change', this); }, this);
        Ext.fly(this.SearchInput.id).on("keydown",
                                        function (e) { e = e || window.event; if (e.keyCode == 13) this.fireEvent('change', this); },
                                        this);
    },

    getValue: function () {
        return this.SearchInput.value;
    },
    setValue: function (value) {
        this.SearchInput.value = value;
        this.fireEvent('change', this);
    }
});