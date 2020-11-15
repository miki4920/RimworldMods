using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Rimdungeon.Traps
{
	public abstract class Dungeon_Trap_Framework : Building
	{
		public Dungeon_Trap_Def trap_def => base.def.GetModExtension<Dungeon_Trap_Def>();
		// Token: 0x17000DCA RID: 3530
		// (get) Token: 0x06004EF5 RID: 20213 RVA: 0x001AACCA File Offset: 0x001A8ECA
		private bool CanSetAutoRearm
		{
			get
			{
				return base.Faction == Faction.OfPlayer && this.def.blueprintDef != null && this.def.IsResearchFinished && trap_def.canAutoRearm;
			}
		}

		// Token: 0x06004EF6 RID: 20214 RVA: 0x001AACF3 File Offset: 0x001A8EF3
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.autoRearm, "autoRearm", false, false);
			Scribe_Collections.Look<Pawn>(ref this.touchingPawns, "testees", LookMode.Reference, Array.Empty<object>());
		}

		// Token: 0x06004EF7 RID: 20215 RVA: 0x001AAD23 File Offset: 0x001A8F23
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				this.autoRearm = (this.CanSetAutoRearm && map.areaManager.Home[base.Position]);
			}
		}

		// Token: 0x06004EF8 RID: 20216 RVA: 0x001AAD58 File Offset: 0x001A8F58
		public override void Tick()
		{
			if (base.Spawned)
			{
				List<Thing> thingList = base.Position.GetThingList(base.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Pawn pawn = thingList[i] as Pawn;
					if (pawn != null && !this.touchingPawns.Contains(pawn))
					{
						this.touchingPawns.Add(pawn);
						this.CheckSpring(pawn);
					}
				}
				for (int j = 0; j < this.touchingPawns.Count; j++)
				{
					Pawn pawn2 = this.touchingPawns[j];
					if (!pawn2.Spawned || pawn2.Position != base.Position)
					{
						this.touchingPawns.Remove(pawn2);
					}
				}
			}
			base.Tick();
		}

		// Token: 0x06004EF9 RID: 20217 RVA: 0x001AAE1C File Offset: 0x001A901C
		private void CheckSpring(Pawn p)
		{
			if (Rand.Chance(this.SpringChance(p)))
			{
				Map map = base.Map;
				this.Spring(p);
				if (p.Faction == Faction.OfPlayer || p.HostFaction == Faction.OfPlayer)
				{
					Find.LetterStack.ReceiveLetter("LetterFriendlyTrapSprungLabel".Translate(p.LabelShort, p).CapitalizeFirst(), "LetterFriendlyTrapSprung".Translate(p.LabelShort, p).CapitalizeFirst(), LetterDefOf.NegativeEvent, new TargetInfo(base.Position, map, false), null, null, null, null);
				}
			}
		}

		// Token: 0x06004EFA RID: 20218 RVA: 0x001AAED0 File Offset: 0x001A90D0
		protected virtual float SpringChance(Pawn p)
		{
			float num = 1f;
			if (this.KnowsOfTrap(p))
			{
				if (p.Faction == null)
				{
					if (p.RaceProps.Animal)
					{
						num = 0.2f;
						num *= this.def.building.trapPeacefulWildAnimalsSpringChanceFactor;
					}
					else
					{
						num = 0.3f;
					}
				}
				else if (p.Faction == base.Faction)
				{
					num = 0.005f;
				}
				else
				{
					num = 0f;
				}
			}
			num *= this.GetStatValue(StatDefOf.TrapSpringChance, true) * p.GetStatValue(StatDefOf.PawnTrapSpringChance, true);
			return Mathf.Clamp01(num);
		}

		// Token: 0x06004EFB RID: 20219 RVA: 0x001AAF64 File Offset: 0x001A9164
		public bool KnowsOfTrap(Pawn p)
		{
			return (p.Faction != null && !p.Faction.HostileTo(base.Faction)) || (p.Faction == null && p.RaceProps.Animal && !p.InAggroMentalState) || (p.guest != null && p.guest.Released) || (!p.IsPrisoner && base.Faction != null && p.HostFaction == base.Faction) || (p.RaceProps.Humanlike && p.IsFormingCaravan()) || (p.IsPrisoner && p.guest.ShouldWaitInsteadOfEscaping && base.Faction == p.HostFaction) || (p.Faction == null && p.RaceProps.Humanlike);
		}

		// Token: 0x06004EFC RID: 20220 RVA: 0x001AB038 File Offset: 0x001A9238
		public override ushort PathFindCostFor(Pawn p)
		{
			if (!this.KnowsOfTrap(p))
			{
				return 0;
			}
			return 800;
		}

		// Token: 0x06004EFD RID: 20221 RVA: 0x001AB04A File Offset: 0x001A924A
		public override ushort PathWalkCostFor(Pawn p)
		{
			if (!this.KnowsOfTrap(p))
			{
				return 0;
			}
			return 40;
		}

		// Token: 0x06004EFE RID: 20222 RVA: 0x001AB059 File Offset: 0x001A9259
		public override bool IsDangerousFor(Pawn p)
		{
			return this.KnowsOfTrap(p);
		}

		// Token: 0x06004EFF RID: 20223 RVA: 0x001AB064 File Offset: 0x001A9264
		public void Spring(Pawn p)
		{
			bool spawned = base.Spawned;
			Map map = base.Map;
			this.SpringSub(p);
			if (this.def.building.trapDestroyOnSpring)
			{
				if (!base.Destroyed)
				{
					this.Destroy(DestroyMode.Vanish);
				}
				if (spawned)
				{
					this.CheckAutoRebuild(map);
				}
			}
		}

		// Token: 0x06004F00 RID: 20224 RVA: 0x001AB0B4 File Offset: 0x001A92B4
		public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
		{
			bool spawned = base.Spawned;
			Map map = base.Map;
			base.Kill(dinfo, exactCulprit);
			if (spawned)
			{
				this.CheckAutoRebuild(map);
			}
		}

		// Token: 0x06004F01 RID: 20225
		protected abstract void SpringSub(Pawn p);

		// Token: 0x06004F02 RID: 20226 RVA: 0x001AB0E0 File Offset: 0x001A92E0
		private void CheckAutoRebuild(Map map)
		{
			if (this.autoRearm && this.CanSetAutoRearm && map != null && GenConstruct.CanPlaceBlueprintAt(this.def, base.Position, base.Rotation, map, false, null, null, base.Stuff).Accepted)
			{
				GenConstruct.PlaceBlueprintForBuild(this.def, base.Position, map, base.Rotation, Faction.OfPlayer, base.Stuff);
			}
		}

		// Token: 0x06004F03 RID: 20227 RVA: 0x001AB14F File Offset: 0x001A934F
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			if (this.CanSetAutoRearm)
			{
				yield return new Command_Toggle
				{
					defaultLabel = "CommandAutoRearm".Translate(),
					defaultDesc = "CommandAutoRearmDesc".Translate(),
					hotKey = KeyBindingDefOf.Misc3,
					icon = TexCommand.RearmTrap,
					isActive = (() => this.autoRearm),
					toggleAction = delegate ()
					{
						this.autoRearm = !this.autoRearm;
					}
				};
			}
			yield break;
			yield break;
		}

		// Token: 0x04002CE5 RID: 11493
		private bool autoRearm;

		// Token: 0x04002CE6 RID: 11494
		private List<Pawn> touchingPawns = new List<Pawn>();

		// Token: 0x04002CE7 RID: 11495
		private const float KnowerSpringChanceFactorSameFaction = 0.005f;

		// Token: 0x04002CE8 RID: 11496
		private const float KnowerSpringChanceFactorWildAnimal = 0.2f;

		// Token: 0x04002CE9 RID: 11497
		private const float KnowerSpringChanceFactorFactionlessHuman = 0.3f;

		// Token: 0x04002CEA RID: 11498
		private const float KnowerSpringChanceFactorOther = 0f;

		// Token: 0x04002CEB RID: 11499
		private const ushort KnowerPathFindCost = 800;

		// Token: 0x04002CEC RID: 11500
		private const ushort KnowerPathWalkCost = 40;
	}
}