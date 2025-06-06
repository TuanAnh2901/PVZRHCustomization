using System;
using System.Runtime.CompilerServices;
using CassidyCustomLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace CassidysGarden.Obsidianzilla
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterTypeInIl2Cpp]
	public class Obsidianzilla : MonoBehaviour
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000023C4 File Offset: 0x000005C4
		public void Awake()
		{
			this.plant.DisableMixPlants();
			this.plant.SetAttackRange();
			this.plant.shoot = base.transform.FindChild("Shoot");
			this.hpRegenCountdown = 5f;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002414 File Offset: 0x00000614
		public void Update()
		{
			bool flag = this.plant != null;
			if (flag)
			{
				bool flag2 = this.plant.targetZombie == null;
				if (flag2)
				{
					this.plant.ChomperSearchZombie(null);
				}
				this.hpRegenCountdown -= Time.deltaTime * GameAPP.gameSpeed;
				bool flag3 = this.hpRegenCountdown <= 0f;
				if (flag3)
				{
					this.RegenHP();
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002490 File Offset: 0x00000690
		public void RegenHP()
		{
			this.hpRegenCountdown = 5f;
			this.plant.Recover(32000 + ((this.plant.thePlantHealth > this.plant.thePlantMaxHealth) ? 1 : 0) * 32000);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000209B File Offset: 0x0000029B
		public UltimateChomper plant
		{
			get
			{
				return base.gameObject.GetComponent<UltimateChomper>();
			}
		}

		// Token: 0x04000003 RID: 3
		public float hpRegenCountdown = 5f;
	}
}
