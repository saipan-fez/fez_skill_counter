using System.Collections.Generic;

namespace SkillUseCounter.Entity
{
    internal class Map
    {
        private const string UnknownMapName = "Unknown";

        public static Map Empty => new Map(UnknownMapName);

        public string Name { get; }

        public Map(string name)
        {
            Name = name;
        }

        public bool IsEmpty()
        {
            return Name == UnknownMapName;
        }

        public override bool Equals(object obj)
        {
            var map = obj as Map;
            return map != null &&
                   Name == map.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Map map1, Map map2)
        {
            return map1.Name == map2.Name;
        }

        public static bool operator !=(Map map1, Map map2)
        {
            return map1.Name != map2.Name;
        }
    }
}
