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
using JetBrains.Annotations;

[assembly: MelonInfo(typeof(SelectCustomPlants.SelectCustomPlants), "SelectCustomPlants", "1.0", "JustNull", null)]
	[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
	[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SelectCustomPlants
{
	public class SelectCustomPlants : MelonMod
	{
		// List to track picked plant IDs
		public static List<PlantType> PickedPlantsList = new List<PlantType>();
		
		// Dictionary to track if a plant is available (not picked)
		public static Dictionary<PlantType, bool> PlantAvailability = new Dictionary<PlantType, bool>();
		// Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
		public override void OnUpdate()
		{
			// Check if game status changed to InGame and update picked plants list
			if (GameAPP.theGameStatus == (int)GameStatus.InGame)
			{
				// Check SeedBank for plants and update PickedPlantsList
				UpdatePickedPlantsList();
			}

			if (Input.GetMouseButtonDown(0) && SelectCustomPlants.MyShowCustomPlantsButton != null)
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject == SelectCustomPlants.MyShowCustomPlantsButton)
				{
					SelectCustomPlants.OpenCustomPlantCards();
				}
			}
			if (Input.GetMouseButtonDown(0) && SelectCustomPlants.MyNextPageButton != null)
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject == SelectCustomPlants.MyNextPageButton)
				{
					SelectCustomPlants.CurrentPage++;
					if (SelectCustomPlants.CurrentPage * SelectCustomPlants.CardsPerPage >= SelectCustomPlants.AllPlantsList.Count)
					{
						SelectCustomPlants.CurrentPage = 0; // Loop back to first page
					}
					SelectCustomPlants.DisplayCurrentPage();
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
			if (SelectCustomPlants.MyNextPageButton != null)
			{
				RaycastHit2D raycastHit2D2 = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (raycastHit2D2.collider != null && raycastHit2D2.collider.gameObject == SelectCustomPlants.MyNextPageButton)
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

			// Create Next Page button
			SelectCustomPlants.MyNextPageButton = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("ui/prefabs/InGameUI").transform.FindChild("Bottom/SeedLibrary/ShowNormal").gameObject, InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary"));
			SelectCustomPlants.MyNextPageButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(580f, -200f);
			SelectCustomPlants.MyNextPageButton.GetComponent<RectTransform>().position = new Vector3(SelectCustomPlants.MyNextPageButton.GetComponent<RectTransform>().position.x, SelectCustomPlants.MyNextPageButton.GetComponent<RectTransform>().position.y, InGameUI.Instance.SeedBank.transform.parent.FindChild("Bottom/SeedLibrary/ShowNormal").position.z);
			SelectCustomPlants.MyNextPageButton.SetActive(true);
			if (SelectCustomPlants.MyNextPageButton.GetComponent<UIButton>() != null)
			{
                UnityEngine.Object.Destroy(SelectCustomPlants.MyNextPageButton.GetComponent<UIButton>());
			}
			for (int i = 0; i < SelectCustomPlants.MyNextPageButton.transform.childCount; i++)
			{
				SelectCustomPlants.MyNextPageButton.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().m_text = "Next Page";
				SelectCustomPlants.MyNextPageButton.transform.GetChild(i).gameObject.SetActive(true);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000229C File Offset: 0x0000049C
		// Update the availability status of plants based on SeedBank
		public static void UpdatePickedPlantsList()
		{
			// Clear the list first to rebuild it
			PickedPlantsList.Clear();
			
			// Check each slot in the SeedBank
			if (InGameUI.Instance != null && InGameUI.Instance.SeedBank != null)
			{
				// Loop through all child objects of SeedBank to find CardUI components
				for (int i = 0; i < InGameUI.Instance.SeedBank.transform.childCount; i++)
				{
					Transform child = InGameUI.Instance.SeedBank.transform.GetChild(i);
					
					// Look for CardUI components in children
					CardUI[] cardUIs = child.GetComponentsInChildren<CardUI>(true);
					foreach (CardUI cardUI in cardUIs)
					{
						// Add the plant type to the picked list if it's not already there
						if (cardUI != null && !PickedPlantsList.Contains(cardUI.thePlantType))
						{
							PickedPlantsList.Add(cardUI.thePlantType);
						}
					}
				}
			}
			
			// Update availability status for all plants
			UpdatePlantAvailability();
		}
		
		// Update the availability status of all plants
		public static void UpdatePlantAvailability()
		{
			// For each plant in AllPlantsList, check if it's in PickedPlantsList
			foreach (PlantType plantType in AllPlantsList)
			{
				PlantAvailability[plantType] = !PickedPlantsList.Contains(plantType);
			}
		}

		public static void OpenCustomPlantCards()
		{
			// Update the picked plants list before opening cards
			UpdatePickedPlantsList();
			
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
			
			// Reset current page when opening cards
			SelectCustomPlants.CurrentPage = 0;
			
			// Collect all plant types
			SelectCustomPlants.AllPlantsList = new List<PlantType>();
			
			// foreach (PlantType plantType in CassidyCore.CustomPlantTypes)
			// {
			// 	if (Enum.IsDefined(typeof(PlantType), plantType) && PlantDataLoader.plantDatas[plantType] != null)
			// 	{
			// 		SelectCustomPlants.AllPlantsList.Add(plantType);
			// 	}
			// }
			foreach (PlantType plantType in CustomCore.CustomPlantTypes)
			{
				if (!Enum.IsDefined(typeof(PlantType), plantType) && PlantDataLoader.plantDatas[plantType] != null)
				{
					SelectCustomPlants.AllPlantsList.Add(plantType);
				}
			}

			// Add Game Ultimate Plants
			for (int ulti = 900; ulti < 944; ulti++)
			{
				if (PlantDataLoader.plantDatas[(PlantType)ulti] != null)
				{
					SelectCustomPlants.AllPlantsList.Add((PlantType)ulti);
					MelonLogger.Msg("Add Ultimate Plant: " + (PlantType)ulti);
				}
			}

			// if CassidyCore
			if (CassidyCore.CustomPlantTypes.Count > 0)
            {
                SelectCustomPlants.AllPlantsList.Add((PlantType)944);
            }

			// Update plant availability status
			UpdatePlantAvailability();
			
			// Display the first page
			DisplayCurrentPage();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000025A4 File Offset: 0x000007A4
		public static void CloseCustomPlantCards()
		{
			if (SelectCustomPlants.MyPageParent != null)
			{
				SelectCustomPlants.MyPageParent.SetActive(false);
			}
		}



		public static void DisplayCurrentPage()
		{
			if (SelectCustomPlants.MyPageParent == null) return;

			GameObject pageContainer = SelectCustomPlants.MyPageParent.transform.GetChild(0).gameObject;
			GameObject cardTemplate = pageContainer.transform.GetChild(0).gameObject;

			// Clear existing cards except the template
			for (int i = pageContainer.transform.childCount - 1; i > 0; i--)
			{
				UnityEngine.Object.Destroy(pageContainer.transform.GetChild(i).gameObject);
			}

			// Calculate start and end indices for current page
			int startIndex = CurrentPage * CardsPerPage;
			int endIndex = Math.Min(startIndex + CardsPerPage, AllPlantsList.Count);

			// Display cards for current page
			for (int j = startIndex; j < endIndex; j++)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(cardTemplate);
				if (gameObject3 != null)
				{
					gameObject3.transform.SetParent(pageContainer.transform);
					gameObject3.SetActive(true);
					gameObject3.transform.position = cardTemplate.transform.position;
					gameObject3.transform.localPosition = cardTemplate.transform.localPosition;
					gameObject3.transform.localScale = cardTemplate.transform.localScale;
					gameObject3.transform.localRotation = cardTemplate.transform.localRotation;
					gameObject3.transform.GetChild(0).gameObject.SetActive(false);
					CardUI component = gameObject3.transform.GetChild(1).GetComponent<CardUI>();
					component.gameObject.SetActive(true);
					
					// Get the plant type for this position
					PlantType plantType = AllPlantsList[j];
					
					// Check if this plant is available (not picked)
					bool isAvailable = !PlantAvailability.ContainsKey(plantType) || PlantAvailability[plantType];
					
					// Set up the card UI
					Mouse.Instance.ChangeCardSprite(plantType, component);
					component.thePlantType = plantType;
					component.theSeedType = (int)(Il2Cpp.PlantType)plantType;
					component.theSeedCost = PlantDataLoader.plantDatas[plantType].field_Public_Int32_1;
					component.fullCD = PlantDataLoader.plantDatas[plantType].field_Public_Single_2;
					
					// If the plant is not available (already picked), make it visually distinct
					if (!isAvailable)
					{
						// Make the card semi-transparent to indicate it's unavailable
						Color cardColor = component.gameObject.GetComponent<UnityEngine.UI.Image>().color;
						cardColor.a = 0.5f; // 50% opacity
						component.gameObject.GetComponent<UnityEngine.UI.Image>().color = cardColor;
						
						// Disable interaction with this card
						component.enabled = false;
					}
				}
			}
		}


        public static GameObject? MyShowCustomPlantsButton;

		public static GameObject? MyPageParent;

		public static GameObject? MyNextPageButton;

		public static int CurrentPage = 0;

		public static List<PlantType> AllPlantsList = new List<PlantType>();

		public static readonly int CardsPerPage = 54; // 9x6 cards per page

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
