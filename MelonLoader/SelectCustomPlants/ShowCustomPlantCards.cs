using System;
using HarmonyLib;
using Il2Cpp;
using SelectCustomPlants;

// Token: 0x02000005 RID: 5
[HarmonyPatch(typeof(Board), "Start")]
public class ShowCustomPlantCards
{
    // Token: 0x06000009 RID: 9 RVA: 0x000025C6 File Offset: 0x000007C6
    [HarmonyPostfix]
    private static void Postfix()
    {
        SelectCustomPlants.SelectCustomPlants.InitCustomCards();
    }
}
