(function () {
    // Согласно http://msdn.microsoft.com/en-US/library/18zw7440.aspx специальными символами являются символы с 0x00 по 0x1F, 0x7F, и с from 0x80 по 0x9F
    // Но символы 9, 10 мы оставляем. Так же игнорируем символ с кодом 13 - отучить вводить его пользователей IE нереально :) - его нужно просто молча удалять на серверной стороне.
    // Помним, что работаем с unicode и диапазон от 127 до 255 не содержит кириллицы. Зато содержит много управляющих символов.
    var controlChars = /[\x00-\x08\x0B\x0C\x0E-\x1F\x7F\x80-\x9F]/g;
    var plainTextNewLineRegexp = new RegExp('\r?\n', 'gim');
    var formattedTextNewLineRegexp = new RegExp('<BR/>|<BR />|<BR>|<P>|<LI>', 'gim');
    var plainTextWordRegexp = new RegExp('-|/|<|>| |\\\\|\n|&nbsp;', 'gim');

    var fileAccessUrlTemplate = Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/AdvertisementElement/{0}';

    var cardButtonHandlers = {
        ResetToDraft: function() {
            this.Items.Toolbar.disable(true);
            var urlTemplate = Ext.SpecialOperationsServiceRestUrl + 'HandleAdsState.svc/rest/transfer/to/draft/{0}';
            var url = String.format(urlTemplate, Ext.getDom("Id").value);

            this.Mask.show();
            window.Ext.Ajax.request({
                url: url,
                timeout: 240000,
                method: 'POST',
                success: function() { this.refresh(); },
                failure: function (xhr) {
                    this.Mask.hide();
                    var response = Ext.decode(xhr.responseText);
                    this.AddNotification(response.Message || xhr.responseText || String.format(Ext.LocalizedResources.ErrorOnServiceAccess, Ext.SpecialOperationsServiceRestUrl), "CriticalError", "ServerError");
                    this.recalcToolbarButtonsAvailability();
                },
                scope: this
            });
        },
        SaveAndVerify: function() {
            this.Save();
        }
    };

    function removeControlChars(text) {
        return text.replace(controlChars, '');
    };

    function removeSpaces(text) {
        return text
            .replace(/&nbsp;/g, '\x20')
            .replace(/(\x20){2,}/g, '\x20');
    };

    function textContainsControlChars(text) {
        return text.match(controlChars);
    };

    function textContainsControlSpace(text) {
        return text.match(/(&nbsp;)|(\x20){2,}/g);
    }

    function characterCountValidationMessage(plainText, maxLength) {
        var diff = maxLength - plainText.replace(plainTextNewLineRegexp, '').length;
        if (diff < 0) {
            return String.format(Ext.LocalizedResources.TextEditorOverflow, maxLength, -1 * diff);
        } else {
            return String.format(Ext.LocalizedResources.SymbolsLeft, diff);
        }
    }

    function maxCharacterInWordValidationMessage(plainText, maxSymbolsInWord) {
        var result = [];
        var words = plainText.split(plainTextWordRegexp);
        Ext.each(words, function (word) {
            if (word.length > maxSymbolsInWord) {
                result.push(String.format(Ext.LocalizedResources.AdsCheckTooLongWord, word, word.length, maxSymbolsInWord));
            }
        });

        return result;
    }

    function plainTextLineBreaksValidationMessage(plainText, maxLineBreaks) {
        var matches = plainText.match(plainTextNewLineRegexp);
        var diff = maxLineBreaks - (matches ? matches.length : 0) - (plainText ? 1 : 0);
        if (diff < 0) {
            return String.format(Ext.LocalizedResources.TextEditorLineBreaksOverflow, maxLineBreaks, -1 * diff);
        } else {
            return String.format(Ext.LocalizedResources.LineBreaksLeft, diff);
        }
    }

    function formattedTextLineBreaksValidationMessage(formattedText, maxLineBreaks) {
        var matches = formattedText.match(formattedTextNewLineRegexp);
        var diff = maxLineBreaks - (matches ? matches.length : 0) - (formattedText ? 1 : 0);
        if (diff < 0) {
            return String.format(Ext.LocalizedResources.TextEditorLineBreaksOverflow, maxLineBreaks, -1 * diff);
        } else {
            return String.format(Ext.LocalizedResources.LineBreaksLeft, diff);
        }
    }

    function getRestrictionValue(fieldName) {
        var field = Ext.getDom(fieldName);
        return field ? field.value * 1 : 0;
    }

    function initPlainTextLegthValidation(fieldWithPrefix) {
        var maxLength = getRestrictionValue(fieldWithPrefix("TemplateTextLengthRestriction"));
        var maxSymbolsInWord = getRestrictionValue(fieldWithPrefix("TemplateMaxSymbolsInWord"));
        var maxLineBreaks = getRestrictionValue(fieldWithPrefix("TemplateTextLineBreaksRestriction"));

        if (maxLength || maxSymbolsInWord || maxLineBreaks) {
            var performTextLength = function (evt, el) {
                var messages = [];

                if (maxLength && maxLength > 0) {
                    messages.push(characterCountValidationMessage(el.value, maxLength));
                }

                if (maxSymbolsInWord && maxSymbolsInWord > 0) {
                    // следует чистать как messages.addRange(...)
                    Array.prototype.push.apply(messages, maxCharacterInWordValidationMessage(el.value, maxSymbolsInWord));
                }

                if (maxLineBreaks && maxLineBreaks > 0) {
                    messages.push(plainTextLineBreaksValidationMessage(el.value, maxLineBreaks));
                }

                var fInfo = { FieldName: fieldWithPrefix("PlainText"), ValidationMessageId: fieldWithPrefix("PlainText_validationMessage") };
                Ext.DoubleGis.FormValidator.updateValidationMessage(fInfo, messages.join('<br/>'));
            };

            Ext.get(fieldWithPrefix('PlainText')).on('keyup', performTextLength);
            this.on('afterpost', function () {
                performTextLength(null, Ext.getDom(fieldWithPrefix('PlainText')));
            });
            this.on('afterbuild', function () {
                performTextLength(null, Ext.getDom(fieldWithPrefix('PlainText')));
            });
        }
    }

    function initFormattedText(fieldWithPrefix) {
        var maxLength = getRestrictionValue(fieldWithPrefix("TemplateTextLengthRestriction"));
        var maxSymbolsInWord = getRestrictionValue(fieldWithPrefix("TemplateMaxSymbolsInWord"));
        var maxLineBreaks = getRestrictionValue(fieldWithPrefix("TemplateTextLineBreaksRestriction"));

        var performLength = function () {
            var body = this.getEd().getBody();
            var plainText = body.textContent || body.innerText || "";
            var messages = [];

            if (maxLength && maxLength > 0) {
                messages.push(characterCountValidationMessage(plainText, maxLength));
            }

            if (maxSymbolsInWord && maxSymbolsInWord > 0) {
                // следует чистать как messages.addRange(...)
                Array.prototype.push.apply(messages, maxCharacterInWordValidationMessage(plainText, maxSymbolsInWord));
            }

            if (maxLineBreaks && maxLineBreaks > 0) {
                // BUG: надо использовать не getRawValue, а plainText
                // но в plainText сейчас не возвращается число переносов строк которые последние, исследовать дополнительно
                messages.push(formattedTextLineBreaksValidationMessage(this.getRawValue(), maxLineBreaks));
            }

            var fInfo = { FieldName: "FormattedTextEditor", ValidationMessageId: fieldWithPrefix("FormattedText_validationMessage") };
            Ext.DoubleGis.FormValidator.updateValidationMessage(fInfo, messages.join('<br/>'));
        };

        Ext.ux.TinyMCE.initTinyMCE();
        this.RTE = new Ext.ux.TinyMCE({
            renderTo: "TxtContainer",
            xtype: "tinymce",
            id: "FormattedTextEditor",
            name: "FormattedTextEditor",
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
                theme_advanced_buttons1: "bold,italic,|,bullist,undo,redo,|,cleanup,code,|,removeformat,visualaid",
                theme_advanced_buttons2: "",
                theme_advanced_buttons3: "",
                theme_advanced_buttons4: "",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,
                valid_elements: "br,strong/b,em/i,ul,ol,li",

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
            value: Ext.getDom(fieldWithPrefix("FormattedText")).value
        });

        var readOnlyField = Ext.getDom("ViewConfig_ReadOnly");
        if (readOnlyField && readOnlyField.checked) {
            this.RTE.disable();
        }

        this.on('beforepost', function () {
            // set plaintext
            var body = this.RTE.getEd().getBody();
            var plainText = body.innerText || body.textContent || "";

            if (plainText) {
                var plainTextWithTags = plainText.match(/(<([^>]+)>)/ig);
                if (plainTextWithTags) {
                    alert(Ext.LocalizedResources.AdvertisementElementTextContainsHtmlTags);
                    return false;
                }
            }

            var formattedText = Ext.getCmp('FormattedTextEditor').getValue()
                .replace(new RegExp('<p>', 'gim'), '')
                .replace(new RegExp('&nbsp;</p>', 'gim'), '<br />')
                .replace(new RegExp('</p>', 'gim'), '<br />');

            if (textContainsControlSpace(plainText) || textContainsControlSpace(formattedText)) {
                // Ext.Msg.Confirm не используется, т.к. он выполняется асинхронно
                var userAgreedToRemoveSpaces = confirm(Ext.LocalizedResources.AdvertisementElementTextContainsControlSpaces);
                if (userAgreedToRemoveSpaces) {
                    formattedText = removeSpaces(formattedText);
                    plainText = removeSpaces(plainText);
                } else {
                    Ext.Msg.alert("",Ext.LocalizedResources.AdvertisementElementWasNotSaved);
                    return false;
                }
            }

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

            Ext.getDom(fieldWithPrefix("PlainText")).value = plainText;
            Ext.getDom(fieldWithPrefix("FormattedText")).value = encodeURI(formattedText);

            return true;
        });

        var fixupStatusDependency = function () {
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

                if (window.Card) {
                    window.Card.isDirty = false;
                }
            }
        };

        this.on('formbind', function (cmp, frm) {
            fixupStatusDependency();
            Ext.getCmp('FormattedTextEditor').setValue(frm.FormattedText.FormattedText);

            if (cmp.ReadOnly) {
                cmp.RTE.disable();
            } else {
                cmp.RTE.enable();
            }
        });

        this.on('afterbuild', fixupStatusDependency);
        this.on('afterpost', performLength, this.RTE);
    }

    function initPlainTextControlCharsValidation(fieldWithPrefix) {
        this.on('beforepost', function () {
            var plainTextField = Ext.getDom(fieldWithPrefix("PlainText"));
            var plainText = plainTextField.value;
            if (textContainsControlChars(plainText)) {
                var userAgreedToRemoveSymbols = confirm(Ext.LocalizedResources.AdvertisementElementTextContainsControlCharacters);
                if (userAgreedToRemoveSymbols) {
                    plainTextField.value = removeControlChars(plainText);
                    return true;
                } else {
                    alert(Ext.LocalizedResources.AdvertisementElementWasNotSaved);
                    return false;
                }
            }
            return true;
        });
    }

    function initFile(fieldWithPrefix) {
        this.on("afterbuild", function () {
            var readOnlyField = Ext.getDom("ViewConfig_ReadOnly");
            var readOnly = readOnlyField ? readOnlyField.checked : false;
            var entityId = Ext.get('Id').getValue();
            var u = new Ext.ux.AsyncFileUpload({
                id:fieldWithPrefix('FileId')+'-id',
                readOnly: readOnly,
                applyTo: fieldWithPrefix('FileId'),
                uploadUrl: '/Upload/AdvertisementElement',
                uploadUrlSuffix: '?entityId=' + entityId,
                downloadUrl: fileAccessUrlTemplate,
                listeners: {
                    fileuploadbegin: function () {
                        if (this.Items && this.Items.Toolbar) {
                            this.Items.Toolbar.disable();
                        }
                    },
                    fileuploadsuccess: function () {
                        this.refresh(true);
                    },
                    fileuploadcomplete: function () {
                        if (this.Items && this.Items.Toolbar) {
                            this.recalcToolbarButtonsAvailability();
                        }
                    },
                    scope: this
                },
                fileInfo: {
                    fileId: Ext.getDom(fieldWithPrefix('FileId')).value,
                    fileName: Ext.getDom(fieldWithPrefix('FileName')).value,
                    contentType: Ext.getDom(fieldWithPrefix('FileContentType')).value,
                    fileSize: Ext.getDom(fieldWithPrefix('FileContentLength')).value
                }
            });
        }, this);

        this.on('formbind', function(cmp, frm) {
            Ext.getCmp(fieldWithPrefix('FileId') + '-id').setReadOnly(cmp.ReadOnly);
        });
    }

    function initImage(fieldWithPrefix) {
        this.on("afterbuild", function () {
            var fileId = Ext.getDom(fieldWithPrefix('FileId')).value;
            if (fileId) {
                Ext.getDom('UploadedImage').src = Ext.urlAppend(
                    String.format(fileAccessUrlTemplate, fileId),
                    Ext.urlEncode({ _dc: Ext.util.Format.cacheBuster() }));
            }
        }, this);
    }

    function addActionsHistoryTab() {
        if (window.Ext.getDom("ViewConfig_Id").value && window.Ext.getDom("ViewConfig_Id").value != "0") {
            this.Items.TabPanel.add({ xtype: "actionshistorytab", pCardInfo: { pTypeName: 'AdvertisementElementStatus', pId: window.Ext.getDom("ViewConfig_Id").value } });
        }
    }

    function buildDenialReasonsList () {
        if (Ext.getDom('NeedsValidation').value.toLowerCase() == 'true' && this.form.Id.value != 0) {
            var cnt = Ext.getCmp('ContentTab_holder');
            var tp = Ext.getCmp('TabWrapper');

            tp.anchor = "100%, 60%";
            delete tp.anchorSpec;
            cnt.add(new Ext.Panel({
                id: 'denialReasonsFrame_holder',
                anchor: '100%, 40%',
                html: '<iframe id="denialReasonsFrame_frame"></iframe>'
            }));
            cnt.doLayout();
            var mask = new window.Ext.LoadMask(window.Ext.get("denialReasonsFrame_holder"));
            mask.show();
            var iframe = Ext.get('denialReasonsFrame_frame');

            iframe.dom.src = '/Grid/View/AdvertisementElementDenialReason/AdvertisementElement/{0}/Inactive?extendedInfo=filterToParent%3Dtrue'.replace(/\{0\}/g, this.form.Id.value);
            iframe.on('load', function(evt, el) {
                el.height = Ext.get(el.parentElement).getComputedHeight();
                el.width = Ext.get(el.parentElement).getComputedWidth();
                el.style.height = "100%";
                el.style.width = "100%";
                el.contentWindow.Ext.onReady(function() {
                    el.contentWindow.IsBottomDenialReasonDataList = true;
                });
                this.hide();
            }, mask);
            cnt.doLayout();
        }
    }

    function prepareRelatedList(card, details) {
        var dataListWindow = details.dataList.ContentContainer.container.dom.document.parentWindow;
        if (dataListWindow.IsBottomDenialReasonDataList) {
            dataListWindow.Ext.getDom('Toolbar').style.display = 'none';
            details.dataList.Items.Grid.getBottomToolbar().hide();
            details.dataList.ContentContainer.doLayout();
        }
    }

    window.InitPage = function () {
        Ext.apply(this, cardButtonHandlers);
        this.on("afterbuild", addActionsHistoryTab);
        this.on("afterbuild", buildDenialReasonsList);
        this.on("afterrelatedlistready", prepareRelatedList);

        var prefix = Ext.getDom('ActualType').value;
        var fieldWithPrefix = function (fieldName) { return prefix + '_' + fieldName; }
        switch (prefix) {
            case 'PlainText':
                initPlainTextLegthValidation.call(this, fieldWithPrefix);
                initPlainTextControlCharsValidation.call(this, fieldWithPrefix);
                break;
            case 'FormattedText':
                initFormattedText.call(this, fieldWithPrefix);
                break;
            case 'FasComment':
                initPlainTextLegthValidation.call(this, fieldWithPrefix);
                initPlainTextControlCharsValidation.call(this, fieldWithPrefix);
                break;
            case 'Link':
                initPlainTextLegthValidation.call(this, fieldWithPrefix);
                initPlainTextControlCharsValidation.call(this, fieldWithPrefix);
                break;
            case 'Image':
                initFile.call(this, fieldWithPrefix);
                initImage.call(this, fieldWithPrefix);
                break;
            case 'File':
                initFile.call(this, fieldWithPrefix);
                break;
        }
    };

    window.InitAdvertisementElement = function () {
        //this.on("afterbuild", addActionsHistoryTab);

        var prefix = Ext.getDom('ActualType').value;
        var fieldWithPrefix = function (fieldName) { return prefix + '_' + fieldName; }
        switch (prefix) {
            case 'PlainText':
                initPlainTextLegthValidation.call(this, fieldWithPrefix);
                initPlainTextControlCharsValidation.call(this, fieldWithPrefix);
                break;
            case 'FormattedText':
                initFormattedText.call(this, fieldWithPrefix);
                break;
            case 'FasComment':
                initPlainTextLegthValidation.call(this, fieldWithPrefix);
                initPlainTextControlCharsValidation.call(this, fieldWithPrefix);
                break;
            case 'Link':
                initPlainTextLegthValidation.call(this, fieldWithPrefix);
                initPlainTextControlCharsValidation.call(this, fieldWithPrefix);
                break;
            case 'Image':
                initFile.call(this, fieldWithPrefix);
                initImage.call(this, fieldWithPrefix);
                break;
            case 'File':
                initFile.call(this, fieldWithPrefix);
                break;
        }
    };
})();