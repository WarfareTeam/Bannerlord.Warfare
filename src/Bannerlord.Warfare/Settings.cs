using TaleWorlds.Localization;

using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;
using MCM.Common;
using System.Data;

namespace Warfare
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        private const string GeneralHeader = "{=EtmesuQ7}General";
        private const string ArmiesHeader = "{=inKr3PbA}Armies";
        private const string AIHeader = "{=baZKI4zX}AI";
        private const string ArmyAIHeader = ArmiesHeader + "/" + AIHeader;
        private const string StrategyHeader = "{=e1CrjgN3}Strategy";
        private const string ArmyStrategyHeader = ArmiesHeader + "/" + StrategyHeader;
        private const string MercenariesHeader = "{=L3vpqJKB}Mercenaries";
        private const string NotificationsHeader = "{=DVi14Dlk}Notifications";
        private const string OtherHeader = "{=krUuhA7I}Other";

        public override string Id => "WarfareSettings";
        public override string DisplayName => new TextObject("{=Wa3DouAN}Warfare").ToString();
        public override string FolderName => "Warfare";
        public override string FormatType => "json2";

        [SettingPropertyBool("{=FZPtQ4Gu}Modify Winter Warfare Tendency (EXPERIMENTAL)", Order = 0, RequireRestart = false, HintText = "{=Uwo79RY3}Decreases the chance of war decisions in winter based on total calculating trait levels of proposer and ruler. Default: Disabled")]
        [SettingPropertyGroup(GeneralHeader, GroupOrder = 0)]
        public bool ModifyWinterWarfareTendency { get; set; } = false;

        [SettingPropertyBool("{=IcF3bsNX}Enable Cohesion Change", Order = 0, RequireRestart = false, HintText = "{=PKL1vtQN}Allows cohesion to be depleted over time. Default: Enabled")]
        [SettingPropertyGroup(ArmiesHeader, GroupOrder = 10)]
        public bool EnableCohesionChange { get; set; } = true;

        [SettingPropertyBool("{=4z4b8UeA}Enable Army Leader Swapping", Order = 1, RequireRestart = false, HintText = "{=G3MUHLeV}Allows the player to change army leaders in the Kingdom Military screen. Default: Enabled")]
        [SettingPropertyGroup(ArmiesHeader, GroupOrder = 10)]
        public bool EnableArmyLeaderSwapping { get; set; } = true;

        [SettingPropertyBool("{=GIm9j9bW}Enable Army Splitting", Order = 2, RequireRestart = false, HintText = "{=8g6Nz64O}Allows the player to split single armies into two new armies in the Kingdom Military screen. Default: Enabled")]
        [SettingPropertyGroup(ArmiesHeader, GroupOrder = 10)]
        public bool EnableArmySplitting { get; set; } = true;

        [SettingPropertyBool("{=tYVLgewn}Enable Cohesion Maintenance", Order = 3, RequireRestart = false, HintText = "{=VRDKshVt}Allows the player to maintain cohesion for armies in the Kingdom Military screen. Default: Enabled")]
        [SettingPropertyGroup(ArmiesHeader, GroupOrder = 10)]
        public bool EnableCohesionMaintenance { get; set; } = true;

        [SettingPropertyBool("{=SwU5nhKX}Enable Modify Maximum Battlefield Agents", Order = 4, RequireRestart = false, HintText = "{=hIbdHP2A}Allows the maximum battlefield agents modifier to be set using the mod, which overrides the vanilla setting which ranges from 200-1000. Default: Disabled")]
        [SettingPropertyGroup(ArmiesHeader, GroupOrder = 10)]
        public bool EnableModifyMaximumBattlefieldAgents { get; set; } = false;

        [SettingPropertyInteger("{=uqq9P40V}Maximum Battlefield Agents", 1000, 2000, Order = 5, RequireRestart = false, HintText = "{=gNUoN5Ia}Sets the maximum number of total agents on battles. NOTE: Setting this too high can result in crashes! It is recommended to test a field battle using lots of cavalary if you plan to set this higher than 1000. Overrides the Battle Size setting in the vanilla Performance settings section. Default: 1000")]
        [SettingPropertyGroup(ArmiesHeader, GroupOrder = 10)]
        public int MaximumBattlefieldAgents { get; set; } = 1000;

        [SettingPropertyBool("{=pLTpHLi0}Modify Army Besiege AI", Order = 0, RequireRestart = false, HintText = "{=V2B0f9AM}Modifies army AI to prevent sieging a settlement that is being besieged by another army depending on the power difference in attacker and defender power. This lowers the chance of a player siege being taken over by an AI and tends to create multiple fronts in a war when enabled. Default: Enabled")]
        [SettingPropertyGroup(ArmyAIHeader, GroupOrder = 0)]
        public bool ModifyArmyBesiegeAI { get; set; } = true;

        [SettingPropertyInteger("{=9vo8PxtF}Minimum Fiefs To Modify Army Besiege AI", 1, 10, Order = 1, RequireRestart = false, HintText = "{=KAnYH0W2}Modifies the army besieging AI only if the target faction has at least the specified number of fiefs. This has no effect if 'Modify Army Besiege AI' is Disabled. Default: 3")]
        [SettingPropertyGroup(ArmyAIHeader, GroupOrder = 0)]
        public int ModifyArmyBesiegeAIMinimumFiefs { get; set; } = 3;

        [SettingPropertyInteger("{=hQ6ROvxB}Time To Prevent Army Besiege AI (Hours)", 1, 168, Order = 2, RequireRestart = false, HintText = "{=OGME2Ekq}Sets the amount of time in hours that the AI will be prevented from attempting to besiege a settlement being besieged by another army. This has no effect if 'Modify Army Besiege AI' is Disabled. Default: 24")]
        [SettingPropertyGroup(ArmyAIHeader, GroupOrder = 0)]
        public int TimeToPreventArmyBesiegeAIHours { get; set; } = 24;

        [SettingPropertyBool(ArmyStrategyHeader, IsToggle = true, Order = 0, RequireRestart = false, HintText = "{=vX5BEbSR}Allows the player to set war strategies for armies in the Kingdom Military screen. Default: Enabled")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public bool EnableStrategies { get; set; } = true;

        [SettingPropertyFloatingInteger("{=WWq8ftLX}Defensive tendency for defensive Army Strategy", 0.0f, 5.0f, "#0%", Order = 0, RequireRestart = false, HintText = "{=0iHIh4HO}Sets the defensive tendency for armies using the defensive strategy. Default: 110%")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public float DefensiveTendencyDefensiveStrategy { get; set; } = 1.1f;

        [SettingPropertyFloatingInteger("{=1raR5mTu}Offensive tendency for defensive Army Strategy", 0.0f, 5.0f, "#0%", Order = 1, RequireRestart = false, HintText = "{=b8IWHpUU}Sets the offensive tendency for armies using the defensive strategy. Default: 65%")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public float OffensiveTendencyDefensiveStrategy { get; set; } = 0.65f;

        [SettingPropertyFloatingInteger("{=1UJ78ara}Chase tendency for defensive Army Strategy", 0.0f, 5.0f, "#0%", Order = 2, RequireRestart = false, HintText = "{=q553aa8M}Sets the party chase tendency for armies using the offensive strategy. Default: 120%")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public float ChaseTendencyDefensiveStrategy { get; set; } = 1.2f;

        [SettingPropertyFloatingInteger("{=h0LG1AKQ}Defensive tendency for offensive Army Strategy", 0.0f, 5.0f, "#0%", Order = 3, RequireRestart = false, HintText = "{=QzPoBQWF}Sets the defensive tendency for armies using the offensive strategy. Default: 65%")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public float DefensiveTendencyOffensiveStrategy { get; set; } = 0.65f;

        [SettingPropertyFloatingInteger("{=fV3Kvnbu}Offensive tendency for offensive Army Strategy", 0.0f, 5.0f, "#0%", Order = 4, RequireRestart = false, HintText = "{=Zs9y8UIJ}Sets the offensive tendency for armies using the offensive strategy. Default: 110%")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public float OffensiveTendencyOffensiveStrategy { get; set; } = 1.1f;

        [SettingPropertyFloatingInteger("{=dpUAnwfP}Chase tendency for offensive Army Strategy", 0.0f, 5.0f, "#0%", Order = 5, RequireRestart = false, HintText = "{=zT85SDWT}Sets the party chase tendency for armies using the offensive strategy. Default: 80%")]
        [SettingPropertyGroup(ArmyStrategyHeader, GroupOrder = 10)]
        public float ChaseTendencyOffensiveStrategy { get; set; } = 0.8f;

        [SettingPropertyDropdown("{=srjDu3DL}Mercenary Contract Type", Order = 0, RequireRestart = false, HintText = "{=jzipGOog}The mercenary contract length. Effects the contract cost as it is relative to daily wages. Default: Seasonal")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public Dropdown<string> MercenaryContractType { get; } = new(new string[] { "{=EVEAh6Eg}Weekly", "{=dIZZ55i0}Seasonal", "{=pYZ9blgd}Annual" }, 1);

        [SettingPropertyFloatingInteger("{=UVQI4HIo}Mercenary Contract Cost Multiplier", 0.0f, 5.0f, Order = 1, RequireRestart = false, HintText = "{=xAl7nwNt}Cost multiplier when hiring a mercenary. Total contract cost is calculated as (Total Daily Party Wages * Total Contract Days * Multiplier). This should be set high enough to pay for food and their troop wages to prevent starvation. Default: 1.25")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public float MercenaryContractGoldCostMultiplier { get; set; } = 1.25f;

        [SettingPropertyBool("{=TMZ4jBjZ}Faction Leader Call Mercenary To Army Without Cost", Order = 2, RequireRestart = false, HintText = "{=642MdHQq}Allows the faction leader to call mercenaries to their army without a cost. Default: Enabled")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public bool FactionLeadersCallMercenaryToArmyWithoutCost { get; set; } = true;

        [SettingPropertyFloatingInteger("{=MlXBzNfg}Cost Multipler To Call Mercenary To Army", 0.0f, 5.0f, Order = 3, RequireRestart = false, HintText = "{=yLoVOwXi}The cost multiplier to call a mercenary party to an army. Default: 1.0")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public float CostMultiplerToCallMercenaryToArmy { get; set; } = 1f;

        [SettingPropertyDropdown("{=tPbzAK5Z}Call Mercenary To Army Cost Type", Order = 4, RequireRestart = false, HintText = "{=Z6MO2Cki}The cost type to call a mercenary party to an army. Default: Denars")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public Dropdown<string> CallMercenaryToArmyCostType { get; } = new(new string[] { "{=hYgmzZJX}Denars", "{=RVPidk5a}Influence" }, 0);

        [SettingPropertyInteger("{=hqpPZFf2}Minimum Wars To Hire Mercenaries", 0, 10, Order = 5, RequireRestart = false, HintText = "{=JIHCDNcu}The minimum amount of wars for AI clans to hire mercenaries. Default: 1")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public int MinimumWarsToHireMercenaries { get; set; } = 1;

        [SettingPropertyInteger("{=JrsHuJB0}Mercenary Clans Per Culture", 0, 10, Order = 6, RequireRestart = false, HintText = "{=b1YjXBJA}Spawns more mercenary clans for each culture until the selected threshold is reached. Default: 5")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public int MercenaryClansPerCulture { get; set; } = 5;

        [SettingPropertyBool("{=rjjUIti9}Spawn Additional Mercenaries", Order = 7, RequireRestart = false, HintText = "{=qTfVx8hh}Spawns more mercenaries when starting a new game than vanilla includes. This will make the world livelier and give more contract possiblities. The mod is balanced around this being enabled. Default: Enabled")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public bool SpawnAdditionalMercenaries { get; set; } = true;

        [SettingPropertyBool("{=rmugSIvK}Maintain Vanilla Names", Order = 8, RequireRestart = false, HintText = "{=BdXcxPA6}Prevents Vanilla mercenary clans and their members from being renamed in a new game. This will cause inconsistency in naming conventions if paired with 'Spawn Additional Mercenaries' option. Default: Disabled")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public bool MaintainVanillaNames { get; set; } = false;

        [SettingPropertyBool("{=U9sTpxGe}Maintain Vanilla Banners", Order = 9, RequireRestart = false, HintText = "{=6Tsx3ntW}Prevents Vanilla mercenary clan banners from being shuffled in a new game. This will cause inconsistency in banner conventions if paired with 'Spawn Additional Mercenaries' option. Default: Disabled")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public bool MaintainVanillaBanners { get; set; } = false;

        [SettingPropertyBool("{=UXEwQKYV}Maintain Vanilla Properties", Order = 10, RequireRestart = false, HintText = "{=EzLcKJwU}Prevents properties of Vanilla mercenary clans and their members from being changed in a new game ie starting gold, renown and troops. Default: Disabled")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public bool MaintainVanillaProperties { get; set; } = false;

        [SettingPropertyBool("{=FBIPOuX5}Use Vanilla Recruitment", Order = 11, RequireRestart = false, HintText = "{=A4KGF9k3}By default, the mod using it's own recruiting scheme to give mercenaries increased importance for richly developed kingdoms, especially as a campaign progresses, similar to Crusader Kings. Toggling this option on will revert to vanilla recruiting behavior. Default: Disabled")]
        [SettingPropertyGroup(MercenariesHeader, GroupOrder = 20)]
        public bool UseVanillaRecruitment { get; set; } = false;

        [SettingPropertyBool(NotificationsHeader, IsToggle = true, Order = 0, RequireRestart = false, HintText = "{=bDcqYCLy}Enables notification settings for when armies are created. Disable to restore vanilla functionality. Default: Enabled")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public bool EnableNotifications { get; set; } = true;

        [SettingPropertyDropdown("{=UXFYkh1n}Army Created Notifications", Order = 1, RequireRestart = false, HintText = "{=S2vYmJwD}The notification type when armies are created. Default: Map Notices")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> ArmyCreatedNotifications { get; } = new(new string[] { "{=gbKP4WXI}None", "{=JhPIjf9E}Map Notices", "{=M0P06g6W}Popups", "{=PGafzLm5}Both" }, 1);

        [SettingPropertyDropdown("{=7pcUYe1h}Army Dispersed Notifications", Order = 2, RequireRestart = false, HintText = "{=PgKmmm1S}The notification type when armies are dispersed. Default: Map Notices")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> ArmyDispersedNotifications { get; } = new(new string[] { "{=gbKP4WXI}None", "{=JhPIjf9E}Map Notices", "{=M0P06g6W}Popups", "{=PGafzLm5}Both" }, 1);

        [SettingPropertyDropdown("{=L57HvIj2}Siege Notifications", Order = 3, RequireRestart = false, HintText = "{=vwv73P46}The notification type when settlements are besieged. Default: Map Notices")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> SiegeNotifications { get; } = new(new string[] { "{=gbKP4WXI}None", "{=JhPIjf9E}Map Notices", "{=M0P06g6W}Popups", "{=PGafzLm5}Both" }, 1);

        [SettingPropertyDropdown("{=v787b8fL}Siege Notification Scope", Order = 4, RequireRestart = false, HintText = "{=2nsISx0j}The scope for notifications when settlements are besieged. This has no effect if 'Siege Notifications' is disabled. Please note that some options may spam you. Default: Player Owned")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> SiegeNotificationScope { get; } = new(new string[] { "{=mqQj47Mg}Player Owned", "{=Wu20hUsM}Player Kingdom" }, 0);

        [SettingPropertyDropdown("{=FJzdaiw3}Raid Notifications", Order = 5, RequireRestart = false, HintText = "{=vHOyBszl}The notification type when villages are raided. Default: Map Notices")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> RaidNotifications { get; } = new(new string[] { "{=gbKP4WXI}None", "{=JhPIjf9E}Map Notices", "{=M0P06g6W}Popups", "{=PGafzLm5}Both" }, 1);

        [SettingPropertyDropdown("{=N0Tep7Mn}Raid Notification Scope", Order = 6, RequireRestart = false, HintText = "{=RsDUECFX}The scope for notifications when villages are raided. This has no effect if 'Raid Notifications' is disabled. Please note that some options may spam you. Default: Player Owned")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> RaidNotificationScope { get; } = new(new string[] { "{=mqQj47Mg}Player Owned", "{=Wu20hUsM}Player Kingdom" }, 0);

        [SettingPropertyDropdown("{=7jmHArxu}Peace Notifications", Order = 7, RequireRestart = false, HintText = "{=hLogSdIh}The notification type when peace is made. Default: Map Notices")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> PeaceNotifications { get; } = new(new string[] { "{=gbKP4WXI}None", "{=JhPIjf9E}Map Notices", "{=M0P06g6W}Popups", "{=PGafzLm5}Both" }, 1);

        [SettingPropertyDropdown("{=TQukRSdC}Peace Notification Scope", Order = 8, RequireRestart = false, HintText = "{=2vEeoIJd}The scope for notifications when peace is made. This has no effect if 'Peace Notifications' is disabled. Please note that some options may spam you. Default: Player Kingdom")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> PeaceNotificationScope { get; } = new(new string[] { "{=Wu20hUsM}Player Kingdom", "{=oA1ePGVL}All Kingdoms" }, 0);

        [SettingPropertyDropdown("{=i75FIDh6}War Notifications", Order = 9, RequireRestart = false, HintText = "{=tzhRdKEb}The notification type when war is declared. Default: Map Notices")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> WarNotifications { get; } = new(new string[] { "{=gbKP4WXI}None", "{=JhPIjf9E}Map Notices", "{=M0P06g6W}Popups", "{=PGafzLm5}Both" }, 1);

        [SettingPropertyDropdown("{=bNsvZ4RR}War Notification Scope", Order = 10, RequireRestart = false, HintText = "{=gMmdELMU}The scope for notifications when war is declared. This has no effect if 'War Notifications' is disabled. Please note that some options may spam you. Default: Player Kingdom")]
        [SettingPropertyGroup(NotificationsHeader, GroupOrder = 30)]
        public Dropdown<string> WarNotificationScope { get; } = new(new string[] { "{=Wu20hUsM}Player Kingdom", "{=oA1ePGVL}All Kingdoms" }, 0);

        [SettingPropertyBool("{=MUSHsQr8}Logging", Order = 0, RequireRestart = false, HintText = "{=1hNL7XjJ}Enables logs for testing and reporting purposes. Enable this only if Author requests a log during issue reproduction. Default: Disabled")]
        [SettingPropertyGroup(OtherHeader, GroupOrder = 100)]
        public bool Logging { get; set; } = false;

        public static Settings Current => Instance!;
    }
}
