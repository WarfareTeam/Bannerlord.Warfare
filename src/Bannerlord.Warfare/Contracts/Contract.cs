using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Warfare.Contracts
{
    internal class Contract
    {

        [SaveableProperty(1)]
        public Kingdom Employer { get; private set; }

        [SaveableProperty(2)]
        public Clan Mercenary { get; private set; }

        [SaveableProperty(3)]
        public CampaignTime Expiration { get; set; }

        public Contract(Kingdom employer, Clan mercenary, CampaignTime expiration)
        {
            Employer = employer;
            Mercenary = mercenary;
            Expiration = expiration;
        }
    }
}
