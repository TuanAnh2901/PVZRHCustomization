using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using SuperHypnoUmbrella.MelonLoader;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "SuperHypnoUmbrella", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SuperHypnoUmbrella.MelonLoader
{
    [HarmonyPatch(typeof(SuperUmbrella), "BlockEffect")]
    public static class SuperUmbrellaPatch
    {
        public static bool Prefix(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)967)
            {
                if (UnityEngine.Random.RandomRangeInt(0, 19) == 1) zombie.SetMindControl();
                zombie.KnockBack(1.5f * (__instance.UmbrellaPot is not null ? 2 : 1));
                return false;
            }
            return true;
        }
    }

    public class Core : MelonMod//967
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superhypnoumbrella");
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperHypnoUmbrella>(967, ab.GetAsset<GameObject>("SuperHypnoUmbrellaPrefab"),
                ab.GetAsset<GameObject>("SuperHypnoUmbrellaPreview"), [(916, 8)], 3, 0, 3600, 4000, 60f, 1800);
            CustomCore.RegisterSuperSkill(967, (plant) =>
            {
                var pos = plant.shadow.transform.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                int i = 1;
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        i++;
                    }
                }
                return 1000 * (10 + (int)(4 * Math.Log(i)));
            },
            (plant) =>
            {
                var pos = plant.shadow.transform.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && ((zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1)))
                    {
                        zombie.SetMindControl();
                    }
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)967);
            CustomCore.AddFusion(916, 967, 26);
            CustomCore.AddPlantAlmanacStrings(967, "魅宝石伞", "魅宝石伞能放大招魅惑周围的僵尸\n<color=#3D1400>贴图作者：@仨硝基甲苯 @摆烂的克莱尔</color>\n<color=#3D1400>特点：</color><color=red>绿宝石伞亚种，使用魅惑菇、卷心菜投手切换。僵尸主动靠近魅宝石伞时有5%概率魅惑，花费1000*(10+4ln(要魅惑的僵尸数+1))钱币释放大招，魅惑周围全部僵尸</color>\n<color=#3D1400>融合配方：</color><color=red>绿宝石伞+魅惑菇</color>\n<color=#3D1400>据说，若有人能找到彩宝石伞最喜爱的颜色，她将短暂地从睡梦中醒来，展露自己的光辉。但她的喜好没有规律可循，就像彩虹不会为任何人停留。</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperHypnoUmbrella : MonoBehaviour
    {
        public SuperHypnoUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperHypnoUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperHypnoUmbrella(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.DisableDisMix();
        }

        public SuperUmbrella plant => gameObject.GetComponent<SuperUmbrella>();
    }
}