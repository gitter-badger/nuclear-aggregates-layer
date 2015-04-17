using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Operations.Generic.File.AdvertisementElements
{
    public sealed class UploadingAdvertisementElementValidator : IUploadingAdvertisementElementValidator
    {
        private static readonly Dictionary<ImageFormat, string[]> ImageFormatToExtensionsMap = new Dictionary<ImageFormat, string[]>
            {
                { ImageFormat.Png, new[] { "png" } },
                { ImageFormat.Gif, new[] { "gif" } },
                { ImageFormat.Bmp, new[] { "bmp" } },
            };

        public void Validate(AdvertisementElementTemplate uploadingElementTemplate, string uploadingFileName, Stream uploadingFileContent)
        {
            if (uploadingFileContent != null && uploadingFileContent.Length > 10485760)
            {
                throw new BusinessLogicException(BLResources.FileSizeMustBeLessThan10MB);
            }

            var messages = ValidateFileAdvertisement(uploadingElementTemplate, uploadingFileName, uploadingFileContent);
            if (messages.Any())
            {
                throw new BusinessLogicException(string.Join("; ", messages));
            }
        }

        public string[] ValidateFileAdvertisement(AdvertisementElementTemplate uploadingElementTemplate, string uploadingFileName, Stream uploadingFileContent)
        {
            var messages = new List<string>();

            if (!ValidateRequired(uploadingFileContent, uploadingElementTemplate))
            {
                messages.Add(BLResources.AdsCheckElemIsRequired);
                return messages.ToArray();
            }

            if (!ValidateFileExtension(uploadingFileName, uploadingElementTemplate))
            {
                messages.Add(string.Format(BLResources.AdsCheckInvalidFileExtension, uploadingElementTemplate.FileExtensionRestriction));
                return messages.ToArray();
            }

            if (!ValidateFileNameLength(uploadingFileName, uploadingElementTemplate))
            {
                messages.Add(string.Format(BLResources.AdsCheckFileNameTooLong, uploadingFileName, uploadingElementTemplate.FileNameLengthRestriction));
            }

            if (!ValidateFileSize(uploadingFileContent, uploadingElementTemplate))
            {
                messages.Add(string.Format(BLResources.AdsCheckFileNameTooBig, uploadingElementTemplate.FileSizeRestriction));
            }

            if (uploadingElementTemplate.RestrictionType == AdvertisementElementRestrictionType.Article)
            {
                try
                {
                    if (!ValidateArticleHasIndexFile(uploadingFileContent))
                    {
                        messages.Add(string.Format(BLResources.AdsCheckArticleContainsIndexFile));
                    }
                }
                catch (Exception)
                {
                    messages.Add(string.Format(BLResources.ErrorWhileReadingChmFile));
                }
            }

            if (uploadingElementTemplate.RestrictionType == AdvertisementElementRestrictionType.Image)
            {
                ValidateImage(uploadingFileContent, uploadingElementTemplate, uploadingFileName, messages);
            }

            return messages.ToArray();
        }

        private static bool ValidateRequired(Stream stream, AdvertisementElementTemplate template)
        {
            if (!template.IsRequired)
            {
                return true;
            }

            return stream != null;
        }

        private static bool ValidateFileExtension(string fileName, AdvertisementElementTemplate template)
        {
            if (string.IsNullOrEmpty(template.FileExtensionRestriction))
            {
                return true;
            }

            var allowedExtensions = ParseExtensions(template.FileExtensionRestriction);
            var extension = GetDotLessExtension(fileName);

            return allowedExtensions.Contains(extension);
        }

        private static bool ValidateFileNameLength(string fileName, AdvertisementElementTemplate template)
        {
            if (template.FileNameLengthRestriction == null)
            {
                return true;
            }

            if (fileName == null)
            {
                return false;
            }

            return fileName.Length <= template.FileNameLengthRestriction.Value;
        }

        private static bool ValidateFileSize(Stream stream, AdvertisementElementTemplate template)
        {
            if (template.FileSizeRestriction == null)
            {
                return true;
            }

            if (stream == null)
            {
                return false;
            }

            var maxFileSize = template.FileSizeRestriction.Value * 1024;
            return stream.Length <= maxFileSize;
        }

        private static bool ValidateArticleHasIndexFile(Stream stream)
        {
            if (stream == null)
            {
                return false;
            }

            return stream.ZipStreamFindFiles(x => string.Equals(x.FileName, "index.html", StringComparison.OrdinalIgnoreCase)).Any();
        }

        private static void ValidateImage(Stream imageStream, AdvertisementElementTemplate template, string fileName, ICollection<string> messages)
        {
            Image image;
            try
            {
                image = Image.FromStream(imageStream);
            }
            catch (ArgumentException)
            {
                messages.Add(string.Format(BLResources.AdsCheckInvalidImageFormatOrExtension));
                return;
            }

            using (image)
            {
                // validate image file extension is synced with internal image fotmat
                var extension = GetDotLessExtension(fileName);
                if (!ImageFormatToExtensionsMap[image.RawFormat].Contains(extension))
                {
                    messages.Add(string.Format(BLResources.AdsCheckInvalidImageFormatOrExtension));
                }

                // validate image size
                if (template.ImageDimensionRestriction != null)
                {
                    var allowedizes = ParseSizes(template.ImageDimensionRestriction);
                    if (!allowedizes.Contains(image.Size))
                    {
                        messages.Add(BLResources.AdsCheckInvalidImageDimension);
                    }
                }
            }
        }

        private static string GetDotLessExtension(string path)
        {
            var dottedExtension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(dottedExtension))
            {
                return dottedExtension;
            }

            var extension = dottedExtension.Trim(new[] { '.' }).ToLowerInvariant();
            return extension;
        }

        private static IEnumerable<Size> ParseSizes(string nonParsedSizes)
        {
            return nonParsedSizes
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                    {
                        var pair = x.Split(new[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
                        var width = int.Parse(pair[0].Trim());
                        var height = int.Parse(pair[1].Trim());

                        return new Size(width, height);
                    }).ToArray();
        }

        private static IEnumerable<string> ParseExtensions(string nonParsedExtensions)
        {
            return nonParsedExtensions
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLowerInvariant())
                .ToArray();
        }
    }
}