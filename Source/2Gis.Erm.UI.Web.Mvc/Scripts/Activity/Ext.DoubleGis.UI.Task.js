﻿Ext.ns("Ext.DoubleGis.UI");
Ext.DoubleGis.UI.Task = Ext.extend(Ext.DoubleGis.UI.ActivityBase, {
    constructor: function (config) {
        Ext.DoubleGis.UI.Task.superclass.constructor.call(this, config);
    },
    getPurpose: function () {
        return Ext.LocalizedResources.Task;
    },
    Build: function () {
        Ext.DoubleGis.UI.Task.superclass.Build.call(this);
    }
});
