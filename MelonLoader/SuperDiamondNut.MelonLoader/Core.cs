using CustomizeLib.MelonLoader;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using SuperDiamondNut.MelonLoader;
using System.Reflection;
using UnityEngine;

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
            if (__instance.thePlantType is (PlantType)161)
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
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superdiamondnut");
            CustomCore.RegisterCustomPlant<SuperSunNut, SuperDiamondNut>(161, ab.GetAsset<GameObject>("SuperDiamondNutPrefab"),
                ab.GetAsset<GameObject>("SuperDiamondNutPreview"), [(905, 31)], 3, 0, 20, 4000, 7.5f, 800);
            CustomCore.RegisterCustomPlantClickEvent(161, SuperDiamondNut.SummonAndRecover);
            CustomCore.AddFusion(905, 161, 1);
            CustomCore.RegisterCustomPlant<BigWallNut>(162, ab.GetAsset<GameObject>("BigDiamondNutPrefab"),
                ab.GetAsset<GameObject>("BigDiamondNutPreview"), [], 3, 0, 1800, 4000, 7.5f, 200);
            CustomCore.TypeMgrExtra.IsNut.Add((PlantType)161);
            CustomCore.TypeMgrExtra.BigNut.Add((PlantType)162);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)162);
            CustomCore.AddPlantAlmanacStrings(161, "钻石帝果", "点击生成钻石保龄球\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>阳光帝果亚种，使用金盏花、向日葵切换，花费3000钱币生成1800/帧伤的钻石保龄球</color>\n<color=#3D1400>融合配方：</color><color=red>阳光帝果+金盏花</color>\n<color=#3D1400>钻石帝果每次和阳光帝果一起出场时，僵尸们总是四散而逃，每当记者采访他时，他总说：“阳光帝果生产阳光时，我反射的光照就会闪瞎他们的眼睛。”这时记者都会一口同声说一句：“天怎么黑了？”</color>");
            CustomCore.AddPlantAlmanacStrings(162, "钻石保龄球", "就是个换皮大保龄球...吗？\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>伤害：</color><color=red>1800/帧伤</color>\n<color=#3D1400>！</color>");
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
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)162, null, default, true);
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().axis.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        public void Awake()
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame && !Board.Instance.isIZ && !Board.Instance.isEveStart && gameObject.GetComponent<SuperSunNut>().thePlantType is (PlantType)161)
            {
                InGameUI.Instance.MoneyBank.SetActive(true);
            }
            plant.DisableDisMix();
        }

        public SuperSunNut plant => gameObject.GetComponent<SuperSunNut>();
    }
}