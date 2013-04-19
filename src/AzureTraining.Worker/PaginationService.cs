using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AzureTraining.Worker
{
    public class PaginationService
    {
        public const int PageSize = 50;

        public string Paginate(string input, out int pagesCount)
        {
            if (Paginated(input))
            {
                pagesCount = 0;
                return input;
            }
            XmlDocument doc = new XmlDocument();
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
            //todo: check tags - if doc contains pagination xml tags return true
            return false;
        }
    }
}
