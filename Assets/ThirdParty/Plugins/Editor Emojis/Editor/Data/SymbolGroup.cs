using System.Collections.Generic;
namespace Anvil.EditorEmojis
{
    internal class SymbolGroup
    {
        internal string GroupName { get; set; }
        internal List<Symbol> Icons { get; set; }

        // Add a parameterless constructor for dynamic creation
        internal SymbolGroup() { }

        // Add a constructor to initialize with group name and icons
        internal SymbolGroup(string groupName, List<Symbol> icons)
        {
            GroupName = groupName;
            Icons = icons;
        }
    }
}
