using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Warfare.Helpers;

namespace Warfare.Patches
{
    /*
    public static class DefaultBuildingTypesPatches
    {
        private static BuildingType _buildingAcademies;

        [HarmonyPatch(typeof(DefaultBuildingTypes), "RegisterAll")]
        public static class DefaultBuildingTypesRegisterAllPatch
        {

            public static bool Prefix()
            {
                _buildingAcademies = Game.Current.ObjectManager.RegisterPresumedObject(new BuildingType("building_academies"));
                return true;
            }
        }

        [HarmonyPatch(typeof(DefaultBuildingTypes), "InitializeAll")]
        public static class DefaultBuildingTypesInitializeAllPatch
        {
            public static bool Prefix(DefaultBuildingTypes __instance)
            {
                _buildingAcademies.Initialize(new TextObject("Military Academies"), new TextObject("Placeholder"), new int[] { 8000, 16000, 32000 }, BuildingLocation.Settlement, new Tuple<BuildingEffectEnum, float, float, float>[1]
                {
                    new Tuple<BuildingEffectEnum, float, float, float>(BuildingEffectEnum.Influence, 0.5f, 1f, 1.5f)
                });
                return true;
            }
        }
    }
    */
}
