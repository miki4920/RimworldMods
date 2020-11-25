using Verse;

namespace Rimdungeon.Traps
{
    public class Dungeon_Trap_Def : DefModExtension
    {
        public int numberOfAttacks = 5;
        public float armorPenetration = 0.015f;

        public float sameFactionSpringChance = 0.005f;
        public float wildAnimalSpringChance = 0.2f;
        public float noFactionSpringChance = 0.3f;

        public int pathFindCost = 800;
        public int pathWalkCost = 40;

        public bool rearmable = false;
        public GraphicData trapUnarmedGraphicData;

        public bool slows = false;

        public bool rebuildable = false;

        public float addHediffChance = 0.05f;
        public HediffDef hediffToAdd = null;

    }
}
