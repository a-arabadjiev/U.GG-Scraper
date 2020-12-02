﻿using System;
using System.Collections.Generic;
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

            var testElement = document.QuerySelector(SelectorConstants.RunesSection);
            //Console.WriteLine(testElement.InnerHtml);

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

            //PickRate

            string pickRatePattern = "[0-9.]+(?=%Pick)";

            string pickRate = Regex.Match(championRankingStatsElement.TextContent, pickRatePattern).ToString();

            //BanRate

            string banRatePattern = "[0-9.]+(?=%Ban)";
                   
            string banRate = Regex.Match(championRankingStatsElement.TextContent, banRatePattern).ToString();

            //Matches

            string totalMatchesPattern = "[0-9,]+(?=Matches)";

            string totalMatches = Regex.Match(championRankingStatsElement.TextContent, totalMatchesPattern).ToString();

            //SummonerSpells

            var summonerSpellsElement = document.QuerySelector(SelectorConstants.SummonerSpellsSection);

            string summonerSpellsPattern = "(?<=Summoner)[A-Za-z]+(?=.)";

            var summonerSpells = Regex.Matches(summonerSpellsElement.InnerHtml, summonerSpellsPattern).ToArray();

            string summonerSpellD = summonerSpells[0].ToString();
            string summonerSpellF = summonerSpells[1].ToString();

            //SummonerSpellsWinRate

            string summonersWinRatePattern = "[0-9.]+(?=% WR)";

            string summonersWinRate = Regex.Match(summonerSpellsElement.TextContent, summonersWinRatePattern).ToString();

            //SummonerSpellsTotalMatches

            string summonersTotalMatchesPattern = "[0-9,]+(?= Matches)";

            string summonersTotalMatches = Regex.Match(summonerSpellsElement.TextContent, summonersTotalMatchesPattern).ToString();

            //RuneTrees

            var runesElement = document.QuerySelector(SelectorConstants.RunesSection);

            string mainRuneTreePattern = "(?<=The Rune Tree )[A-Z][a-z]+";

            var runeTrees = Regex.Matches(runesElement.InnerHtml, mainRuneTreePattern).ToArray();

            string mainRuneTree = runeTrees[0].ToString();
            string secondaryRuneTree = runeTrees[1].ToString();

            //RunesWinRate

            string runeWinRatePattern = "(?<=Build)[0-9.]+";

            string runeWinRate = Regex.Match(runesElement.TextContent, runeWinRatePattern).ToString();

            //RunesMatchesCount

            string runeMatchesCountPattern = "[0-9,]+(?= Matches)";

            string runeMatchesCount = Regex.Matches(runesElement.TextContent, runeMatchesCountPattern)[0].ToString();

            //PrimaryRuneTreeRunes

            var primaryRunes = new List<string>();

            var primaryRuneTreeElements = document.QuerySelector(SelectorConstants.PrimaryRuneTreeSection).Children.ToList();
            primaryRuneTreeElements.RemoveAt(0);

            string runeNamePattern = "(?<=The Rune |The Keystone )[A-Za-z :]+";

            foreach (var element in primaryRuneTreeElements)
            {
                foreach (var innerElement in element.Children)
                {
                    foreach (var innerInnerElement in innerElement.Children.Where(x => x.ClassName == "perk perk-active" || x.ClassName == "perk keystone perk-active"))
                    {
                        string runeName = Regex.Match(innerInnerElement.InnerHtml, runeNamePattern).ToString();
                        primaryRunes.Add(runeName);
                    }
                }
            }            

            //foreach (var element in primaryRuneTreeElements.Where(x => x.ClassName == "perk keystone perk - active"))
            //{
            //    Console.WriteLine(element.InnerHtml);
            //    Console.WriteLine();
            //}

            //SecondaryRuneTreeRunes


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
