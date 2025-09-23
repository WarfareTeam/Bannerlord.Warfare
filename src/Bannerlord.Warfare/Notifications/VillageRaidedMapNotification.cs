using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Warfare.Notifications
{
    public class VillageRaidedMapNotification : InformationData
    {
        public override TextObject TitleText => new TextObject("{=str_raid}Raid", null);

        public override string SoundEventPath => "event:/ui/notification/settlement_under_siege";

        [SaveableProperty(1)]
        public MobileParty Raider {  get; private set; }

        [SaveableProperty(2)]
        public Village Village { get; private set; }

        public VillageRaidedMapNotification(MobileParty raider, Village village, TextObject description) : base(description)
        {
            Raider = raider;
            Village = village;
        }
    }
}
