namespace MetaSrcScraping
{
    using AngleSharp;
    using AngleSharp.Dom;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            //Create browsing context
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            //Get html
            var document = await context.OpenAsync("https://www.metasrc.com/5v5/na/champion/aatrox/top");

            //Test

            var testElement = document.QuerySelector(SelectorConstants.MainItemsSection);
            //Console.WriteLine(testElement.TextContent);

            //StartingItems

            List<string> startingItems = new List<string>();

            var startingItemElements = document.QuerySelector(SelectorConstants.StartingItemsDiv).Children;

            string startingItemPattern = "(?<=alt=\")[A-Za-z ']+(?=\"><)";

            foreach (var itemElement in startingItemElements)
            {
                string item = Regex.Match(itemElement.InnerHtml, startingItemPattern).ToString();
                startingItems.Add(item);
            }

            //StartingItemsWin&PickRate

            var startingItemsSectionElement = document.QuerySelector(SelectorConstants.StartingItemsSection);

            string startingItemsRatesPattern = @"(?<=<span>)[0-9]+(?=%</span>)";

            var startingItemRates = Regex.Matches(startingItemsSectionElement.InnerHtml, startingItemsRatesPattern).ToArray();

            string startingItemsWinRate = startingItemRates[0].ToString();
            string startingItemsPickRate = startingItemRates[1].ToString();

            //MainItems

            Dictionary<string, string> itemWinRateKvp = new Dictionary<string, string>();

            var mainItemsSectionElements = document.QuerySelector(SelectorConstants.MainItemsSection).Children;

            string mainItemPattern = "[A-Za-z ']+(?=[0-9]+ [0-9]+)";
            string mainItemWinRatePattern = @"(?<=\+)[0-9]+(?=%)";

            foreach (var itemElement in mainItemsSectionElements)
            {
                string item = Regex.Match(itemElement.TextContent, mainItemPattern).ToString();
                string winRate = Regex.Match(itemElement.TextContent, mainItemWinRatePattern).ToString();

                itemWinRateKvp[item] = winRate;
            }

            foreach (var item in itemWinRateKvp)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }
    }
}
