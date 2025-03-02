using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace ElectricPeaReborn.BepInEx
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

    [BepInPlugin("inf75.electricpea", "ElectricPeaReborn", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ClassInjector.RegisterTypeInIl2Cpp<ElectricPea>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ElectricPeaReborn.BepInEx.electricpea");
            GameAPP.bulletPrefab[50] = Resources.Load<GameObject>("bullet/prefabs/ProjectileElectricPea");
            CustomCore.RegisterCustomPlant<Shooter, ElectricPea>(960, ab.GetAsset<GameObject>("ElectricPeaPrefab"),
                ab.GetAsset<GameObject>("ElectricPeaPreview"), [(1005, 1103), (1103, 1005)], 3, 0, 20, 300, 7.5f, 300);
        }
    }

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

        public Plant plant => gameObject.GetComponent<Plant>();
    }
}