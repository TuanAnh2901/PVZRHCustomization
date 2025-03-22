using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
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
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.3f, plant.thePlantRow, 11, 0).theBulletDamage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.2f, plant.thePlantRow, 11, 0).theBulletDamage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.1f, plant.thePlantRow, 11, 0).theBulletDamage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, 11, 0).theBulletDamage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.1f, plant.thePlantRow, 11, 0).theBulletDamage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.2f, plant.thePlantRow, 11, 0).theBulletDamage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.3f, plant.thePlantRow, 11, 0).theBulletDamage = 150;
            }
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(3);
            var tag = plant.plantTag;
            tag.doubleBoxPlant = true;
            plant.plantTag = tag;
            plant.isConnected = true;
        }

        public BigGatling plant => gameObject.GetComponent<BigGatling>();
    }

    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ironpeas");
            CustomCore.RegisterCustomPlant<BigGatling, BigIronGatlingPea>(1900, ab.GetAsset<GameObject>("BigIronGatlingPeaPrefab"),
                ab.GetAsset<GameObject>("BigIronGatlingPeaPreview"), [], 0.1f, 0, 150, 40000, 1, 1000);
            CustomCore.RegisterCustomPlantClickEvent(1900, (p) => { p.anim.SetTriggerString("shoot"); });
            CustomCore.RegisterCustomPlant<SuperSnowGatling, SuperIronGatling>(963, ab.GetAsset<GameObject>("SuperIronGatlingPrefab"),
                ab.GetAsset<GameObject>("SuperIronGatlingPreview"), [(1008, 1020), (1020, 1008)], 0.1f, 0, 150, 400000, 1, 800);
            CustomCore.RegisterCustomUseItemOnPlantEvent(PlantType.BigGatling, BucketType.Bucket, (PlantType)1900);
            CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)963, BucketType.SuperMachine, (PlantType)1900);
            CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)963, BucketType.Bucket, SuperIronGatling.Shooter);
            //CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)1900, BucketType.Bucket, SuperIronGatling.Shooter);
            CustomCore.TypeMgrExtra.DoubleBoxPlants.Add((PlantType)1900);
            CustomCore.AddPlantAlmanacStrings(1900, "铁桶机枪豌豆炮台", "会发射铁豌豆的巨型豌豆炮台\n<color=#3D1400>贴图作者：@屑红leong</color>\n<color=#3D1400>伤害：</color><color=red>80</color>\n<color=#3D1400>融合配方：</color><color=red>巨型豌豆炮台+铁桶</color>\n<color=#3D1400>铁桶机枪豌豆炮台认为，身上的每一处缺口，每一道磨痕，都象征着一场艰苦的战斗。每一次打磨，都是为了在下一场战斗中更加无坚不摧。</color>");
            CustomCore.AddPlantAlmanacStrings(963, "超级铁豌豆机枪", "会发射铁豌豆的超级机枪射手\n<color=#3D1400>贴图作者：@屑红leong</color>\n<color=#3D1400>伤害：</color><color=red>80</color>\n<color=#3D1400>融合配方：</color><color=red>超级机枪射手+铁豌豆</color>\n<color=#3D1400>超级铁豌豆机枪站在前线，像一支军队般横扫着战场上的敌人。僵尸们或许以为自己能冲破防线，但很快就会发现，面对钢铁子弹的洪流，他们毫无胜算。</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperIronGatling : MonoBehaviour
    {
        public SuperIronGatling() : base(ClassInjector.DerivedConstructorPointer<SuperIronGatling>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperIronGatling(IntPtr i) : base(i)
        {
        }
        public static void Shooter(Plant plant)
        {
            if (plant.board.theMoney >= 10000)
            {
                plant.board.theMoney -= 3000;
                plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn +1, plant.thePlantRow, (PlantType)928, null, default, true);
                var pos = plant.shoot.transform.position;
                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, 24, 0).theBulletDamage = 99999;
                //if (plant.board.theMoney >= 70000)
                //{
                //    GameObject gameObject2 = CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)922, null, default, true);
                //}
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(0);
        }

        public SuperSnowGatling plant => gameObject.GetComponent<SuperSnowGatling>();

        
    }
}