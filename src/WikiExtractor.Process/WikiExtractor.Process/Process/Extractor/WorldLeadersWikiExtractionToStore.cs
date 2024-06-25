using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class WorldLeadersWikiExtractionToStore : WikiExtractionToStoreBase
    {
        private readonly WorldLeadersWikiFinder wikiFinder = new();

        public List<WikiWhatToExtractModel> ExtractListTabularData(string country, string route, List<string>? tags)
        {
            if (country.EqualsIgnoreCase("Canada"))
            {
                return wikiFinder.ExtractListTabularData_Canada(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("UnitedKingdom"))
            {
                return wikiFinder.ExtractListTabularData_UnitedKingdom(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("UnitedStates"))
            {
                return wikiFinder.ExtractListTabularData_UnitedStates(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("India"))
            {
                return wikiFinder.ExtractListTabularData_India(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Australia"))
            {
                return wikiFinder.ExtractListTabularData_Australia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Germany"))
            {
                return wikiFinder.ExtractListTabularData_Germany(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("France"))
            {
                return wikiFinder.ExtractListTabularData_France(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("NewZealand"))
            {
                return wikiFinder.ExtractListTabularData_NewZealand(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Japan"))
            {
                return wikiFinder.ExtractListTabularData_Japan(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else
            {
                return null;
            }
        }
    }
}
