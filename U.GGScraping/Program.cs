namespace U.GGScraping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;

    class Program
    {
        static async Task Main()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync("https://u.gg/lol/champions/aatrox/build");

            //Test

            var testElement = document.QuerySelector(SelectorConstants.SkillsSection);
            //Console.WriteLine(testElement.InnerHtml);

            //Lane

            var championHeaderElement = document.QuerySelector(SelectorConstants.ChampionHeader);

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

            //MatchesCount

            string matchesCountPattern = "[0-9,]+(?=Matches)";

            string matchesCount = Regex.Match(championRankingStatsElement.TextContent, matchesCountPattern).ToString();

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

            foreach (var primaryRuneTreeElement in primaryRuneTreeElements)
            {
                foreach (var runeRowElement in primaryRuneTreeElement.Children)
                {
                    foreach (var activeRuneElement in runeRowElement.Children.Where(x => x.ClassName == "perk perk-active" || x.ClassName == "perk keystone perk-active"))
                    {
                        string runeName = Regex.Match(activeRuneElement.InnerHtml, runeNamePattern).ToString();
                        primaryRunes.Add(runeName);
                    }
                }
            }

            //SecondaryRuneTreeRunes

            var secondaryRunes = new List<string>();

            var secondaryRuneTreeElements = document.QuerySelector(SelectorConstants.SecondaryRuneTreeSection).Children.ToList();
            secondaryRuneTreeElements.RemoveAt(0);

            foreach (var secondaryRuneTreeElement in secondaryRuneTreeElements)
            {
                foreach (var runeRowElement in secondaryRuneTreeElement.Children)
                {
                    foreach (var activeRuneElement in runeRowElement.Children.Where(x => x.ClassName == "perk perk-active"))
                    {
                        string runeName = Regex.Match(activeRuneElement.InnerHtml, runeNamePattern).ToString();
                        secondaryRunes.Add(runeName);
                    }
                }
            }

            //StatRunes

            var statRunes = new List<string>();

            var statRuneTreeElements = document.QuerySelector(SelectorConstants.StatsRuneSection).Children.ToList();
            string statRuneNamePatter = "(?<=alt=\")[A-Za-z ]+";

            foreach (var statRuneElement in statRuneTreeElements)
            {
                foreach (var runeRowElement in statRuneElement.Children)
                {
                    foreach (var activeRuneElement in runeRowElement.Children.Where(x => x.ClassName == "shard shard-active"))
                    {
                        string runeName = Regex.Match(activeRuneElement.InnerHtml, statRuneNamePatter).ToString();
                        statRunes.Add(runeName);
                    }
                }
            }

            //SkillPriority

            var skills = new List<string>();

            var skillsSectionElement = document.QuerySelector(SelectorConstants.SkillsSection);

            string skillsPattern = "[A-Z]{1}(?=:)";
            
            var skillElements = Regex.Matches(skillsSectionElement.InnerHtml, skillsPattern).ToArray();

            foreach (var skill in skillElements)
            {
                skills.Add(skill.ToString());
            }

            //SkillsWinRate

            string skillsWinRatioPattern = "[0-9.]+(?=%)";

            string skillsWinRatio = Regex.Match(skillsSectionElement.InnerHtml, skillsWinRatioPattern).ToString();

            //SkillsMatchesCount

            string skillsMatchesCountPattern = "[0-9,]+(?= Matches)";

            string skillsMatchesCount = Regex.Match(skillsSectionElement.InnerHtml, skillsMatchesCountPattern).ToString();

            //StartingItems


            //MythicAndCoreItems


            //FourthItemOptions


            //FifthItemOptions


            //SixthItemOptions


        }
    }
}
