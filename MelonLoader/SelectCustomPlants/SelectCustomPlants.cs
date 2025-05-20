using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CustomizeLib.MelonLoader;
using CassidyCustomLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

	[assembly: MelonInfo(typeof(SelectCustomPlants.SelectCustomPlants), "SelectCustomPlants", "1.0", "JustNull", null)]
	[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
	[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SelectCustomPlants
{
	public class SelectCustomPlants : MelonMod
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
		public override void OnUpdate()
		{
			if (Input.GetMouseButtonDown(0) && SelectCustomPlants.MyShowCustomPlantsButton != null)
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject == SelectCustomPlants.MyShowCustomPlantsButton)
				{
					SelectCustomPlants.OpenCustomPlantCards();
				}
			}
			if (SelectCustomPlants.MyShowCustomPlantsButton != null)
			{
				RaycastHit2D raycastHit2D2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (raycastHit2D2.collider != null && raycastHit2D2.collider.gameObject == SelectCustomPlants.MyShowCustomPlantsButton)
				{
					CursorChange.SetClickCursor();
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002140 File Offset: 0x00000340
		public static void InitCustomCards()
		{
			SelectCustomPlants.MyShowCustomPlantsButton = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("ui/prefabs/InGameUI").transform.FindChild("Bottom/SeedLibrary/ShowNormal").gameObject, InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary"));
			SelectCustomPlants.MyShowCustomPlantsButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(580f, -230f);
			SelectCustomPlants.MyShowCustomPlantsButton.GetComponent<RectTransform>().position = new Vector3(SelectCustomPlants.MyShowCustomPlantsButton.GetComponent<RectTransform>().position.x, SelectCustomPlants.MyShowCustomPlantsButton.GetComponent<RectTransform>().position.y, InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary/ShowNormal").position.z);
			SelectCustomPlants.MyShowCustomPlantsButton.SetActive(true);
			if (SelectCustomPlants.MyShowCustomPlantsButton.GetComponent<UIButton>() != null)
			{
                UnityEngine.Object.Destroy(SelectCustomPlants.MyShowCustomPlantsButton.GetComponent<UIButton>());
			}
			for (int i = 0; i < SelectCustomPlants.MyShowCustomPlantsButton.transform.childCount; i++)
			{
				SelectCustomPlants.MyShowCustomPlantsButton.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().m_text = "Test";
				SelectCustomPlants.MyShowCustomPlantsButton.transform.GetChild(i).gameObject.SetActive(true);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000229C File Offset: 0x0000049C
		public static void OpenCustomPlantCards()
		{
			InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary/Grid/Pages").gameObject.SetActive(false);
			InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary/Grid/ColorfulCards").gameObject.SetActive(false);
			if (SelectCustomPlants.MyPageParent != null)
			{
				SelectCustomPlants.MyPageParent.SetActive(true);
				return;
			}
			SelectCustomPlants.MyPageParent = UnityEngine.Object.Instantiate<GameObject>(InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary/Grid/ColorfulCards").gameObject.gameObject, InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary/Grid"));
			SelectCustomPlants.MyPageParent.gameObject.SetActive(true);
			GameObject gameObject = SelectCustomPlants.MyPageParent.transform.GetChild(0).gameObject;
			gameObject.gameObject.SetActive(true);
			GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
			gameObject2.gameObject.SetActive(false);
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				if (i != 0)
				{
                    UnityEngine.Object.Destroy(gameObject.transform.GetChild(i).gameObject);
				}
			}
			List<PlantType> list = new List<PlantType>();
			// foreach (PlantType plantType in CassidyCore.CustomPlantTypes)
			// {
			// 	if (Enum.IsDefined(typeof(PlantType), plantType) && PlantDataLoader.plantDatas[plantType] != null)
			// 	{
			// 		list.Add(plantType);
			// 	}
			// }
			foreach (PlantType plantType in CustomCore.CustomPlantTypes)
			{
				if (!Enum.IsDefined(typeof(PlantType), plantType) && PlantDataLoader.plantDatas[plantType] != null)
				{
					list.Add(plantType);
					
					
				}
			}

            // if CassidyCore
			if (CassidyCore.CustomPlantTypes.Count > 0)
            {
                list.Add((PlantType)944);
            }


            for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject2);
				if (gameObject3 != null)
				{
					gameObject3.transform.SetParent(gameObject.transform);
					gameObject3.SetActive(true);
					gameObject3.transform.position = gameObject2.transform.position;
					gameObject3.transform.localPosition = gameObject2.transform.localPosition;
					gameObject3.transform.localScale = gameObject2.transform.localScale;
					gameObject3.transform.localRotation = gameObject2.transform.localRotation;
					gameObject3.transform.GetChild(0).gameObject.SetActive(false);
					CardUI component = gameObject3.transform.GetChild(1).GetComponent<CardUI>();
					component.gameObject.SetActive(true);
					Mouse.Instance.ChangeCardSprite(list[j], component);
					component.thePlantType = list[j];
					component.theSeedType = (int)(Il2Cpp.PlantType)list[j];
					component.theSeedCost = PlantDataLoader.plantDatas[list[j]].field_Public_Int32_1;
					component.fullCD = PlantDataLoader.plantDatas[list[j]].field_Public_Single_2;
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000025A4 File Offset: 0x000007A4
		public static void CloseCustomPlantCards()
		{
			if (SelectCustomPlants.MyPageParent != null)
			{
				SelectCustomPlants.MyPageParent.SetActive(false);
			}
		}


        public static GameObject? MyShowCustomPlantsButton;

		public static GameObject? MyPageParent;

		// Token: 0x02000005 RID: 5
		[HarmonyPatch(typeof(Board), "Start")]
		public class ShowCustomPlantCards
		{
			// Token: 0x06000009 RID: 9 RVA: 0x000025C6 File Offset: 0x000007C6
			[HarmonyPostfix]
			private static void Postfix()
			{
				SelectCustomPlants.InitCustomCards();
			}
		}

		// Token: 0x02000006 RID: 6
		[HarmonyPatch(typeof(UIButton), "OnMouseUpAsButton")]
		public class HideCustomPlantCards
		{
			// Token: 0x0600000B RID: 11 RVA: 0x000025D5 File Offset: 0x000007D5
			[HarmonyPostfix]
			private static void Postfix()
			{
				SelectCustomPlants.CloseCustomPlantCards();
			}
		}
	}
}
