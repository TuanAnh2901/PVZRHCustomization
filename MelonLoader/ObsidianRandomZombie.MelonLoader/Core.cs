using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using ObsidianRandomZombie.MelonLoader;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "ObsidianRandomZombie", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace ObsidianRandomZombie.MelonLoader
{
    [HarmonyPatch(typeof(DiamondRandomZombie))]
    public static class DiamondRandomZombiePatch
    {
        [HarmonyPatch("FirstArmorBroken")]
        [HarmonyPrefix]
        public static bool PreFirstArmorBroken(DiamondRandomZombie __instance)
        {
            if (__instance.theZombieType is (ZombieType)98)
            {
                if (__instance.theFirstArmorHealth < __instance.theFirstArmorMaxHealth * 2 / 3)
                {
                    __instance.theFirstArmorBroken = 1;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[204];
                    return false;
                }
                if (__instance.theFirstArmorHealth < __instance.theFirstArmorMaxHealth / 3)
                {
                    __instance.theFirstArmorBroken = 2;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[205];
                    return false;
                }
                if (__instance.theFirstArmorHealth >= __instance.theFirstArmorMaxHealth * 2 / 3)
                {
                    __instance.theFirstArmorBroken = 0;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[207];
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch("SetRandomZombie")]
        [HarmonyPrefix]
        public static bool PreSetRandomZombie(DiamondRandomZombie __instance, ref GameObject __result)
        {
            if (__instance.theZombieType is (ZombieType)98)
            {
                Vector3 position = __instance.axis.position;
                List<int> ids = [];
                if (Lawnf.TravelDebuff(ObsidianRandomZombie.Debuff))
                {
                    for (int i = 0; i < GameAPP.zombiePrefab.Length; i++)
                    {
                        if (TypeMgr.IsBossZombie((ZombieType)i) && GameAPP.zombiePrefab[i] is not null)
                        {
                            ids.Add(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < GameAPP.zombiePrefab.Length; i++)
                    {
                        if (GameAPP.zombiePrefab[i] is not null && !TypeMgr.IsBossZombie((ZombieType)i) && !TypeMgr.NotRandomZombie((ZombieType)i))
                        {
                            ids.Add(i);
                        }
                    }
                }
                if (!__instance.isMindControlled)
                {
                    __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)ids[UnityEngine.Random.RandomRangeInt(0, ids.Count)], __instance.transform.position.x);
                }
                else
                {
                    __result = CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)ids[UnityEngine.Random.RandomRangeInt(0, ids.Count)], __instance.transform.position.x);
                }
                UnityEngine.Object.Instantiate(GameAPP.particlePrefab[11], new Vector3(__instance.transform.position.x, position.y + 1f, 0f), Quaternion.identity).transform.SetParent(GameAPP.board.transform);
                __instance.summoned = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Mower))]
    public static class MowerPatch
    {
        [HarmonyPatch("OnTriggerStay2D")]
        [HarmonyPrefix]
        public static bool PreOnTriggerStay2D(Mower __instance, ref Collider2D collision)
        {
            GameObject gameObject = collision.gameObject;
            if (gameObject.CompareTag("Zombie") && gameObject.TryGetComponent<ObsidianRandomZombie>(out var z) && z.zombie is not null
                && z.zombie.theZombieRow == __instance.theMowerRow && !z.zombie.isMindControlled)
            {
                if (!z.HasMower)
                {
                    GameObject mower = __instance.gameObject;
                    UnityEngine.Object.Destroy(mower.GetComponent<BoxCollider2D>());
                    UnityEngine.Object.Destroy(mower.GetComponent<Animator>());
                    UnityEngine.Object.Destroy(mower.GetComponent<Mower>());
                    mower.transform.SetParent(z.gameObject.transform.FindChild("Zombie_innerarm_hand"));
                    mower.transform.localPosition = new(-0.8f, -1.6f);
                    mower.layer = mower.transform.parent.gameObject.layer;
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Zombie))]
    public static class ZombiePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void PostStart(Zombie __instance)
        {
            if (__instance.TryCast<UltimateGargantuar>() is not null)
            {
                if (__instance.isMindControlled)
                {
                    CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)98, __instance.transform.position.x);
                }
                else
                {
                    CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)98, __instance.transform.position.x);
                }
            }
        }

        [HarmonyPatch("AttackEffect")]
        [HarmonyPrefix]
        public static bool PreAttackEffect(Zombie __instance, ref Plant plant)
        {
            if (__instance.theZombieType is (ZombieType)99 && __instance.gameObject.TryGetComponent<ObsidianRandomZombie>(out var z)
                && !__instance.isMindControlled && z.HasMower)
            {
                plant.Die();
                return false;
            }

            return true;
        }

        [HarmonyPatch("SetCold")]
        [HarmonyPatch("SetFreeze")]
        [HarmonyPatch("Warm")]
        [HarmonyPatch("KnockBack")]
        [HarmonyPrefix]
        public static bool PreKnockBack(Zombie __instance) => __instance.theZombieType is not (ZombieType)98;

        [HarmonyPatch("TakeDamage")]
        [HarmonyPrefix]
        public static void PreTakeDamage(Zombie __instance, ref int theDamage)
        {
            if (__instance.theZombieType is (ZombieType)98)
            {
                Board.Instance.theMoney -= theDamage;
            }
        }
    }

    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "obsidianrandomzombie");
            CustomCore.RegisterCustomZombie<DiamondRandomZombie, ObsidianRandomZombie>(98,
                ab.GetAsset<GameObject>("ObsidianRandomZombie"), 206, 50, 40000, 12000, 0);
            CustomCore.RegisterCustomSprite(204, ab.GetAsset<Sprite>("ObsidianRandomZombie_head2"));
            CustomCore.RegisterCustomSprite(205, ab.GetAsset<Sprite>("ObsidianRandomZombie_head3"));
            CustomCore.RegisterCustomSprite(206, ab.GetAsset<Sprite>("ObsidianRandomZombie_0"));
            CustomCore.RegisterCustomSprite(207, ab.GetAsset<Sprite>("ObsidianRandomZombie_head1"));
            ObsidianRandomZombie.Debuff = CustomCore.RegisterCustomBuff("黑曜石盲盒僵尸只开出领袖僵尸", BuffType.Debuff, () => true, 0);
            CustomCore.AddZombieAlmanacStrings(98, "黑曜石盲盒僵尸", "?????!!!!!\n\n<color=#3D1400>头套贴图作者：@林秋AutumnLin @E杯芒果奶昔 @暗影Dev</color>\n<color=#3D1400>韧性：</color><color=red>12000</color>\n<color=#3D1400>特点：</color><color=red>究极黑曜石巨人生成时伴生。免疫击退、冰冻、红温，遇到小推车时会将其拾起并回满血，此后啃咬植物直接代码杀，受到攻击时扣除与减伤前伤害等量钱币，究极机械保龄球替伤无效，死亡时变成随机非领袖僵尸</color>\n<color=#3D1400>词条：</color><color=red>黑曜石盲盒僵尸只开出领袖僵尸</color>\n<color=#3D1400>“小植物们，快来看我的另一个新发明，黑曜石盲盒，看起来很棒对不对，我觉得非常好，他不但无比坚硬，还很看运气。不过有也给了一个小小的礼物，让你一定玩的「开心」，还有，不要再用大嘴花解决我的发明了！！“ \n(埃德加博士留的)</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class ObsidianRandomZombie : MonoBehaviour
    {
        public ObsidianRandomZombie() : base(ClassInjector.DerivedConstructorPointer<ObsidianRandomZombie>()) => ClassInjector.DerivedConstructorBody(this);

        public ObsidianRandomZombie(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            if (GameAPP.theGameStatus is 0 && zombie is not null)
            {
                zombie.theFirstArmor = gameObject.transform.FindChild("Zombie_head").GetChild(0).gameObject;
                zombie.butterHead = zombie.theFirstArmor;
            }
        }

        public void Start()
        {
            zombie!.theFirstArmorType = Zombie.FirstArmorType.Doll;
            zombie.theZombieType = (ZombieType)98;
        }

        public static int Debuff { get; set; } = -1;
        public bool HasMower { get; set; } = false;
        public DiamondRandomZombie? zombie => gameObject.TryGetComponent<DiamondRandomZombie>(out var z) ? z : null;
    }
}