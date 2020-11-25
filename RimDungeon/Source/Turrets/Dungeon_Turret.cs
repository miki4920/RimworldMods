using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimdungeon.Turrets
{
    public class Dungeon_Turret : Building_TurretGun
    {
        public Dungeon_Turret_Def TurretDef => base.def.GetModExtension<Dungeon_Turret_Def>();
        bool secondaryGun = false;
        public override void PostMake()
        {
            base.PostMake();
            this.DetermineGun();

        }
        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = this.gun.TryGetComp<CompEquippable>().AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = this;
                verb.castCompleteCallback = new Action(this.BurstComplete);
            }
        }
        public void DetermineGun()
        {
            if (secondaryGun)
            {
                this.gun = ThingMaker.MakeThing(TurretDef.secondaryGun, null);
            }
            else
            {
                this.gun = ThingMaker.MakeThing(this.def.building.turretGunDef, null);
                
            }
            this.UpdateGunVerbs();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            if (!(TurretDef.secondaryGun is null))
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "SecondGun".Translate(),
                    defaultDesc = "SecondGunDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire", true),
                    toggleAction = delegate ()
                    {
                        secondaryGun = !secondaryGun;
                        DetermineGun();
                    },
                    isActive = (() => secondaryGun)
                };
            }
            yield break;

        }
    }
}
