Ext.namespace('Ext.ux');
Ext.ux.LookupFieldOwner = Ext.extend(Object, {
    constructor: function (config)
    {
        window.Ext.apply(this, config);
    },
    init: function (c)
    {
        this.component = c;

        // do not show reserve user
        this.component.extendedInfo = "hideReserveUser=true";
        
        this.component.on("beforequery",
        function ()
        {
           
            if (window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value * 1 !== 0)
            {
                var params = "dialogWidth:450px; dialogHeight:300px; status:yes; scroll:no;resizable:no;";
                var sUrl = "/GroupOperation/Assign/" + window.Ext.getDom("ViewConfig_EntityName").value;
                var result = window.showModalDialog(sUrl, [window.Ext.getDom("ViewConfig_Id").value], params);
                if (result === true)
                {
                    window.Card.refresh(true);
                }
                return false;
            }
            else
            {
                return true;
            }
        }, this);
        this.component.on("contentkeyup", function () { return false; }, this);
        this.component.on("contentkeypress", function () { return false; }, this);
    }
});
Ext.preg('lookupfieldowner', Ext.ux.LookupFieldOwner);