using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using SuperGarlicFume.MelonLoader;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(Core), "SuperGarlicFume", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SuperGarlicFume.MelonLoader
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "supergarlicfume");
            CustomCore.RegisterCustomPlant<UltimateFume, SuperGarlicFume>(965, ab.GetAsset<GameObject>("SuperGarlicFumePrefab"),
                ab.GetAsset<GameObject>("SuperGarlicFumePreview"), [(904, 29)], 3, 0, 150, 300, 30, 700);
            CustomCore.AddFusion(904, 965, 8);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)965);
            SuperGarlicFume.Buff1 = CustomCore.RegisterCustomBuff("满屋蒜香：究极蒜大喷菇攻击时取消僵尸蒜值上限", BuffType.UltimateBuff, (PlantType)965, ab.GetAsset<Sprite>("SuperGarlicFume_0"));
            SuperGarlicFume.Buff2 = CustomCore.RegisterCustomBuff("超剧毒：究极蒜大喷菇可以对僵尸额外造成总血量*蒜值%的伤害", BuffType.UltimateBuff, (PlantType)965, ab.GetAsset<Sprite>("SuperGarlicFume_0"));
            CustomCore.AddPlantAlmanacStrings(965, "究极蒜大喷菇", "究极蒜大喷菇的孢子能同时造成减速和中毒效果。\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>究极大喷菇亚种，使用大蒜、魅惑菇切换。持续攻击，每0.5s对本行所有僵尸造成150+蒜值*20伤害并减速，同时对每个受到攻击的僵尸附加10点冻结值和1点蒜值</color>\n<color=#3D1400>融合配方：</color><color=red>究极大喷菇+大蒜</color>\n<color=#3D1400>\n<color=#3D1400>词条1：</color><color=red>凛风刺骨：攻击力×3</color>\n<color=#3D1400>词条2：</color><color=red>三尺之寒：冻结值积累速度×5</color>\n<color=#3D1400>词条3：</color><color=red>满屋蒜香：究极蒜大喷菇攻击时取消僵尸蒜值上限</color>\n<color=#3D1400>词条4：</color><color=red>超剧毒：究极蒜大喷菇可以对僵尸额外造成总血量*蒜值%的真实伤害</color>\n<color=#3D1400>在经历一切的事情后，他决定退休，看一些老电影，欣赏向日葵歌剧，甚至是安排退休生活，不过僵尸不会给他机会。</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class SuperGarlicFume : MonoBehaviour
    {
        public SuperGarlicFume() : base(ClassInjector.DerivedConstructorPointer<SuperGarlicFume>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperGarlicFume(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.emission.enabled = false;
            plant.DisableDisMix();
            var tag = plant.plantTag;
            tag.icePlant = true;
            plant.plantTag = tag;
        }

        public void StartShoot()
        {
            plant.emission.enabled = true;
        }

        public void StopShoot()
        {
            plant.emission.enabled = false;
        }

        public void SuperAttackZombie()
        {
            plant.zombieList.Clear();
            foreach (var z in Board.Instance.zombieArray)
            {
                if (z is not null && !z.IsDestroyed() && !z.isMindControlled && !TypeMgr.IsAirZombie(z.theZombieType) && z.theZombieRow == plant.thePlantRow && z.shadow.transform.position.x > plant.shadow.transform.position.x)
                {
                    plant.zombieList.Add(z);
                }
            }
            foreach (var z in plant.zombieList)
            {
                if (z is not null && !z.IsDestroyed() && !z.isMindControlled)
                {
                    z.TakeDamage(DmgType.IceAll, (150 + z.poisonLevel * 20 + z.theMaxHealth * (Lawnf.TravelUltimate(Buff2) ? z.poisonLevel / 100 : 0)) * (Lawnf.TravelUltimate(4) ? 3 : 1));
                    if (Lawnf.TravelUltimate(Buff2))
                    {
                        z.TakeDamage(DmgType.IceAll, z.theMaxHealth * z.poisonLevel / 100, true);
                    }
                    if (Lawnf.TravelUltimate(Buff1))
                    {
                        z.SetPoison(10);
                    }
                    else
                    {
                        z.AddPoisonLevel();
                    }
                    z.SetCold(10);
                    z.AddfreezeLevel(10 * (Lawnf.TravelUltimate(5) ? 5 : 1));
                }
            }
        }

        public static int Buff1 { get; set; } = -1;
        public static int Buff2 { get; set; } = -1;

        public UltimateFume plant => gameObject.GetComponent<UltimateFume>();
    }
}