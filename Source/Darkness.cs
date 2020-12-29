using RimWorld;
using UnityEngine;
using Verse;

namespace RimDungeon
{
	public class GameCondition_NoSunlight : GameCondition
	{
		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06003D95 RID: 15765 RVA: 0x0010028C File Offset: 0x000FE48C
		public override int TransitionTicks
		{
			get
			{
				return 200;
			}
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x0014521E File Offset: 0x0014341E
		public override float SkyTargetLerpFactor(Map map)
		{
			return GameConditionUtility.LerpInOutValue(this, (float)this.TransitionTicks, 1f);
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x0014588C File Offset: 0x00143A8C
		public override SkyTarget? SkyTarget(Map map)
		{
			return new SkyTarget?(new SkyTarget(0f, this.EclipseSkyColors, 1f, 0f));
		}

		// Token: 0x040024EA RID: 9450
		private SkyColorSet EclipseSkyColors = new SkyColorSet(new Color(0.037f, 0.041f, 0.048f), new Color(0.437f, 0.441f, 0.448f), new Color(1f, 1f, 1f), 1.25f);
	}
}