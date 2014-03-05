window.InitPage = function () {
    var maxLength;
    var maxSymbolsInWord;
    var maxLineBreaks;

    if (Ext.getDom('TemplateRestrictionType').value == 'Text' && Ext.getDom('TemplateFormattedText').value == 'False') {

        if (Ext.getDom('TemplateAdvertisementLink').value == 'True') {
            var localizedLabel = window.Ext.select('label[for=PlainText]', true, this.form);
            var localizedText = (localizedLabel && localizedLabel.elements && localizedLabel.elements[0]) ? localizedLabel.elements[0].dom.innerText : 'PlainText';

            var field =
            {
                FieldName: "PlainText",
                ReplaceValidationMessageContents: true,
                ValidationMessageId: "PlainText_validationMessage",
                ValidationRules: [
                    {
                        ErrorMessage: String.format(Ext.LocalizedResources.InputValidationInvalidUrl, localizedText),
                        ValidationParameters: {},
                        ValidationType: "url"
                    }],
                validators: {}
            };
            this.validator.attachValidator(this.validator.forms.EntityForm, field, field.ValidationRules[0]);
        }

        if (Ext.getDom("TemplateTextLengthRestriction").value * 1
            || Ext.getDom("TemplateTextLineBreaksRestriction").value * 1
            || Ext.getDom("TemplateMaxSymbolsInWord").value * 1) {
            maxLength = Ext.getDom("TemplateTextLengthRestriction").value * 1;
            maxSymbolsInWord = Ext.getDom("TemplateMaxSymbolsInWord").value * 1;
            maxLineBreaks = Ext.getDom("TemplateTextLineBreaksRestriction").value * 1;
            var performTextLength = function (evt, el) {
                var fInfo = { FieldName: "PlainText", ValidationMessageId: "PlainText_validationMessage" };
                Ext.DoubleGis.FormValidator.updateValidationMessage(fInfo, '');
                var resultMsg = '';

                var diff;
                if (maxLength) {
                    diff = maxLength - el.value.replace(/\n\r?/g, '').replace(/\r?/g, '').length;
                    if (diff < 0) {
                        resultMsg += String.format(Ext.LocalizedResources.TextEditorOverflow, maxLength, -1 * diff);
                    } else {
                        resultMsg += String.format(Ext.LocalizedResources.SymbolsLeft, diff);
                    }
                }
                resultMsg = resultMsg ? resultMsg + '<br/>' : '';

                if (maxSymbolsInWord && maxSymbolsInWord > 0) {

                    var words = el.value.split(new RegExp('-|/|<|>| |\\\\|\n|&nbsp;'));
                    if (words) {
                        for (var i = 0; i < words.length; i++) {
                            if (words[i].length > maxSymbolsInWord) {
                                resultMsg += String.format(Ext.LocalizedResources.AdsCheckTooLongWord, words[i], words[i].length, maxSymbolsInWord);
                                resultMsg += '<br/>';
                            }
                        }
                    }
                }

                resultMsg = resultMsg ? resultMsg + '<br/>' : '';
                if (maxLineBreaks && maxLineBreaks > 0) {
                    var matches = el.value.match(/\n\r?/g);
                    diff = maxLineBreaks - (matches ? matches.length : 0) - (el.value ? 1 : 0);
                    if (diff < 0) {
                        resultMsg += String.format(Ext.LocalizedResources.TextEditorLineBreaksOverflow, maxLineBreaks, -1 * diff);
                    } else {
                        resultMsg += String.format(Ext.LocalizedResources.LineBreaksLeft, diff);
                    }
                }

                Ext.DoubleGis.FormValidator.updateValidationMessage(fInfo, resultMsg);
            };
            Ext.get('PlainText').on('keyup', performTextLength);
            this.on('afterpost', function () {

                performTextLength(null, Ext.getDom('PlainText'));
            });
            this.on('afterbuild', function () {
                performTextLength(null, Ext.getDom('PlainText'));
            });

        }
    }

    // Согласно http://msdn.microsoft.com/en-US/library/18zw7440.aspx специальными символами являются символы с 0x00 по 0x1F, 0x7F, и с from 0x80 по 0x9F
    // Но символы 9, 10 мы оставляем. Так же игнорируем символ с кодом 13 - отучить вводить его пользователей IE нереально :) - его нужно просто молча удалять на серверной стороне.
    // Помним, что работаем с unicode и диапазон от 127 до 255 не содержит кириллицы. Зато содержит много управляющих символов.
    var controlChars = /[\x00-\x08\x0B\x0C\x0E-\x1F\x7F\x80-\x9F]/g;

    var removeControlChars = function (text) {
        return text.replace(controlChars, '');
    };

    var textContainsControlChars = function (text) {
        return text.match(controlChars);
    };
    
    if (Ext.getDom('TemplateRestrictionType').value == 'Text' && Ext.getDom('TemplateFormattedText').value == 'True') {
        Ext.ux.TinyMCE.initTinyMCE();
        maxLength = Ext.getDom("TemplateTextLengthRestriction").value * 1;
        maxSymbolsInWord = Ext.getDom("TemplateMaxSymbolsInWord").value * 1;
        maxLineBreaks = Ext.getDom("TemplateTextLineBreaksRestriction").value * 1;
        var performLength = function () {
            var fInfo = { FieldName: "FormattedText", ValidationMessageId: "FormattedText_validationMessage" };
            var resultMsg = '';
            var diff;

            var body = this.getEd().getBody();
            var plainText = (body.textContent || body.innerText);

            // число символов
            if (maxLength && maxLength > 0) {
                var characterCount = plainText.replace(new RegExp('\r\n', 'gim'), '');

                diff = maxLength - characterCount.length;
                if (diff < 0) {
                    resultMsg += String.format(Ext.LocalizedResources.TextEditorOverflow, maxLength, -1 * diff);
                } else {
                    resultMsg += String.format(Ext.LocalizedResources.SymbolsLeft, diff);
                }
            }
            resultMsg = resultMsg ? resultMsg + '<br/>' : '';

            // число символов в слове
            if (maxSymbolsInWord && maxSymbolsInWord > 0) {

                var words = plainText.split(/\W+/);
                if (words) {
                    for (var i = 0; i < words.length; i++) {
                        if (words[i].length > maxSymbolsInWord) {
                            resultMsg += String.format(Ext.LocalizedResources.AdsCheckTooLongWord, words[i], words[i].length, maxSymbolsInWord);
                            resultMsg += '<br/>';
                        }
                    }
                }
            }

            // количество строк
            if (maxLineBreaks && maxLineBreaks > 0) {
                // BUG: надо использовать не getRawValue, а plainText
                // но в plainText сейчас не возвращается число переносов строк которые последние, исследовать дополнительно
                var rawValue = this.getRawValue();

                var matches = rawValue.match(new RegExp('<BR/>|<BR />|<BR>|<P>|<LI>', 'gim'));
                diff = maxLineBreaks - (matches ? matches.length + 1 : 0);
                if (diff == maxLineBreaks) {
                    diff = maxLineBreaks - (rawValue.length ? 1 : 0);
                }
                if (diff < 0) {
                    resultMsg += String.format(Ext.LocalizedResources.TextEditorLineBreaksOverflow, maxLineBreaks, -1 * diff);
                } else {
                    resultMsg += String.format(Ext.LocalizedResources.LineBreaksLeft, diff);
                }
            }

            Ext.DoubleGis.FormValidator.updateValidationMessage(fInfo, resultMsg);
        };

        this.RTE = new Ext.ux.TinyMCE({
            renderTo: "TxtContainer",
            xtype: "tinymce",
            id: "FormattedText",
            name: "FormattedText",
            width: 600,
            height: 400,
            listeners:
            {
                editorcreated: performLength,
                keyup: performLength
            },
            tinymceSettings: {
                theme: "advanced",
                skin: "o2k7",
                language: Ext.CultureInfo.TwoLetterISOLanguageName,
                plugins: "pagebreak,style,layer,preview,searchreplace,paste,directionality,noneditable,visualchars,nonbreaking,xhtmlxtras,template",
                theme_advanced_buttons1: "bold,italic,|" + /*",justifyleft,justifycenter,justifyright,|"+*/",bullist" + /*,numlist,|,cut,copy,paste,pastetext,pasteword,|"+*/",undo,redo,|,cleanup,code,|,removeformat,visualaid",
                theme_advanced_buttons2: "",
                theme_advanced_buttons3: "",
                theme_advanced_buttons4: "",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,
                valid_elements: "br,strong/b,em/i,ul,ol,li",

                // todo: запилить нормальный paste из word
                //paste_auto_cleanup_on_paste: true,
                //paste_text_linebreaktype: 'br',
                //paste_strip_class_attributes: 'all',
                //paste_retain_style_properties: 'none',
                //paste_convert_middot_lists: true,
                //paste_postprocess : function(pl, o) {
                //    o.node.innerHTML = o.node.innerHTML.replace(/&nbsp;/ig, '');
                //},

                // очищаем формат при вставке, иначе tinymce намертво повисает
                paste_remove_styles: true,
                paste_remove_spans: true,
                paste_preprocess: function (pl, o) {
                    o.content = Ext.util.Format.stripTags(o.content);
                },

                force_br_newlines: true,
                force_p_newlines: false,
                forced_root_block: false,
                convert_newlines_to_brs: true
            },
            value: Ext.getDom("ctnt").innerText
        });
        
        this.on('beforepost', function () {
                // set plaintext
                var body = this.RTE.getEd().getBody();
                var plainText = (body.innerText || body.textContent);

                if (plainText) {
                    var plainTextWithTags = plainText.match(/(<([^>]+)>)/ig);
                    if (plainTextWithTags) {
                        alert(Ext.LocalizedResources.AdvertisementElementTextContainsHtmlTags);
                        return false;
                    }
                }

                var formattedText = Ext.getCmp('FormattedText').getValue()
                    .replace(new RegExp('<p>', 'gim'), '')
                    .replace(new RegExp('&nbsp;</p>', 'gim'), '<br />')
                    .replace(new RegExp('</p>', 'gim'), '<br />');

                // Если в тексте РМ есть управляющие символы, пользователю предлагается их удалить автоматически.
                // Как правило, это неотображаемые символы и они могут быть безболезненно удалены, 
                // однако ползьзователь может отказаться и отредактировать текст РМ вручную.
                if (textContainsControlChars(plainText) || textContainsControlChars(formattedText)) {
                    var userAgreedToRemoveSymbols = confirm(Ext.LocalizedResources.AdvertisementElementTextContainsControlCharacters);
                    if (userAgreedToRemoveSymbols) {
                        formattedText = removeControlChars(formattedText);
                        plainText = removeControlChars(plainText);
                    } else {
                        alert(Ext.LocalizedResources.AdvertisementElementWasNotSaved);
                        return false;
                    }
                }

                Ext.getDom("PlainText").value = plainText;
                Ext.getDom("FormattedText").value = encodeURI(formattedText);

                return true;
            });

        var fixupStatusDependency = function (removeNullValue) {
            var status = Ext.getDom('Status');
            if (status) {
                // Костыль для работы fireEvent в Chrome
                if (document.createEvent) {
                    evObj = document.createEvent('Events');
                    evObj.initEvent('change', true, true);
                    status.dispatchEvent(evObj);
                } else if (document.createEventObject) {
                    evObj = document.createEventObject();
                    status.fireEvent('onchange', evObj);
                }
                window.Card.isDirty = false;
            }
        };

        this.on('formbind',
            function (cmp, frm) {
                fixupStatusDependency(false);
                Ext.getCmp('FormattedText').setValue(frm.FormattedText);
            });

        this.on('afterbuild', function () { fixupStatusDependency(true); });
        this.on('afterpost', performLength, this.RTE);
    }
    else if (Ext.getDom('TemplateRestrictionType').value == 'Text' || Ext.getDom('TemplateRestrictionType').value == 'FasComment') {
        this.on('beforepost', function () {
            var plainText = Ext.getDom("PlainText").value;
            if (textContainsControlChars(plainText)) {
                var userAgreedToRemoveSymbols = confirm(Ext.LocalizedResources.AdvertisementElementTextContainsControlCharacters);
                if (userAgreedToRemoveSymbols) {
                    Ext.getDom("PlainText").value = removeControlChars(plainText);
                    return true;
                } else {
                    alert(Ext.LocalizedResources.AdvertisementElementWasNotSaved);
                    return false;
                }
            }
        });
    }

    if (Ext.getDom('TemplateRestrictionType').value == 'Image' || Ext.getDom('TemplateRestrictionType').value == 'Article') {
        this.on("afterbuild", function () {
            var u = new Ext.ux.AsyncFileUpload(
                {
                    applyTo: 'FileId',
                    uploadUrl: '/Upload/AdvertisementElement',
                    uploadUrlSuffix: '?entityId=' + this.form.Id.value,
                    downloadUrl: Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/AdvertisementElement/{0}',
                    listeners: {
                        fileuploadbegin: function () {
                            this.Items.Toolbar.disable();
                        },
                        fileuploadsuccess: function () {
                            if (Ext.getDom('TemplateRestrictionType').value == "Image" && (Ext.getDom('FileId').value * 1)) {
                                Ext.getDom('UploadedImage').src = String.format(Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/AdvertisementElement/{0}?_dc={1}', Ext.getDom('FileId').value, Ext.util.Format.cacheBuster());
                            }

                            this.refresh(true);
                        },
                        fileuploadcomplete: function () {
                            this.Items.Toolbar.enable();
                        },
                        scope: this
                    },
                    fileInfo:
                    {
                        fileId: Ext.getDom('FileId').value,
                        fileName: Ext.getDom('FileName').value,
                        contentType: Ext.getDom('FileContentType').value,
                        fileSize: Ext.getDom('FileContentLength').value
                    }
                });
            if (Ext.getDom('TemplateRestrictionType').value == "Image" && (Ext.getDom('FileId').value * 1)) {
                Ext.getDom('UploadedImage').src = String.format(Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/AdvertisementElement/{0}?_dc={1}', Ext.getDom('FileId').value, Ext.util.Format.cacheBuster());
            }
        }, this);
    }

    this.on("afterbuild", function () {

        if (window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {

            this.Items.TabPanel.add(
                {
                    xtype: "actionshistorytab",
                    pCardInfo:
                    {
                        pTypeId: this.Settings.EntityId,
                        pId: window.Ext.getDom("ViewConfig_Id").value,
                        pTypeName: Ext.get("ViewConfig_EntityName").dom.value
                    }
                });
        }
    });
};