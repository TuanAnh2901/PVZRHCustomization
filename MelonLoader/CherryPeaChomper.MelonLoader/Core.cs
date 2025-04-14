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
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet((float)(position.x + 0.1f), position.y, plant.thePlantRow, BulletType.Bullet_cherry, 0);
            bullet.Damage = plant.attackDamage;
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
            CreateBullet.Instance.SetBullet(pos.x - 5, pos.y, plant.thePlantRow, BulletType.Bullet_doom, 0).Damage = 99999;
            if (gameObject is not null)
            {
                Vector3 position = gameObject.GetComponent<Plant>().axis.transform.position;
                Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
            }
        }

        public PeaChomper plant => gameObject.GetComponent<PeaChomper>();
    }

    public class Core : MelonMod//301
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "cherrypeachomper");
            CustomCore.RegisterCustomPlant<PeaChomper, CherryPeaChomper>(301, ab.GetAsset<GameObject>("CherryPeaChomperPrefab"),
                ab.GetAsset<GameObject>("CherryPeaChomperPreview"), [(0, 1016), (1016, 0), (5, 1001), (1001, 5), (2, 1011), (1011, 2)], 3, 0, 900, 300, 7.5f, 400);
            CustomCore.AddPlantAlmanacStrings(301, "樱桃豌豆大嘴花", "咬僵尸时产生爆炸，咀嚼时会发射樱桃子弹。\n<color=#3D1400>美术组：@墨白秋影 @麦蔻杰沈 @暗影Dev @仨硝基甲苯_ @Infinite75</color>\n<color=#3D1400>伤害：</color><color=red>1800(爆炸)，900(樱桃子弹)</color>\n<color=#3D1400>融合配方：</color><color=red>豌豆射手+大嘴花+樱桃炸弹</color>\n<color=#3D1400>可能需要警惕下看起开很可疑的店家或是讯息，不然会重复卷心瓜的惨案，但“退化战神”是之后才听的警讯...</color>");
            CustomCore.AddFusion(903, 301, 1012);
            CustomCore.AddFusion(903, 1012, 301);
        }
    }
}