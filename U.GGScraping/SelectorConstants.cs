namespace U.GGScraping
{
    public class SelectorConstants
    {
        //Champion section
        public const string ChampionHeader = "div.champion-header";

        public const string ChampionRankingStatsSection = "div.champion-profile-page > div > div.content-section.champion-ranking-stats";

        //Summoner spells section
        public const string SummonerSpellsSection = "div.champion-profile-page > div > div.content-section.content-section_no-padding.grid-1 > div.content-section_content.summoner-spells";

        //Runes section
        public const string RunesSection = "div.champion-profile-page > div > div.content-section.content-section_no-padding.grid-1";

        public const string PrimaryRuneTreeSection = "div.rune-trees-container-2.media-query.media-query_MOBILE_LARGE__DESKTOP_LARGE > div:nth-child(1) > div";

        public const string SecondaryRuneTreeSection = "div.champion-profile-page > div > div.content-section.content-section_no-padding.grid-1 > div.content-section_content.recommended-build_runes > div:nth-child(2) > div.rune-trees-container-2.media-query.media-query_MOBILE_LARGE__DESKTOP_LARGE > div.secondary-tree > div:nth-child(1) > div";

        public const string StatsRuneSection = " div.champion-profile-page > div > div.content-section.content-section_no-padding.grid-1 > div.content-section_content.recommended-build_runes > div:nth-child(2) > div.rune-trees-container-2.media-query.media-query_MOBILE_LARGE__DESKTOP_LARGE > div.secondary-tree > div:nth-child(3) > div";

        //Counter champions section        
        public const string CounterChampionsSection = "div.champion-profile-page > div > div.content-section.toughest-matchups.undefined > div.matchups";

        //Skills section
        public const string SkillsSection = "div.champion-profile-page > div > div.content-section.content-section_no-padding.recommended-build_skills > div.content-section_content.skill-priority > div.skill-priority_content";
    }
}