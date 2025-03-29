using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppTMPro;
using MelonLoader;
using MilkyBlover.MelonLoader;
using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(Core), "MilkyBlover", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace MilkyBlover.MelonLoader
{
    [HarmonyPatch(typeof(Blover), "AnimBlow")]
    public static class BloverPatch
    {
        public static void Postfix(Blover __instance)
        {
            if (__instance.gameObject.TryGetComponent<MilkyBlover>(out var p) && Lawnf.TravelAdvanced(45))
            {
                MelonCoroutines.Start(p.CreateStar());
            }
        }
    }

    [HarmonyPatch(typeof(InitBoard), "RightMoveCamera")]
    public static class InitBoardPatch
    {
        public static void Postfix()
        {
            var slot = GameObject.Find("CustomPeashooter");
            if (slot.transform.GetChildCount() > 0)
            {
                for (int i = 0; i < slot.transform.GetChildCount(); i++)
                {
                    UnityEngine.Object.Destroy(slot.transform.GetChild(i).gameObject);
                }
            }
            var template = GameObject.Find("MixBomb");
            var cardBg = template.transform.GetChild(0).gameObject;
            var card = template.transform.GetChild(1).gameObject;
            var mkbBg = UnityEngine.Object.Instantiate(cardBg, slot.transform);
            Lawnf.ChangeCardSprite((PlantType)169, mkbBg);
            mkbBg.GetComponent<Image>().sprite = GameAPP.spritePrefab[208];
            mkbBg.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = PlantDataLoader.plantData[169].field_Public_Int32_1.ToString();
            if (Board.Instance is not null && (Board.Instance.boardTag.enableTravelPlant || Board.Instance.boardTag.enableAllTravelPlant || GameAPP.developerMode))
            {
                var mkb = UnityEngine.Object.Instantiate(card, slot.transform);
                Lawnf.ChangeCardSprite((PlantType)169, mkb);
                mkb.GetComponent<Image>().sprite = GameAPP.spritePrefab[208];
                mkb.GetComponent<CardUI>().parent = slot;
                mkb.GetComponent<CardUI>().CD = PlantDataLoader.plantData[169].field_Public_Single_2;
                mkb.GetComponent<CardUI>().theSeedCost = PlantDataLoader.plantData[169].field_Public_Int32_1;
                mkb.GetComponent<CardUI>().thePlantType = (PlantType)169;
                mkb.GetComponent<CardUI>().theZombieType = (ZombieType)169;
            }
        }
    }

    public class Core : MelonMod//169
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "milkyblover");
            CustomCore.RegisterCustomPlant<Blover, MilkyBlover>(169, ab.GetAsset<GameObject>("MilkyBloverPrefab"),
                ab.GetAsset<GameObject>("MilkyBloverPreview"), [], 3, 0, 80, 300, 60f, 500);
            CustomCore.RegisterCustomSprite(208, ab.GetAsset<Sprite>("SeedPacket_MilkyBlover"));
            CustomCore.AddPlantAlmanacStrings(169, "兔子三叶草", "似乎只是个比较可爱的三叶草...吗???\n<color=#3D1400>贴图作者：@Just Eris</color>\n<color=#3D1400>特点：</color><color=red>二创彩蛋植物，不参与融合，一般情况下同三叶草，当词条星神合一解锁时每0.3s召唤一个阳光陨星，3s后消失</color>\n<color=#3D1400>花费：</color><color=red>500</color>\n<color=#3D1400>冷却时间：</color><color=red>60s</color>\n<color=#3D1400></color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class MilkyBlover : MonoBehaviour
    {
        public MilkyBlover() : base(ClassInjector.DerivedConstructorPointer<MilkyBlover>()) => ClassInjector.DerivedConstructorBody(this);

        public MilkyBlover(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public IEnumerator CreateStar()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (plant is not null && !plant.IsDestroyed() && Board.Instance is not null && !Board.Instance.IsDestroyed() && Lawnf.TravelAdvanced(45))
                    {
                        Board.Instance.CreateUltimateMateorite();
                    }
                    else
                    {
                        break;
                    }
                }
                catch { break; }
                yield return new WaitForSeconds(0.3f);
            }
        }

        public Blover? plant => gameObject.TryGetComponent<Blover>(out var p) ? p : null;
    }
}