using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using Unity.VisualScripting;
using UnityEngine;

namespace CherryHypnoGatlingBlover.MelonLoader
{
	[RegisterTypeInIl2Cpp]
	public class CherryHypnoGatlingBlover : MonoBehaviour
	{
		public Shooter plant
		{
			get
			{
				return base.gameObject.GetComponent<Shooter>();
			}
		}

		public static int Buff1 { get; set; } = -1;

		public static int Buff2 { get; set; } = -1;

		public void Awake()
		{
			this.plant.shoot = this.plant.gameObject.transform.GetChild(0).FindChild("Shoot");
		}

		public void MyAnimShoot()
		{
			Board.Instance.GetComponent<CreateBullet>().SetBullet(this.plant.shoot.position.x + 0.1f, this.plant.shoot.position.y, this.plant.thePlantRow, (BulletType)965, 0, false).Damage = this.plant.attackDamage;
			this.MyUpdateInterval();
		}

		public void MyUpdateInterval()
		{
			this.plant.thePlantAttackInterval = 0.1f;
			float thePlantAttackInterval = this.plant.thePlantAttackInterval;
			foreach (Plant plant in Lawnf.Get1x1Plants(this.plant.thePlantColumn, this.plant.thePlantRow))
			{
				if (plant.thePlantType != (PlantType)965 && plant.thePlantAttackInterval < thePlantAttackInterval)
				{
					thePlantAttackInterval = plant.thePlantAttackInterval;
				}
			}
			this.plant.thePlantAttackCountDown = thePlantAttackInterval;
		}

		// Token: 0x04000003 RID: 3
		public static int PlantID = 965;

		// Token: 0x04000004 RID: 4
		public static int BulletID = 965;

		// Token: 0x02000007 RID: 7
		[HarmonyPatch(typeof(Plant), "Die")]
		public class CherryHypnoGatlingBlover_Die
		{
			[HarmonyPrefix]
			private static bool Prefix(Plant __instance, Plant.DieReason reason)
			{
				if (__instance.thePlantType == (PlantType)965 && reason == (Plant.DieReason)7)
				{
					Lawnf.SetDroppedCard(__instance.transform.position, (PlantType)1176, 0);
					Lawnf.SetDroppedCard(__instance.transform.position, (PlantType)939, 0);
				}
				return true;
			}
		}

		// Token: 0x02000008 RID: 8
		[HarmonyPatch(typeof(CreatePlant), "SetPlant")]
		public class CherryHypnoGatlingBlover_CreatePlant
		{
			[HarmonyPrefix]
			private static bool Prefix(CreatePlant __instance, int newColumn, int newRow, PlantType theSeedType)
			{
				if (theSeedType == (PlantType)965 && Board.Instance.GameObject().transform.GetComponentsInChildren<CherryHypnoGatlingBlover>().Length > Board.Instance.rowNum * 999 - 1)
				{
					InGameText.Instance.ShowText("种植数量达到上限", 5f, false);
					return false;
				}
				return true;
			}
		}
	}
}
