using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AzureTraining.Core.Interfaces;

namespace AzureTraining.Core.Services
{
    public class PaginationService : IPaginationService
    {
        public const int PageSize = 3000;
        private const string SplitRegex = @"(?<=[.!?])";
        private const string ProcessedMark = "x2oPfv9IJkkzsJF8c9k3fmY+ZEGQ9tpQjNcXDBGBePqVS5RzoGbh6HL+/WhqIXh65KNC1Q9VlgaPi01TIpovUvgSOF5tMIqcjPLMkaFaJl8vc7UVh8t/RA0YHeS7ePLP0XFTpmKURhtCO8lPsdO/NeKh+mEpYtelUQUc23srjnrSQKQO6mMGYp9wF24RoTz";

        #region xml defines
        
        private const string RootNode = "Document";
        private const string PageNode = "Page";
        private const string PageNumberAttribute = "number";
        private const string ProcessedMarkAttribute = "ProcessedMark";
        
        #endregion

        public int GetPagesCount(string content)
        {
            if (!IsPaginated(content))
                return -1;

            var doc = new XmlDocument();
            doc.LoadXml(content);
            var pageNodes = doc.SelectNodes(string.Format("//{0}", PageNode));
            if (pageNodes == null)
            {
                throw new Exception("Document is marked as processed but no pages found");
            }
            return pageNodes.Count;
        }

        public string GetDocumentPage(string documentContent, int page)
        {
            if (!IsPaginated(documentContent))
                return string.Empty;

            var doc = new XmlDocument();
            doc.LoadXml(documentContent);
            var node = doc.SelectSingleNode(string.Format("//{0}[@{1}='{2}']", PageNode, PageNumberAttribute, page));
            
            if (node == null)
                return string.Empty;
            return node.InnerText;
        }
        
        public string Paginate(string input)
        {
            var doc = new XmlDocument();
            if (IsPaginated(input))
            {
                doc.LoadXml(input);
                return input;
            }
            var rootElement = doc.CreateElement(RootNode);
            rootElement.SetAttribute(ProcessedMarkAttribute, ProcessedMark);
            var root = (XmlElement)doc.AppendChild(rootElement);

            var pagesContent = SeparateContent(input);

            foreach (var pageContent in pagesContent)
            {
                var pageNode = root.AppendChild(doc.CreateElement(PageNode));
                var pageNumberAttribute = doc.CreateAttribute(PageNumberAttribute);
                pageNumberAttribute.Value = pagesContent.IndexOf(pageContent).ToString(CultureInfo.InvariantCulture);
                if (pageNode.Attributes != null) 
                    pageNode.Attributes.Append(pageNumberAttribute);
                pageNode.InnerText = pageContent;
            }
            return doc.OuterXml;
        }

        private static IList<string> SeparateContent(string input)
        {
            var result = new List<string>();
            var sentenses = Regex.Split(input, SplitRegex);
            var page = new StringBuilder();
            foreach (var sentense in sentenses)
            {
                if ((page.Length + sentense.Length) <= PageSize && sentense.Length <= PageSize)
                {
                    page.Append(sentense);
                }
                else
                {
                    result.Add(page.ToString());
                    page = new StringBuilder();
                    var manualDevidedPages = Split(sentense, PageSize);
                    for (var i = 0; i < manualDevidedPages.Count - 1; i++)
                    {
                        result.Add(manualDevidedPages[i]);
                    }

                    if (manualDevidedPages.Last().Length == PageSize)
                    {
                        result.Add(manualDevidedPages.Last());
                    }
                    else
                    {
                        page.Append(manualDevidedPages.Last());
                    }
                }
            }
            result.Add(page.ToString());
            return result;
        }

        private static List<string> Split(string str, int chunkSize)
        {
            if (str.Length <= chunkSize)
                return new List<string> { str };

            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList();
        }

        private static bool IsPaginated(string input)
        {
            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(input);
            }
            catch (XmlException)
            {
                return false;
            }
            var pageNodeList = doc.SelectNodes(string.Format("//{0}[@{1}='{2}']", RootNode, ProcessedMarkAttribute, ProcessedMark));
            return pageNodeList != null;
        }
    }
}
