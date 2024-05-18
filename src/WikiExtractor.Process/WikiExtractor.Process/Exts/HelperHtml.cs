using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Exts
{
    internal class HelperHtml
    {
        public HtmlNodeCollection LoadHtmlAndSelectNodes(string html, string xpath)
        {
            var tempDoc = new HtmlDocument();
            tempDoc.LoadHtml(html);
            return tempDoc.DocumentNode.SelectNodes(xpath);
        }
        public HtmlNodeCollection LoadHtmlAndSelectNodes(HtmlDocument html, string xpath)
        {
            return html.DocumentNode.SelectNodes(xpath);
        }

        public HtmlDocument LoadHtmlDocument(string html)
        {
            var tempDoc = new HtmlDocument();
            tempDoc.LoadHtml(html);
            return tempDoc;
        }


    }
}
