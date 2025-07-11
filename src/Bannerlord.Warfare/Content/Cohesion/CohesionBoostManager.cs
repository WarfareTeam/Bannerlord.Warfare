using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Warfare.Content.Contracts
{
    internal sealed class CohesionBoostManager
    {
        [SaveableField(1)]
        private List<CohesionBoost> _cohesionBoosts;

        internal CohesionBoostManager() => _cohesionBoosts ??= new();

        internal void CheckCohesionBoosts()
        {
            if (!Settings.Current.EnableCohesionMaintenance)
            {
                _cohesionBoosts.Clear();
                return;
            }
            int change = 0;
            foreach (CohesionBoost boost in _cohesionBoosts.ToListQ())
            {
                if (boost.Army == null || !boost.Army.LeaderParty.IsActive)
                {
                    RemoveCohesionBoost(boost.Army);
                    continue;
                }
                if (boost.Army.Cohesion < 100f && boost.Army.Cohesion < -boost.Army.DailyCohesionChange + 30f)
                {
                    int cohesionToAdd = 100 - (int)boost.Army.Cohesion;
                    int cost = Campaign.Current.Models.ArmyManagementCalculationModel.GetCohesionBoostInfluenceCost(boost.Army, cohesionToAdd);
                    if (Clan.PlayerClan.Influence >= cost)
                    {
                        boost.Army.Cohesion += cohesionToAdd;
                        ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -cost);
                        change -= cost;
                    }
                    else
                    {
                        RemoveCohesionBoost(boost.Army);
                        TextObject information = new TextObject("{=CdZaf4Ir}Insufficient influence to maintain cohesion for army: {ARMY}");
                        information.SetTextVariable("ARMY", boost.Army.Name.ToString());
                        InformationManager.DisplayMessage(new InformationMessage(information.ToString(), new Color(1f, 0f, 0f, 1f)));
                    }
                }
            }
            TextObject information2 = new TextObject("{=QETGd5n6}Daily Influence Change: {CHANGE}{INFLUENCE_ICON}");
            information2.SetTextVariable("CHANGE", change);
            information2.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"8\">");
            InformationManager.DisplayMessage(new InformationMessage(information2.ToString()));
        }

        public void AddCohesionBoost(Army army)
        {
            CohesionBoost boost = FindCohesionBoost(army);
            if (boost == null)
            {
                _cohesionBoosts.Add(new CohesionBoost(army));
            }
        }

        public void RemoveCohesionBoost(Army army)
        {
            CohesionBoost boost = FindCohesionBoost(army);
            if (boost == null)
            {
                return;
            }
            _cohesionBoosts.Remove(boost);
        }

        public CohesionBoost FindCohesionBoost(Army army)
        {
            foreach (CohesionBoost boost in _cohesionBoosts)
            {
                if (boost.Army == army)
                {
                    return boost;
                }
            }
            return null!;
        }
    }
}
