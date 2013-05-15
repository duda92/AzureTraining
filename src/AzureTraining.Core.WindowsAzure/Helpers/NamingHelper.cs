using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace AzureTraining.Core.WindowsAzure.Helpers
{
    public static class NamingHelper
    {
        private const string FileRepeatSuffixTemplate = "_{0}";

        public static string GetProcessedFileName(string originalFileName)
        {
            return originalFileName + "_processed";
        }

        public static string RemoveFullPath(string inputFileName)
        {
            return inputFileName.Substring(inputFileName.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);
        }

        public static void AttachCopyNumberToFileName(Document document, int copyNumber)
        {
            var extension = System.IO.Path.GetExtension(document.Name);
            DetachPreviousCopyNumberFromFileName(copyNumber, document, extension);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(document.Name);

            var copySuffix = copyNumber == 0 ? string.Empty : string.Format(FileRepeatSuffixTemplate, copyNumber);
            var identityString = document.Owner + fileName + copySuffix + extension;

            document.DocumentId = GetSlug(identityString);
            document.Name = fileName + copySuffix + extension;
        }

        private static void DetachPreviousCopyNumberFromFileName(int copyNumber, Document document, string extension)
        {
            if (copyNumber > 1)
            {
                document.Name = document.Name.Replace(string.Format(FileRepeatSuffixTemplate + extension, copyNumber - 1), extension);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Slug are always lowercase.")]
        private static string GetSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = text.ToLowerInvariant();
            text = text.Replace(":", string.Empty);
            text = text.Replace("/", string.Empty);
            text = text.Replace("?", string.Empty);
            text = text.Replace("¿", string.Empty);
            text = text.Replace("!", string.Empty);
            text = text.Replace("¡", string.Empty);
            text = text.Replace("#", string.Empty);
            text = text.Replace("[", string.Empty);
            text = text.Replace("]", string.Empty);
            text = text.Replace("@", string.Empty);
            text = text.Replace("*", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace(",", string.Empty);
            text = text.Replace("\"", string.Empty);
            text = text.Replace("&", string.Empty);
            text = text.Replace("'", string.Empty);
            text = text.Replace(" ", "-");
            text = RemoveDiacritics(text);
            text = RemoveExtraHyphen(text);

            return HttpUtility.UrlEncode(text).Replace("%", string.Empty);
        }

        private static string RemoveExtraHyphen(string text)
        {
            if (text.Contains("--"))
            {
                text = text.Replace("--", "-");
                return RemoveExtraHyphen(text);
            }

            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
