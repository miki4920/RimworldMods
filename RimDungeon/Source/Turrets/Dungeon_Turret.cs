using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
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
            DetermineGun();

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
            this.burstCooldownTicksLeft = this.BurstCooldownTime().SecondsToTicks();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            if (!(TurretDef.secondaryGun is null) && !secondaryGun)
            {
                yield return new Command_Action
                {
                    defaultLabel = "SecondGun".Translate(),
                    defaultDesc = "SecondGunDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire", true),
                    action = delegate ()
                    {
                        secondaryGun = true;
                        DetermineGun();
                    },
                };
            }
            if (!(TurretDef.secondaryGun is null) && secondaryGun)
            {
                yield return new Command_Action
                {
                    defaultLabel = "FirstGun".Translate(),
                    defaultDesc = "FirstGunDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire", true),
                    action = delegate ()
                    {
                        secondaryGun = false;
                        DetermineGun();
                    },
                };
            }
            yield break;

        }
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string inspectString = base.GetInspectString();
            if (!inspectString.NullOrEmpty())
            {
                stringBuilder.AppendLine(inspectString);
            }
            if (!secondaryGun)
            {
                stringBuilder.AppendLine("FirstGunMode".Translate());
            }
            if (secondaryGun)
            {
                stringBuilder.AppendLine("SecondGunMode".Translate());
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref secondaryGun, "secondaryGun", false, false);
        }
    }
}
