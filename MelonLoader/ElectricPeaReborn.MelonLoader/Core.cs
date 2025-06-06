using CustomizeLib.MelonLoader;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using Unity.VisualScripting;
using UnityEngine;
using static Il2CppSystem.Collections.Hashtable;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(ElectricPeaReborn.MelonLoader.Core), "ElectricPeaReborn", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace ElectricPeaReborn.MelonLoader
{
    [RegisterTypeInIl2Cpp]
    public class Bullet_electricPea : MonoBehaviour
    {
        public Bullet_electricPea() : base(ClassInjector.DerivedConstructorPointer<Bullet_electricPea>()) => ClassInjector.DerivedConstructorBody(this);

        public Bullet_electricPea(IntPtr i) : base(i)
        {
        }

        public void Start()
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

        public void Update()
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame)
            {
                bullet.normalSpeed = 3;
                var pos = bullet.transform.position;
                LayerMask layermask = bullet.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1.5f);
                foreach (var z in array)
                {
                    if (z is not null && !z.IsDestroyed() && z.TryGetComponent<Zombie>(out var zombie) && zombie is not null && !zombie.isMindControlled && !zombie.IsDestroyed())
                    {
                        zombie.TakeDamage(DmgType.Normal, bullet.Damage);
                        GameAPP.PlaySound(UnityEngine.Random.RandomRange(0, 3));
                        CreateParticle.SetParticle(53, new(zombie.axis.position.x, zombie.axis.position.y + 0.5f, zombie.axis.position.z), zombie.theZombieRow);
                        if (Lawnf.TravelAdvanced(ElectricPea.Buff))
                        {
                            zombie.BodyTakeDamage((int)(0.05 * (zombie.theHealth + zombie.theFirstArmorHealth + zombie.theSecondArmorHealth)));
                        }
                    }
                }
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        public Bullet bullet => gameObject.GetComponent<Bullet>();
    }

    public class Core : MelonMod//960
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "electricpea");
            CustomCore.RegisterCustomBullet<Bullet, Bullet_electricPea>((BulletType)903, ab.GetAsset<GameObject>("ProjectileElectricPea"));
            CustomCore.RegisterCustomPlant<Shooter, ElectricPea>(960, ab.GetAsset<GameObject>("ElectricPeaPrefab"),
                ab.GetAsset<GameObject>("ElectricPeaPreview"), [(1005, 3), (3, 1005)], 0.5f, 0, 1000, 32000, 1.5f, 1000);
            CustomCore.AddPlantAlmanacStrings(960, "Electric Pea", "Shoots an electric ball causing extremely fast damage\n<color=#3D1400>Image Author: </color>\n<color=#3D1400>Damage: </color><color=red>300/0.1s</color>\n<color=#3D1400>Fusion Recipe: </color><color=red>Double Cherry Pea + Wallnut</color>\n<color=#3D1400>Incredibly powerful, tearing through space with each ultra-powerful electric shot!</color>");
            
            ElectricPea.Buff = CustomCore.RegisterCustomBuff("电涌穿透：电能豌豆的子弹每次攻击对本体额外造成总血量5%的伤害", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<ElectricPea>(), 36100, "red", (PlantType)960);
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
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet((float)(plant.shoot.position.x + 0.1f), plant.shoot.position.y, plant.thePlantRow, (BulletType)903, 0);
            bullet.Damage = plant.attackDamage;
            return bullet;
        }

        public void Start()
        {
            plant.shoot = plant.gameObject.transform.FindChild("Shoot");
        }

        public static int Buff { get; set; } = -1;
        
        public Shooter plant => gameObject.GetComponent<Shooter>();
    }
}