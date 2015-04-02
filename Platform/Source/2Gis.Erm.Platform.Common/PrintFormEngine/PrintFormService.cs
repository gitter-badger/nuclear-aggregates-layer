using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
    public sealed class PrintFormService : IPrintFormService
    {
        private readonly IFormatterFactory _formatterFactory;

        public PrintFormService(IFormatterFactory formatterFactory)
        {
            _formatterFactory = formatterFactory;
        }

        void IPrintFormService.PrintToDocx(Stream stream, object data, int currencyIsoCode)
        {
            using (var doc = WordprocessingDocument.Open(stream, true))
            {
                if (doc.DocumentType == WordprocessingDocumentType.Template)
                {
                    doc.ChangeDocumentType(WordprocessingDocumentType.Document);
                }

                ProcessOptionalBlocks(doc.MainDocumentPart, data);

                var allSdtBlocks = doc.MainDocumentPart.Document.Descendants<SdtElement>()
                    .Union(doc.MainDocumentPart.HeaderParts.SelectMany(x => x.Header.Descendants<SdtElement>()))
                    .Union(doc.MainDocumentPart.FooterParts.SelectMany(x => x.Footer.Descendants<SdtElement>()))
                    .Where(block => !block.GetTag().StartsWith("Optional"))
                    .ToArray();

                var sdtCells = allSdtBlocks.Where(b => b is SdtRun && !b.Descendants<Table>().Any()).Cast<SdtRun>();
                foreach (var sdtCell in sdtCells)
                {
                    FillSdtCell(sdtCell, data, currencyIsoCode);
                }

                var sdtRows = allSdtBlocks.Where(b => b is SdtRow);
                foreach (var sdtRow in sdtRows)
                {
                    object rowData;
                    if (!TryGetValueByPath(sdtRow.GetName(), data, out rowData))
                    {
                        continue;
                    }

                    FillSdtRow(sdtRow, rowData, currencyIsoCode);
                }

                var sdtTables = allSdtBlocks.Where(b => b is SdtBlock && b.Descendants<Table>().Count() == 1);
                foreach (var sdtTable in sdtTables)
                {
                    object tableData;
                    if (!TryGetValueByPath(sdtTable.GetName(), data, out tableData))
                    {
                        continue;
                    }

                    FillSdtTable(sdtTable, (IEnumerable)tableData, currencyIsoCode);
                }

                MarkReferencesAsDirty(doc);

                doc.Close();
                stream.Position = 0;
            }
        }

        private void ProcessOptionalBlocks(MainDocumentPart document, object dataObject)
        {
            // Старый функционал, основанный на анонимных объектах, не поддерживаем.
            var printData = dataObject as PrintData;
            if (printData == null)
            {
                return;
            }

            var optionalSdtBlocks = document.Document.Descendants<SdtElement>()
                                            .Union(document.HeaderParts.SelectMany(x => x.Header.Descendants<SdtElement>()))
                                            .Union(document.FooterParts.SelectMany(x => x.Footer.Descendants<SdtElement>()))
                                            .Where(block => string.IsNullOrEmpty(block.GetName()) && block.GetTag().StartsWith("Optional"))
                                            .ToArray();

            foreach (var sdtBlock in optionalSdtBlocks)
            {
                var tag = sdtBlock.GetTag();
                var parent = sdtBlock.Parent;

                var dataPath = tag.Split(',').Last();
                var flagValue = printData.GetData(dataPath);
                var blockMustExist = flagValue != null && (bool)flagValue;
                if (!blockMustExist)
                {
                    parent.RemoveChild(sdtBlock);
                }
            }
        }

        Stream IPrintFormService.MergeDocuments(IEnumerable<Stream> documents)
        {
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XNamespace r = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            var stream = CreateEmptyDocument();
            var ordinal = 0;

            var breakerStream = CreateEmptyDocument();
            using (var doc = WordprocessingDocument.Open(breakerStream, true))
            {
                doc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
            }
            breakerStream.Position = 0;

            using (var doc = WordprocessingDocument.Open(stream, true))
            {
                var mainPart = doc.MainDocumentPart;
                var mainDocumentXDoc = GetXDocument(doc);

                var breakerChunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.WordprocessingML, "breaker-chunk");
                breakerChunk.FeedData(breakerStream);
                
                foreach (Stream document in documents)
                {
                    if(ordinal > 0)
                    {
                        var breakerChunkElement = new XElement(w + "altChunk", new XAttribute(r + "id", "breaker-chunk"));
                        mainDocumentXDoc.Root.Element(w + "body").Add(breakerChunkElement);                        
                    }

                    var altChunkId = "part-" + ++ordinal;
                    var chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                    chunk.FeedData(document);

                    var altChunk = new XElement(w + "altChunk", new XAttribute(r + "id", altChunkId));
                    mainDocumentXDoc.Root.Element(w + "body").Add(altChunk);
                }

                SaveXDocument(doc, mainDocumentXDoc);
            }

            stream.Position = 0;

            return stream;
        }

        private void MarkReferencesAsDirty(WordprocessingDocument document)
        {
            /*
             * WordprocessingDocument представляет собой xml-файл, слегка облагороженный api
             * Целью является пометка полей-ссылок как подлежащих перевычислению,
             * но однозначно идентифицировать их не так-то легко.
             * Во-первых они не являюся цельным элементом: это три последовательных элемента,
             * разбросанныые по разным родительским.
             * 
             * Сначала идёт fldChar с атрибутом type, равным begin. Именно этот элемент помечается isDirty.
             * Ему должен следовать fldChar с атрибутом type, равным end.
             * Однако, эта пара элементов может обозначать не только вычисляемую ссылку.
             * Для того, чтобы можно было сказать, что имеем дело с ссылкой нужно найти между ними 
             * элемент instrText с текстом, начинающимся с " REF".
             * 
             * Кроме REF я встречал DOCVARIABLE.
             * 
             * https://msdn.microsoft.com/en-us/library/office/aa213346(v=office.11).aspx
             * https://msdn.microsoft.com/en-us/library/office/aa172854(v=office.11).aspx
             */
            var fieldBeginMarks = document.MainDocumentPart.Document.Descendants()
                                          .Where(c => c is FieldChar || c is FieldCode)
                                          .ToArray();

            var fieldState = new FieldState();
            foreach (var xmlElement in fieldBeginMarks)
            {
                var fieldChar = xmlElement as FieldChar;
                var fieldCode = xmlElement as FieldCode;

                if (fieldChar != null && fieldChar.FieldCharType == "begin")
                {
                    fieldState.Begin = fieldChar;
                }

                if (fieldChar != null && fieldChar.FieldCharType == "end")
                {
                    fieldState.End = fieldChar;
                }

                if (fieldCode != null)
                {
                    fieldState.Code = fieldCode;
                }

                if (fieldState.IsComplete)
                {
                    if (fieldState.IsReference)
                    {
                        fieldState.Begin.Dirty = new OnOffValue(true);
                    }

                    fieldState = new FieldState();
                }
            }
        }

        private static Stream CreateEmptyDocument()
        {
            var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
            }
            stream.Position = 0;

            return stream;
        }

        private static void SaveXDocument(WordprocessingDocument myDoc, XDocument mainDocumentXDoc)
        {
            using (var stream = myDoc.MainDocumentPart.GetStream(FileMode.Create, FileAccess.Write))
                using (var xmlWriter = XmlWriter.Create(stream))
                    mainDocumentXDoc.Save(xmlWriter);
        }

        private static XDocument GetXDocument(WordprocessingDocument myDoc)
        {
            XDocument mainDocumentXDoc;
            using (var str = myDoc.MainDocumentPart.GetStream())
                using (var xr = XmlReader.Create(str))
                    mainDocumentXDoc = XDocument.Load(xr);
            return mainDocumentXDoc;
        }

        private void FillSdtCell(SdtElement sdtCell, object data, int currencyIsoCode)
        {
            // value to apply format
            var pathToDataField = sdtCell.GetName();

            object value;
            if (!TryGetValueByPath(pathToDataField, data, out value))
            {
                SetOpenXmlElementValue(sdtCell, ResPlatform.PrintFormFieldNotFilled);
                return;
            }

            // format type
            var formatTypeNonParsed = sdtCell.GetTag();
            FormatType formatType;
            if (!Enum.TryParse(formatTypeNonParsed, true, out formatType))
                formatType = FormatType.Unspecified;

            var formattedData = FormatData(formatType, value, currencyIsoCode);

            SetOpenXmlElementValue(sdtCell, formattedData);
        }

        private void FillSdtRow(OpenXmlElement sdtRow, object dataTable, int currencyIsoCode)
        {
            if (dataTable == null)
                return;

            var hasControls = sdtRow.Descendants<SdtElement>().Any();
            if (!hasControls)
                return;

            foreach (var sdtCell in sdtRow.Descendants<SdtElement>())
                FillSdtCell(sdtCell, dataTable, currencyIsoCode);
        }

        private void FillSdtTable(OpenXmlElement sdtTable, IEnumerable dataTable, int currencyIsoCode)
        {
            if (dataTable == null)
                return;

            var table = sdtTable.Descendants<Table>().FirstOrDefault();
            if (table == null)
                return;

            var rows = table.ChildElements.OfType<TableRow>().ToArray();
            if (rows.Length == 0)
                return;

            var templateRow = rows.Count() == 1 ? rows[0] : rows[1];
            var previousRow = templateRow;
            foreach (var dataRow in dataTable)
            {
                var newRow = (TableRow)templateRow.CloneNode(true);
                table.InsertAfter(newRow, previousRow);
                foreach (var sdtCell in newRow.Descendants<SdtElement>())
                {
                    FillSdtCell(sdtCell, dataRow, currencyIsoCode);
                }
                previousRow = newRow;
            }
            table.RemoveChild(templateRow);
        }

        private static void SetOpenXmlElementValue(OpenXmlElement openXmlElement, string value)
        {
            var firstRunElement = openXmlElement.Descendants<Run>().FirstOrDefault();
            if (firstRunElement == null)
                return;

            var firstRunParent = firstRunElement.Parent;
            if (firstRunParent == null)
                return;

            var textElement = (Text)null;
            var redundantElements = new List<OpenXmlElement>();
            foreach (var run in firstRunParent.ChildElements.OfType<Run>())
            {
                redundantElements.Add(run);

                if (textElement != null)
                    continue;

                textElement = run.GetFirstChild<Text>();
                if (textElement != null)
                {
                    redundantElements.Remove(run);
                }
            }
            if (textElement == null)
                return;

            foreach (var redundantElement in redundantElements)
            {
                firstRunParent.RemoveChild(redundantElement);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                textElement.Text = null;
                return;
            }

            var lines = Regex.Split(value, Environment.NewLine);
            if (lines.Length == 1)
            {
                textElement.Text = value;
                return;
            }

            textElement.Text = null;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line))
                    continue;

                if (i > 0)
                    textElement.Parent.AppendChild(new Break());
                textElement.Parent.AppendChild(new Text(lines[i]));
            }
        }

        private string FormatData(FormatType formatType, object data, int currencyIsoCode)
        {
            return (data == null) ? null : _formatterFactory.Create(data.GetType(), formatType, currencyIsoCode).Format(data);
        }

        private static bool TryGetValueByPath(string dottedPathToValue, object valueHolder, out object value)
        {
            var printData = valueHolder as PrintData;
            if (printData != null)
            {
                value = printData.GetData(dottedPathToValue);
                return true;
            }

            if (valueHolder == null || string.IsNullOrEmpty(dottedPathToValue))
            {
                value = null;
                return false;                
            }

            var words = dottedPathToValue.Split('.');
            var fieldName = words[0];

            object fieldValue;
            if (!TryGetFieldValue(valueHolder, fieldName, out fieldValue))
            {
                value = null;
                return false;                
            }

            if (words.Length > 1)
            {
                return TryGetValueByPath(dottedPathToValue.Substring(fieldName.Length + 1), fieldValue, out value);
            }

            value = fieldValue;
            return true;
        }

        private static bool TryGetFieldValue(object data, string fieldName, out object value)
        {
            if (data == null || string.IsNullOrEmpty(fieldName))
            {
                value = null;
                return false;
            }

            var propertyInfo = data.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(fieldName));
            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(data, null);
                return true;
            }

            var fieldInfo = data.GetType().GetFields().FirstOrDefault(p => p.Name.Equals(fieldName));
            if (fieldInfo != null)
            {
                value = fieldInfo.GetValue(data);
                return true;
            }

            value = null;
            return false;
        }

        private class FieldState
        {
            private FieldChar _begin;
            private FieldChar _end;
            private FieldCode _code;

            public bool IsComplete
            {
                get { return _end != null; }
            }

            public bool IsReference
            {
                get { return _code != null && _code.Text.Trim().StartsWith("REF"); }
            }

            public FieldChar Begin
            {
                get { return _begin; }
                set
                {
                    if (_begin != null)
                    {
                        throw new InvalidOperationException("Начало поля уже добавлено");
                    }

                    _begin = value;
                }
            }

            public FieldChar End
            {
                get { return _end; }
                set
                {
                    if (_begin == null)
                    {
                        throw new InvalidOperationException("Начало поля не было добавлено");
                    }

                    _end = value;
                }
            }

            public FieldCode Code
            {
                get { return _code; }
                set
                {
                    if (_begin == null)
                    {
                        throw new InvalidOperationException("Начало поля не было добавлено");
                    }

                    _code = value;
                }
            }
        }
    }
}