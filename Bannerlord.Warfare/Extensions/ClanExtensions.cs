using System.Linq;

using TaleWorlds.CampaignSystem;

namespace Warfare.Extensions
{
    internal static class ClanExtensions
    {
        internal static int GetRosterSize(this Clan clan) => clan.WarPartyComponents.Select(x => x.Party.NumberOfAllMembers).Sum();

        internal static int GetMercenaryWage(this Clan clan) => (int)clan.WarPartyComponents.Sum(x => x.MobileParty.TotalWage * 1.25) * CampaignTime.DaysInSeason;

        internal static int GetRosterLimit(this Clan clan) => (int)(clan.WarPartyComponents.Select(x => Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(x.Party).ResultNumber).Sum() + Campaign.Current.CampaignStartTime.ElapsedWeeksUntilNow);
    }
}