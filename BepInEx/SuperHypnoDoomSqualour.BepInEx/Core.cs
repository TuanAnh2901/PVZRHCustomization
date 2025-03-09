using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;

namespace SuperDoomSqualour.BepInEx
{
    [HarmonyPatch(typeof(Squalour), "LourDie")]
    public static class SqualourPatch
    {
        public static bool Prefix(Squalour __instance)
        {
            if (__instance.thePlantType is (PlantType)964)
            {
                __instance.Die();
                return false;
            }
            return true; ;
        }
    }

    [BepInPlugin("inf75.superdoomsqualour", "SuperDoomSqualour", "1.0")]
    public class Core : BasePlugin//964
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SuperDoomSqualour>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superdoomsqualour");
            CustomCore.RegisterCustomPlant<Squalour, SuperDoomSqualour>(964, ab.GetAsset<GameObject>("SuperDoomSqualourPrefab"),
                ab.GetAsset<GameObject>("SuperDoomSqualourPreview"), [(928, 248)], 3, 0, 3600, 300, 60f, 800);
            CustomCore.AddFusion(928, 964, 8);
            CustomCore.AddPlantAlmanacStrings(964, "幻灭猫瓜", "幻灭猫瓜从***那里获得了控制核能的力量，但最好不要让它再次红温。\n<color=#3D1400>贴图作者：@仨硝基甲苯</color>\n<color=#3D1400>特点：</color><color=red>幻灭菇亚种，使用猫瓜、魅惑菇切换。压扁僵尸时对全屏僵尸造成3600灰烬伤害，同时对所有僵尸附加死亡时毁灭爆炸效果</color>\n<color=#3D1400>融合配方：</color><color=red>幻灭菇+猫瓜</color>\n<color=#3D1400>别看幻灭猫瓜一副不好惹的模样，实际上她正在幻境中苦练技术，目前已经有了针对每一个僵尸的幻境，不过你得先将她从胜利的幻境中叫醒。</color>");
        }
    }

    public class SuperDoomSqualour : MonoBehaviour
    {
        public SuperDoomSqualour() : base(ClassInjector.DerivedConstructorPointer<SuperDoomSqualour>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperDoomSqualour(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.DisableDisMix();
        }

        public void SuperAttackZombie()
        {
            for (int i = Board.Instance.zombieArray.Count - 1; i >= 0; i--)
            {
                var z = Board.Instance.zombieArray[i];
                if (z is not null && !z.isMindControlled)
                {
                    if (z is not null && !z.IsDestroyed())
                        z.TakeDamage(DmgType.Explode, 3600);
                    if (z is not null && !z.IsDestroyed())
                    {
                        z.isDoom = true;
                        z.doomWithPit = false;
                        z.SetColor(Zombie.ZombieColor.Doom);
                    }
                }
            }
            Board.Instance.SetDoom(plant.thePlantColumn, plant.thePlantRow, true);
        }

        public Squalour plant => gameObject.GetComponent<Squalour>();
    }
}