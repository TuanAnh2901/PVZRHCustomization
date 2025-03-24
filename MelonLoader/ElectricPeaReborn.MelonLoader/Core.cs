using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using ElectricPeaReborn.MelonLoader;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using static Il2CppSystem.Collections.Hashtable;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(ElectricPeaReborn.MelonLoader.Core), "ElectricPeaReborn", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace ElectricPeaReborn.MelonLoader
{
    [HarmonyPatch(typeof(EletricPeaBullet), "Update")]
    public static class EletricPeaBulletPatch
    {
        public static bool Prefix(EletricPeaBullet __instance)
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame)
                __instance.DamageZombies();
            return false;
        }
    }

    public class Core : MelonMod//960
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "ElectricPeaReborn.MelonLoader.electricpea");
            /*
            var ab = AssetBundle.LoadFromFileAsync("Mods/electricpea");
            if (ab is null)
            {
                LoggerInstance.Error("Missing Resources!!!");
                return;
            }
            GameObject? prefab = null;
            GameObject? preview = null;
            foreach (var ase in ab.assetBundle.LoadAllAssets())
            {
                if (ase.TryCast<GameObject>()?.name is "ElectricPeaPrefab")
                {
                    prefab = ase.Cast<GameObject>();
                }
                if (ase.TryCast<GameObject>()?.name is "ElectricPeaPreview")
                {
                    preview = ase.TryCast<GameObject>()!;
                }
            }
            if (prefab is null || preview is null) return;*/
            //var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "electricpea");
            //GameAPP.bulletPrefab[50] = Resources.Load<GameObject>("bullet/prefabs/ProjectileElectricPea");
            //CustomCore.RegisterCustomPlant<Shooter, ElectricPea>(960, ab.GetAsset<GameObject>("ElectricPeaPrefab"),
            //    ab.GetAsset<GameObject>("ElectricPeaPreview"), [(1005, 1103), (1103, 1005)], 3, 0, 20, 300, 7.5f, 300);
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "electricpea");
            GameAPP.bulletPrefab[50] = Resources.Load<GameObject>("bullet/prefabs/ProjectileElectricPea");
            CustomCore.RegisterCustomPlant<Shooter, ElectricPea>(960, ab.GetAsset<GameObject>("ElectricPeaPrefab"),
                ab.GetAsset<GameObject>("ElectricPeaPreview"), [(1005, 3), (3, 1005)], 0.5f, 0, 1000, 32000, 1.5f, 1000);
            CustomCore.AddPlantAlmanacStrings(960, "Electric Pea", "Shoots an electric ball causing extremely fast damage\n<color=#3D1400>Image Author: </color>\n<color=#3D1400>Damage: </color><color=red>300/0.1s</color>\n<color=#3D1400>Fusion Recipe: </color><color=red>Double Cherry Pea + Wallnut</color>\n<color=#3D1400>Incredibly powerful, tearing through space with each ultra-powerful electric shot!</color>");

        }
    }

    [RegisterTypeInIl2Cpp]
    public class ElectricPea : MonoBehaviour
    {
        public ElectricPea() : base(ClassInjector.DerivedConstructorPointer<ElectricPea>()) => ClassInjector.DerivedConstructorBody(this);

        public ElectricPea(IntPtr i) : base(i)
        {
        }
        public static void SummonAndRecover(Plant plant)
        {
            if (plant.board.theMoney >= 3000)
            {
                plant.board.theMoney -= 3000;
                plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
                //GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)962, null, default, true);
                //if (plant.board.theMoney >= 70000)
                //{
                //    GameObject gameObject2 = CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)922, null, default, true);
                //}
                //if (gameObject is not null)
                //{
                //    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                //    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
                //}
                plant.isShort = true;
                plant.keepShooting = true;
            }
        }

        public Bullet AnimShooting()
        {
            Vector3 position = transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet((float)(position.x + 0.1f), position.y, plant.thePlantRow, 50, 0);
            bullet.theBulletDamage = 1000;
            return bullet;
        }

        public Shooter plant => gameObject.GetComponent<Shooter>();
    }
}