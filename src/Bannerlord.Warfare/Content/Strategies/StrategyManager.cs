using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.LinQuick;

namespace Warfare.Content.Strategies
{
    internal sealed class StrategyManager
    {
        [SaveableField(1)]
        private List<Strategy> _strategies;

        internal StrategyManager() => _strategies ??= new();

        internal void CheckValidStrategies()
        {
            foreach (Strategy strategy in _strategies.ToListQ())
            {
                Hero hero = strategy.Subject;
                if (!hero.IsActive || hero.IsDead || hero.PartyBelongedTo == null || !hero.PartyBelongedTo.IsActive || !hero.PartyBelongedTo.IsLordParty || !hero.IsPartyLeader)
                {
                    _strategies.Remove(strategy);
                }
            }
        }

        public int GetPriority(Hero subject)
        {
            return FindStrategy(subject).Priority;
        }

        public void SetPriority(Hero subject, int priority)
        {
            FindStrategy(subject).Priority = priority;
        }

        public Strategy FindStrategy(Hero subject)
        {
            foreach (Strategy strategy in _strategies)
            {
                if (subject == strategy.Subject)
                {
                    return strategy;
                }
            }
            Strategy strategy2 = new Strategy(subject, 0);
            _strategies.Add(strategy2);
            return strategy2;
        }
    }
}
