using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;

namespace SuperGarlicFume.BepInEx
{
    [BepInPlugin("inf75.supergarlicfume", "SuperGarlicFume", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SuperGarlicFume>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "supergarlicfume");
            CustomCore.RegisterCustomPlant<UltimateFume, SuperGarlicFume>(965, ab.GetAsset<GameObject>("SuperGarlicFumePrefab"),
                ab.GetAsset<GameObject>("SuperGarlicFumePreview"), [(904, 29)], 3, 0, 150, 300, 30, 700);
            CustomCore.AddFusion(904, 965, 8);
            CustomCore.AddPlantAlmanacStrings(965, "究极蒜大喷菇", "究极蒜大喷菇的孢子能同时造成减速和中毒效果。\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>究极大喷菇亚种，使用大蒜、魅惑菇切换。持续攻击，每0.5s对本行所有僵尸造成150+蒜值*20伤害并减速，同时对每个受到攻击的僵尸附加10点冻结值和1点蒜值</color>\n<color=#3D1400>融合配方：</color><color=red>究极大喷菇+大蒜</color>\n<color=#3D1400>在经历一切的事情后，他决定退休，看一些老电影，欣赏向日葵歌剧，甚至是安排退休生活，不过僵尸不会给他机会。</color>");
        }
    }

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
                    z.TakeDamage(DmgType.Ice, (150 + z.poisonLevel * 20) * (Lawnf.TravelUltimate(4) ? 3 : 1));
                    z.AddPoisonLevel();
                    z.SetCold(10);
                    z.AddfreezeLevel(10 * (Lawnf.TravelUltimate(5) ? 5 : 1));
                }
            }
        }

        public UltimateFume plant => gameObject.GetComponent<UltimateFume>();
    }
}