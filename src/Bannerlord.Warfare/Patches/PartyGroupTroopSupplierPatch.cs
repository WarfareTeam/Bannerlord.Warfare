using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Warfare;

namespace Bannerlord.Warfare.Patches
{
    [HarmonyPatch(typeof(PartyGroupTroopSupplier), MethodType.Constructor, new Type[]
    {
        typeof(MapEvent),
        typeof(BattleSideEnum),
        typeof(FlattenedTroopRoster),
        typeof(Func<UniqueTroopDescriptor, MapEventParty, bool>)
    })]
    public class PartyGroupTroopSupplierPatch
    {
        public static float mountfootratio;
        public static float troops;
        public static float mounts;
        
        private static void Postfix()
        {
            if (MapEvent.PlayerMapEvent.EventType == MapEvent.BattleTypes.Siege)
            {
                mountfootratio = 1f;
                return;
            }
            if (troops == 0f && mounts == 0f)
            {
                foreach (MapEventParty party in MapEvent.PlayerMapEvent.AttackerSide.Parties)
                {
                    foreach (CharacterObject troop in party.Troops.Troops)
                    {
                        troops++;
                        if (troop.IsMounted)
                        {
                            mounts++;
                        }
                    }
                }
                foreach (MapEventParty party2 in MapEvent.PlayerMapEvent.DefenderSide.Parties)
                {
                    foreach (CharacterObject troop2 in party2.Troops.Troops)
                    {
                        troops++;
                        if (troop2.IsMounted)
                        {
                            mounts++;
                        }
                    }
                }
            }
            mountfootratio = 1f + mounts / troops;
        }
        public static int GetMaximumAgents()
        {
            int threshold = Settings.Current.MaximumBattlefieldAgents;
            int agents = (int)(mountfootratio * troops);
            if (agents > threshold || agents == 0)
            {
                return threshold;
            }
            return agents - 1;
        }

        public static int GetMaximumTroops()
        {
            return (int)(GetMaximumAgents() / mountfootratio);
        }
    }
}
