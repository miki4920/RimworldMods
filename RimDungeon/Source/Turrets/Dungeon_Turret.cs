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
        bool changedShell = false;
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
            CompChangeableProjectile projectile = null;
            int burst_count = 1;
            if (this.gun != null)
            {
                projectile = this.gun.TryGetComp<CompChangeableProjectile>();
                burst_count = this.AttackVerb.verbProps.burstShotCount;
            }
            if (secondaryGun)
            {
                this.gun = ThingMaker.MakeThing(TurretDef.secondaryGun, null);
            }
            else
            {
                this.gun = ThingMaker.MakeThing(this.def.building.turretGunDef, null);

            }

            this.UpdateGunVerbs();
            if (projectile != null && projectile.Loaded && projectile.LoadedShell != this.AttackVerb.verbProps.defaultProjectile)
            {
                ThingDef shell = projectile.LoadedShell;
                shell.projectileWhenLoaded = this.AttackVerb.verbProps.defaultProjectile;
                this.gun.TryGetComp<CompChangeableProjectile>().LoadShell(shell, this.AttackVerb.verbProps.burstShotCount);
            }
            this.burstCooldownTicksLeft = this.BurstCooldownTime().SecondsToTicks();
        }
        
        public override void Tick()
        {
            base.Tick();
            CompChangeableProjectile projectile = this.gun.TryGetComp<CompChangeableProjectile>();
            int burst_count = this.AttackVerb.verbProps.burstShotCount;
            if (!changedShell && projectile.Loaded && projectile.LoadedShell != this.AttackVerb.verbProps.defaultProjectile)
            {
                changedShell = true;
                ThingDef shell = projectile.LoadedShell;
                shell.projectileWhenLoaded = this.AttackVerb.verbProps.defaultProjectile;
                this.gun.TryGetComp<CompChangeableProjectile>().LoadShell(shell, this.AttackVerb.verbProps.burstShotCount);
            }
            else if(!projectile.Loaded)
            {
                changedShell = false;
            }
            {

            }
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                if(!gizmo.ToString().Contains("Extract"))
                {
                    yield return gizmo;
                }
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
