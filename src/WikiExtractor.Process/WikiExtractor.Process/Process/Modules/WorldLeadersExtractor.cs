using System.Collections.Concurrent;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Process.Extractor;

namespace WikiExtractor.Process.Modules
{
    public class WorldLeadersExtractor : DataExtractorBase
    {
        protected WorldLeadersWikiExtractionToStore? toStore = null;

        public WorldLeadersExtractor() : base("World Leaders", "WikiStoreWorldLeaders.db") { }

        protected override void Initialize(bool doClean)
        {
            base.Initialize(doClean);
            toStore = new WorldLeadersWikiExtractionToStore();
        }

        public void ExtractData(string? targetTitle = null)
        {
            Initialize(true);
            int menuItemCounter = 0;

            if (wikiAppController == null) return;
            if (toStore == null) return;

            wikiAppController.AddMenuItem("World Leaders", "All", "World Leaders", menuItemCounter++);
            wikiAppController.AddMenuItem("Australia", "AUS PM", "Prime ministers of Australia", menuItemCounter++);
            wikiAppController.AddMenuItem("New Zealand", "NewZealand PM", "Prime ministers of New Zealand", menuItemCounter++);
            wikiAppController.AddMenuItem("Japan", "JPN PM", "Prime ministers of Japan", menuItemCounter++);
            wikiAppController.AddMenuItem("United States", "US Pre", "Presidents of United States", menuItemCounter++);
            wikiAppController.AddMenuItem("United Kingdom", "UK PM", "Prime ministers of United Kingdom", menuItemCounter++);
            wikiAppController.AddMenuItem("India", "IN PM", "Prime ministers of India", menuItemCounter++);
            wikiAppController.AddMenuItem("Canada", "CN PM", "Prime ministers of Canada", menuItemCounter++);
            wikiAppController.AddMenuItem("Germany", "GER PM", "Presidents of Germany", menuItemCounter++);
            wikiAppController.AddMenuItem("France", "FR PM", "Presidents of France", menuItemCounter++);
            wikiAppController.AddMenuItem("Sweden", "SWE PM", "Prime ministers of Sweden", menuItemCounter++);
            wikiAppController.AddMenuItem("Norway", "NOR PM", "Prime ministers of Norway", menuItemCounter++);
            wikiAppController.AddMenuItem("Denmark", "DEN PM", "Prime ministers of Denmark", menuItemCounter++);
            wikiAppController.AddMenuItem("Finland", "FIN PM", "Prime ministers of Finland", menuItemCounter++);
            wikiAppController.AddMenuItem("Netherlands", "NLD PM", "Prime ministers of Netherlands", menuItemCounter++);
            wikiAppController.AddMenuItem("Belgium", "BEL PM", "Prime ministers of Belgium", menuItemCounter++);
            wikiAppController.AddMenuItem("Italy", "ITA PM", "Prime ministers of Italy", menuItemCounter++);
            wikiAppController.AddMenuItem("Spain", "ESP PM", "Prime ministers of Spain", menuItemCounter++);
            wikiAppController.AddMenuItem("Poland", "POL PM", "Prime ministers of Poland", menuItemCounter++);
            wikiAppController.AddMenuItem("Ireland", "IRL PM", "Prime ministers of Ireland", menuItemCounter++);
            wikiAppController.AddMenuItem("Portugal", "PRT PM", "Prime ministers of Portugal", menuItemCounter++);
            wikiAppController.AddMenuItem("Greece", "GRC PM", "Prime ministers of Greece", menuItemCounter++);
            wikiAppController.AddMenuItem("Czech Republic", "CZE PM", "Prime ministers of Czech Republic", menuItemCounter++);
            wikiAppController.AddMenuItem("Hungary", "HUN PM", "Prime ministers of Hungary", menuItemCounter++);
            wikiAppController.AddMenuItem("Austria", "AUT PM", "Chancellors of Austria", menuItemCounter++);
            wikiAppController.AddMenuItem("Switzerland", "CHE Pre", "Presidents of Switzerland", menuItemCounter++);
            wikiAppController.AddMenuItem("Romania", "ROU Pre", "Presidents of Romania", menuItemCounter++);
            wikiAppController.AddMenuItem("Bulgaria", "BGR Pre", "Presidents of Bulgaria", menuItemCounter++);
            wikiAppController.AddMenuItem("Croatia", "HRV Pre", "Presidents of Croatia", menuItemCounter++);
            wikiAppController.AddMenuItem("Serbia", "SRB Pre", "Presidents of Serbia", menuItemCounter++);
            wikiAppController.AddMenuItem("Slovakia", "SVK Pre", "Presidents of Slovakia", menuItemCounter++);
            wikiAppController.AddMenuItem("Slovenia", "SVN Pre", "Presidents of Slovenia", menuItemCounter++);
            wikiAppController.AddMenuItem("Ukraine", "UKR Pre", "Presidents of Ukraine", menuItemCounter++);
            wikiAppController.AddMenuItem("Russia", "RUS Pre", "Presidents of Russia", menuItemCounter++);
            wikiAppController.AddMenuItem("Iceland", "ISL Pre", "Presidents of Iceland", menuItemCounter++);
            wikiAppController.AddMenuItem("Estonia", "EST Pre", "Presidents of Estonia", menuItemCounter++);
            wikiAppController.AddMenuItem("Latvia", "LVA Pre", "Presidents of Latvia", menuItemCounter++);
            wikiAppController.AddMenuItem("Lithuania", "LTU Pre", "Presidents of Lithuania", menuItemCounter++);
            wikiAppController.AddMenuItem("Brazil", "BRA Pre", "Presidents of Brazil", menuItemCounter++);
            wikiAppController.AddMenuItem("Mexico", "MEX Pre", "Presidents of Mexico", menuItemCounter++);
            wikiAppController.AddMenuItem("Argentina", "ARG Pre", "Presidents of Argentina", menuItemCounter++);
            wikiAppController.AddMenuItem("Chile", "CHL Pre", "Presidents of Chile", menuItemCounter++);
            wikiAppController.AddMenuItem("Colombia", "COL Pre", "Presidents of Colombia", menuItemCounter++);
            wikiAppController.AddMenuItem("Peru", "PER Pre", "Presidents of Peru", menuItemCounter++);
            wikiAppController.AddMenuItem("Venezuela", "VEN Pre", "Presidents of Venezuela", menuItemCounter++);
            wikiAppController.AddMenuItem("Uruguay", "URY Pre", "Presidents of Uruguay", menuItemCounter++);
            wikiAppController.AddMenuItem("Paraguay", "PRY Pre", "Presidents of Paraguay", menuItemCounter++);
            wikiAppController.AddMenuItem("Bolivia", "BOL Pre", "Presidents of Bolivia", menuItemCounter++);
            wikiAppController.AddMenuItem("Ecuador", "ECU Pre", "Presidents of Ecuador", menuItemCounter++);
            wikiAppController.AddMenuItem("Costa Rica", "CRI Pre", "Presidents of Costa Rica", menuItemCounter++);
            wikiAppController.AddMenuItem("Panama", "PAN Pre", "Presidents of Panama", menuItemCounter++);
            wikiAppController.AddMenuItem("Cuba", "CUB Pre", "Presidents of Cuba", menuItemCounter++);
            wikiAppController.AddMenuItem("Dominican Republic", "DOM Pre", "Presidents of Dominican Republic", menuItemCounter++);
            wikiAppController.AddMenuItem("Guatemala", "GTM Pre", "Presidents of Guatemala", menuItemCounter++);
            wikiAppController.AddMenuItem("Honduras", "HND Pre", "Presidents of Honduras", menuItemCounter++);
            wikiAppController.AddMenuItem("El Salvador", "SLV Pre", "Presidents of El Salvador", menuItemCounter++);
            wikiAppController.AddMenuItem("China", "CHN Pre", "Presidents of China", menuItemCounter++);
            wikiAppController.AddMenuItem("South Korea", "KOR Pre", "Presidents of South Korea", menuItemCounter++);
            wikiAppController.AddMenuItem("North Korea", "PRK Ldr", "Leaders of North Korea", menuItemCounter++);
            wikiAppController.AddMenuItem("Indonesia", "IDN Pre", "Presidents of Indonesia", menuItemCounter++);
            wikiAppController.AddMenuItem("Malaysia", "MYS PM", "Prime ministers of Malaysia", menuItemCounter++);
            wikiAppController.AddMenuItem("Singapore", "SGP PM", "Prime ministers of Singapore", menuItemCounter++);
            wikiAppController.AddMenuItem("Thailand", "THA PM", "Prime ministers of Thailand", menuItemCounter++);
            wikiAppController.AddMenuItem("Vietnam", "VNM Pre", "Presidents of Vietnam", menuItemCounter++);
            wikiAppController.AddMenuItem("Philippines", "PHL Pre", "Presidents of Philippines", menuItemCounter++);
            wikiAppController.AddMenuItem("Pakistan", "PAK PM", "Prime ministers of Pakistan", menuItemCounter++);
            wikiAppController.AddMenuItem("Bangladesh", "BGD PM", "Prime ministers of Bangladesh", menuItemCounter++);
            wikiAppController.AddMenuItem("Sri Lanka", "LKA Pre", "Presidents of Sri Lanka", menuItemCounter++);
            wikiAppController.AddMenuItem("Nepal", "NPL PM", "Prime ministers of Nepal", menuItemCounter++);
            wikiAppController.AddMenuItem("Myanmar", "MMR Pre", "Presidents of Myanmar", menuItemCounter++);
            wikiAppController.AddMenuItem("Kazakhstan", "KAZ Pre", "Presidents of Kazakhstan", menuItemCounter++);
            wikiAppController.AddMenuItem("Uzbekistan", "UZB Pre", "Presidents of Uzbekistan", menuItemCounter++);
            wikiAppController.AddMenuItem("Israel", "ISR PM", "Prime ministers of Israel", menuItemCounter++);
            wikiAppController.AddMenuItem("Turkey", "TUR Pre", "Presidents of Turkey", menuItemCounter++);
            wikiAppController.AddMenuItem("Iran", "IRN Pre", "Presidents of Iran", menuItemCounter++);
            wikiAppController.AddMenuItem("Iraq", "IRQ Pre", "Presidents of Iraq", menuItemCounter++);
            wikiAppController.AddMenuItem("Saudi Arabia", "SAU Kng", "Kings of Saudi Arabia", menuItemCounter++);
            wikiAppController.AddMenuItem("Jordan", "JOR Kng", "Kings of Jordan", menuItemCounter++);
            wikiAppController.AddMenuItem("Morocco", "MAR Kng", "Kings of Morocco", menuItemCounter++);
            wikiAppController.AddMenuItem("United Arab Emirates", "UAE Pre", "Presidents of United Arab Emirates", menuItemCounter++);
            wikiAppController.AddMenuItem("Qatar", "QAT Emir", "Emirs of Qatar", menuItemCounter++);
            wikiAppController.AddMenuItem("Kuwait", "KWT Emir", "Emirs of Kuwait", menuItemCounter++);
            wikiAppController.AddMenuItem("South Africa", "ZAF Pre", "Presidents of South Africa", menuItemCounter++);
            wikiAppController.AddMenuItem("Nigeria", "NGA Pre", "Presidents of Nigeria", menuItemCounter++);
            wikiAppController.AddMenuItem("Kenya", "KEN Pre", "Presidents of Kenya", menuItemCounter++);
            wikiAppController.AddMenuItem("Egypt", "EGY Pre", "Presidents of Egypt", menuItemCounter++);
            wikiAppController.AddMenuItem("Ethiopia", "ETH Pre", "Presidents of Ethiopia", menuItemCounter++);
            wikiAppController.AddMenuItem("Ghana", "GHA Pre", "Presidents of Ghana", menuItemCounter++);
            wikiAppController.AddMenuItem("Zimbabwe", "ZWE Pre", "Presidents of Zimbabwe", menuItemCounter++);
            wikiAppController.AddMenuItem("Zambia", "ZMB Pre", "Presidents of Zambia", menuItemCounter++);
            wikiAppController.AddMenuItem("Uganda", "UGA Pre", "Presidents of Uganda", menuItemCounter++);
            wikiAppController.AddMenuItem("Tanzania", "TZA Pre", "Presidents of Tanzania", menuItemCounter++);
            wikiAppController.AddMenuItem("Angola", "AGO Pre", "Presidents of Angola", menuItemCounter++);
            wikiAppController.AddMenuItem("Mozambique", "MOZ Pre", "Presidents of Mozambique", menuItemCounter++);
            wikiAppController.AddMenuItem("Namibia", "NAM Pre", "Presidents of Namibia", menuItemCounter++);
            wikiAppController.AddMenuItem("Botswana", "BWA Pre", "Presidents of Botswana", menuItemCounter++);
            wikiAppController.AddMenuItem("Madagascar", "MDG Pre", "Presidents of Madagascar", menuItemCounter++);
            wikiAppController.AddMenuItem("Algeria", "DZA Pre", "Presidents of Algeria", menuItemCounter++);
            wikiAppController.AddMenuItem("Tunisia", "TUN Pre", "Presidents of Tunisia", menuItemCounter++);
            wikiAppController.AddMenuItem("Libya", "LBY Ldr", "Leaders of Libya", menuItemCounter++);
            wikiAppController.AddMenuItem("Sudan", "SDN Pre", "Presidents of Sudan", menuItemCounter++);
            wikiAppController.AddMenuItem("Cameroon", "CMR Pre", "Presidents of Cameroon", menuItemCounter++);
            wikiAppController.AddMenuItem("Ivory Coast", "CIV Pre", "Presidents of Ivory Coast", menuItemCounter++);
            wikiAppController.AddMenuItem("Senegal", "SEN Pre", "Presidents of Senegal", menuItemCounter++);

            EnablePrimaryMetadataContent();

            var stkAustralia = toStore.ExtractListTabularData("Australia", "/wiki/List_of_prime_ministers_of_Australia", new List<string> { "All", "AUS PM" }).ToStack();
            var stkNewZealand = toStore.ExtractListTabularData("NewZealand", "/wiki/List_of_prime_ministers_of_New_Zealand", new List<string> { "All", "NewZealand PM" }).ToStack();
            var stkJapan = toStore.ExtractListTabularData("Japan", "/wiki/List_of_prime_ministers_of_Japan", new List<string> { "All", "JPN PM" }).ToStack();
            var stkUnitedStates = toStore.ExtractListTabularData("UnitedStates", "/wiki/List_of_presidents_of_the_United_States", new List<string> { "All", "US Pre" }).ToStack();
            var stkUnitedKingdom = toStore.ExtractListTabularData("UnitedKingdom", "/wiki/List_of_prime_ministers_of_the_United_Kingdom", new List<string> { "All", "UK PM" }).ToStack();
            var stkIndia = toStore.ExtractListTabularData("India", "/wiki/List_of_prime_ministers_of_India", new List<string> { "All", "IN PM" }).ToStack();
            var stkCanada = toStore.ExtractListTabularData("Canada", "/wiki/List_of_prime_ministers_of_Canada", new List<string> { "All", "CN PM" }).ToStack();
            var stkFrance = toStore.ExtractListTabularData("France", "/wiki/List_of_presidents_of_France", new List<string> { "All", "FR PM" }).ToStack();
            var stkGermany = toStore.ExtractListTabularData("Germany", "/wiki/List_of_presidents_of_Germany", new List<string> { "All", "GER PM" }).ToStack();
            var stkSweden = toStore.ExtractListTabularData("Sweden", "/wiki/List_of_prime_ministers_of_Sweden", new List<string> { "All", "SWE PM" }).ToStack();
            var stkNorway = toStore.ExtractListTabularData("Norway", "/wiki/List_of_prime_ministers_of_Norway", new List<string> { "All", "NOR PM" }).ToStack();
            var stkDenmark = toStore.ExtractListTabularData("Denmark", "/wiki/List_of_prime_ministers_of_Denmark", new List<string> { "All", "DEN PM" }).ToStack();
            var stkFinland = toStore.ExtractListTabularData("Finland", "/wiki/List_of_prime_ministers_of_Finland", new List<string> { "All", "FIN PM" }).ToStack();
            var stkNetherlands = toStore.ExtractListTabularData("Netherlands", "/wiki/List_of_prime_ministers_of_the_Netherlands", new List<string> { "All", "NLD PM" }).ToStack();
            var stkBelgium = toStore.ExtractListTabularData("Belgium", "/wiki/List_of_prime_ministers_of_Belgium", new List<string> { "All", "BEL PM" }).ToStack();
            var stkItaly = toStore.ExtractListTabularData("Italy", "/wiki/List_of_prime_ministers_of_Italy", new List<string> { "All", "ITA PM" }).ToStack();
            var stkSpain = toStore.ExtractListTabularData("Spain", "/wiki/List_of_prime_ministers_of_Spain", new List<string> { "All", "ESP PM" }).ToStack();
            var stkPoland = toStore.ExtractListTabularData("Poland", "/wiki/List_of_prime_ministers_of_Poland", new List<string> { "All", "POL PM" }).ToStack();
            var stkIreland = toStore.ExtractListTabularData("Ireland", "/wiki/List_of_Irish_heads_of_government", new List<string> { "All", "IRL PM" }).ToStack();
            var stkPortugal = toStore.ExtractListTabularData("Portugal", "/wiki/List_of_prime_ministers_of_Portugal", new List<string> { "All", "PRT PM" }).ToStack();
            var stkGreece = toStore.ExtractListTabularData("Greece", "/wiki/List_of_prime_ministers_of_Greece", new List<string> { "All", "GRC PM" }).ToStack();
            var stkCzechRepublic = toStore.ExtractListTabularData("CzechRepublic", "/wiki/List_of_prime_ministers_of_the_Czech_Republic", new List<string> { "All", "CZE PM" }).ToStack();
            var stkHungary = toStore.ExtractListTabularData("Hungary", "/wiki/List_of_prime_ministers_of_Hungary", new List<string> { "All", "HUN PM" }).ToStack();
            var stkAustria = toStore.ExtractListTabularData("Austria", "/wiki/List_of_chancellors_of_Austria", new List<string> { "All", "AUT PM" }).ToStack();
            var stkSwitzerland = toStore.ExtractListTabularData("Switzerland", "/wiki/List_of_presidents_of_the_Swiss_Confederation", new List<string> { "All", "CHE Pre" }).ToStack();
            var stkRomania = toStore.ExtractListTabularData("Romania", "/wiki/List_of_presidents_of_Romania", new List<string> { "All", "ROU Pre" }).ToStack();
            var stkBulgaria = toStore.ExtractListTabularData("Bulgaria", "/wiki/List_of_presidents_of_Bulgaria", new List<string> { "All", "BGR Pre" }).ToStack();
            var stkCroatia = toStore.ExtractListTabularData("Croatia", "/wiki/List_of_presidents_of_Croatia", new List<string> { "All", "HRV Pre" }).ToStack();
            var stkSerbia = toStore.ExtractListTabularData("Serbia", "/wiki/List_of_presidents_of_Serbia", new List<string> { "All", "SRB Pre" }).ToStack();
            var stkSlovakia = toStore.ExtractListTabularData("Slovakia", "/wiki/List_of_presidents_of_Slovakia", new List<string> { "All", "SVK Pre" }).ToStack();
            var stkSlovenia = toStore.ExtractListTabularData("Slovenia", "/wiki/List_of_presidents_of_Slovenia", new List<string> { "All", "SVN Pre" }).ToStack();
            var stkUkraine = toStore.ExtractListTabularData("Ukraine", "/wiki/List_of_presidents_of_Ukraine", new List<string> { "All", "UKR Pre" }).ToStack();
            var stkRussia = toStore.ExtractListTabularData("Russia", "/wiki/List_of_presidents_of_Russia", new List<string> { "All", "RUS Pre" }).ToStack();
            var stkIceland = toStore.ExtractListTabularData("Iceland", "/wiki/List_of_presidents_of_Iceland", new List<string> { "All", "ISL Pre" }).ToStack();
            var stkEstonia = toStore.ExtractListTabularData("Estonia", "/wiki/List_of_presidents_of_Estonia", new List<string> { "All", "EST Pre" }).ToStack();
            var stkLatvia = toStore.ExtractListTabularData("Latvia", "/wiki/List_of_presidents_of_Latvia", new List<string> { "All", "LVA Pre" }).ToStack();
            var stkLithuania = toStore.ExtractListTabularData("Lithuania", "/wiki/List_of_presidents_of_Lithuania", new List<string> { "All", "LTU Pre" }).ToStack();
            var stkBrazil = toStore.ExtractListTabularData("Brazil", "/wiki/List_of_presidents_of_Brazil", new List<string> { "All", "BRA Pre" }).ToStack();
            var stkMexico = toStore.ExtractListTabularData("Mexico", "/wiki/List_of_presidents_of_Mexico", new List<string> { "All", "MEX Pre" }).ToStack();
            var stkArgentina = toStore.ExtractListTabularData("Argentina", "/wiki/List_of_presidents_of_Argentina", new List<string> { "All", "ARG Pre" }).ToStack();
            var stkChile = toStore.ExtractListTabularData("Chile", "/wiki/List_of_presidents_of_Chile", new List<string> { "All", "CHL Pre" }).ToStack();
            var stkColombia = toStore.ExtractListTabularData("Colombia", "/wiki/List_of_presidents_of_Colombia", new List<string> { "All", "COL Pre" }).ToStack();
            var stkPeru = toStore.ExtractListTabularData("Peru", "/wiki/List_of_presidents_of_Peru", new List<string> { "All", "PER Pre" }).ToStack();
            var stkVenezuela = toStore.ExtractListTabularData("Venezuela", "/wiki/List_of_presidents_of_Venezuela", new List<string> { "All", "VEN Pre" }).ToStack();
            var stkUruguay = toStore.ExtractListTabularData("Uruguay", "/wiki/List_of_presidents_of_Uruguay", new List<string> { "All", "URY Pre" }).ToStack();
            var stkParaguay = toStore.ExtractListTabularData("Paraguay", "/wiki/List_of_presidents_of_Paraguay", new List<string> { "All", "PRY Pre" }).ToStack();
            var stkBolivia = toStore.ExtractListTabularData("Bolivia", "/wiki/List_of_presidents_of_Bolivia", new List<string> { "All", "BOL Pre" }).ToStack();
            var stkEcuador = toStore.ExtractListTabularData("Ecuador", "/wiki/List_of_presidents_of_Ecuador", new List<string> { "All", "ECU Pre" }).ToStack();
            var stkCostaRica = toStore.ExtractListTabularData("CostaRica", "/wiki/List_of_presidents_of_Costa_Rica", new List<string> { "All", "CRI Pre" }).ToStack();
            var stkPanama = toStore.ExtractListTabularData("Panama", "/wiki/List_of_presidents_of_Panama", new List<string> { "All", "PAN Pre" }).ToStack();
            var stkCuba = toStore.ExtractListTabularData("Cuba", "/wiki/List_of_presidents_of_Cuba", new List<string> { "All", "CUB Pre" }).ToStack();
            var stkDominicanRepublic = toStore.ExtractListTabularData("DominicanRepublic", "/wiki/List_of_presidents_of_the_Dominican_Republic", new List<string> { "All", "DOM Pre" }).ToStack();
            var stkGuatemala = toStore.ExtractListTabularData("Guatemala", "/wiki/List_of_presidents_of_Guatemala", new List<string> { "All", "GTM Pre" }).ToStack();
            var stkHonduras = toStore.ExtractListTabularData("Honduras", "/wiki/List_of_presidents_of_Honduras", new List<string> { "All", "HND Pre" }).ToStack();
            var stkElSalvador = toStore.ExtractListTabularData("ElSalvador", "/wiki/List_of_presidents_of_El_Salvador", new List<string> { "All", "SLV Pre" }).ToStack();
            var stkChina = toStore.ExtractListTabularData("China", "/wiki/List_of_presidents_of_China", new List<string> { "All", "CHN Pre" }).ToStack();
            var stkSouthKorea = toStore.ExtractListTabularData("SouthKorea", "/wiki/List_of_presidents_of_South_Korea", new List<string> { "All", "KOR Pre" }).ToStack();
            var stkNorthKorea = toStore.ExtractListTabularData("NorthKorea", "/wiki/List_of_leaders_of_North_Korea", new List<string> { "All", "PRK Ldr" }).ToStack();
            var stkIndonesia = toStore.ExtractListTabularData("Indonesia", "/wiki/List_of_presidents_of_Indonesia", new List<string> { "All", "IDN Pre" }).ToStack();
            var stkMalaysia = toStore.ExtractListTabularData("Malaysia", "/wiki/List_of_prime_ministers_of_Malaysia", new List<string> { "All", "MYS PM" }).ToStack();
            var stkSingapore = toStore.ExtractListTabularData("Singapore", "/wiki/List_of_prime_ministers_of_Singapore", new List<string> { "All", "SGP PM" }).ToStack();
            var stkThailand = toStore.ExtractListTabularData("Thailand", "/wiki/List_of_prime_ministers_of_Thailand", new List<string> { "All", "THA PM" }).ToStack();
            var stkVietnam = toStore.ExtractListTabularData("Vietnam", "/wiki/List_of_presidents_of_Vietnam", new List<string> { "All", "VNM Pre" }).ToStack();
            var stkPhilippines = toStore.ExtractListTabularData("Philippines", "/wiki/List_of_presidents_of_the_Philippines", new List<string> { "All", "PHL Pre" }).ToStack();
            var stkPakistan = toStore.ExtractListTabularData("Pakistan", "/wiki/List_of_prime_ministers_of_Pakistan", new List<string> { "All", "PAK PM" }).ToStack();
            var stkBangladesh = toStore.ExtractListTabularData("Bangladesh", "/wiki/List_of_prime_ministers_of_Bangladesh", new List<string> { "All", "BGD PM" }).ToStack();
            var stkSriLanka = toStore.ExtractListTabularData("SriLanka", "/wiki/List_of_presidents_of_Sri_Lanka", new List<string> { "All", "LKA Pre" }).ToStack();
            var stkNepal = toStore.ExtractListTabularData("Nepal", "/wiki/List_of_prime_ministers_of_Nepal", new List<string> { "All", "NPL PM" }).ToStack();
            var stkMyanmar = toStore.ExtractListTabularData("Myanmar", "/wiki/List_of_presidents_of_Myanmar", new List<string> { "All", "MMR Pre" }).ToStack();
            var stkKazakhstan = toStore.ExtractListTabularData("Kazakhstan", "/wiki/List_of_presidents_of_Kazakhstan", new List<string> { "All", "KAZ Pre" }).ToStack();
            var stkUzbekistan = toStore.ExtractListTabularData("Uzbekistan", "/wiki/List_of_leaders_of_Uzbekistan", new List<string> { "All", "UZB Pre" }).ToStack();
            var stkIsrael = toStore.ExtractListTabularData("Israel", "/wiki/List_of_prime_ministers_of_Israel", new List<string> { "All", "ISR PM" }).ToStack();
            var stkTurkey = toStore.ExtractListTabularData("Turkey", "/wiki/List_of_presidents_of_Turkey", new List<string> { "All", "TUR Pre" }).ToStack();
            var stkIran = toStore.ExtractListTabularData("Iran", "/wiki/List_of_presidents_of_Iran", new List<string> { "All", "IRN Pre" }).ToStack();
            var stkIraq = toStore.ExtractListTabularData("Iraq", "/wiki/List_of_presidents_of_Iraq", new List<string> { "All", "IRQ Pre" }).ToStack();
            var stkSaudiArabia = toStore.ExtractListTabularData("SaudiArabia", "/wiki/List_of_kings_of_Saudi_Arabia", new List<string> { "All", "SAU Kng" }).ToStack();
            var stkJordan = toStore.ExtractListTabularData("Jordan", "/wiki/List_of_kings_of_Jordan", new List<string> { "All", "JOR Kng" }).ToStack();
            var stkMorocco = toStore.ExtractListTabularData("Morocco", "/wiki/List_of_rulers_of_Morocco", new List<string> { "All", "MAR Kng" }).ToStack();
            var stkUnitedArabEmirates = toStore.ExtractListTabularData("UnitedArabEmirates", "/wiki/List_of_presidents_of_the_United_Arab_Emirates", new List<string> { "All", "UAE Pre" }).ToStack();
            var stkQatar = toStore.ExtractListTabularData("Qatar", "/wiki/List_of_emirs_of_Qatar", new List<string> { "All", "QAT Emir" }).ToStack();
            var stkKuwait = toStore.ExtractListTabularData("Kuwait", "/wiki/List_of_emirs_of_Kuwait", new List<string> { "All", "KWT Emir" }).ToStack();
            var stkSouthAfrica = toStore.ExtractListTabularData("SouthAfrica", "/wiki/List_of_heads_of_state_of_South_Africa", new List<string> { "All", "ZAF Pre" }).ToStack();
            var stkNigeria = toStore.ExtractListTabularData("Nigeria", "/wiki/List_of_presidents_of_Nigeria", new List<string> { "All", "NGA Pre" }).ToStack();
            var stkKenya = toStore.ExtractListTabularData("Kenya", "/wiki/List_of_heads_of_state_of_Kenya", new List<string> { "All", "KEN Pre" }).ToStack();
            var stkEgypt = toStore.ExtractListTabularData("Egypt", "/wiki/List_of_presidents_of_Egypt", new List<string> { "All", "EGY Pre" }).ToStack();
            var stkEthiopia = toStore.ExtractListTabularData("Ethiopia", "/wiki/List_of_presidents_of_Ethiopia", new List<string> { "All", "ETH Pre" }).ToStack();
            var stkGhana = toStore.ExtractListTabularData("Ghana", "/wiki/List_of_heads_of_state_of_Ghana", new List<string> { "All", "GHA Pre" }).ToStack();
            var stkZimbabwe = toStore.ExtractListTabularData("Zimbabwe", "/wiki/List_of_presidents_of_Zimbabwe", new List<string> { "All", "ZWE Pre" }).ToStack();
            var stkZambia = toStore.ExtractListTabularData("Zambia", "/wiki/List_of_presidents_of_Zambia", new List<string> { "All", "ZMB Pre" }).ToStack();
            var stkUganda = toStore.ExtractListTabularData("Uganda", "/wiki/List_of_presidents_of_Uganda", new List<string> { "All", "UGA Pre" }).ToStack();
            var stkTanzania = toStore.ExtractListTabularData("Tanzania", "/wiki/List_of_presidents_of_Tanzania", new List<string> { "All", "TZA Pre" }).ToStack();
            var stkAngola = toStore.ExtractListTabularData("Angola", "/wiki/List_of_presidents_of_Angola", new List<string> { "All", "AGO Pre" }).ToStack();
            var stkMozambique = toStore.ExtractListTabularData("Mozambique", "/wiki/List_of_presidents_of_Mozambique", new List<string> { "All", "MOZ Pre" }).ToStack();
            var stkNamibia = toStore.ExtractListTabularData("Namibia", "/wiki/List_of_presidents_of_Namibia", new List<string> { "All", "NAM Pre" }).ToStack();
            var stkBotswana = toStore.ExtractListTabularData("Botswana", "/wiki/List_of_presidents_of_Botswana", new List<string> { "All", "BWA Pre" }).ToStack();
            var stkMadagascar = toStore.ExtractListTabularData("Madagascar", "/wiki/List_of_presidents_of_Madagascar", new List<string> { "All", "MDG Pre" }).ToStack();
            var stkAlgeria = toStore.ExtractListTabularData("Algeria", "/wiki/List_of_presidents_of_Algeria", new List<string> { "All", "DZA Pre" }).ToStack();
            var stkTunisia = toStore.ExtractListTabularData("Tunisia", "/wiki/List_of_presidents_of_Tunisia", new List<string> { "All", "TUN Pre" }).ToStack();
            var stkLibya = toStore.ExtractListTabularData("Libya", "/wiki/List_of_heads_of_state_of_Libya", new List<string> { "All", "LBY Ldr" }).ToStack();
            var stkSudan = toStore.ExtractListTabularData("Sudan", "/wiki/List_of_heads_of_state_of_Sudan", new List<string> { "All", "SDN Pre" }).ToStack();
            var stkCameroon = toStore.ExtractListTabularData("Cameroon", "/wiki/List_of_presidents_of_Cameroon", new List<string> { "All", "CMR Pre" }).ToStack();
            var stkIvoryCoast = toStore.ExtractListTabularData("IvoryCoast", "/wiki/List_of_presidents_of_Ivory_Coast", new List<string> { "All", "CIV Pre" }).ToStack();
            var stkSenegal = toStore.ExtractListTabularData("Senegal", "/wiki/List_of_presidents_of_Senegal", new List<string> { "All", "SEN Pre" }).ToStack();

            var allStacks = new List<Stack<WikiWhatToExtractModel>>
            {
                stkAustralia, stkNewZealand, stkJapan, stkUnitedStates, stkUnitedKingdom,
                stkIndia, stkCanada, stkFrance, stkGermany, stkSweden,
                stkNorway, stkDenmark, stkFinland, stkNetherlands, stkBelgium,
                stkItaly, stkSpain, stkPoland, stkIreland, stkPortugal,
                stkGreece, stkCzechRepublic, stkHungary, stkAustria, stkSwitzerland,
                stkRomania, stkBulgaria, stkCroatia, stkSerbia, stkSlovakia,
                stkSlovenia, stkUkraine, stkRussia, stkIceland, stkEstonia,
                stkLatvia, stkLithuania, stkBrazil, stkMexico, stkArgentina,
                stkChile, stkColombia, stkPeru, stkVenezuela, stkUruguay,
                stkParaguay, stkBolivia, stkEcuador, stkCostaRica, stkPanama,
                stkCuba, stkDominicanRepublic, stkGuatemala, stkHonduras, stkElSalvador,
                stkChina, stkSouthKorea, stkNorthKorea, stkIndonesia, stkMalaysia,
                stkSingapore, stkThailand, stkVietnam, stkPhilippines, stkPakistan,
                stkBangladesh, stkSriLanka, stkNepal, stkMyanmar, stkKazakhstan,
                stkUzbekistan, stkIsrael, stkTurkey, stkIran, stkIraq,
                stkSaudiArabia, stkJordan, stkMorocco, stkUnitedArabEmirates, stkQatar,
                stkKuwait, stkSouthAfrica, stkNigeria, stkKenya, stkEgypt,
                stkEthiopia, stkGhana, stkZimbabwe, stkZambia, stkUganda,
                stkTanzania, stkAngola, stkMozambique, stkNamibia, stkBotswana,
                stkMadagascar, stkAlgeria, stkTunisia, stkLibya, stkSudan,
                stkCameroon, stkIvoryCoast, stkSenegal,
            };

            List<WikiWhatToExtractModel> worldLeadersCollection = new();

            bool hasElements;
            do
            {
                hasElements = false;
                foreach (var stack in allStacks)
                {
                    if (stack.Count > 0)
                    {
                        worldLeadersCollection.Add(stack.Pop());
                        hasElements = true;
                    }
                }
            } while (hasElements);

            if (!string.IsNullOrWhiteSpace(targetTitle))
            {
                worldLeadersCollection = worldLeadersCollection
                    .Where(l => l.Title.Contains(targetTitle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                Console.WriteLine($"\n[WorldLeaders] Target filter '{targetTitle}' → {worldLeadersCollection.Count} match(es)");
            }

            int totalCount = worldLeadersCollection.Count;
            int currentIndex = 1;

            Console.WriteLine($"\n[WorldLeaders] Collection assembled: {totalCount} leaders across {allStacks.Count} countries");

            ConcurrentBag<Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>> bag = new();
            ConcurrentBag<Guid> fetchFailedIds = new();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = ProcessConstants.UseCache ? 5 : 1
            };

            LogPhase("Fetch pages");
            long fetchStart = Environment.TickCount64;
            Parallel.ForEach(worldLeadersCollection, parallelOptions, leader =>
            {
                int idx;
                lock (_lock) { idx = currentIndex++; }
                LogProgress("Fetch", idx, totalCount, fetchStart, $"{leader.Title}  ({leader.Route})");
                try
                {
                    var rawData = toStore.SinglePageContentExtract(leader);
                    bag.Add(new Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>(rawData.Item1, rawData.Item2, leader));
                }
                catch (Exception ex)
                {
                    fetchFailedIds.Add(leader.Id);
                    Console.WriteLine($"  [FETCH ERROR] {leader.Title}: {ex.Message}");
                }
            });
            LogPhaseSummary("Fetch", totalCount, fetchStart);

            // Warn about any items that came back empty
            foreach (var leader in worldLeadersCollection)
            {
                var bagItem = bag.FirstOrDefault(f => f.Item3.Id == leader.Id);
                if (bagItem == null || bagItem.Item1 == null || bagItem.Item2 == null)
                    Console.WriteLine($"  [WARN] No page data for [{leader.Title}]: {leader.Route} — extraction likely failed");
            }

            currentIndex = 1;
            ConcurrentDictionary<Guid, int> storedMasterIds = new();
            LogPhase("Store to DB");
            long storeStart = Environment.TickCount64;
            Parallel.ForEach(worldLeadersCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, leader =>
            {
                int idx;
                lock (_lock) { idx = currentIndex++; }
                LogProgress("Store", idx, totalCount, storeStart, leader.Title);
                try
                {
                    var bagItem = bag.FirstOrDefault(f => f.Item3.Id == leader.Id);
                    if (bagItem == null) return;
                    var masterId = toStore.SinglePageContentStore(bagItem.Item1, bagItem.Item2, bagItem.Item3);
                    storedMasterIds[leader.Id] = masterId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [STORE ERROR] {leader.Title}: {ex.Message}");
                }
            });
            LogPhaseSummary("Store", totalCount, storeStart);

            ////Clean the data
            //CleanDataWithDump();

            var extractionRecords = worldLeadersCollection.Select(leader =>
            {
                var bagItem = bag.FirstOrDefault(f => f.Item3.Id == leader.Id);
                return new ExtractionReporter.ExtractionRecord
                {
                    Item = leader,
                    PageModel = bagItem?.Item1,
                    Metadatas = bagItem?.Item2,
                    PageFetchFailed = fetchFailedIds.Contains(leader.Id),
                    StoredMasterId = storedMasterIds.TryGetValue(leader.Id, out var mid) ? mid : 0,
                };
            }).ToList();

            var reportFolder = Path.Combine(Path.GetDirectoryName(ProcessConstants.DatabasePath)!, "..", "Reports");
            var reporter = new ExtractionReporter(reportFolder, "WorldLeaders");
            reporter.WriteReports(extractionRecords, imageValidationDelayMs: ProcessConstants.UseCache ? 0 : 2000, skipImageValidation: true);
        }

        public void EnablePrimaryMetadataContent()
        {
            if (wikiAppController == null)
            {
                Initialize(false);
            }
            wikiAppController!.EnableWithPrimaryMetadataContent(new List<string>
            {
                "Country",
                "Birth-Death",
                "Preceded by",
                "Succeeded by",
                "Political party",
                "Party",
                "Monarch",
            }, 6);

        }
        public void CleanDataWithDump()
        {
            Initialize(false);

            var data = wikiAppController.GetListOfWikiItems(new List<string> { "All" }).ToList();

            foreach (var item in data)
            {
                Console.WriteLine($"Primary image fix for: {item.Name}");
                var personaData = wikiAppController?.GetViewModelByIdAsync(item.Id).GetAwaiter().GetResult();
                if (personaData != null && personaData.Metadatas?.Any(f => f.Key == "Portrait") == true)
                {
                    wikiAppController.UpdatePrimaryImage(item.Id, personaData.Metadatas?.First(f => f.Key == "Portrait").Description);
                }
            }

            foreach (var item in data)
            {
                Console.WriteLine($"Removing metadata [not required]: {item.Name}");
                wikiAppController.RemoveMetadataInfo(item.Id, "Portrait", "No", "Website");
            }
        }

        public void TestData()
        {
            Initialize(false);
            var data = wikiAppController?.GetListOfWikiItems(new List<string> { "All" }).ToList();
            int counter = 0;

            foreach (var item in data)
            {
                counter++;
                Console.WriteLine($"Testing data for [{counter++}/{data.Count}]: {item.Name}");
                var personaData = wikiAppController?.GetViewModelByIdAsync(item.Id).GetAwaiter().GetResult();
            }
        }

        public void Test()
        {
            Initialize(false);
            wikiAppController.CommonMetadata();
        }
    }
}
