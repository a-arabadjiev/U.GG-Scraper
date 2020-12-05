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
            //Create browsing context
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            string championName = "taliyah";

            await GetChampionStatistics(context, championName);
        }

        private static async Task GetChampionStatistics(IBrowsingContext context, string championName)
        {
            //Get html
            var document = await context.OpenAsync($"https://u.gg/lol/champions/{championName}/build");

            if (document.DocumentElement.TextContent.Contains("THIS PAGEDOESN'T EXIST"))
            {
                Console.WriteLine("Champion page doesn't exist!");
                return;
            }

            //Test

            var testElement = document.QuerySelector(SelectorConstants.SummonerSpellsSection);
            //Console.WriteLine(testElement.InnerHtml);

            //Lane

            var championHeaderElement = document.QuerySelector(SelectorConstants.ChampionHeader);

            string lanePattern = "(?<=for )[A-Za-z]+(?=,)";

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

            Console.WriteLine(championRankingStatsElement.TextContent);

            double banRate = double.Parse(Regex.Match(championRankingStatsElement.TextContent, banRatePattern).ToString());

            //MatchesCount

            string matchesCountPattern = "[0-9,]+(?=Matches)";

            string matchesCount = Regex.Match(championRankingStatsElement.TextContent, matchesCountPattern).ToString();

            //SummonerSpells

            var summonerSpellsElement = document.QuerySelector(SelectorConstants.SummonerSpellsSection);

            string summonerSpellsPattern = "(?<=Summoner Spell )[A-Za-z]+";

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

            primaryRuneTreeElements.RemoveAt(0);  //removes unnecesary div element

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
            secondaryRuneTreeElements.RemoveAt(0); //removes unnecesary div element

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


            //CounterChampions
            List<string> counterChampions = new List<string>();

            var counterChampionsSectionElement = document.QuerySelector(SelectorConstants.CounterChampionsSection);

            string bestCounterChampionPattern = "[A-Za-z &.]+(?=[0-9])"; //TODO: Improve regex

            string bestCounterChampion = Regex.Match(counterChampionsSectionElement.TextContent, bestCounterChampionPattern).ToString();

            counterChampions.Add(bestCounterChampion);

            string counterChampionsPattern = "(?<=Matches)[A-Za-z &.]+(?=[0-9])";

            var counterChampionElements = Regex.Matches(counterChampionsSectionElement.TextContent, counterChampionsPattern).ToArray();

            foreach (var counterChamp in counterChampionElements)
            {
                counterChampions.Add(counterChamp.ToString());
            }

            //Print Data

            //Console.WriteLine($"Lane - {lane}");
            //Console.WriteLine($"Tier - {tier}");
            //Console.WriteLine($"Patch - {patch}");
            //Console.WriteLine($"ChampWinRate - {winRate}");
            //Console.WriteLine($"ChampPickRate - {pickRate}");
            Console.WriteLine($"ChampBanRate - {banRate}");
            //Console.WriteLine($"ChampMatchesCount - {matchesCount}");
            //Console.WriteLine($"SummonerSpells - {summonerSpellD} {summonerSpellF}");
            //Console.WriteLine($"SummonerSpellsWinRate - {summonersWinRate}");
            //Console.WriteLine($"SummonerSpellsTotalMatches - {summonersTotalMatches}");
            //Console.WriteLine($"MainRuneTree - {mainRuneTree}");
            //Console.WriteLine($"SecondaryRuneTree - {secondaryRuneTree}");
            //Console.WriteLine($"RunesWinRate - {runeWinRate}");
            //Console.WriteLine($"RunesMatchesCount - {runeMatchesCount}");

            //Console.WriteLine("Primary Runes:");
            //foreach (var rune in primaryRunes)
            //{
            //    Console.WriteLine($"--{rune}");
            //}

            //Console.WriteLine("Secondary Runes:");
            //foreach (var rune in secondaryRunes)
            //{
            //    Console.WriteLine($"--{rune}");
            //}

            //Console.WriteLine("Stat Runes:");
            //foreach (var rune in statRunes)
            //{
            //    Console.WriteLine($"--{rune}");
            //}

            //Console.WriteLine("Skills:");
            //foreach (var skill in skills)
            //{
            //    Console.WriteLine($"--{skill}");
            //}

            //Console.WriteLine($"SkillsWinRate - {skillsWinRatio}");
            //Console.WriteLine($"SkillsMatchesCount - {skillsMatchesCount}");

            //Console.WriteLine("Counter Champions:");
            //foreach (var champion in counterChampions)
            //{
            //    Console.WriteLine($"--{champion}");
            //}
        }
    }
}
