using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Warfare.Content.Strategies
{
    internal class Strategy
    {
        [SaveableProperty(1)]
        public Hero Subject { get; private set; }

        [SaveableProperty(2)]
        public int Priority { get; set; }

        public Strategy(Hero subject, int priority)
        {
            Subject = subject;
            Priority = priority;
        }
    }
}
