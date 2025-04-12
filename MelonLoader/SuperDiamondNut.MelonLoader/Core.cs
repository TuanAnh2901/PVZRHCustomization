using CustomizeLib;
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
                var damage = Lawnf.TravelAdvanced(5) ? 1 : 5;
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 36, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 41, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn + 2, __instance.thePlantRow, 4, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn -1, __instance.thePlantRow, 6, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn - 1, __instance.thePlantRow, 1, 0);
                __instance.brightness = 999f;
                __instance.alwaysLightUp = true;
                __instance.exchangeSpeed = 999999;
                __instance.GiveSunInIZ();
                __instance.isCrashed = false;
                __instance.thePlantMaxHealth += 16000;
                __instance.regeneration = true;
                //__instance.invincible = true;
                //SuperDiamondNut.SpawnItem("Items/SuperMachine");
                //Board.Instance.CreateUltimateMateorite();
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
                ab.GetAsset<GameObject>("SuperDiamondNutPreview"), [(905, 31)], 3, 0, 200, 4000000, 1.5f, 800);
            CustomCore.RegisterCustomPlantClickEvent(961, SuperDiamondNut.SummonAndRecover);
            CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)961, BucketType.Helmet, SuperDiamondNut.SunExchange);
            CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)961, BucketType.Bucket, SuperDiamondNut.PlantUpgrade);
            CustomCore.AddFusion(905, 961, 1);
            CustomCore.AddFusion(961, 31, 3);
            CustomCore.AddFusion(961, 3, 31);
            CustomCore.RegisterCustomPlant<BigWallNut>(962, ab.GetAsset<GameObject>("BigDiamondNutPrefab"),
                ab.GetAsset<GameObject>("BigDiamondNutPreview"), [], 3, 0, 18000, 4000000, 1.5f, 200);
            CustomCore.TypeMgrExtra.IsNut.Add((PlantType)961);
            CustomCore.TypeMgrExtra.BigNut.Add((PlantType)962);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)962);
            CustomCore.AddPlantAlmanacStrings(961, "钻石帝果", "点击生成钻石保龄球\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>阳光帝果亚种，使用金盏花、向日葵切换，花费3000钱币生成1800/帧伤的钻石保龄球</color>\n<color=#3D1400>融合配方：</color><color=red>阳光帝果+金盏花</color>\n<color=#3D1400>钻石帝果每次和阳光帝果一起出场时，僵尸们总是四散而逃，每当记者采访他时，他总说：“阳光帝果生产阳光时，我反射的光照就会闪瞎他们的眼睛。”这时记者都会一口同声说一句：“天怎么黑了？”</color>");
            CustomCore.AddPlantAlmanacStrings(962, "钻石保龄球", "就是个换皮大保龄球...吗？\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>伤害：</color><color=red>1800/帧伤</color>\n<color=#3D1400>！</color>");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            List<Bucket> buckets = GetBuckets();

            foreach (var bucket in buckets)
            {
                // Giảm existTime xuống
                if (bucket.existTime > 0)
                {
                    bucket.existTime--;
                }

                // Kiểm tra khi existTime bằng 0
                if (bucket.existTime == 0 || bucket.disappear == true)
                {
                    // Tạo SuperDiamondNut và hồi máu
                    SuperSunNut superSunNutPlant = new SuperSunNut();
                    superSunNutPlant.thePlantMaxHealth += 32000;
                    superSunNutPlant.Recover(superSunNutPlant.thePlantMaxHealth);

                    // Gọi phương thức ExistTime
                    SuperDiamondNut.ExistTime(bucket);

                    // Đặt lại existTime nếu cần
                    bucket.existTime = 3;
                    bucket.disappear = false;
                }
            }
        }

        public static List<Bucket> GetBuckets()
        {
            // Trả về danh sách các bucket hiện có
            return new List<Bucket>
            {
                new Bucket { itemType = BucketType.Bucket, existTime = 3 },
                new Bucket { itemType = BucketType.Helmet, existTime = 3 },
                new Bucket { itemType = BucketType.Machine, existTime = 3 },
                new Bucket { itemType = BucketType.SuperMachine, existTime = 3 },
                new Bucket { itemType = BucketType.Door, existTime = 3 },
                new Bucket { itemType = BucketType.IronHead, existTime = 3 },
                new Bucket { itemType = BucketType.Jackbox, existTime = 3 },
                new Bucket { itemType = BucketType.Jumper, existTime = 3 },
                new Bucket { itemType = BucketType.Ladder, existTime = 3 },
                new Bucket { itemType = BucketType.Pickaxe, existTime = 3 },
                new Bucket { itemType = BucketType.RedIronHead, existTime = 3 },
                // Thêm các bucket khác nếu cần
            };
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperDiamondNut : MonoBehaviour //961 - 962
    {
        public SuperDiamondNut() : base(ClassInjector.DerivedConstructorPointer<SuperDiamondNut>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperDiamondNut(IntPtr i) : base(i)
        {
        }

        public static void SummonAndRecover(Plant plant)
        {
            if (plant is not null && Input.GetKey(KeyCode.LeftShift))
            {
                plant.board.theMoney += 35500;
                plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                SuperSunNut __instance = new SuperSunNut();
                CreateItem.Instance.SetCoin(__instance.thePlantColumn + 2, __instance.thePlantRow, 4, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn - 1, __instance.thePlantRow, 6, 0);
                CreateItem.Instance.SetCoin(__instance.thePlantColumn, __instance.thePlantRow, 1, 0);
                SuperDiamondNut.SpawnItem("Items/SuperMachine");
                SuperDiamondNut.SpawnItem("Items/SuperMachine");
                SuperDiamondNut.SpawnItem("Items/SuperMachine");
                SuperDiamondNut.SpawnItem("Items/SuperMachine");
                SuperDiamondNut.SpawnItem("Items/SuperMachine");
                SuperDiamondNut.SpawnItem("Items/SuperMachine");
                Board.Instance.CreateUltimateMateorite();
                GameAPP.board.GetComponent<InitBoard>().InitMower();
                GameAPP.board.GetComponent<InitBoard>().InitMower();
                GameAPP.board.GetComponent<InitBoard>().InitMower();
                GameAPP.board.GetComponent<InitBoard>().InitMower();
                GameAPP.board.GetComponent<InitBoard>().InitMower(); // 5
                
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn - 1, plant.thePlantRow, (PlantType)1151, null, default, true); //1151: Giga Mecha Nut
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        //[HarmonyPatch(typeof(SuperSunNut))]
        //public static class SuperSunNutPatch
        //{
        //    [HarmonyPatch("Fixed Update")]
        //    private static void SSP( SuperSunNut instance)
        //    {
        //        List<Bucket> buckets = GetBuckets();

        //        foreach (var bucket in buckets)
        //        {
        //            if (bucket.existTime == 0)
        //            {
        //                SuperDiamondNut superDiamondNut = new SuperDiamondNut();
        //                superDiamondNut.plant.thePlantMaxHealth += 32000;
        //                superDiamondNut.plant.Recover(superDiamondNut.plant.thePlantMaxHealth);
        //            }
        //        }
        //    }
        //}

        public static List<Bucket> GetBuckets()
        {
            // Trả về danh sách các bucket hiện có
            return new List<Bucket>
            {
                new Bucket { itemType = BucketType.Bucket, existTime = 3 },
                new Bucket { itemType = BucketType.Helmet, existTime = 3 },
                new Bucket { itemType = BucketType.Machine, existTime = 3 },
                new Bucket { itemType = BucketType.SuperMachine, existTime = 3 },
                new Bucket { itemType = BucketType.Door, existTime = 3 },
                new Bucket { itemType = BucketType.IronHead, existTime = 3 },
                new Bucket { itemType = BucketType.Jackbox, existTime = 3 },
                new Bucket { itemType = BucketType.Jumper, existTime = 3 },
                new Bucket { itemType = BucketType.Ladder, existTime = 3 },
                new Bucket { itemType = BucketType.Pickaxe, existTime = 3 },
                new Bucket { itemType = BucketType.RedIronHead, existTime = 3 },
                // Thêm các bucket khác nếu cần
            };
        }

        public static void SunExchange(Plant plant)
        {
            if (plant.board.theSun != 0)
            {
                plant.board.SetSun(90000);
                plant.board.theSun += 50000;
                plant.Recover(999999);
                //int n = 0;
                for (int i = 0; i < 10; i++)
                {
                    // Thực hiện hành động với biến i
                    SuperDiamondNut.SpawnItem("Items/SuperMachine");
                }
                
                GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)937, null, default, true);
                if (gameObject is not null)
                {
                    Vector3 position = gameObject.GetComponent<Plant>().axis.position;
                    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                }
            }
        }

        public static void PlantUpgrade(Plant plant) // Cần thêm chức năng
        {
            // Duyệt ngược từ cuối mảng plantArray
            for (int i = Board.Instance.plantArray.Count - 1; i >= 0; i--)
            {
                // Lấy từng plant trong mảng
                var p = Board.Instance.plantArray[i];

                // Kiểm tra plant không phải null
                if (p is not null)
                {
                    // Các điều kiện nâng cấp áp dụng cho từng plant
                    if (Board.Instance.theSun >= 0 && Board.Instance.theSun < 10000)
                    {
                        p.Upgrade(1, true);
                        plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                    }
                    else if (Board.Instance.theSun >= 10000 && Board.Instance.theSun < 20000)
                    {
                        p.Upgrade(2, true);
                        plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                    }
                    else if (Board.Instance.theSun >= 20000 && Board.Instance.theSun < 40000)
                    {
                        p.Upgrade(3, true);
                        plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                    }
                    //else if (Board.Instance.theSun > 999999 && Input.GetKey(KeyCode.LeftShift))
                    //{
                    //    p.Upgrade(4, true);
                    //}
                    else
                    {
                        p.Upgrade(3, true);
                    }
                }
            }
        }

        public static void SpawnItem(string resourcePath)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            GameObject gameObject = Resources.Load<GameObject>(resourcePath);
            if (gameObject != null)
            {
                UnityEngine.Object.Instantiate<GameObject>(gameObject, mousePosition, Quaternion.identity);//, GameAPP.board.transform);
                return;
            }
        }

        public static void ExistTime(Bucket bucket)
        {
            if (bucket.existTime == 0)
            {
                SuperDiamondNut superDiamondNut = new SuperDiamondNut();
                superDiamondNut.plant.Recover(999999);
            }
        }

        //public List<Bucket> GetBuckets()
        //{
        //    // Trả về danh sách các bucket hiện có
        //    return new List<Bucket>
        //    {
        //        new Bucket { itemType = BucketType.Bucket, existTime = 3 },
        //        new Bucket { itemType = BucketType.Helmet, existTime = 3 },
        //        new Bucket { itemType = BucketType.Machine, existTime = 3 },
        //        new Bucket { itemType = BucketType.SuperMachine, existTime = 3 },
        //        new Bucket { itemType = BucketType.Door, existTime = 3 },
        //        new Bucket { itemType = BucketType.IronHead, existTime = 3 },
        //        new Bucket { itemType = BucketType.Jackbox, existTime = 3 },
        //        new Bucket { itemType = BucketType.Jumper, existTime = 3 },
        //        new Bucket { itemType = BucketType.Ladder, existTime = 3 },
        //        new Bucket { itemType = BucketType.Pickaxe, existTime = 3 },
        //        new Bucket { itemType = BucketType.RedIronHead, existTime = 3 },
        //        // Thêm các bucket khác nếu cần
        //    };
        //}



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