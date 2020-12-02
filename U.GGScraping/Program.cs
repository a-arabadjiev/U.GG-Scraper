using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace U.GGScraping
{
    class Program
    {
        static async Task Main()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync("https://u.gg/lol/champions/aatrox/build");

            string selector = string.Empty;
            IElement element;

            //Lane

            selector = SelectorConstants.ChampionHeader;
            element = document.QuerySelector(selector);

            string lanePattern = "(?<=for )[A-Z]{1}[a-z]+";

            string lane = Regex.Match(element.TextContent, lanePattern).ToString();

            //Tier

            char tier = element.TextContent[0];

            //Patch

            string patchPattern = "[0-9]+.[0-9]+";

            string patch = Regex.Match(element.TextContent, patchPattern).ToString();

            //WinRate

            

            //PickRate


            //BanRate


            //SummonerSpells


            //MainRuneTreeName


            //MainRuneTreeRunes


            //SecondaryRuneTreeName


            //SecondaryRuneTreeRunes


            //RunesWinRate


            //Offense


            //Flex


            //Defense


            //SkillPriority


            //StartingItems


            //MythicAndCoreItems


            //FourthItemOptions


            //FifthItemOptions


            //SixthItemOptions


        }
    }
}
