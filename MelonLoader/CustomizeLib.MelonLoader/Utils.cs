using Unity.VisualScripting;
using UnityEngine;

namespace CustomizeLib.MelonLoader
{
    public struct CustomPlantData
    {
        public int ID { get; set; }
        public PlantDataLoader.PlantData_ PlantData { get; set; }
        public GameObject Prefab { get; set; }
        public GameObject Preview { get; set; }
    }

    public static class Extensions
    {
        public static void DisableDisMix(this Plant plant) => (plant.firstParent, plant.secondParent) = (PlantType.Nothing, PlantType.Nothing);

        //递归，找shoot，但是一些奇怪的植物不行
        public static void FindShoot(this Plant plant, Transform parent)
        {
            // 遍历当前对象的所有组件
            Component[] components = parent.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component.name == "Shoot" || component.name == "Shoot1")
                {
                    plant.shoot = component.transform;
                }

                if (component.name == "Shoot2")
                {
                    plant.shoot2 = component.transform;
                }
            }

            // 递归遍历所有子对象
            for (int i = 0; i < parent.childCount; i++)
            {
                plant.FindShoot(parent.GetChild(i));
            }
        }

        public static T GetAsset<T>(this AssetBundle ab, string name) where T : UnityEngine.Object
        {
            foreach (var ase in ab.LoadAllAssetsAsync().allAssets)
            {
                if (ase.TryCast<T>()?.name == name)
                {
                    return ase.Cast<T>();
                }
            }
            throw new ArgumentException($"Could not find {name} from {ab.name}");
        }

        public static int GetTotalHealth(this Zombie zombie) => (int)zombie.theHealth + zombie.theFirstArmorHealth + zombie.theSecondArmorHealth;

        public static bool ObjectExist<T>(this Board board) => board.GameObject().transform.GetComponentsInChildren<T>().Length > 0;
    }
}