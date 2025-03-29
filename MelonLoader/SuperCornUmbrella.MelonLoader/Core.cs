using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using SuperCornUmbrella.MelonLoader;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "SuperCornUmbrella", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SuperCornUmbrella.MelonLoader
{
    [HarmonyPatch(typeof(SuperUmbrella), "BlockEffect")]
    public static class SuperUmbrellaPatch
    {
        public static bool Prefix(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)166)
            {
                if (Lawnf.TravelAdvanced(23) && Board.Instance.theMoney > 6000 && __instance.SuperSkill())
                {
                    CustomCore.SuperSkills[__instance.thePlantType].Item2(__instance);
                    __instance.AnimSuperShoot();
                    Money.Instance.UsedEvent(__instance.thePlantColumn, __instance.thePlantRow, 6000);
                    Money.Instance.OtherSuperSkill(__instance);
                }
                else
                {
                    zombie.TakeDamage(DmgType.Normal, 80);
                    zombie.KnockBack(1.5f * (__instance.UmbrellaPot is not null ? 2 : 1));
                    zombie.Buttered(6);
                }
                return false;
            }
            return true;
        }
    }

    public class Core : MelonMod//166
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "supercornumbrella");
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperCornUmbrella>(166, ab.GetAsset<GameObject>("SuperCornUmbrellaPrefab"),
                ab.GetAsset<GameObject>("SuperCornUmbrellaPreview"), [(916, 28)], 3, 0, 80, 4000, 60f, 800);
            CustomCore.RegisterSuperSkill(166, (_) => 6000, (plant) =>
            {
                var pos = plant.shadow.transform.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && ((zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1)))
                    {
                        zombie.KnockBack(1.5f * (plant.Cast<SuperUmbrella>().UmbrellaPot is not null ? 2 : 1));
                        zombie.Buttered(6);
                    }
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)166);
            CustomCore.AddFusion(916, 166, 26);
            CustomCore.AddPlantAlmanacStrings(166, "黄宝石伞", "黄宝石伞能用黄油黏住靠近的僵尸，又能放出大招黏住一定范围的僵尸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @暗影Dev</color>\n<color=#3D1400>特点：</color><color=red>绿宝石伞亚种，使用玉米投手、卷心菜投手切换。僵尸主动靠近黄宝石伞时特性同黄油伞，花费6000钱币释放大招，对周围僵尸施加黄油效果并击退。可同时享受人工智能词条</color>\n<color=#3D1400>融合配方：</color><color=red>绿宝石伞+玉米投手</color>\n<color=#3D1400>作为餐厅的主厨，黄宝石伞做的菜一直饱受好评，“这要归功于师傅娴熟的按摩技术，以及作为主要厨具的自我修养。”</color>");
            foreach (var d in MixData.disMixDatas)
            {
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperCornUmbrella : MonoBehaviour
    {
        public SuperCornUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperCornUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperCornUmbrella(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.DisableDisMix();
        }

        public SuperUmbrella plant => gameObject.GetComponent<SuperUmbrella>();
    }
}