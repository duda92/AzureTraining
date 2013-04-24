using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AzureTraining.Core
{
    public class PaginationService
    {
        public const int PageSize = 50;

        public string GetDocumentPage(string documentContent, int page)
        {
            if (!Paginated(documentContent))
                return string.Empty;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(documentContent);
            var node = doc.SelectSingleNode(string.Format("//Page[@number='{0}']", page));
            
            if (node == null)
                return string.Empty;
            return node.InnerText;
        }
        
        public string Paginate(string input, out int pagesCount)
        {
            XmlDocument doc = new XmlDocument();
            if (Paginated(input))
            {
                doc.LoadXml(input);
                var pageNodes = doc.SelectNodes("//Page");
                pagesCount = pageNodes.Count;
                return input;
            }
            XmlElement root = (XmlElement)doc.AppendChild(doc.CreateElement("Document"));

            var pagesContent = SeparateContent(input);

            foreach (var pageContent in pagesContent)
            {
                XmlNode pageNode = root.AppendChild(doc.CreateElement("Page"));
                var pageNumberAttribute = doc.CreateAttribute("number");
                pageNumberAttribute.Value = pagesContent.IndexOf(pageContent).ToString();
                pageNode.Attributes.Append(pageNumberAttribute);
                pageNode.InnerText = pageContent;
            }
            pagesCount = pagesContent.Count;
            return doc.OuterXml;
        }

        private IList<string> SeparateContent(string input)
        {
            var result = new List<string>();
            var lenght = input.Length;
            for (int i = 0; i < lenght; i += PageSize)
            {
                if (!(lenght - i < PageSize))
                {
                    result.Add(input.Substring(i, PageSize));
                }
                else
                {
                    result.Add(input.Substring(i, lenght - i));
                }
            }
            return result;
        }
  
        private bool Paginated(string input)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(input);
            }
            catch (XmlException)
            {
                return false;
            }
            return doc.SelectNodes("//Page").Count != 0;
        }
    }
}
