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
        public int GetPriority(Hero subject) => _strategyManager.GetPriority(subject);

        public void SetPriority(Hero subject, int priority) => _strategyManager.SetPriority(subject, priority);

        public Strategy FindStrategy(Hero subject) => _strategyManager.FindStrategy(subject);

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
