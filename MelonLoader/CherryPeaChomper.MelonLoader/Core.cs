using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(CherryPeaChomper.MelonLoader.Core), "CherryPeaChomper", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace CherryPeaChomper.MelonLoader
{
    [RegisterTypeInIl2Cpp]
    public class CherryPeaChomper : MonoBehaviour
    {
        public CherryPeaChomper() : base(ClassInjector.DerivedConstructorPointer<CherryPeaChomper>()) => ClassInjector.DerivedConstructorBody(this);

        public CherryPeaChomper(IntPtr i) : base(i)
        {
        }

        public Bullet AnimShooting()
        {
            Vector3 position = plant.shoot.transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet((float)(position.x + 0.1f), position.y, plant.thePlantRow, 3, 0);
            bullet.theBulletDamage = 900;
            return bullet;
        }

        public void Awake()
        {
            plant.shoot = transform.Find("Shoot");
        }

        public void Bite()
        {
            Instantiate(GameAPP.particlePrefab[2], plant.shoot.transform.position, Quaternion.identityQuaternion).GetComponent<BombCherry>().bombRow = plant.thePlantRow;
            ScreenShake.shakeDuration = 0.03f;
            GameAPP.PlaySound(40);
            GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)913, null, default, true); // 913 : Obsidian Nut
            plant.thePlantMaxHealth += plant.thePlantHealth / 5;
            plant.Recover(plant.thePlantMaxHealth);
            var pos = plant.shoot.transform.position;
            CreateBullet.Instance.SetBullet(pos.x - 5, pos.y, plant.thePlantRow, 24, 0).theBulletDamage = 99999;
            if (gameObject is not null)
            {
                Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
                Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
            }
        }

        public PeaChomper plant => gameObject.GetComponent<PeaChomper>();
    }

    public class Core : MelonMod//1901
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "cherrypeachomper");
            CustomCore.RegisterCustomPlant<PeaChomper, CherryPeaChomper>(1901, ab.GetAsset<GameObject>("CherryPeaChomperPrefab"),
                ab.GetAsset<GameObject>("CherryPeaChomperPreview"), [(0, 1016), (1016, 0), (5, 1001), (1001, 5), (2, 1011), (1011, 2)], 3, 0, 900, 50000, 0.5f, 400);
            CustomCore.AddPlantAlmanacStrings(1901, "Cherry Bomb Chomper", "Causes an explosion when biting zombies, and fires cherry bullets when chewing.\n<color=#3D1400>美术组：@墨白秋影 @麦蔻杰沈 @摆烂的克莱尔 @仨硝基甲苯 @Infinite75</color>\n<color=#3D1400>Damage：</color><color=red>1800(Explosion)，900(Bite)</color>\n<color=#3D1400>Fusion：</color><color=red>Pea + Cherry Bomb + Chomper</color>\n<color=#3D1400>You should be wary of suspicious-looking stores or messages, or else you may repeat the tragedy of the cabbage. But the Degenerated War God warning came later...</color>");
            CustomCore.AddFusion(903, 1901, 1012);
            CustomCore.AddFusion(903, 1012, 1901);
        }
    }
}