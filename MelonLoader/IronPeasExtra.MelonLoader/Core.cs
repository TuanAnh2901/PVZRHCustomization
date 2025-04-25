using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2Cpp;
using IronPeasExtra.MelonLoader;
using MelonLoader;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "IronPeasExtra", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace IronPeasExtra.MelonLoader
{
    [HarmonyPatch(typeof(Board))]
    public static class MoneyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void PostUpdate(Board __instance)
        {
            //__instance.theMoney = theMoney;
            //theMoney = __instance.theMoney;
            if (__instance.theMoney >= 9999)
            {
                //__instance.theMoney = theMoney;
                __instance.theMoney = int.MaxValue - 5;
            }
            //if (__instance.theMoney > 999)
            //{
            //    __instance.theMoney = 0;
            //}
            //__instance.theSun = theSun;
            //theSun = __instance.theSun;
            if (__instance.theSun >= 9999)
            {
                //__instance.theSun = theSun;
                __instance.theSun = __instance.theSun;
            }
            if (__instance.theSun >= 100000)
            {
                //__instance.theSun = theSun;
                __instance.theSun = __instance.theSun;
            }
        }
    }

    [HarmonyPatch(typeof(SuperSnowGatling))]
    public static class SuperSnowGatlingPatch
    {
        //[HarmonyPostfix]
        //[HarmonyPatch("AnimShoot")]
        //public static void PostAnimShoot(ref Bullet __result)
        //{
        //    if (Lawnf.TravelAdvanced(SuperIronGatling.Buff))
        //    {
        //        __result.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[39];
        //        __result.Damage *= 6;
        //    }
        //}

        [HarmonyPostfix]
        [HarmonyPatch("GetBulletType")]
        public static void PostGetBulletType(SuperSnowGatling __instance, ref BulletType __result)
        {
            if (__instance.thePlantType is (PlantType)163)
            {
                __result = BulletType.Bullet_ironPea;
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    public class BigIronGatlingPea : MonoBehaviour
    {
        // Counter for bullets created
        private int bulletCounter = 0;
        // Threshold for creating a zombie
        private const int ZOMBIE_CREATION_THRESHOLD = 30;

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
            if (plant is not null)
            {
                if (plant.theStatus is not PlantStatus.BigGatling_raised) return;
                var pos = plant.shoot.transform.position;

                // Create bullets
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.3f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.2f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.1f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.1f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.2f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.3f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;

                // Increment bullet counter (7 bullets per shot)
                bulletCounter += 7;

                // Log the current bullet count
                MelonLogger.Msg($"BigIronGatlingPea bullet count: {bulletCounter}");

                // Check if we've reached the threshold to create a zombie
                if (bulletCounter >= ZOMBIE_CREATION_THRESHOLD)
                {
                    // Create a mind-controlled zombie
                    CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimatePaperZombie, pos.x, false);

                    // Log zombie creation
                    MelonLogger.Msg("Created mind-controlled UltimatePaperZombie after reaching bullet threshold");

                    // Reset counter (subtract threshold to account for any excess)
                    bulletCounter -= ZOMBIE_CREATION_THRESHOLD;
                }
            }
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(3);
            var tag = plant.plantTag;
            tag.doubleBoxPlant = true;
            plant.plantTag = tag;
            plant.isConnected = true;

            // Initialize bullet counter
            bulletCounter = 0;
        }

        public BigGatling plant => gameObject.GetComponent<BigGatling>();
    }

    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ironpeas");
            CustomCore.RegisterCustomPlant<BigGatling, BigIronGatlingPea>(301, ab.GetAsset<GameObject>("BigIronGatlingPeaPrefab"),
                ab.GetAsset<GameObject>("BigIronGatlingPeaPreview"), [], 0.1f, 0, 150, 40000, 1, 1000);
            CustomCore.RegisterCustomPlant<SuperSnowGatling, SuperIronGatling>(163, ab.GetAsset<GameObject>("SuperIronGatlingPrefab"),
                ab.GetAsset<GameObject>("SuperIronGatlingPreview"), [(1168, 1020), (1020, 1168)], 0.1f, 0, 150, 400000, 1, 800);
            CustomCore.AddFusion(163, 1008, 1008);
            CustomCore.RegisterCustomPlantClickEvent(163, SuperIronGatling.SummonSuperDoomSqualour);
            CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)163, BucketType.Machine, SuperIronGatling.Shooter);
            CustomCore.RegisterCustomUseItemOnPlantEvent(PlantType.BigGatling, BucketType.Bucket, (PlantType)301);
            CustomCore.TypeMgrExtra.DoubleBoxPlants.Add((PlantType)301);

            SuperIronGatling.Buff = CustomCore.RegisterCustomBuff("炽热铁豆：超级铁豌豆机枪发射红色铁豆，6倍伤害", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<SuperIronGatling>(), 5400, null, (PlantType)163);
            CustomCore.AddPlantAlmanacStrings(301, "铁桶机枪豌豆炮台", "会发射铁豌豆的巨型豌豆炮台\n<color=#3D1400>贴图作者：@屑红leong</color>\n<color=#3D1400>伤害：</color><color=red>80</color>\n<color=#3D1400>融合配方：</color><color=red>巨型豌豆炮台+铁桶</color>\n<color=#3D1400>铁桶机枪豌豆炮台认为，身上的每一处缺口，每一道磨痕，都象征着一场艰苦的战斗。每一次打磨，都是为了在下一场战斗中更加无坚不摧。</color>");
            CustomCore.AddPlantAlmanacStrings(163, "超级铁豌豆机枪", "会发射铁豌豆的超级机枪射手\n<color=#3D1400>贴图作者：@屑红leong</color>\n<color=#3D1400>伤害：</color><color=red>80</color>\n<color=#3D1400>融合配方：</color><color=red>超级机枪射手+铁豌豆</color>\n<color=#3D1400>词条：</color><color=red>炽热铁豆：超级铁豌豆机枪发射红色铁豆，6倍伤害(解锁条件：场上存在超级铁豌豆机枪)</color>\n<color=#3D1400>超级铁豌豆机枪站在前线，像一支军队般横扫着战场上的敌人。僵尸们或许以为自己能冲破防线，但很快就会发现，面对钢铁子弹的洪流，他们毫无胜算。</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperIronGatling : MonoBehaviour //163 + 301
    {
        public SuperIronGatling() : base(ClassInjector.DerivedConstructorPointer<SuperIronGatling>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperIronGatling(IntPtr i) : base(i)
        {
        }
        public static void Shooter(Plant plant)
        {
            if (plant is not null )
            {
                plant.board.theMoney -= 5000;
                plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn +1, plant.thePlantRow, (PlantType)928, null, default, true); // 928 = Disillusioned Mushroom
                var pos = plant.shoot.transform.position;
                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, BulletType.Bullet_superStar, 0).Damage = 99999;
                
                //if (plant.board.theMoney >= 70000)
                //{
                //    GameObject gameObject2 = CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)922, null, default, true);
                //}
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().axis.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        

        public static void SummonSuperDoomSqualour(Plant plant)
        {
            if (plant != null && Input.GetKey(KeyCode.LeftShift))
            {
                int i = 1;
                plant.board.theSun -= 300;
                plant.Upgrade(3, true);
                plant.thePlantMaxHealth = plant.thePlantMaxHealth + 16000 * i;
                i += 2;
                plant.UpdateText();
                GameObject game = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, PlantType.AbyssSwordStar, null, default, true);
                if (game is not null)
                {
                    Vector3 position = game.GetComponent<Plant>().axis.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        //public static void RedIronPeaPatch(Plant plant)
        //{
        //    Bullet bullet = new Bullet();
        //    SpriteRenderer component = bullet.GetComponent<SpriteRenderer>();
        //    Sprite sprite = GameAPP.spritePrefab[39];
        //    component.sprite = sprite;
        //    bullet.theBulletDamage = (int)((ulong)320L);
        //    bullet.GetComponent<Bullet>().isHot = true;
        //}

        public void Awake()
        {
            //var pos = plant.shoot.transform.position;
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(0);
            //plant.SuperShoot(5f, 100, pos.x, pos.y);
        }



        public static int shootnum = 0;
        public void AnimShoot()
        {

            if (plant is not null)
            {
                var pos = plant.shoot.transform.position;

                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = 150;

                shootnum += 2;
                if (shootnum > 10)
                {
                    CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateMachineNutZombie, pos.x, false);
                    //return;
                    // Last Change
                    //plant.flashCountDown = 5f;
                    plant.timer = 0.1f;
                    //plant.flashCountDown = 5f;
                    plant.AttributeEvent();
                    plant.thePlantAttackCountDown = 0.1f;
                    plant.thePlantAttackInterval = 0.1f;
                    //plant.anim.SetBool("shooting", true);
                    int thePlantMaxHealth = plant.thePlantMaxHealth;
                    plant.Recover(thePlantMaxHealth);
                    //shootnum = 0;
                    MelonLogger.Msg($"shootnum reached {shootnum}, condition > 10 triggered.");
                }
                if (shootnum >= 15)
                {
                    plant.timer = 0.1f;
                    //plant.flashCountDown = 5f;
                    plant.AttributeEvent();
                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 4, 0, default, true);
                    plant.anim.SetBool("shooting", true);
                    int thePlantMaxHealth = plant.thePlantMaxHealth;
                    plant.Recover(thePlantMaxHealth);
                    //shootnum = 0;
                    MelonLogger.Msg($"shootnum reached {shootnum}, condition >= 15 triggered.");
                }
                // set an if when shootnum goes to 30, then trigger the AttributeEvent and set plant.timer to 0.1f.
                if (shootnum >= 30)
                {
                    plant.timer = 0.1f;
                    //plant.flashCountDown = 5f;
                    plant.AttributeEvent();
                    CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateMachineNutZombie, pos.x, false);
                    plant.anim.SetBool("shooting", true);
                    plant.keepShooting = false;
                    int thePlantMaxHealth = plant.thePlantMaxHealth;
                    plant.Recover(thePlantMaxHealth);
                    //shootnum = 0;
                    MelonLogger.Msg($"shootnum reached {shootnum}, condition >= 30 triggered.");
                }
                if (shootnum >= 50)
                {
                    MelonLogger.Msg($"shootnum reached {shootnum}, condition >= 50 triggered. Reset shootnum");
                    shootnum = 0;
                    plant.keepShooting = true;
                    //plant.SuperShoot(50f, 100, plant.thePlantColumn, plant.thePlantRow);
                    MelonLogger.Msg($"shootnum reached {shootnum}, condition >= 50 triggered. Resetted shootnum");
                }
                //if (shootnum >= 30)
                //{
                //    plant.timer = 1f;
                //    plant.flashCountDown = 5f;
                //    plant.AttributeEvent();
                //    plant.anim.SetBool("shooting", true);
                //    int thePlantMaxHealth = plant.thePlantMaxHealth;
                //    plant.Recover(thePlantMaxHealth);
                //    shootnum = 0;
                //    // and show a message on MelonLoader console to show log the shootnum and what condition it has been triggered, like the first one is > 10, the second one is >= 30.

                //}
                //Bullet bullet = new Bullet();
                //bullet.theBulletDamage = (int)((ulong)320L);
                //bullet.GetComponent<Bullet>().isHot = true;
                //plant.timer -= 2 * Time.deltaTime;

                //


            }

        }


        public static int Buff { get; set; } = -1;
        public SuperSnowGatling plant => gameObject.GetComponent<SuperSnowGatling>();


    }
}