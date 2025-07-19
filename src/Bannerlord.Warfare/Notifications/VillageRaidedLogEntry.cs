using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Warfare.Notifications
{
    public class VillageRaidedLogEntry : LogEntry, IEncyclopediaLog, IChatNotification, IWarLog
    {
        [SaveableField(1)]
        public readonly Village Village;
        [SaveableField(2)]
        public readonly MobileParty Party;
        [SaveableField(3)]
        public readonly IFaction BesiegerFaction;

        public VillageRaidedLogEntry(Village village, MobileParty party)
        {
            Village = village;
            Party = party;
            BesiegerFaction = party.MapFaction;
        }

        public override string ToString()
        {
            return GetEncyclopediaText().ToString();
        }

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=xss7eP0f}{PARTY} is raiding {VILLAGE}");
            textObject.SetTextVariable("PARTY", Party.Owner.Name);
            textObject.SetTextVariable("VILLAGE", Village.Name);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected)
        {
            IFaction faction = stance.Faction1;
            IFaction faction2 = stance.Faction2;
            effector = BesiegerFaction;
            effected = Village.Settlement.MapFaction;
            return (BesiegerFaction == faction && Village.Settlement.MapFaction == faction2) || (BesiegerFaction == faction2 && Village.Settlement.MapFaction == faction);
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Party.Party.Owner || obj == Village.Settlement;
        }

        public bool IsVisibleNotification => true;
    }
}
