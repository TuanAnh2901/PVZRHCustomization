using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using IronPeasExtra.MelonLoader;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(Core), "IronPeasExtra", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace IronPeasExtra.MelonLoader
{
    [HarmonyPatch(typeof(SuperSnowGatling))]
    public static class SuperSnowGatlingPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetBulletType")]
        public static void PostGetBulletType(SuperSnowGatling __instance, ref int __result)
        {
            if (__instance.thePlantType is (PlantType)963)
            {
                __result = 11;
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class BigIronGatlingPea : MonoBehaviour
    {
        public BigIronGatlingPea() : base(ClassInjector.DerivedConstructorPointer<BigIronGatlingPea>()) => ClassInjector.DerivedConstructorBody(this);

        public BigIronGatlingPea(IntPtr i) : base(i)
        {
        }

        public void AnimRaised()
        {
            if (plant is not null)
                plant.theStatus = PlantStatus.BigGatling_raised;
        }

        public void AnimShooting()
        {
            if (plant.thePlantType is (PlantType)1900)
            {
                if (plant.theStatus is not PlantStatus.BigGatling_raised) return;
                var pos = plant.shoot.transform.position;
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.3f, plant.thePlantRow, 11, 0).theBulletDamage = 80;
                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, 11, 0).theBulletDamage = 80;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.3f, plant.thePlantRow, 11, 0).theBulletDamage = 80;
            }
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(3);
        }

        public BigGatling plant => gameObject.GetComponent<BigGatling>();
    }

    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "IronPeasExtra.MelonLoader.ironpeas");
            CustomCore.RegisterCustomPlant<BigGatling, BigIronGatlingPea>(1900, ab.GetAsset<GameObject>("BigIronGatlingPeaPrefab"),
                ab.GetAsset<GameObject>("BigIronGatlingPeaPreview"), [], 0.3f, 0, 80, 2500, 15, 1000);
            CustomCore.RegisterCustomPlantClickEvent(1900, (p) => { p.anim.SetTriggerString("shoot"); });
            CustomCore.RegisterCustomPlant<SuperSnowGatling, SuperIronGatling>(963, ab.GetAsset<GameObject>("SuperIronGatlingPrefab"),
                ab.GetAsset<GameObject>("SuperIronGatlingPreview"), [(1168, 1020), (1020, 1168)], 0.3f, 0, 80, 2500, 15, 800);
            CustomCore.RegisterCustomUseItemOnPlantEvent(PlantType.BigGatling, BucketType.Bucket, (PlantType)1900);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperIronGatling : MonoBehaviour
    {
        public SuperIronGatling() : base(ClassInjector.DerivedConstructorPointer<SuperIronGatling>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperIronGatling(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(0);
        }

        public SuperSnowGatling plant => gameObject.GetComponent<SuperSnowGatling>();
    }
}