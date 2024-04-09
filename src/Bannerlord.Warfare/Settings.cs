using TaleWorlds.Localization;

using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace Warfare
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        private const string GeneralHeader = "{=mzPAERnA}General";

        public override string Id => "WarfareSettings";
        public override string DisplayName => new TextObject("{=Wa3DouAN}Warfare").ToString();
        public override string FolderName => "Warfare";
        public override string FormatType => "json2";

        [SettingPropertyDropdown("{=srjDu3DL}Mercenary Contract Type", Order = 0, RequireRestart = false, HintText = "{=jzipGOog}The mercenary contract length. Effects the contract cost as it is relative to daily wages. Default: Seasonal")]
        [SettingPropertyGroup(GeneralHeader)]
        public Dropdown<string> MercenaryContractType { get; } = new(new string[] { "{=EVEAh6Eg}Weekly", "{=dIZZ55i0}Seasonal", "{=pYZ9blgd}Annual" }, 1);

        [SettingPropertyFloatingInteger("{=UVQI4HIo}Mercenary Contract Gold Cost Multiplier", 0.0f, 5.0f, Order = 1, RequireRestart = false, HintText = "{=xAl7nwNt}Multiplier for the costs when hiring a mercenary. The total contract cost is calculated by multiplying the total daily cost of mercenary party wages and the amount of days the mercenary will be contracted for. WARNING: If they aren't paid enough to at least cover troop wages, they may not properly feed their troops as well as having other side effects. Default: 1.25")]
        [SettingPropertyGroup(GeneralHeader)]
        public float MercenaryContractGoldCostMultiplier { get; set; } = 1.25f;

        [SettingPropertyBool("{=TMZ4jBjZ}Faction Leaders Can Call Mercenary To Army Without Influence", Order = 2, RequireRestart = false, HintText = "{=642MdHQq}Allows the faction leader to call mercenaries to their army without an influence cost. Default: Enabled")]
        [SettingPropertyGroup(GeneralHeader)]
        public bool FactionLeadersCallMercenaryToArmyWithoutInfluence { get; set; } = true;

        [SettingPropertyFloatingInteger("{=MlXBzNfg}Influence Cost To Call Mercenary To Army", 0.0f, 5.0f, Order = 3, RequireRestart = false, HintText = "{=yLoVOwXi}The influence cost to call a mercenary party to an army. Default: 1.0")]
        [SettingPropertyGroup(GeneralHeader)]
        public float InfluenceCostToCallMercenaryToArmy { get; set; } = 1f;

        [SettingPropertyBool("{=IcF3bsNX}Enable Cohesion Change", Order = 4, RequireRestart = false, HintText = "{=PKL1vtQN}Allows cohesion to be depleted over time. Default: Enabled")]
        [SettingPropertyGroup(GeneralHeader)]
        public bool EnableCohesionChange { get; set; } = true;

        [SettingPropertyInteger("{=hqpPZFf2}Minimum Wars To Hire Mercenaries", 0, 10, Order = 6, RequireRestart = false, HintText = "{=JIHCDNcu}The minimum amount of wars for AI clans to hire mercenaries. Default: 1")]
        [SettingPropertyGroup(GeneralHeader)]
        public int MinimumWarsToHireMercenaries { get; set; } = 1;

        [SettingPropertyBool("{=MUSHsQr8}Logging", Order = 0, RequireRestart = false, HintText = "{=Wa3DouAN}Enables logs for testing and reporting purposes. Default: Disabled")]
        [SettingPropertyGroup("{=krUuhA7I}Other", GroupOrder = 100)]
        public bool Logging { get; set; } = false;

        public static Settings Current => Instance!;
    }
}
