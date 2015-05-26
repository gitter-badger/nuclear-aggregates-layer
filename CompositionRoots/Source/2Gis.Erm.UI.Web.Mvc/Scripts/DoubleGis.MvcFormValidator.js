Ext.ns('Ext.DoubleGis');
Ext.DoubleGis.MvcFormValidator = Ext.extend(Ext.util.Observable, {
    constructor: function (config)
    {
        this.addEvents('beforeregister', 'afterregister', 'attach', 'detach');
        Ext.apply(this, config);
        window.Ext.DoubleGis.MvcFormValidator.superclass.constructor.call(this, config);
    },

    forms: {},
    init: function ()
    {
        var allFormOptions = window.mvcClientValidationMetadata;
        if (allFormOptions)
        {
            while (allFormOptions.length > 0)
            {
                var formOptions = allFormOptions.pop();
                this.register(formOptions);
            }
        }
    },
    register: function (formOptions)
    {
        if (this.fireEvent('beforeregister', formOptions) === false)
        {
            return;
        }
        this.forms[formOptions.FormId] = formOptions;
        formOptions.form = window.Ext.get(formOptions.FormId);
        this.initValidators(formOptions);
        this.fireEvent('afterregister', formOptions);
    },
    validate: function (form)
    {
        var formOptions = this.forms[form.id];
        if (window.Ext.isEmpty(formOptions) || this.fireEvent('beforevalidate', formOptions) === false)
        {
            return true;
        }

        delete formOptions.errors;
        formOptions.errors = [];
        for (var i = 0; i < formOptions.Fields.length; i++)
        {
            if (formOptions.Fields[i].ValidationRules.length)
            {
                this.validateField(formOptions.Fields[i], formOptions, form);
            }
        }

        if (!window.Ext.isEmpty(formOptions.ValidationSummaryId))
        {
            this.updateValidationSummary(formOptions);
        }
        this.fireEvent('aftervalidate', formOptions);
        return formOptions.errors.length == 0;
    },
    validateField: function (field, formOptions, formContext)
    {
        this.updateValidationMessage(field, '');
        var fieldId = field.FieldName.replace(".", "_");
        window.Ext.each(field.ValidationRules, function (rule)
        {
            var validator = field.validators[rule.ValidationType];
            if (!window.Ext.isEmpty(validator))
            {
                var result;
                try
                {
                    var el = window.Ext.get(fieldId);
                    result = validator(el.getValue(), {
                        eventName: formContext.type,
                        fieldContext: {
                            elements: [el.dom],
                            formContext: formContext
                        },
                        validation: {
                            fieldErrorMessage: rule.ErrorMessage
                        }
                    });
                } catch (ex)
                {
                    result = true;
                }
                if (result !== true)
                {
                    var msg = typeof result == 'string' ? result : rule.ErrorMessage;
                    formOptions.errors.push({
                        message: msg
                    });
                    this.updateValidationMessage(field, msg);
                }
            }
        }, this);
    },
    initValidators: function (formOptions)
    {
        window.Ext.each(formOptions.Fields, function (field)
        {
            field.validators = {};
            window.Ext.each(field.ValidationRules, function (rule)
            {
                this.attachValidator(formOptions, field, rule);
            }, this);
        }, this);
    },
    attachValidator: function (formOptions, field, rule)
    {
        if (field.validators[rule.ValidationType])
        {
            return;
        }

        if (window.Ext.DoubleGis.ValidatorRegistry.validators[rule.ValidationType])
        {
            field.validators[rule.ValidationType] = window.Ext.DoubleGis.ValidatorRegistry.validators[rule.ValidationType](rule);
        }
        this.fireEvent('attach', formOptions, field, rule);
    },
    detachValidator: function (formOptions, field, ruleName)
    {
        if (!field.validators[ruleName])
        {
            return;
        }
        delete field.validators[ruleName];

        this.fireEvent('detach', formOptions, field, ruleName);
    },
    updateValidationMessage: function (field, msg)
    {
        var fieldId = field.FieldName.replace(".", "_");
        window.Ext.fly(field.ValidationMessageId).update(msg);
        var b = window.Ext.isEmpty(msg);
        window.Ext.fly(field.ValidationMessageId)[b ? 'removeClass' : 'addClass']("field-validation-error");
        window.Ext.fly(field.ValidationMessageId)[b ? 'addClass' : 'removeClass']("field-validation-valid");
        window.Ext.fly(fieldId)[b ? 'removeClass' : 'addClass']("input-validation-error");
    },
    updateValidationSummary: function (formOptions)
    {
        var el = window.Ext.get(formOptions.ValidationSummaryId);
        var b = formOptions.errors.length == 0;
        el[b ? 'removeClass' : 'addClass']("validation-summary-errors");
        el[b ? 'addClass' : 'removeClass']("validation-summary-valid");
        if (formOptions.ReplaceValidationSummary === true)
        {
            el.last().update(b ? '' : this.createListItemsString(formOptions));
        }
    },
    clearMessages: function() 
    {
        Ext.each(Ext.query('span.field-validation-error'), function (el, index)
        {
            Ext.get(el).update('').addClass('field-validation-valid').removeClass('field-validation-error');
        });  
    },
    createListItemsString: function (formOptions)
    {
        var html = '';
        window.Ext.each(formOptions.errors, function (error, i)
        {
            html += '<li>' + formOptions.errors[i].message + '</li>';
        });
        return html;
    }
});

