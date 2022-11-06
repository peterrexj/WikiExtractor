using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WikiExtractor.Exts
{
    public static class HtmlAgilityEx
    {
        public static string DecodedInnerText(this HtmlNode node, bool removeNewLine)
        {
            if (node == null) return "";
            if (node.InnerText.IsEmpty()) return "";

            return Clean(HttpUtility.HtmlDecode(node.InnerText), removeNewLine);
        }

        public static string DecodedInnerText(string content, bool removeNewLine)
        {
            if (content.IsEmpty()) return string.Empty;

            return Clean(HttpUtility.HtmlDecode(content), removeNewLine);
        }
        private static string Clean(string content, bool replaceNewLine)
        {
            if (content.IsEmpty()) return string.Empty;
            content = Regex.Replace(content, @"\[(.*?)\]", "");
            content = Regex.Replace(content, @"\s+", " ");
            if (replaceNewLine)
            {
                content = content.Replace(Environment.NewLine, " ");
            }
            return content;
        }
    }
}

