using Bannerlord.UIExtenderEx;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using Warfare.Behaviors;
using Warfare.Models;

namespace Warfare
{
    public class SubModule : MBSubModuleBase
    {
        public static bool Diplomacy = false;
        public static bool NavalDLC = false;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            Diplomacy = Utilities.GetModulesNames().Contains("Bannerlord.Diplomacy");
            NavalDLC = Utilities.GetModulesNames().Contains("NavalDLC");

            var extender = UIExtender.Create("bannerlord.warfare");
            extender.Register(typeof(SubModule).Assembly);
            extender.Enable();

            var harmony = new Harmony("bannerlord.warfare.patches");
            harmony.PatchAll();

        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter starter)
            {
                starter.AddBehavior(new ContractBehavior());
                starter.AddBehavior(new CohesionBoostBehavior());
                starter.AddBehavior(new MercenaryBehavior());
                //starter.AddBehavior(new PrisonerBehavior());
                starter.AddBehavior(new StrategyBehavior());
                starter.AddModel(new WarfareArmyManagementCalculationModel(GetModel<ArmyManagementCalculationModel>(starter)));
                starter.AddModel(new WarfareTargetScoreCalculatingModel(GetModel<TargetScoreCalculatingModel>(starter)));
            }
        }

        private T GetModel<T>(CampaignGameStarter starter)
        {
            foreach (GameModel model in starter.Models)
            {
                if (model is T gameModel)
                {
                    return gameModel;
                }
            }
            return default!;
        }

        public static void Log(string message)
        {
            string text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Logs");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            string path = System.IO.Path.Combine(text, "Warfare.txt");
            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message);
            }
        }
    }
}