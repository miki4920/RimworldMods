using Verse;

namespace Rimdungeon.Traps
{
    public class Dungeon_Trap_Def : DefModExtension
    {
        public bool canAutoRearm = true;
        public int numberOfAttacks = 5;
        public float sameFactionSpringChance = 0.005f;
        public float wildAnimalSpringChance = 0.2f;
        public float noFactionSpringChance = 0.3f;
        public ushort pathFindCost = 800;
        public ushort pathWalkCost = 40;
    }
}
