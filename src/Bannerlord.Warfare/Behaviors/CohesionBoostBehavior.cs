using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

using Warfare.Content.Contracts;


namespace Warfare.Behaviors
{
    internal sealed class CohesionBoostBehavior : CampaignBehaviorBase
    {
        [SaveableField(1)]
        private CohesionBoostManager _cohesionManager;

        public CohesionBoostBehavior() => _cohesionManager ??= new();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public void OnDailyTick() => _cohesionManager.CheckCohesionBoosts();

        public void AddCohesionBoost(Army army) => _cohesionManager.AddCohesionBoost(army);

        public void RemoveCohesionBoost(Army army) => _cohesionManager.RemoveCohesionBoost(army);

        public CohesionBoost FindCohesionBoost(Army army) => _cohesionManager.FindCohesionBoost(army);

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_cohesionManager", ref _cohesionManager);
            if (dataStore.IsLoading)
            {
                _cohesionManager ??= new();
            }
        }
    }
}
