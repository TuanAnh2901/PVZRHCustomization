using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using MelonLoader.InternalUtils;
using System.Reflection;
using UltimateWinterMelonCannon.MelonLoader;
using Unity.VisualScripting;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "UltimateWinterMelonCannon", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace UltimateWinterMelonCannon.MelonLoader
{
    [HarmonyPatch(typeof(LittleGoldCannonBullet))]
    public static class LittleGoldCannonBulletPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitLand")]
        public static bool PreHitLand(LittleGoldCannonBullet __instance)
        {
            if (__instance.theBulletType == UltimateWinterMelonCannon.BulletId)
            {
                if (!Lawnf.TravelAdvanced(UltimateWinterMelonCannon.Buff1))
                {
                    CreateParticle.SetParticle(200, new(__instance.cannonPos.x, __instance.cannonPos.y), __instance.theBulletRow);
                    var pos = __instance.transform.position;
                    LayerMask layermask = __instance.zombieLayer.m_Mask;
                    var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                    foreach (var z in array)
                    {
                        if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !TypeMgr.IsAirZombie(zombie.theZombieType) && !zombie.isMindControlled)
                        {
                            zombie.TakeDamage(DmgType.IceAll, __instance.theBulletDamage * (Lawnf.TravelUltimate(14) ? 3 : 1));
                            zombie.SetFreeze(10);
                            zombie.AddfreezeLevel(400);
                            zombie.SetCold(30);
                        }
                    }
                    GameAPP.PlaySound(UnityEngine.Random.RandomRangeInt(104, 106));
                    __instance.Die();
                }
                else
                {
                    Board.Instance.SetDoom(Mouse.Instance.GetColumnFromX(__instance.transform.position.x), __instance.theBulletRow, false, true, default, 3600, 1);
                    __instance.Die();
                }
                return false;
            }
            return true;
        }
    }

    public class Core : MelonMod//968
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatewintermeloncannon");
            CustomCore.RegisterCustomPlant<MelonCannon, UltimateWinterMelonCannon>(968, ab.GetAsset<GameObject>("UltimateWinterMelonCannonPrefab"),
                ab.GetAsset<GameObject>("UltimateWinterMelonCannonPreview"), [(915, 32)], 24, 24, 450, 1000, 60f, 1200);
            CustomCore.RegisterCustomBullet<LittleGoldCannonBullet>(UltimateWinterMelonCannon.BulletId, ab.GetAsset<GameObject>("ProjectileCannon_UltimateWinterMelon"));
            CustomCore.RegisterCustomParticle(200, ab.GetAsset<GameObject>("CannonUltimateWinterMelonSplat"));
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)968);
            CustomCore.TypeMgrExtra.DoubleBoxPlants.Add((PlantType)968);
            CustomCore.AddFusion(915, 968, 28);
            CustomCore.AddPlantAlmanacStrings(968, "冰毁瓜加农炮", "手动发射寒冰毁灭西瓜，范围全屏\n<color=#3D1400>贴图作者：@林秋AutumnLin @摆烂的克莱尔</color>\n<color=#3D1400>特点：</color><color=red>究极加农炮亚种，使用西瓜投手、玉米投手切换。点击发射60个伤害450的寒冰毁灭西瓜子弹，范围全屏，受击僵尸冻结15s</color>\n<color=#3D1400>融合配方：</color><color=red>究极加农炮+西瓜投手</color>\n<color=#3D1400>词条1：</color><color=red>兵贵神速：装填时间降为10秒</color>\n<color=#3D1400>词条2：</color><color=red>中心爆破：单个子弹伤害提升至1350</color>\n<color=#3D1400>词条3：</color><color=red>真正的冰毁瓜：每个冰毁瓜子弹落地时直接生成3600伤害的寒冰毁灭菇爆炸，范围全屏(解锁条件：解锁了词条1、2且场上存在冰毁瓜加农炮)</color>\n<color=#3D1400>“包装自己的最有效策略？”冰毁西瓜炮低声说道：“首先要冷静，谦逊……然后，从沉默中醒来——轰！整个世界都会记住你的名字。”</color>");
            UltimateWinterMelonCannon.Buff1 = CustomCore.RegisterCustomBuff("真正的冰毁瓜：每个冰毁瓜子弹落地时直接生成3600伤害的寒冰毁灭菇爆炸，范围全屏", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<UltimateWinterMelonCannon>() && Lawnf.TravelUltimate(14) && Lawnf.TravelUltimate(15), 28800, "red", (PlantType)968);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class UltimateWinterMelonCannon : MonoBehaviour
    {
        public UltimateWinterMelonCannon() : base(ClassInjector.DerivedConstructorPointer<UltimateWinterMelonCannon>()) => ClassInjector.DerivedConstructorBody(this);

        public UltimateWinterMelonCannon(IntPtr i) : base(i)
        {
        }

        public void AnimShooting()
        {
            GameAPP.PlaySound(4, 1.0f);
            var RowFromY = Mouse.Instance.GetRowFromY(plant.target.x, plant.target.y);
            var bullet = plant.board.GetComponent<CreateBullet>().SetBullet(plant.shoot.transform.position.x, plant.shoot.transform.position.y, RowFromY, BulletId, 14);
            var pos2 = bullet.cannonPos;
            pos2.x = plant.target.x;
            pos2.y = plant.target.y;
            bullet.cannonPos = pos2;
            bullet.rb.velocity = new(1.5f, 0);
            bullet.specitalType = BulletStatus.GoldMelon_cannon;
            bullet.theBulletDamage = plant.attackDamage;
        }

        public void Awake()
        {
            plant.DisableDisMix();
            plant.shoot = gameObject.transform.FindChild("Shoot");
        }

        public void Update()
        {
            if (Lawnf.TravelUltimate(15) && plant.attributeCountdown > 10f)
            {
                plant.attributeCountdown = 10f;
            }
        }

        public static int Buff1 { get; set; } = -1;
        public static int BulletId { get; set; } = 126;

        public MelonCannon plant => gameObject.GetComponent<MelonCannon>();
    }
}