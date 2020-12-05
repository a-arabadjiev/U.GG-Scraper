namespace Testing
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using U.GGScraping;
    using AngleSharp;
    using RiotSharp;
    using System.Text.RegularExpressions;
    using Enums;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static async Task Main(string[] args)
        {
            var bag = ReturnChampionPageInfo();

            foreach (var dto in bag)
            {
                Console.WriteLine(dto.Name);
            }
        }

        public static ConcurrentBag<ChampionPageDto> ReturnChampionPageInfo()
        {
            //Create browsing context
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var concurrentBag = new ConcurrentBag<ChampionPageDto>();

            var championKeys = GetAllChampionKeys();

            Parallel.For(0, championKeys.Length, (i) =>
            {
                var championPageDto = GetChampionStatistics(context, championKeys[i]);
                concurrentBag.Add(championPageDto);
            });

            //for (int i = 0; i < championKeys.Length; i++)
            //{
            //    var championPageDto = GetChampionStatistics(context, championKeys[i]);
            //    concurrentBag.Add(championPageDto);
            //}

            return concurrentBag;
        }

        private static ChampionPageDto GetChampionStatistics(IBrowsingContext context, string championName)
        {
            //Console.Write(championName);

            var uggDocument = context
                .OpenAsync($"https://u.gg/lol/champions/{championName}/build")
                .GetAwaiter()
                .GetResult();

            if (uggDocument.StatusCode == System.Net.HttpStatusCode.NotFound || uggDocument.DocumentElement.TextContent.Contains("THIS PAGEDOESN'T EXIST"))
            {
                throw new InvalidOperationException();
            }

            var championPageDto = new ChampionPageDto();

            championPageDto.Name = championName;

            // U.GG Scraping
            // Lane
            var championHeaderElement = uggDocument.QuerySelector(SelectorConstants.ChampionHeader);

            string lanePattern = "(?<=for )[A-Za-z]+(?=,)";

            string lane = Regex.Match(championHeaderElement.TextContent, lanePattern).ToString();

            championPageDto.Role = (Role)Enum.Parse(typeof(Role), lane);

            // Tier
            championPageDto.ChampionTier = championHeaderElement.TextContent[0].ToString();

            // Patch
            string patchPattern = "[0-9]+.[0-9]+";

            championPageDto.Patch = Regex.Match(championHeaderElement.TextContent, patchPattern).ToString();

            // WinRate
            var championRankingStatsElement = uggDocument.QuerySelector(SelectorConstants.ChampionRankingStatsSection);

            string winRatePattern = "[0-9.]+(?=%Win)";

            championPageDto.ChampionWinRate = double.TryParse(Regex.Match(championRankingStatsElement.TextContent, winRatePattern).ToString(), out double resultWinRate)
                ? double.Parse(Regex.Match(championRankingStatsElement.TextContent, winRatePattern).ToString()) : 0;

            // PickRate
            string pickRatePattern = "[0-9.]+(?=%Pick)";

            championPageDto.ChampionPickRate = double.TryParse(Regex.Match(championRankingStatsElement.TextContent, pickRatePattern).ToString(), out double resultPickRate)
                ? double.Parse(Regex.Match(championRankingStatsElement.TextContent, pickRatePattern).ToString()) : 0;

            // BanRate
            string banRatePattern = "[0-9.]+(?=%Ban)";

            championPageDto.ChampionBanRate = double.TryParse(Regex.Match(championRankingStatsElement.TextContent, banRatePattern).ToString(), out double resultBanRate) 
                ? double.Parse(Regex.Match(championRankingStatsElement.TextContent, banRatePattern).ToString()) : 0;

            // MatchesCount
            string matchesCountPattern = "[0-9,]+(?=Matches)";

            championPageDto.ChampionTotalMatches = int.Parse(Regex.Match(championRankingStatsElement.TextContent, matchesCountPattern).ToString().Replace(",", ""));

            // SummonerSpells
            var summonerSpellsElement = uggDocument.QuerySelector(SelectorConstants.SummonerSpellsSection);

            string summonerSpellsPattern = "(?<=Summoner Spell )[A-Za-z]+";

            var summonerSpells = Regex.Matches(summonerSpellsElement.InnerHtml, summonerSpellsPattern).ToArray();

            string summonerSpellD = summonerSpells[0].ToString();
            string summonerSpellF = summonerSpells[1].ToString();

            championPageDto.SummonerSpells.Add(summonerSpellD);
            championPageDto.SummonerSpells.Add(summonerSpellF);

            // SummonerSpellsWinRate
            string summonersWinRatePattern = "[0-9.]+(?=% WR)";

            championPageDto.SummonerSpellsWinRate = double.Parse(Regex.Match(summonerSpellsElement.TextContent, summonersWinRatePattern).ToString());

            // SummonerSpellsTotalMatches
            string summonersTotalMatchesPattern = "[0-9,]+(?= Matches)";

            championPageDto.SummonerSpellsTotalMatches = int.Parse(Regex.Match(summonerSpellsElement.TextContent, summonersTotalMatchesPattern).ToString().Replace(",", ""));

            // RuneTrees
            var runesElement = uggDocument.QuerySelector(SelectorConstants.RunesSection);

            string mainRuneTreePattern = "(?<=The Rune Tree )[A-Z][a-z]+";

            var runeTrees = Regex.Matches(runesElement.InnerHtml, mainRuneTreePattern).ToArray();

            string mainRuneTree = runeTrees[0].ToString();
            string secondaryRuneTree = runeTrees[1].ToString();

            championPageDto.MainRuneTree = (RunePath)Enum.Parse(typeof(RunePath), mainRuneTree);
            championPageDto.SecondaryRuneTree = (RunePath)Enum.Parse(typeof(RunePath), secondaryRuneTree);

            // RunesWinRate
            string runeWinRatePattern = "(?<=Build)[0-9.]+";

            championPageDto.RunesWinRate = double.Parse(Regex.Match(runesElement.TextContent, runeWinRatePattern).ToString());

            // RunesMatchesCount
            string runeMatchesCountPattern = "[0-9,]+(?= Matches)";

            championPageDto.RunesMatchesCount = int.Parse(Regex.Matches(runesElement.TextContent, runeMatchesCountPattern)[0].ToString().Replace(",", ""));

            // PrimaryRuneTreeRunes
            var primaryRuneTreeElements = uggDocument.QuerySelector(SelectorConstants.PrimaryRuneTreeSection).Children.ToList();

            primaryRuneTreeElements.RemoveAt(0);  // removes unnecesary div element

            string runeNamePattern = "(?<=The Rune |The Keystone )[A-Za-z :]+";

            foreach (var primaryRuneTreeElement in primaryRuneTreeElements)
            {
                foreach (var runeRowElement in primaryRuneTreeElement.Children)
                {
                    foreach (var activeRuneElement in runeRowElement.Children.Where(x => x.ClassName == "perk perk-active" || x.ClassName == "perk keystone perk-active"))
                    {
                        string runeName = Regex.Match(activeRuneElement.InnerHtml, runeNamePattern).ToString();
                        championPageDto.PrimaryRunes.Add(runeName);
                    }
                }
            }

            // SecondaryRuneTreeRunes
            var secondaryRuneTreeElements = uggDocument.QuerySelector(SelectorConstants.SecondaryRuneTreeSection).Children.ToList();
            secondaryRuneTreeElements.RemoveAt(0); // removes unnecesary div element

            foreach (var secondaryRuneTreeElement in secondaryRuneTreeElements)
            {
                foreach (var runeRowElement in secondaryRuneTreeElement.Children)
                {
                    foreach (var activeRuneElement in runeRowElement.Children.Where(x => x.ClassName == "perk perk-active"))
                    {
                        string runeName = Regex.Match(activeRuneElement.InnerHtml, runeNamePattern).ToString();
                        championPageDto.SecondaryRunes.Add(runeName);
                    }
                }
            }

            // StatRunes
            var statRunes = new List<string>();

            var statRuneTreeElements = uggDocument.QuerySelector(SelectorConstants.StatsRuneSection).Children.ToList();
            string statRuneNamePatter = "(?<=alt=\")[A-Za-z ]+";

            foreach (var statRuneElement in statRuneTreeElements)
            {
                foreach (var runeRowElement in statRuneElement.Children)
                {
                    foreach (var activeRuneElement in runeRowElement.Children.Where(x => x.ClassName == "shard shard-active"))
                    {
                        string runeName = Regex.Match(activeRuneElement.InnerHtml, statRuneNamePatter).ToString();
                        championPageDto.StatRunes.Add(runeName);
                    }
                }
            }

            // SkillPriority
            var skills = new List<string>();

            var skillsSectionElement = uggDocument.QuerySelector(SelectorConstants.SkillsSection);

            string skillsPattern = "[A-Z]{1}(?=:)";

            var skillElements = Regex.Matches(skillsSectionElement.InnerHtml, skillsPattern).ToArray();

            foreach (var skill in skillElements)
            {
                championPageDto.SkillsPriority.Add(skill.ToString());
            }

            // SkillsWinRate
            string skillsWinRatioPattern = "[0-9.]+(?=%)";

            championPageDto.SkillsWinRate = double.Parse(Regex.Match(skillsSectionElement.InnerHtml, skillsWinRatioPattern).ToString());

            // SkillsMatchesCount
            string skillsMatchesCountPattern = "[0-9,]+(?= Matches)";

            championPageDto.SkillsMatchesCount = int.Parse(Regex.Match(skillsSectionElement.InnerHtml, skillsMatchesCountPattern).ToString().Replace(",", ""));

            // CounterChampions
            List<string> counterChampions = new List<string>();

            var counterChampionsSectionElement = uggDocument.QuerySelector(SelectorConstants.CounterChampionsSection);

            string bestCounterChampionPattern = "[A-Za-z &.]+(?=[0-9])";

            string bestCounterChampion = Regex.Match(counterChampionsSectionElement.TextContent, bestCounterChampionPattern).ToString();

            counterChampions.Add(bestCounterChampion);

            string counterChampionsPattern = "(?<=Matches)[A-Za-z &.]+(?=[0-9])";

            var counterChampionElements = Regex.Matches(counterChampionsSectionElement.TextContent, counterChampionsPattern).ToArray();

            foreach (var counterChamp in counterChampionElements)
            {
                championPageDto.CounterChampions.Add(counterChamp.ToString());
            }

            // Meta.src Scraping
            // StartingItems
            var metaSrcDocument = context
                .OpenAsync($"https://www.metasrc.com/5v5/kr/champion/{championName}/{lane}")
                .GetAwaiter()
                .GetResult();

            var startingItemElements = metaSrcDocument.QuerySelector(SelectorConstants.StartingItemsDiv).Children;

            string startingItemPattern = "(?<=alt=\")[A-Za-z ']+(?=\"><)";

            foreach (var itemElement in startingItemElements)
            {
                string item = Regex.Match(itemElement.InnerHtml, startingItemPattern).ToString();
                championPageDto.StartingItems.Add(item);
            }

            // StartingItemsWin&PickRate
            var startingItemsSectionElement = metaSrcDocument.QuerySelector(SelectorConstants.StartingItemsSection);

            string startingItemsRatesPattern = @"(?<=<span>)[0-9]+(?=%</span>)";

            var startingItemRates = Regex.Matches(startingItemsSectionElement.InnerHtml, startingItemsRatesPattern).ToArray();

            championPageDto.StartingItemsWinRate = int.Parse(startingItemRates[0].ToString());
            championPageDto.StartingItemsPickRate = int.Parse(startingItemRates[1].ToString());

            // MainItems
            var mainItemsSectionElements = metaSrcDocument.QuerySelector(SelectorConstants.MainItemsSection).Children;

            string mainItemPattern = "[A-Za-z ']+(?=[0-9]+ [0-9]+)";
            string mainItemWinRatePattern = @"(?<=>)[0-9]+(?=%<\/div>)";

            foreach (var itemElement in mainItemsSectionElements)
            {
                string item = Regex.Match(itemElement.TextContent, mainItemPattern).ToString();
                string winRate = Regex.Match(itemElement.InnerHtml, mainItemWinRatePattern).ToString();

                championPageDto.ItemsWinRateKvp[item] = int.Parse(winRate);
            }

            Console.WriteLine($"{championName} - Complete");
            return championPageDto;
        }

        private static string[] GetAllChampionKeys()
        {
            var riotApi = RiotApi.GetDevelopmentInstance("RGAPI-46dba1f0-2bbc-4b15-95c3-75f9bab62f9b");
            string latestVersion = riotApi.StaticData.Versions.GetAllAsync().GetAwaiter().GetResult()[0];

            return riotApi.StaticData.Champions.GetAllAsync(latestVersion).GetAwaiter().GetResult().Champions.Values.Select(x => x.Key).ToArray();
        }
    }
}
