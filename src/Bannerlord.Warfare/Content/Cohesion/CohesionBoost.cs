using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Warfare.Content.Contracts
{
    internal class CohesionBoost
    {

        [SaveableProperty(1)]
        public Army Army { get; private set; }

        public CohesionBoost(Army army)
        {
            Army = army;
        }
    }
}
