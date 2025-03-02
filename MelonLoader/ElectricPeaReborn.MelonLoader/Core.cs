using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;

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

    public class Core : MelonMod
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
            GameAPP.bulletPrefab[50] = Resources.Load<GameObject>("bullet/prefabs/ProjectileElectricPea");
            CustomCore.RegisterCustomPlant<Shooter, ElectricPea>(960, ab.GetAsset<GameObject>("ElectricPeaPrefab"),
                ab.GetAsset<GameObject>("ElectricPeaPreview"), [(1005, 1103), (1103, 1005)], 3, 0, 20, 300, 7.5f, 300);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class ElectricPea : MonoBehaviour
    {
        public ElectricPea() : base(ClassInjector.DerivedConstructorPointer<ElectricPea>()) => ClassInjector.DerivedConstructorBody(this);

        public ElectricPea(IntPtr i) : base(i)
        {
        }

        public Bullet AnimShooting()
        {
            Vector3 position = transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet((float)(position.x + 0.1f), position.y, plant.thePlantRow, 50, 0);
            bullet.theBulletDamage = 20;
            return bullet;
        }

        public Shooter plant => gameObject.GetComponent<Shooter>();
    }
}