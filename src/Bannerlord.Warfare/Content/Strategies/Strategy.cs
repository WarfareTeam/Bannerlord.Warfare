using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Warfare.Content.Strategies
{
    internal class Strategy
    {
        [SaveableProperty(1)]
        public Hero Hero { get; private set; }

        [SaveableProperty(2)]
        public int Priority { get; set; }

        public Strategy(Hero hero, int priority)
        {
            Hero = hero;
            Priority = priority;
        }
    }
}
