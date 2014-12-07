using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements
{
    // TODO {i.maslennikov, 04.03.2014}: Этот валидатор очень логично будет передать в компонент BL
    public sealed class ModifyingAdvertisementElementValidator : IModifyingAdvertisementElementValidator
    {
        public void Validate(AdvertisementElementTemplate elementTemplate, IEnumerable<AdvertisementElement> elements, string elementPlainText, string elementFormattedText)
        {
            if (elementTemplate.IsAdvertisementLink)
            {
                Uri uri;
                if (!string.IsNullOrEmpty(elementPlainText)
                    && (!Uri.TryCreate(elementPlainText, UriKind.Absolute, out uri)
                        || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                        || uri.HostNameType != UriHostNameType.Dns))
                {
                    throw new BusinessLogicException(string.Format(CultureInfo.CurrentCulture,
                                                                   BLResources.InputValidationInvalidUrl,
                                                                   MetadataResources.Text));
                }
            }

            foreach (var element in elements)
            {
                ValidateAdvertisementElement(elementTemplate, element, elementPlainText, elementFormattedText);
            }
        }

        private static void ValidateAdvertisementElement(AdvertisementElementTemplate elementTemplate, AdvertisementElement element, string plainText, string formattedText)
        {
            switch (elementTemplate.RestrictionType)
            {
                case AdvertisementElementRestrictionType.Text:
                case AdvertisementElementRestrictionType.FasComment:
                { 
                    var errors = new List<string>();
                    if (string.IsNullOrEmpty(plainText))
                    {
                        if (elementTemplate.IsRequired)
                        {
                            throw new BusinessLogicException(BLResources.AdsCheckElemIsRequired);
                        }

                        break;
                    }

                    if (elementTemplate.TextLengthRestriction != null)
                    {
                        string textLengthError;
                        if (!ValidateTextLength(plainText, elementTemplate.TextLengthRestriction.Value, out textLengthError))
                        {
                            errors.Add(textLengthError);
                        }
                    }

                    if (elementTemplate.TextLineBreaksCountRestriction != null)
                    {
                        string textLineBreaksCountError;
                        if (!ValiadteTextLineBreaksCount(plainText, elementTemplate.TextLineBreaksCountRestriction.Value, out textLineBreaksCountError))
                        {
                            errors.Add(textLineBreaksCountError);
                        }
                    }

                    if (elementTemplate.MaxSymbolsInWord != null)
                    {
                        string maxSymbolsInWordError;
                        if (!ValidateWordLength(plainText, elementTemplate.MaxSymbolsInWord.Value, out maxSymbolsInWordError))
                        {
                            errors.Add(maxSymbolsInWordError);
                        }
                    }

                    var restrictedStringsInText = string.Empty;

                    // Проверка на отсутствие неразрывного пробела
                    // Неразрывный пробел приводит к некорректному форматированию в конечном продукте (вплоть до того, что весь РМ отображается в одну строку)
                    const char NonBreakingSpaceChar = (char)160;
                    const string NonBreakingSpaceString = "&nbsp;";

                    if (plainText.Contains(NonBreakingSpaceChar.ToString(CultureInfo.InvariantCulture))
                        || (!string.IsNullOrWhiteSpace(formattedText)
                            && formattedText.Contains(NonBreakingSpaceString)))
                    {
                        restrictedStringsInText = string.Format("({0})", BLResources.NonBreakingSpace);
                    }

                    // Проверка на отсутствие запрещенных сочетаний типа "\r", "\n"
                    var restrictedStrings = new[] { @"\r", @"\n", @"\p", @"\i" };

                    foreach (var restrictedString in restrictedStrings.Where(restrictedString => plainText.ToLower().Contains(restrictedString)))
                    {
                        if (restrictedStringsInText != string.Empty)
                        {
                            restrictedStringsInText += ", " + string.Format("'{0}'", restrictedString);
                        }
                        else
                        {
                            restrictedStringsInText += string.Format("'{0}'", restrictedString);
                        }
                    }

                    if (restrictedStringsInText != string.Empty)
                    {
                        errors.Add(string.Format(BLResources.RestrictedSymbolInAdvertisementElement, restrictedStringsInText));
                    }

                    // Защищаемся от управляющих символов, за исключеним 10 (перенос строки) и 9 (табуляция), которые допустимы в рекламном материале.
                    if (plainText.Any(c => char.IsControl(c) && c != 10 && c != 9) || (formattedText ?? string.Empty).Any(c => char.IsControl(c) && c != 10 && c != 9))
                    {
                        errors.Add("Управляющие неотображаемые символы в тексте ЭРМ");
                    }

                    if (errors.Any())
                    {
                        throw new BusinessLogicException(string.Join("; ", errors));
                    }
                    
                    break;
                }
                case AdvertisementElementRestrictionType.Date:
                {
                    if (element.BeginDate == null || element.EndDate == null)
                    {
                        if (elementTemplate.IsRequired)
                        {
                            throw new BusinessLogicException(BLResources.AdsCheckElemIsRequired);
                        }

                        break;
                    }

                    if (element.BeginDate.Value > element.EndDate.Value)
                    {
                        throw new BusinessLogicException(BLResources.AdsCheckInvalidDateRange);
                    }

                    if (element.EndDate.Value - element.BeginDate.Value < TimeSpan.FromDays(4))
                    {
                        throw new BusinessLogicException(BLResources.AdvertisementPeriodError);
                    }
                    
                    break;
                }
                case AdvertisementElementRestrictionType.Article:
                case AdvertisementElementRestrictionType.Image:
                {
                    if (element.FileId == null && elementTemplate.IsRequired)
                    {
                        throw new BusinessLogicException(BLResources.AdsCheckElemIsRequired);
                    }
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool ValidateTextLength(string text, int maxTextLength, out string error)
        {
            text = text.Replace("\n", string.Empty);

            if (text.Length > maxTextLength)
            {
                error = string.Format(CultureInfo.CurrentCulture, BLResources.AdsCheckTextIsTooLong, maxTextLength);
                return false;
            }

            error = null;
            return true;
        }

        private static bool ValiadteTextLineBreaksCount(string text, int maxLineBreaks, out string error)
        {
            var matchesCount = Regex.Matches(text, "\n", RegexOptions.IgnoreCase).Count + 1;
            if (matchesCount > maxLineBreaks)
            {
                error = string.Format(CultureInfo.CurrentCulture, BLResources.AdsCheckTooMuchLineBreaks, maxLineBreaks);
                return false;
            }

            error = null;
            return true;
        }

        private static bool ValidateWordLength(string text, int maxSymbolsInWord, out string error)
        {
            // todo: заменить на regexp W - это же и значит типа word
            var words = text.Split(new[] { " ", "-", "\\", "/", "<", ">", "\n", "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);

            var longWords = words.Where(x => x.Length > maxSymbolsInWord).ToArray();
            if (longWords.Any())
            {
                error = string.Join(", ", longWords.Select(x => string.Format(BLResources.AdsCheckTooLongWord, x, x.Length, maxSymbolsInWord)));
                return false;
            }

            error = null;
            return true;
        }
    
    }
}