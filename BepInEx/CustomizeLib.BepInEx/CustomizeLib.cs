using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace CustomizeLib
{
    public struct CustomPlantData
    {
        public int ID { get; set; }
        public PlantDataLoader.PlantData_ PlantData { get; set; }
        public GameObject Prefab { get; set; }
        public GameObject Preview { get; set; }
    }

    [HarmonyPatch(typeof(AlmanacMgr), "InitNameAndInfoFromJson")]
    public static class AlmanacMgrPatch
    {
        public static bool Prefix(AlmanacMgr __instance)
        {
            if (CustomCore.PlantsAlmanac.ContainsKey(__instance.theSeedType))
            {
                __instance.pageCount = 2;
                for (int i = 0; i < __instance.transform.childCount; i++)
                {
                    Transform childTransform = __instance.transform.GetChild(i);
                    if (childTransform == null)
                        continue;
                    if (childTransform.name == "Name")
                    {
                        childTransform.GetComponent<TextMeshPro>().text = CustomCore.PlantsAlmanac[__instance.theSeedType].Item1;
                        childTransform.GetChild(0).GetComponent<TextMeshPro>().text = CustomCore.PlantsAlmanac[__instance.theSeedType].Item1;
                    }
                    if (childTransform.name == "Info")
                    {
                        TextMeshPro info = childTransform.GetComponent<TextMeshPro>();
                        info.overflowMode = TextOverflowModes.Overflow;
                        info.fontSize = 40;
                        info.text = CustomCore.PlantsAlmanac[__instance.theSeedType].Item2;
                    }
                    if (childTransform.name == "Cost")
                        childTransform.GetComponent<TextMeshPro>().text = "";
                }
                return false;
            }
            return true;
        }
    }

    public static class Extensions
    {
        public static void DisableDisMix(this Plant plant) => (plant.firstParent, plant.secondParent) = (PlantType.Nothing, PlantType.Nothing);

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

    [HarmonyPatch(typeof(Money))]
    public static class MoneyPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ReinforcePlant")]
        public static bool PreReinforcePlant(Money __instance, ref Plant plant)
        {
            if (CustomCore.SuperSkills.ContainsKey(plant.thePlantType))
            {
                var cost = CustomCore.SuperSkills[plant.thePlantType].Item1(plant);

                if (Board.Instance.theMoney < cost)
                {
                    InGameText.Instance.EnableText($"大招需要{cost}金币", 5);
                    return false;
                }
                if (plant.SuperSkill())
                {
                    CustomCore.SuperSkills[plant.thePlantType].Item2(plant);
                    plant.AnimSuperShoot();
                    __instance.UsedEvent(plant.thePlantColumn, plant.thePlantRow, cost);
                    __instance.OtherSuperSkill(plant);
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Mouse))]
    public static class MousePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LeftClickWithNothing")]
        public static void PostLeftClickWithNothing()
        {
            foreach (GameObject gameObject in (List<GameObject>)[..from RaycastHit2D raycastHit2D in
                                           (RaycastHit2D[])Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                           Vector2.zero)                          select raycastHit2D.collider.gameObject])
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
                else if (__instance.theGardenPlantOnGlove is not null && CustomCore.CustomPlantTypes.Contains(__instance.theGardenPlantOnGlove.thePlantType))
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
                __instance.preview = null;
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

    [BepInPlugin("inf75.customizelib", "CustomizeLib", "1.2")]
    public class CustomCore : BasePlugin
    {
        public static void AddFusion(int target, int item1, int item2) => CustomFusions.Add((target, item1, item2));

        public static void AddPlantAlmanacStrings(int id, string name, string description) => PlantsAlmanac.Add(id, (name, description));

        public static AssetBundle GetAssetBundle(Assembly assembly, string name)
        {
            try
            {
                using Stream stream = assembly.GetManifestResourceStream(assembly.FullName!.Split(",")[0] + "." + name) ?? assembly.GetManifestResourceStream(name)!;
                using MemoryStream stream1 = new();
                stream.CopyTo(stream1);
                var ab = AssetBundle.LoadFromMemory(stream1.ToArray());
                ArgumentNullException.ThrowIfNull(ab);
                BepInEx.Logging.Logger.CreateLogSource("CustomizeLib").LogInfo($"Successfully load AssetBundle {name}.");

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
                BepInEx.Logging.Logger.CreateLogSource("CustomizeLib").LogError($"Duplicate Plant ID: {id}");
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
                BepInEx.Logging.Logger.CreateLogSource("CustomizeLib").LogError($"Duplicate Plant ID: {id}");
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

        public static void RegisterSuperSkill([NotNull] int id, [NotNull] Func<Plant, int> cost, [NotNull] Action<Plant> skill) => SuperSkills.Add((PlantType)id, (cost, skill));

        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public static List<(int, int, int)> CustomFusions { get; set; } = [];
        public static Dictionary<PlantType, Action<Plant>> CustomPlantClicks { get; set; } = [];
        public static Dictionary<PlantType, CustomPlantData> CustomPlants { get; set; } = [];
        public static List<PlantType> CustomPlantTypes { get; set; } = [];
        public static Dictionary<(PlantType, BucketType), Action<Plant>> CustomUseItems { get; set; } = [];
        public static Dictionary<int, (string, string)> PlantsAlmanac { get; set; } = [];
        public static Dictionary<PlantType, (Func<Plant, int>, Action<Plant>)> SuperSkills { get; set; } = [];
    }
}