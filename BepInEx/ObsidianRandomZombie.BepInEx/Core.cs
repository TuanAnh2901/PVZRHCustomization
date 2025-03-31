using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;

namespace ObsidianRandomZombie.BepInEx
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
                Vector3 position = __instance.shadow.transform.position;
                List<int> ids = [212, 218, 219, 220, 221, 222, 223, 224];
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

    [HarmonyPatch(typeof(Zombie))]
    public static class ZombiePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void PostStart(Zombie __instance)
        {
            if (__instance.TryCast<UltimateGargantuar>() is not null && UnityEngine.Random.RandomRangeInt(0, 2) == 1)
            {
                CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)98, __instance.transform.position.x);
            }
        }

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

    [BepInPlugin("inf75.obsidianrandomzombie", "ObsidianRandomZombie", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<ObsidianRandomZombie>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "obsidianrandomzombie");
            CustomCore.RegisterCustomZombie<DiamondRandomZombie, ObsidianRandomZombie>(98,
                ab.GetAsset<GameObject>("ObsidianRandomZombie"), 206, 50, 40000, 4500, 0);
            CustomCore.RegisterCustomSprite(204, ab.GetAsset<Sprite>("ObsidianRandomZombie_head2"));
            CustomCore.RegisterCustomSprite(205, ab.GetAsset<Sprite>("ObsidianRandomZombie_head3"));
            CustomCore.RegisterCustomSprite(206, ab.GetAsset<Sprite>("ObsidianRandomZombie_0"));
            CustomCore.RegisterCustomSprite(207, ab.GetAsset<Sprite>("ObsidianRandomZombie_head1"));
            CustomCore.AddZombieAlmanacStrings(98, "黑曜石盲盒僵尸", "?????!!!!!\n\n<color=#3D1400>头套贴图作者：@林秋AutumnLin @暗影Dev</color>\n<color=#3D1400>韧性：</color><color=red>4500</color>\n<color=#3D1400>特点：</color><color=red>究极黑曜石巨人生成时有50%概率伴生。免疫击退，每隔一段时间自动换行，受到攻击时扣除与减伤前伤害等量钱币，究极机械保龄球替伤无效，死亡时变成随机领袖僵尸</color>\n<color=#3D1400>“小植物们，快来看我的另一个新发明，黑曜石盲盒，看起来很棒对不对，我觉得非常好，他不但无比坚硬，还很看运气。不过有也给了一个小小的礼物，让你一定玩的「开心」，还有，不要再用大嘴花解决我的发明了！！“ \n(埃德加博士留的)</color>");
        }
    }

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
                this.StartCoroutine(ChangeRoad());
            }
        }

        [HideFromIl2Cpp]
        public IEnumerator ChangeRoad()
        {
            do
            {
                yield return new WaitForSeconds(3);
                try
                {
                    if (Board.Instance is not null && zombie is not null && !zombie.isPreview && gameObject is not null
                        && !gameObject.IsDestroyed() && zombie is not null)
                    {
                        zombie.Garliced();
                    }
                    else
                    {
                        break;
                    }
                }
                catch { break; }
            } while (Board.Instance is not null && zombie is not null && zombie.isActiveAndEnabled);
        }

        public void Start()
        {
            zombie!.theFirstArmorType = Zombie.FirstArmorType.Doll;
            zombie.theZombieType = (ZombieType)98;
        }

        public DiamondRandomZombie? zombie => gameObject.TryGetComponent<DiamondRandomZombie>(out var z) ? z : null;
    }
}