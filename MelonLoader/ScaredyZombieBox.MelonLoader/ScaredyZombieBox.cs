using CustomizeLib.MelonLoader;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using ScaredyZombieBox.MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: MelonInfo(typeof(Core), "ScaredyZombieBox", "1.0", "Infinite75", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace ScaredyZombieBox.MelonLoader
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(MelonAssembly.Assembly, "scaredyzombiebox");
            CustomCore.RegisterCustomPlant<ScaredyShroom, ScaredyZombieBox>(305, ab.GetAsset<GameObject>("ScaredyZombieBoxPrefab"),
                ab.GetAsset<GameObject>("ScaredyZombieBoxPreview"), [(250, 1024)], 0.8f, 0, 40, 300, 7.5f, 300);
            CustomCore.AddPlantAlmanacStrings(305, "僵尸盒子胆小菇(305)", "发射孢子，害怕时会缩头并生成一个魅惑黄金盲盒。\n<color=#3D1400>贴图作者：@林秋AutumnLin </color>\n<color=#3D1400>伤害：</color><color=red>40</color>\n<color=#3D1400>融合配方：</color><color=red>僵尸礼盒+魅惑胆小菇(有序)</color>\n<color=#3D1400>每当礼物胆小菇缩头时，身后都会传来一阵嘲笑，“我们知道是你” 。但这次却没有了笑声，因为在他的周围，全是僵尸礼盒...</color>");
        }
    }

    [RegisterTypeInIl2Cpp]
    public class ScaredyZombieBox : MonoBehaviour
    {
        public ScaredyZombieBox() : base(ClassInjector.DerivedConstructorPointer<ScaredyZombieBox>()) => ClassInjector.DerivedConstructorBody(this);

        public ScaredyZombieBox(IntPtr i) : base(i)
        {
        }

        public void AnimScared()
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.RandomPlusZombie, transform.position.x);
            Instantiate(GameAPP.particlePrefab[11], transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
        }

        public ScaredyShroom plant => gameObject.GetComponent<ScaredyShroom>();
    }
}