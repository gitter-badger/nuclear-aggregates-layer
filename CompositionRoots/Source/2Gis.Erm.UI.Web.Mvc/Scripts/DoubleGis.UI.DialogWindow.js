Ext.onReady(function ()
{
    var onValidatorAttach = function (formOptions, field, rule) {
        var fieldId = field.FieldName.replace(".", "_");
        var el = window.Ext.getDom(fieldId);
        if (rule.ValidationType == "required")
        {
            el = window.Ext.select('label[for=' + fieldId + ']', true, formOptions.form.id);
            if (el && el.elements && el.elements[0])
            {
                window.Ext.DomHelper.insertHtml('afterEnd', el.elements[0].dom, '<span class="req" id="' + fieldId + "-req" + '">*</span>');
            }
        }
        if (rule.ValidationType == "stringlength")
        {
            if (el && rule.ValidationParameters.maximumLength)
            {
                el.maxLength = rule.ValidationParameters.maximumLength;
            }
        }
        if (rule.ValidationType == "email")
        {
            new Ext.ux.LinkField(
                {
                    applyTo: el,
                    contactTypeCfg:
                        {
                            linkCls: Ext.ux.LinkField.prototype.contactTypeRegistry.email.linkCls,
                            protocolPrefix: Ext.ux.LinkField.prototype.contactTypeRegistry.email.protocolPrefix,
                            protocolRegex: Ext.ux.LinkField.prototype.contactTypeRegistry.email.protocolRegex,
                            validator: field.validators.email,
                            validationMessage: rule.ErrorMessage
                        },
                    listeners: {
                        invalid: function (el, msg)
                        {
                            Ext.DoubleGis.FormValidator.updateValidationMessage(field, msg);
                        },
                        valid: function (el)
                        {
                            Ext.DoubleGis.FormValidator.updateValidationMessage(field, '');
                        }
                    }
                });

        }
        if (rule.ValidationType == "url")
        {
            new Ext.ux.LinkField(
                {
                    applyTo: el,
                    contactTypeCfg:
                        {
                            linkCls: Ext.ux.LinkField.prototype.contactTypeRegistry.url.linkCls,
                            protocolPrefix: Ext.ux.LinkField.prototype.contactTypeRegistry.url.protocolPrefix,
                            protocolRegex: Ext.ux.LinkField.prototype.contactTypeRegistry.url.protocolRegex,
                            validator: field.validators.url,
                            validationMessage: rule.ErrorMessage
                        },
                    listeners: {
                        invalid: function (el, msg)
                        {
                            Ext.DoubleGis.FormValidator.updateValidationMessage(field, msg);
                        },
                        valid: function (el)
                        {
                            Ext.DoubleGis.FormValidator.updateValidationMessage(field);
                        }
                    }
                });
        }
    };
    var onValidatorDetach = function (formOptions, field, ruleName)
    {
        var fieldId = field.FieldName.replace(".", "_");
        var el;
        if (ruleName == "required")
        {
            el = window.Ext.getDom(fieldId + "-req");
            if (el)
            {
                Ext.removeNode(el);
            }
        }
    };
    Ext.DoubleGis.FormValidator = new Ext.DoubleGis.MvcFormValidator(
            {
                listeners:
                {
                    attach: onValidatorAttach,
                    detach: onValidatorDetach
                }
            });
    Ext.DoubleGis.FormValidator.init();

    Ext.getBody().on("keypress", function (e)
    {
        if (e.keyCode == e.ESC)
        {
            window.close();
        }
        if (e.keyCode == e.ENTER)
        {
            if (e.target && e.target.form && e.target.form.length == 1)
            {
                e.preventDefault();
                Ext.getDom("OK").click();
            }
        }
    });

    Ext.each(Ext.CardLookupSettings, function (item, i) {
        new Ext.ux.LookupField(item);
    }, this);
});