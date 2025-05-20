using System;
using HarmonyLib;
using Il2Cpp;
using SelectCustomPlants;

// Token: 0x02000006 RID: 6
[HarmonyPatch(typeof(UIButton), "OnMouseUpAsButton")]
public class HideCustomPlantCards
{
    // Token: 0x0600000B RID: 11 RVA: 0x000025D5 File Offset: 0x000007D5
    [HarmonyPostfix]
    private static void Postfix()
    {
        //SelectCustomPlants.MelonLoader.SelectCustomPlants.CloseCustomPlantCards();
        
    }
}
