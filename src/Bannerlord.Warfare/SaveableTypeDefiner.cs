using System.Collections.Generic;

using Warfare.Content.Contracts;

namespace Warfare
{
    internal class SaveableTypeDefiner : TaleWorlds.SaveSystem.SaveableTypeDefiner
    {
        public SaveableTypeDefiner() : base((0x68a3cf << 8) | 123) { }

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
