using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CustomizeLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(CherryHypnoGatlingBlover.MelonLoader.Core), "CherryHypnoGatlingBlover", "2.5.1", "Infinite75 & JustNull", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace CherryHypnoGatlingBlover.MelonLoader
{
	[RegisterTypeInIl2Cpp]
	public class Bullet_hypnocherry : MonoBehaviour
	{
        public Bullet_hypnocherry() : base(ClassInjector.DerivedConstructorPointer<Bullet_hypnocherry>()) => ClassInjector.DerivedConstructorBody(this);

        public Bullet_hypnocherry(IntPtr i) : base(i)
        {
        }

        public void Start()
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
        }

        public void Update()
        {
            if (GameAPP.theGameStatus is (int)GameStatus.InGame)
            {
                bullet.normalSpeed = 10;
				bullet.Damage = 400;
            }
        }
        
		public Bullet bullet => gameObject.GetComponent<Bullet>();
	}

	public class Core : MelonMod //965
	{
		public override void OnInitializeMelon()
		{
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomizeLib.MelonLoader.CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "cherryhypnogatlingblover");
            if(ab == null)
    {
                MelonLogger.Error("Failed to load asset bundle: cherryhypnogatlingblover. Make sure it is embedded and the name matches.");
                return;
            }
            CustomizeLib.MelonLoader.CustomCore.RegisterCustomPlant<Shooter, CherryHypnoGatlingBlover>(965, ab.GetAsset<GameObject>("CherryHypnoGatlingBloverPrefab"),
                ab.GetAsset<GameObject>("CherryHypnoGatlingBloverPreview"), [(1003, 1004), (1004, 1003)], 0.1f, 0, 400, 400000, 10, 300);
            MelonLogger.Msg("Registering Custom Bullet for Hypno Cherry");
            CustomizeLib.MelonLoader.CustomCore.RegisterCustomBullet<Bullet_HypnoCherry>((int)(BulletType)965, ab.GetAsset<GameObject>("Bullet_HypnoCherry"));
            CustomizeLib.MelonLoader.CustomCore.TypeMgrExtra.FlyingPlants.Add((PlantType)965);
            CustomizeLib.MelonLoader.CustomCore.AddPlantAlmanacStrings(965, "HypnoCherryDrone", "The enchanting Flying Cherry has a chance to charm zombies with its bullets and transform them into the Ultimate Machine Gun Reader\n<color=#3D1400>Image Author: Infinite75 & Miracle</color>\n<color=#3D1400>Damage: </color><color=red>400</color>\n<color=#3D1400>Fusion Recipe: </color><color=red>1003 + 1004</color>\n<color=#3D1400>If the attack speed of the plant below is less than 1.5s, it will synchronize the attack speed</color>\n<color=#FF0000>Has a chance to charm zombies with less than 40% health, and after being charmed, there’s a 10% chance to transform the zombie into the Charming Cherry Machine</color>\n<color=#FF0000>When removed, it will drop cards for both the Ultimate Cherry Shooter and the Charming Clover</color>\n\n<color=#905000>\"Show off, I’ll make you fly, did you hear that, little brat\" The Flying Cherry Shooter says this every day in front of other plants, but the other plants have never reduced their desire to let her fly on top of them</color>");
        }
	}

    //[RegisterTypeInIl2Cpp]
    //public class CherryHypnoBlover : MonoBehaviour
    //{
    //    public CherryHypnoBlover() : base(ClassInjector.DerivedConstructorPointer<CherryHypnoBlover>()) => ClassInjector.DerivedConstructorBody(this);

    //    public CherryHypnoBlover(IntPtr i) : base(i)
    //    {
    //    }
    //    public static void SummonAndRecover(Plant plant)
    //    {
    //        if (plant.board.theMoney >= 3000)
    //        {
    //            plant.board.theMoney += 3000;
    //            plant.Recover(Lawnf.TravelAdvanced(4) ? 999999 : 400000);
    //            //GameObject gameObject = CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, (PlantType)962, null, default, true);
    //            //if (plant.board.theMoney >= 70000)
    //            //{
    //            //    GameObject gameObject2 = CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)922, null, default, true);
    //            //}
    //            //if (gameObject is not null)
    //            //{
    //            //    Vector3 position = gameObject.GetComponent<Plant>().shadow.transform.position;
    //            //    Instantiate(GameAPP.particlePrefab[11], position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
    //            //}
    //            plant.isShort = true;
    //            plant.keepShooting = true;
    //        }
    //    }

    //    public Bullet AnimShooting()
    //    {
    //        Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet((float)(plant.shoot.position.x + 0.1f), plant.shoot.position.y, plant.thePlantRow, (BulletType)965, 0);
    //        bullet.Damage = plant.attackDamage;
    //        return bullet;
    //    }

    //    public void Start()
    //    {
    //        plant.shoot = plant.gameObject.transform.FindChild("Shoot");
    //    }


    //    public Shooter plant => gameObject.GetComponent<Shooter>();
    //}
}


