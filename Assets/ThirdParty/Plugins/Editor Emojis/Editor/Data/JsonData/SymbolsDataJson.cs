using System;
namespace Anvil.EditorEmojis
{
    [Serializable]
    public class SymbolsDataJson
    {
        public SymbolsGroupJson[] groups;
    }

    [Serializable]
    public class SymbolsGroupJson
    {
        public string name;
        public SymbolJson[] icons;
    }

    [Serializable]
    public class SymbolJson
    {
        public string name;
        public string imagename;
    }
}