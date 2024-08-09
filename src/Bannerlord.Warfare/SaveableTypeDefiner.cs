using System.Collections.Generic;

using Warfare.Content.Contracts;
using Warfare.Content.Strategies;

namespace Warfare
{
    internal class SaveableTypeDefiner : TaleWorlds.SaveSystem.SaveableTypeDefiner
    {
        public SaveableTypeDefiner() : base((0x68a3cf << 8) | 123) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(Contract), 1);
            AddClassDefinition(typeof(ContractManager), 2);
            //AddClassDefinition(typeof(Prisoner), 3);
            //AddClassDefinition(typeof(PrisonerManager), 4);
            AddClassDefinition(typeof(Strategy), 5);
            AddClassDefinition(typeof(StrategyManager), 6);
            AddClassDefinition(typeof(CohesionBoost), 7);
            AddClassDefinition(typeof(CohesionBoostManager), 8);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<Contract>));
            ConstructContainerDefinition(typeof(List<Strategy>));
            ConstructContainerDefinition(typeof(List<CohesionBoost>));
        }
    }
}
