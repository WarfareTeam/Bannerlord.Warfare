using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace Warfare.Patches
{

    [HarmonyPatch(typeof(HeroSpawnCampaignBehavior), "SpawnLordParty")]
    public static class SpawnMinorFactionHeroesPatch
    {
        public static bool Prefix(Hero hero)
        {
            return hero == null || !hero.IsActive || !hero.IsLord || !hero.Clan.IsMinorFaction || hero == Hero.MainHero;
        }
    }
}
