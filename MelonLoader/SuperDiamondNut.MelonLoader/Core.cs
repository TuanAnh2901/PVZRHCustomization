﻿using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Common.XrefScans;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.XrefScans;
using MelonLoader;
using SuperDiamondNut.MelonLoader;
using System.Reflection;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(Core), "SuperDiamondNut", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SuperDiamondNut.MelonLoader
{
    [HarmonyPatch(typeof(SuperSunNut), "TakeDamage")]
    public static class SuperSunNutPatch
    {
        [HarmonyPrefix]
        public static unsafe bool PreTakeDamage(SuperSunNut __instance)
        {
            if (__instance.thePlantType is (PlantType)961)
            {
                var damage = Lawnf.TravelAdvanced(5) ? 1 : 5;
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 36, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 36, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 1, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn + 1, __instance.thePlantRow, 4, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn -1, __instance.thePlantRow, 6, 0);
                IL2CPP.Il2CppObjectBaseToPtrNotNull(__instance);
                IntPtr* ptr = stackalloc IntPtr[2];
                *ptr = (nint)(&damage);
                int i = 0;
                *(int**)((byte*)ptr + checked(1u * unchecked((nuint)sizeof(IntPtr)))) = &i;
                System.Runtime.CompilerServices.Unsafe.SkipInit(out IntPtr exc);
                IL2CPP.il2cpp_runtime_invoke((IntPtr)(typeof(Plant).GetField("NativeMethodInfoPtr_TakeDamage_Public_Virtual_New_Void_Int32_Int32_0", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null))!, IL2CPP.Il2CppObjectBaseToPtrNotNull(__instance), (void**)ptr, ref exc);
                Il2CppException.RaiseExceptionIfNecessary(exc);
                __instance.ReplaceSprite();
                return false;
            }
            else
            {
                return true;
            }
        }

        public static MethodInfo TakeDamage => typeof(Plant).GetMethod("TakeDamage")!;
    }

    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superdiamondnut");
            CustomCore.RegisterCustomPlant<SuperSunNut, SuperDiamondNut>(961, ab.GetAsset<GameObject>("SuperDiamondNutPrefab"),
                ab.GetAsset<GameObject>("SuperDiamondNutPreview"), [(905, 31)], 3, 0, 20, 4000, 7.5f, 150);
            CustomCore.RegisterCustomPlantClickEvent(961, SuperDiamondNut.SummonAndRecover);
            CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)961, BucketType.Helmet, SuperDiamondNut.SunExchange);
            CustomCore.AddFusion(905, 961, 1);
            CustomCore.AddFusion(961, 31, 3);
            CustomCore.RegisterCustomPlant<BigWallNut>(962, ab.GetAsset<GameObject>("BigDiamondNutPrefab"),
                ab.GetAsset<GameObject>("BigDiamondNutPreview"), [], 3, 0, 1800, 400000, 7.5f, 200);
            CustomCore.TypeMgrExtra.IsNut.Add((PlantType)961);
            CustomCore.TypeMgrExtra.BigNut.Add((PlantType)962);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)962);
            CustomCore.AddPlantAlmanacStrings(961, "钻石帝果", "点击生成钻石保龄球\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>阳光帝果亚种，使用金盏花、向日葵切换，花费3000钱币生成1800/帧伤的钻石保龄球</color>\n<color=#3D1400>融合配方：</color><color=red>阳光帝果+金盏花</color>\n<color=#3D1400>钻石帝果每次和阳光帝果一起出场时，僵尸们总是四散而逃，每当记者采访他时，他总说：“阳光帝果生产阳光时，我反射的光照就会闪瞎他们的眼睛。”这时记者都会一口同声说一句：“天怎么黑了？”</color>");
            CustomCore.AddPlantAlmanacStrings(962, "钻石保龄球", "就是个换皮大保龄球...吗？\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>伤害：</color><color=red>1800/帧伤</color>\n<color=#3D1400>！</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperDiamondNut : MonoBehaviour
    {
        public SuperDiamondNut() : base(ClassInjector.DerivedConstructorPointer<SuperDiamondNut>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperDiamondNut(IntPtr i) : base(i)
        {
        }

        public static void SummonAndRecover(Plant plant)
        {
            if (plant.board.theMoney < 70000)
            {
                plant.board.theMoney += 35500;
                plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)962, null, default, true);
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        public static void SunExchange(Plant plant)
        {
            if (plant.board.theMoney >= 70000)
            {
                plant.board.theMoney -= 50000;
                plant.board.SetSun(90000);
                
                plant.Recover(999999);
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)937, null, default, true);
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        public void Awake()
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame && !Board.Instance.isIZ && !Board.Instance.isEveStart && gameObject.GetComponent<SuperSunNut>().thePlantType is (PlantType)961)
            {
                InGameUIMgr.Instance.MoneyBank.SetActive(true);
            }
            plant.DisableDisMix();
        }

        public SuperSunNut plant => gameObject.GetComponent<SuperSunNut>();
    }
}