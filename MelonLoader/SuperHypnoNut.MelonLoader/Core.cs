using CustomizeLib;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using SuperHypnoNut.MelonLoader;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "SuperHypnoNut", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace SuperHypnoNut.MelonLoader
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superhypnonut");
            CustomCore.RegisterCustomPlant<Squalour>(969, ab.GetAsset<GameObject>("SuperHypnoNutPrefab"),
                ab.GetAsset<GameObject>("SuperHypnoNutPreview"), [(905, 8)], 3, 0, 3600, 300, 60f, 800);
        }
    }
}