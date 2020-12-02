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

            //Test

            //var elements = document.QuerySelector(SelectorConstants.ChampionRankingStatsSection);
            //Console.WriteLine(elements.TextContent);

            //Lane

            string championHeaderSelector = SelectorConstants.ChampionHeader;
            var championHeaderElement = document.QuerySelector(championHeaderSelector);

            string lanePattern = "(?<=for )[A-Z]{1}[a-z]+";

            string lane = Regex.Match(championHeaderElement.TextContent, lanePattern).ToString();

            //Tier

            char tier = championHeaderElement.TextContent[0];

            //Patch

            string patchPattern = "[0-9]+.[0-9]+";

            string patch = Regex.Match(championHeaderElement.TextContent, patchPattern).ToString();

            //WinRate

            var championRankingStatsElement = document.QuerySelector(SelectorConstants.ChampionRankingStatsSection);

            string winRatePattern = "[0-9.]+(?=%Win)";

            string winRate = Regex.Match(championRankingStatsElement.TextContent, winRatePattern).ToString();

            Console.WriteLine(winRate);

            //PickRate

            string pickRatePattern = "[0-9.]+(?=%Pick)";

            string pickRate = Regex.Match(championRankingStatsElement.TextContent, pickRatePattern).ToString();

            Console.WriteLine(pickRate);

            //BanRate

            string banRatePattern = "[0-9.]+(?=%Ban)";
                   
            string banRate = Regex.Match(championRankingStatsElement.TextContent, banRatePattern).ToString();

            Console.WriteLine(banRate);

            //Matches

            string totalMatchesPattern = "[0-9,]+(?=Matches)";

            string totalMatches = Regex.Match(championRankingStatsElement.TextContent, totalMatchesPattern).ToString();

            Console.WriteLine(totalMatches);

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
