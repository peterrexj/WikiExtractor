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
            else if (country.EqualsIgnoreCase("Sweden"))
            {
                return wikiFinder.ExtractListTabularData_Sweden(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Norway"))
            {
                return wikiFinder.ExtractListTabularData_Norway(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Denmark"))
            {
                return wikiFinder.ExtractListTabularData_Denmark(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Finland"))
            {
                return wikiFinder.ExtractListTabularData_Finland(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Netherlands"))
            {
                return wikiFinder.ExtractListTabularData_Netherlands(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Belgium"))
            {
                return wikiFinder.ExtractListTabularData_Belgium(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Italy"))
            {
                return wikiFinder.ExtractListTabularData_Italy(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Spain"))
            {
                return wikiFinder.ExtractListTabularData_Spain(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Poland"))
            {
                return wikiFinder.ExtractListTabularData_Poland(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Ireland"))
            {
                return wikiFinder.ExtractListTabularData_Ireland(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Portugal"))
            {
                return wikiFinder.ExtractListTabularData_Portugal(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Greece"))
            {
                return wikiFinder.ExtractListTabularData_Greece(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("CzechRepublic"))
            {
                return wikiFinder.ExtractListTabularData_CzechRepublic(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Hungary"))
            {
                return wikiFinder.ExtractListTabularData_Hungary(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Austria"))
            {
                return wikiFinder.ExtractListTabularData_Austria(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Switzerland"))
            {
                return wikiFinder.ExtractListTabularData_Switzerland(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Romania"))
            {
                return wikiFinder.ExtractListTabularData_Romania(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Bulgaria"))
            {
                return wikiFinder.ExtractListTabularData_Bulgaria(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Croatia"))
            {
                return wikiFinder.ExtractListTabularData_Croatia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Serbia"))
            {
                return wikiFinder.ExtractListTabularData_Serbia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Slovakia"))
            {
                return wikiFinder.ExtractListTabularData_Slovakia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Slovenia"))
            {
                return wikiFinder.ExtractListTabularData_Slovenia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Ukraine"))
            {
                return wikiFinder.ExtractListTabularData_Ukraine(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Russia"))
            {
                return wikiFinder.ExtractListTabularData_Russia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Iceland"))
            {
                return wikiFinder.ExtractListTabularData_Iceland(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Estonia"))
            {
                return wikiFinder.ExtractListTabularData_Estonia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Latvia"))
            {
                return wikiFinder.ExtractListTabularData_Latvia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Lithuania"))
            {
                return wikiFinder.ExtractListTabularData_Lithuania(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Brazil"))
            {
                return wikiFinder.ExtractListTabularData_Brazil(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Mexico"))
            {
                return wikiFinder.ExtractListTabularData_Mexico(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Argentina"))
            {
                return wikiFinder.ExtractListTabularData_Argentina(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Chile"))
            {
                return wikiFinder.ExtractListTabularData_Chile(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Colombia"))
            {
                return wikiFinder.ExtractListTabularData_Colombia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Peru"))
            {
                return wikiFinder.ExtractListTabularData_Peru(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Venezuela"))
            {
                return wikiFinder.ExtractListTabularData_Venezuela(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Uruguay"))
            {
                return wikiFinder.ExtractListTabularData_Uruguay(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Paraguay"))
            {
                return wikiFinder.ExtractListTabularData_Paraguay(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Bolivia"))
            {
                return wikiFinder.ExtractListTabularData_Bolivia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Ecuador"))
            {
                return wikiFinder.ExtractListTabularData_Ecuador(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("CostaRica"))
            {
                return wikiFinder.ExtractListTabularData_CostaRica(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Panama"))
            {
                return wikiFinder.ExtractListTabularData_Panama(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Cuba"))
            {
                return wikiFinder.ExtractListTabularData_Cuba(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("DominicanRepublic"))
            {
                return wikiFinder.ExtractListTabularData_DominicanRepublic(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Guatemala"))
            {
                return wikiFinder.ExtractListTabularData_Guatemala(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Honduras"))
            {
                return wikiFinder.ExtractListTabularData_Honduras(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("ElSalvador"))
            {
                return wikiFinder.ExtractListTabularData_ElSalvador(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("China"))
            {
                return wikiFinder.ExtractListTabularData_China(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("SouthKorea"))
            {
                return wikiFinder.ExtractListTabularData_SouthKorea(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("NorthKorea"))
            {
                return wikiFinder.ExtractListTabularData_NorthKorea(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Indonesia"))
            {
                return wikiFinder.ExtractListTabularData_Indonesia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Malaysia"))
            {
                return wikiFinder.ExtractListTabularData_Malaysia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Singapore"))
            {
                return wikiFinder.ExtractListTabularData_Singapore(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Thailand"))
            {
                return wikiFinder.ExtractListTabularData_Thailand(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Vietnam"))
            {
                return wikiFinder.ExtractListTabularData_Vietnam(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Philippines"))
            {
                return wikiFinder.ExtractListTabularData_Philippines(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Pakistan"))
            {
                return wikiFinder.ExtractListTabularData_Pakistan(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Bangladesh"))
            {
                return wikiFinder.ExtractListTabularData_Bangladesh(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("SriLanka"))
            {
                return wikiFinder.ExtractListTabularData_SriLanka(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Nepal"))
            {
                return wikiFinder.ExtractListTabularData_Nepal(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Myanmar"))
            {
                return wikiFinder.ExtractListTabularData_Myanmar(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Kazakhstan"))
            {
                return wikiFinder.ExtractListTabularData_Kazakhstan(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Uzbekistan"))
            {
                return wikiFinder.ExtractListTabularData_Uzbekistan(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Israel"))
            {
                return wikiFinder.ExtractListTabularData_Israel(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Turkey"))
            {
                return wikiFinder.ExtractListTabularData_Turkey(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Iran"))
            {
                return wikiFinder.ExtractListTabularData_Iran(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Iraq"))
            {
                return wikiFinder.ExtractListTabularData_Iraq(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("SaudiArabia"))
            {
                return wikiFinder.ExtractListTabularData_SaudiArabia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Jordan"))
            {
                return wikiFinder.ExtractListTabularData_Jordan(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Morocco"))
            {
                return wikiFinder.ExtractListTabularData_Morocco(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("UnitedArabEmirates"))
            {
                return wikiFinder.ExtractListTabularData_UnitedArabEmirates(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Qatar"))
            {
                return wikiFinder.ExtractListTabularData_Qatar(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Kuwait"))
            {
                return wikiFinder.ExtractListTabularData_Kuwait(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("SouthAfrica"))
            {
                return wikiFinder.ExtractListTabularData_SouthAfrica(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Nigeria"))
            {
                return wikiFinder.ExtractListTabularData_Nigeria(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Kenya"))
            {
                return wikiFinder.ExtractListTabularData_Kenya(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Egypt"))
            {
                return wikiFinder.ExtractListTabularData_Egypt(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Ethiopia"))
            {
                return wikiFinder.ExtractListTabularData_Ethiopia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Ghana"))
            {
                return wikiFinder.ExtractListTabularData_Ghana(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Zimbabwe"))
            {
                return wikiFinder.ExtractListTabularData_Zimbabwe(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Zambia"))
            {
                return wikiFinder.ExtractListTabularData_Zambia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Uganda"))
            {
                return wikiFinder.ExtractListTabularData_Uganda(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Tanzania"))
            {
                return wikiFinder.ExtractListTabularData_Tanzania(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Angola"))
            {
                return wikiFinder.ExtractListTabularData_Angola(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Mozambique"))
            {
                return wikiFinder.ExtractListTabularData_Mozambique(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Namibia"))
            {
                return wikiFinder.ExtractListTabularData_Namibia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Botswana"))
            {
                return wikiFinder.ExtractListTabularData_Botswana(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Madagascar"))
            {
                return wikiFinder.ExtractListTabularData_Madagascar(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Algeria"))
            {
                return wikiFinder.ExtractListTabularData_Algeria(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Tunisia"))
            {
                return wikiFinder.ExtractListTabularData_Tunisia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Libya"))
            {
                return wikiFinder.ExtractListTabularData_Libya(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Sudan"))
            {
                return wikiFinder.ExtractListTabularData_Sudan(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Cameroon"))
            {
                return wikiFinder.ExtractListTabularData_Cameroon(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("IvoryCoast"))
            {
                return wikiFinder.ExtractListTabularData_IvoryCoast(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else if (country.EqualsIgnoreCase("Senegal"))
            {
                return wikiFinder.ExtractListTabularData_Senegal(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
            }
            else
            {
                return null;
            }
        }
    }
}
