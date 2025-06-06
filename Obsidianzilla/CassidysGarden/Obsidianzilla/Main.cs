using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CassidyCustomLib;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace CassidysGarden.Obsidianzilla
{
	// Token: 0x02000005 RID: 5
	public class Main : MelonMod
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000020BC File Offset: 0x000002BC
		public override void OnInitializeMelon()
		{
			AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync("Mods/Cassidy's Garden/obsidianzilla");
			bool flag = assetBundleCreateRequest == null;
			if (flag)
			{
				MelonLogger.Msg("Null Obsidianzilla bundle!");
			}
			else
			{
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				foreach (Object @object in assetBundleCreateRequest.assetBundle.LoadAllAssets())
				{
					GameObject gameObject3 = @object.TryCast<GameObject>();
					bool flag2 = gameObject3 != null;
					if (flag2)
					{
						bool flag3 = gameObject3.name == "ObsidianChomperPrefab";
						if (flag3)
						{
							gameObject = gameObject3.Cast<GameObject>();
						}
						else
						{
							bool flag4 = gameObject3.name == "ObsidianChomperPreview";
							if (flag4)
							{
								gameObject2 = gameObject3.Cast<GameObject>();
							}
						}
					}
				}
				bool flag5 = gameObject != null && gameObject2 != null;
				if (flag5)
				{
					CassidyCore.AddCustomPlant<UltimateChomper, Obsidianzilla>(944, gameObject, gameObject2, new List<ValueTuple<int, int>>
					{
						new ValueTuple<int, int>(903, 913),
						new ValueTuple<int, int>(913, 903)
					}, 0f, 0f, 1000, 32000, 7.5f, 300);
					CassidyCore.TypeMgrExtra.IsNut.Add(944);
					CassidyCore.TypeMgrExtra.IsTallNut.Add(944);
				}
			}
		}

		// Token: 0x02000006 RID: 6
		[HarmonyPatch(typeof(UltimateChomper))]
		public static class ObsidianChomper_Patch
		{
			// Token: 0x06000007 RID: 7 RVA: 0x00002234 File Offset: 0x00000434
			[NullableContext(1)]
			[HarmonyPatch("TakeDamage")]
			[HarmonyPrefix]
			public static bool TakeDamage(UltimateChomper __instance, ref int damage, ref int damageType)
			{
				bool flag = __instance.thePlantType == 943;
				bool result;
				if (flag)
				{
					int num = 0;
					foreach (PlantStatistics plantStatistics in Board.Instance.plantStatistics)
					{
						bool flag2 = plantStatistics.thePlantType == 944;
						if (flag2)
						{
							num = plantStatistics.useTimes;
						}
					}
					int num2 = (int)Math.Floor(Math.Min(damage, 200) / num);
					foreach (Plant plant in Board.Instance.plantArray)
					{
						bool flag3 = plant.thePlantType == 944 && plant != null;
						if (flag3)
						{
							plant.thePlantHealth -= num2;
							plant.FlashOnce();
						}
					}
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			}
		}

		// Token: 0x02000007 RID: 7
		[HarmonyPatch(typeof(Plant))]
		public static class Plant_Patch
		{
			// Token: 0x06000008 RID: 8 RVA: 0x00002328 File Offset: 0x00000528
			[NullableContext(1)]
			[HarmonyPatch("Crashed")]
			[HarmonyPrefix]
			public static bool Crashed(Plant __instance, ref int level, ref int soundID, [Optional] ref Zombie zombie)
			{
				bool flag = __instance.thePlantType == 944;
				bool result;
				if (flag)
				{
					bool flag2 = zombie != null;
					if (flag2)
					{
						zombie.transform.position = new Vector3(zombie.transform.position.x + 0.8f, zombie.transform.position.y, zombie.transform.position.z);
						GameAPP.PlaySound(72, 0.5f, 1f);
					}
					__instance.isCrashed = false;
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			}
		}
	}
}
