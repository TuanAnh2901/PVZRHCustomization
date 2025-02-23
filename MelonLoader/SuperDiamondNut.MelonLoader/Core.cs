using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
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
                var damage = Lawnf.TravelAdvanced(5) ? 10 : 50;
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 34, 0);
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
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "SuperDiamondNut.MelonLoader.superdiamondnut");
            GameObject? prefab = null;
            GameObject? preview = null;
            foreach (var ase in ab.LoadAllAssets())
            {
                if (ase.TryCast<GameObject>()?.name is "SuperDiamondNutPrefab")
                {
                    prefab = ase.Cast<GameObject>();
                    prefab.AddComponent<SuperSunNut>().thePlantType = (PlantType)961;
                    prefab.AddComponent<SuperDiamondNut>();
                }
                if (ase.TryCast<GameObject>()?.name is "SuperDiamondNutPreview")
                {
                    preview = ase.TryCast<GameObject>()!;
                }
            }
            if (prefab is null || preview is null) return;
            CustomCore.RegisterCustomPlant(961, prefab, preview, [(905, 31)], 3, 0, 20, 4000, 7.5f, 150);
            CustomCore.RegisterCustomPlantClickEvent(961, SuperDiamondNut.SummonAndRecover);
            CustomCore.AddFusion(905, 961, 1);
            GameObject? prefab2 = null;
            GameObject? preview2 = null;
            foreach (var ase in ab.LoadAllAssets())
            {
                if (ase.TryCast<GameObject>()?.name is "BigDiamondNutPrefab")
                {
                    prefab2 = ase.Cast<GameObject>();
                    prefab2.AddComponent<BigWallNut>().thePlantType = (PlantType)962;
                }
                if (ase.TryCast<GameObject>()?.name is "BigDiamondNutPreview")
                {
                    preview2 = ase.TryCast<GameObject>()!;
                }
            }
            if (prefab2 is null || preview2 is null) return;
            CustomCore.RegisterCustomPlant(962, prefab2, preview2, [], 3, 0, 1800, 4000, 7.5f, 150);
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
            if (plant.board.theMoney >= 3000)
            {
                plant.board.theMoney -= 3000;
                plant.Recover(Lawnf.TravelAdvanced(4) ? 4000 : 1500);
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)962, null, default, true);
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        public void Awake()
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame && gameObject.GetComponent<SuperSunNut>().thePlantType is (PlantType)961)
            {
                InGameUIMgr.Instance.MoneyBank.SetActive(true);
            }
        }
    }
}