Ext.DoubleGis.ValidatorRegistry = {
    validators: {
        range: function (rule)
        {
            var min = rule.ValidationParameters.min;
            var max = rule.ValidationParameters.max;
            return function (value, context)
            {
                if (window.Ext.isEmpty(value))
                {
                    return true;
                }
                return value >= min && value <= max;
            };
        },
        required: function (rule)
        {
            return function (value, context)
            {
                return !window.Ext.isEmpty((value + "").trim());
            };
        },
        stringlength: function (rule)
        {
            var min = rule.ValidationParameters.minimumLength;
            var max = rule.ValidationParameters.maximumLength;
            return function (value, context)
            {
                if (window.Ext.isEmpty(value))
                {
                    return true;
                }
                return (min ? value.length >= min : true) && (max ? value.length <= max : true);
            };
        },
        number: function (rule)
        {
            return function (value, context)
            {
                var uniValue = value==''?'':Number.parseFromLocal(value);
                return uniValue == '' || window.Ext.isNumber(parseFloat(+uniValue));
            };
        },
        email: function (rule)
        {
            return function (value, context)
            {
                return value == '' || /^[а-яёa-z0-9_+.-]+\@([а-яёa-z0-9-]+\.)+[а-яёa-z0-9]{2,4}$/i.test(value);
            };
        },
        url: function (rule)
        {
            return function (value, context)
            {
                return value == '' || /^https?:\/\/([а-яёa-z0-9-_]+\.)+[а-яёa-z0-9]{2,4}.*$/.test(value);

            };
        },
        phone: function (rule)
        {
            return function (value, context)
            {
                return true;

            };
        },
        regex: function (rule)
        {
            var r = new RegExp(rule.ValidationParameters.pattern);
            return function (value, context)
            {
                return value == '' || r.test(value);
            };
        },
        greaterorequalthan: function (rule)
        {
            return function (value, context)
            {
                var anotherValue = window.Ext.get(rule.ValidationParameters.anotherProperty).dom.value;
                return value && anotherValue && Date(value) >= Date(anotherValue);
            };
        },
        customvalidation: function (rule)
        {
            return window.Ext.DoubleGis.CustomValidatorRegistry[rule.ValidationParameters.validationfunction];
        },
        date: function (rule) {
            return function (value, context) {
                var message = context.fieldContext.elements[0].validationMessage;
                return message ? message : true;
            };
        }
    }
};
Ext.DoubleGis.CustomValidatorRegistry = [];
