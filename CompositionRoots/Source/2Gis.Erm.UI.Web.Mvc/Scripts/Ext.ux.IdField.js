Ext.namespace('Ext.ux');
Ext.ux.IdField = Ext.extend(Ext.Component, {
    serviceUrl: "",
    btnDis: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_dis_lookup.gif"),
    btnOff: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_off_lookup.gif"),
    btnOn: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_on_lookup.gif"),
    template: new window.Ext.Template(
        '<table id="{name}_Wrapper" class="x-lookup">',
        '<tr>',
        '<td>',
        '<input type="text" maxLength=19 id="{name}" name="{name}" class="x-lookup-normal inputfields"/>',
        '</td>',
        '<td width="22">',
        '<img id="{name}_Btn" alt="" title="" class="x-lookup" src="{btnOff}" />',
        '</td>',
        '</tr>',
        '</table>'),

    initComponent: function() {
        window.Ext.ux.LookupField.superclass.initComponent.call(this);
        this.addEvents("change");
    },
    onRender: function() {
        var name = this.el.id;
        var x = this.template.insertBefore(this.el, {
            name: name,
            btnOff: this.btnOff
        });

        this.el.remove();
        this.el = window.Ext.get(name);
        this.button = window.Ext.get(name + "_Btn");
        this.button.on("mouseout", this.onButtonMouseOut, this);
        this.button.on("mouseover", this.onButtonMouseOver, this);
        this.button.on("click", this.onButtonClick, this);
        this.mask = new Ext.LoadMask(x);
        this.serviceUrl = Ext.IdentityServiceRestUrl;
    },
    onButtonMouseOut: function(event) {
        if (event.target) {
            event.target.src = this.btnOff;
        }
    },
    onButtonMouseOver: function(event) {
        if (event.target) {
            event.target.src = this.btnOn;
        }
    },
    disable: function () {
        this.el.dom.readOnly = true;
        this.button.un("click", this.onButtonClick, this);
        this.button.un("mouseout", this.onButtonMouseOut, this);
        this.button.un("mouseover", this.onButtonMouseOver, this);
        this.button.dom.src = this.btnDis;
    },
    setValue: function (value) {
        this.el.dom.value = value;
    },
    onButtonClick: function () {
        window.Card.Mask.show();
        window.Ext.Ajax.request({
            scope: this,
            method: 'GET',
            url: this.serviceUrl + '/NewIdentity',
            timeout: 60000,
            success: function (result, options) {
                window.Card.Mask.hide();
                this.setValue(result.responseText);
            },
            failure: function (xhr, options) {
                window.Card.Mask.hide();
                Ext.MessageBox.show({
                    title: 'Ошибка получения идентификатора',
                    msg: xhr.isTimeout ? 'Timeout expired' : xhr.responseText,
                    buttons: window.Ext.MessageBox.OK,
                    width: 300,
                    icon: window.Ext.MessageBox.ERROR
                });
            }
        });
    }
});
Ext.reg('idfield', Ext.ux.IdField);
