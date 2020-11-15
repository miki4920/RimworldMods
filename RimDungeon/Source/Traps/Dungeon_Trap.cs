using RimWorld;
using System;
using Verse;
using Verse.Sound;


namespace Rimdungeon.Traps
{
    class Dungeon_Trap : Dungeon_Trap_Framework
    {
		// Token: 0x06004F0A RID: 20234 RVA: 0x001AB193 File Offset: 0x001A9393
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				SoundDefOf.TrapArm.PlayOneShot(new TargetInfo(base.Position, map, false));
			}
		}

		// Token: 0x06004F0B RID: 20235 RVA: 0x001AB1BC File Offset: 0x001A93BC
		protected override void SpringSub(Pawn p)
		{
			SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			if (p == null)
			{
				return;
			}
			float num = this.GetStatValue(StatDefOf.TrapMeleeDamage, true) * DamageRandomFactorRange.RandomInRange / this.trap_def.numberOfAttacks;
			float armorPenetration = num * 0.015f;
			int num2 = 0;
			while ((float)num2 < this.trap_def.numberOfAttacks)
			{
				DamageInfo dinfo = new DamageInfo(DamageDefOf.Stab, num, armorPenetration, -1f, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
				DamageWorker.DamageResult damageResult = p.TakeDamage(dinfo);
				if (num2 == 0)
				{
					BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(p, RulePackDefOf.DamageEvent_TrapSpike, null);
					Find.BattleLog.Add(battleLogEntry_DamageTaken);
					damageResult.AssociateWithLog(battleLogEntry_DamageTaken);
				}
				num2++;
			}
		}

		// Token: 0x04002CED RID: 11501
		private static readonly FloatRange DamageRandomFactorRange = new FloatRange(0.8f, 1.2f);
	}
}
