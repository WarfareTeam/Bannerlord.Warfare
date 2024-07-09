using TaleWorlds.CampaignSystem;

using Warfare.Content.Strategies;

namespace Warfare.Behaviors
{
    internal sealed class StrategyBehavior : CampaignBehaviorBase
    {
        internal StrategyManager _strategyManager;

        public StrategyBehavior() => _strategyManager ??= new();

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        }
        public int GetPriority(Hero hero) => _strategyManager.GetPriority(hero);

        public void SetPriority(Hero hero, int priority) => _strategyManager.SetPriority(hero, priority);

        public Strategy FindStrategy(Hero hero) => _strategyManager.FindStrategy(hero);

        public void OnHourlyTick() => _strategyManager.CheckValidStrategies();

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_strategyManager", ref _strategyManager);
            if (dataStore.IsLoading)
            {
                _strategyManager ??= new();
            }
        }
    }
}
