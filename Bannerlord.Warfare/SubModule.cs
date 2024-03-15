using System;
using System.IO;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

using Bannerlord.UIExtenderEx;
using HarmonyLib;

using Warfare.Behaviors;
using Warfare.Models;

namespace Warfare
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var extender = new UIExtender("bannerlord.warfare");
            extender.Register(typeof(SubModule).Assembly);
            extender.Enable();

            var harmony = new Harmony("bannerlord.warfare.patches");
            harmony.PatchAll();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject is CampaignGameStarter)
            {
                CampaignGameStarter starter = (CampaignGameStarter)gameStarterObject;
                starter.AddBehavior(new ContractBehavior());
                starter.AddBehavior(new MercenaryBehavior());
                starter.AddModel(new WarfareArmyManagementCalculationModel(GetModel<ArmyManagementCalculationModel>(starter)));
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
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord", "Logs");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            string path = Path.Combine(text, "Warfare.txt");
            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message);
            }
        }
    }
}