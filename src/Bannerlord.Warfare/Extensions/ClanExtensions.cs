using System.Linq;

using TaleWorlds.CampaignSystem;

namespace Warfare.Extensions
{
    internal static class ClanExtensions
    {

        internal static int GetMercenaryWage(this Clan clan)
        {
            int contractType = Settings.Current.MercenaryContractType.SelectedIndex;
            int days = contractType == 0 ? CampaignTime.DaysInWeek : contractType == 1 ? CampaignTime.DaysInSeason : CampaignTime.DaysInYear;
            return (int)(clan.WarPartyComponents.Sum(x => x.MobileParty.TotalWage * Settings.Current.MercenaryContractGoldCostMultiplier) * days);
        }

        internal static int GetRosterSize(this Clan clan)
        {
            return clan.WarPartyComponents.Select(x => x.Party.NumberOfAllMembers).Sum();
        }

        internal static int GetRosterLimit(this Clan clan)
        {
            float elapsedWeeksUntilNow = Campaign.Current.Models.CampaignTimeModel.CampaignStartTime.ElapsedWeeksUntilNow;
            return (int)(clan.WarPartyComponents.Select(x => Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(x.Party).ResultNumber).Sum() + elapsedWeeksUntilNow);
        }
    }
}