using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.LinQuick;
using TaleWorlds.SaveSystem;

namespace Warfare.Content.Strategies
{
    internal sealed class StrategyManager
    {
        [SaveableField(1)]
        private List<Strategy> _strategies;

        internal StrategyManager() => _strategies ??= new();

        internal void CheckValidStrategies()
        {
            if (!Settings.Current.EnableStrategies)
            {
                _strategies.Clear();
                return;
            }
            foreach (Strategy strategy in _strategies.ToListQ())
            {
                Hero hero = strategy.Hero;
                if (!hero.IsActive || hero.IsDead || hero.PartyBelongedTo == null || !hero.PartyBelongedTo.IsActive || !hero.PartyBelongedTo.IsLordParty || !hero.IsPartyLeader)
                {
                    _strategies.Remove(strategy);
                }
            }
        }

        public void AddStrategy(Hero hero, int priority)
        {
            _strategies.Add(new Strategy(hero, priority));
        }

        public int GetPriority(Hero hero)
        {
            Strategy strategy = FindStrategy(hero);
            if (strategy != null)
            {
                return strategy.Priority;
            }
            return 0;
        }

        public void SetPriority(Hero hero, int priority)
        {
            Strategy strategy = FindStrategy(hero);
            if (strategy != null)
            {
                strategy.Priority = priority;
                return;
            }
            AddStrategy(hero, priority);
        }

        public Strategy FindStrategy(Hero hero)
        {
            foreach (Strategy strategy in _strategies)
            {
                if (hero == strategy.Hero)
                {
                    return strategy;
                }
            }
            return null!;
        }
    }
}
