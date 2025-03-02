using CustomizeLib;
using HarmonyLib;
using MelonLoader;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(CustomCore), "PVZRHCustomization", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace CustomizeLib
{
    public struct CustomPlantData
    {
        public int ID { get; set; }
        public PlantDataLoader.PlantData_ PlantData { get; set; }
        public GameObject Prefab { get; set; }
        public GameObject Preview { get; set; }
    }

    public static class Extensions
    {
        public static T GetAsset<T>(this AssetBundle ab, string name) where T : UnityEngine.Object
        {
            foreach (var ase in ab.LoadAllAssetsAsync().allAssets)
            {
                if (ase.TryCast<T>()?.name == name)
                {
                    return ase.Cast<T>();
                }
            }
            throw new ArgumentException($"Could not find {name} from {ab.name}");
        }
    }

    [HarmonyPatch(typeof(GameAPP))]
    public static class GameAPPPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LoadResources")]
        public static void LoadResources()
        {
            foreach (var plant in CustomCore.CustomPlants)
            {
                GameAPP.plantPrefab[(int)plant.Key] = plant.Value.Prefab;
                PlantDataLoader.plantData[(int)plant.Key] = plant.Value.PlantData;
            }
            Il2CppSystem.Array array = MixData.data.Cast<Il2CppSystem.Array>();
            foreach (var f in CustomCore.CustomFusions)
            {
                array.SetValue(f.Item1, f.Item2, f.Item3);
            }
            foreach (var plant in CustomCore.CustomPlants)
            {
                GameAPP.prePlantPrefab[(int)plant.Key] = plant.Value.Preview;
            }
        }
    }

    [HarmonyPatch(typeof(Mouse))]
    public static class MousePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LeftClickWithNothing")]
        public static void PostLeftClickWithNothing(Mouse __instance)
        {
            foreach (GameObject gameObject in (List<GameObject>)[..from RaycastHit2D raycastHit2D in
                                           (RaycastHit2D[])Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                           Vector2.zero) select raycastHit2D.collider.gameObject])
            {
                if (gameObject.TryGetComponent<Plant>(out var plant) && CustomCore.CustomPlantClicks.ContainsKey(plant.thePlantType))
                {
                    CustomCore.CustomPlantClicks[plant.thePlantType](plant);
                    return;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("LeftClickWithSomeThing")]
        public static bool PreLeftClickWithSomeThing(Mouse __instance)
        {
            if (CustomCore.CustomPlantTypes.Contains(__instance.thePlantTypeOnMouse))
            {
                if (__instance.thePlantOnGlove is not null && CustomCore.CustomPlantTypes.Contains(__instance.thePlantOnGlove.thePlantType))

                {
                    __instance.TryToSetPlantByGlove();
                }
                else
                {
                    __instance.TryToSetPlantByCard();
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("PutDownItem")]
        public static bool PrePutDownItem(Mouse __instance)
        {
            if (CustomCore.CustomPlantTypes.Contains(__instance.thePlantTypeOnMouse))
            {
                __instance.theCardOnMouse?.PutDown();
                UnityEngine.Object.Destroy(__instance.theItemOnMouse);
                __instance.ClearItemOnMouse();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Plant))]
    public static class PlantPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UseItem")]
        public static void PostUseItem(Plant __instance, ref BucketType type, ref Bucket bucket)
        {
            if (CustomCore.CustomUseItems.ContainsKey((__instance.thePlantType, type)))
            {
                CustomCore.CustomUseItems[(__instance.thePlantType, type)](__instance);
                UnityEngine.Object.Destroy(bucket.gameObject);
            }
        }
    }

    public class CustomCore : MelonMod
    {
        public static void AddFusion(int target, int item1, int item2) => CustomFusions.Add((target, item1, item2));

        public static AssetBundle GetAssetBundle(Assembly assembly, string name)
        {
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(name)!;
                using MemoryStream stream1 = new();
                stream.CopyTo(stream1);
                var ab = AssetBundle.LoadFromMemory(stream1.ToArray());
                ArgumentNullException.ThrowIfNull(ab);
                MelonLogger.Msg($"Successfully load AssetBundle {name}.");
                return ab;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to load {name} \n{e}");
            }
        }

        public static void RegisterCustomPlant<TBase, TClass>([NotNull] int id, [NotNull] GameObject prefab, [NotNull] GameObject preview,
                    List<(int, int)> fusions, float attackInterval, float produceInterval, int attackDamage, int maxHealth, float cd, int sun)
                    where TBase : Plant where TClass : MonoBehaviour
        {
            prefab.AddComponent<TBase>().thePlantType = (PlantType)id;
            prefab.AddComponent<TClass>();
            if (!CustomPlantTypes.Contains((PlantType)id))
            {
                CustomPlantTypes.Add((PlantType)id);
                CustomPlants.Add((PlantType)id, new CustomPlantData()
                {
                    ID = id,
                    Prefab = prefab,
                    Preview = preview,
                    PlantData = new()
                    {
                        attackDamage = attackDamage,
                        field_Public_PlantType_0 = (PlantType)id,
                        field_Public_Single_0 = attackInterval,
                        field_Public_Single_1 = produceInterval,
                        field_Public_Int32_0 = maxHealth,
                        field_Public_Single_2 = cd,
                        field_Public_Int32_1 = sun
                    }
                });
                foreach (var f in fusions)
                {
                    AddFusion(id, f.Item1, f.Item2);
                }
            }
            else
            {
                MelonLogger.Error($"Duplicate Plant ID: {id}");
            }
        }

        public static void RegisterCustomPlant<TBase>([NotNull] int id, [NotNull] GameObject prefab, [NotNull] GameObject preview,
            List<(int, int)> fusions, float attackInterval, float produceInterval, int attackDamage, int maxHealth, float cd, int sun)
            where TBase : Plant
        {
            prefab.AddComponent<TBase>().thePlantType = (PlantType)id;
            if (!CustomPlantTypes.Contains((PlantType)id))
            {
                CustomPlantTypes.Add((PlantType)id);
                CustomPlants.Add((PlantType)id, new CustomPlantData()
                {
                    ID = id,
                    Prefab = prefab,
                    Preview = preview,
                    PlantData = new()
                    {
                        attackDamage = attackDamage,
                        field_Public_PlantType_0 = (PlantType)id,
                        field_Public_Single_0 = attackInterval,
                        field_Public_Single_1 = produceInterval,
                        field_Public_Int32_0 = maxHealth,
                        field_Public_Single_2 = cd,
                        field_Public_Int32_1 = sun
                    }
                });
                foreach (var f in fusions)
                {
                    AddFusion(id, f.Item1, f.Item2);
                }
            }
            else
            {
                MelonLogger.Error($"Duplicate Plant ID: {id}");
            }
        }

        public static void RegisterCustomPlantClickEvent([NotNull] int id, [NotNull] Action<Plant> action) => CustomPlantClicks.Add((PlantType)id, action);

        public static void RegisterCustomUseItemOnPlantEvent([NotNull] PlantType id, [NotNull] BucketType bucketType, [NotNull] Action<Plant> callback) => CustomUseItems.Add((id, bucketType), callback);

        public static void RegisterCustomUseItemOnPlantEvent([NotNull] PlantType id, [NotNull] BucketType bucketType, [NotNull] PlantType newPlant)
            => CustomUseItems.Add((id, bucketType), (p) =>
            {
                p.Die();
                CreatePlant.Instance.SetPlant(p.thePlantColumn, p.thePlantRow, newPlant);
            });

        public static List<(int, int, int)> CustomFusions { get; set; } = [];
        public static Dictionary<PlantType, Action<Plant>> CustomPlantClicks { get; set; } = [];
        public static Dictionary<PlantType, CustomPlantData> CustomPlants { get; set; } = [];

        public static List<PlantType> CustomPlantTypes { get; set; } = [];
        public static Dictionary<(PlantType, BucketType), Action<Plant>> CustomUseItems { get; set; } = [];
    }
}