using System.Collections.Generic;

using TaleWorlds.SaveSystem;

using Warfare.Contracts;

namespace Warfare
{
    internal class WarfareSaveableTypeDefiner : SaveableTypeDefiner
    {
        public WarfareSaveableTypeDefiner() : base((0x68a3cf << 8) | 123) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(Contract), 1);
            AddClassDefinition(typeof(ContractManager), 2);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(List<Contract>));
        }
    }
}
