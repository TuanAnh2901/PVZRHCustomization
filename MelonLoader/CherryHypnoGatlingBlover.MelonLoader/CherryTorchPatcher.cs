using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(CherryTorchPatcher.Core), "CherryTorchPatcher", "1.0.0", "YourName", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
//[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace CherryTorchPatcher
{
    public class Core : MelonMod
    {
        // Dictionary để lưu trữ danh sách cherry flies không giới hạn cho mỗi CherryTorch
        public static Dictionary<CherryTorch, List<CherryLittleFly>> unlimitedCherryFlies = new Dictionary<CherryTorch, List<CherryLittleFly>>();

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Cherry Torch Patcher đã được load!");
            MelonLogger.Msg("- Fire times: 30 -> 1");
            MelonLogger.Msg("- Cherry flies: không giới hạn");
            MelonLogger.Msg("- Damage boost: +300 x số cherry flies");
        }
    }

    // Patch OnTriggerEnter2D để giảm fireTimes requirement
    [HarmonyPatch(typeof(CherryTorch), "OnTriggerEnter2D")]
    public class Patch_OnTriggerEnter2D
    {
        [HarmonyPrefix]
        static bool Prefix(CherryTorch __instance, Collider2D collision)
        {
            Bullet bullet = null;
            bool hasBullet = collision.TryGetComponent<Bullet>(out bullet);

            if (hasBullet)
            {
                bool canFire = __instance.CheckFire(bullet);
                if (canFire)
                {
                    __instance.fireTimes++;

                    // Giảm từ 30 xuống 1
                    if (__instance.fireTimes > 1)
                    {
                        // Gọi SummonPlant với damage gốc
                        __instance.SummonPlant(300);
                    }
                }
            }

            return false; // Skip original method
        }
    }

    // Patch SummonPlant để bỏ giới hạn 3 cherry flies
    [HarmonyPatch(typeof(CherryTorch), "SummonPlant")]
    public class Patch_SummonPlant
    {
        [HarmonyPrefix]
        static bool Prefix(CherryTorch __instance, int dmg)
        {
            // Khởi tạo list nếu chưa có
            if (!Core.unlimitedCherryFlies.ContainsKey(__instance))
            {
                Core.unlimitedCherryFlies[__instance] = new List<CherryLittleFly>();
            }

            // Dọn dẹp cherry flies đã chết
            Core.unlimitedCherryFlies[__instance].RemoveAll(fly => fly == null || fly.WasCollected);

            // Tạo cherry fly mới
            Transform shoot = __instance.shoot;
            GameObject cherryFlyPrefab = __instance.cherryFlyPrefab;

            Vector3 spawnPos = shoot.position;
            GameObject newFlyObj = UnityEngine.Object.Instantiate(
                cherryFlyPrefab,
                spawnPos,
                Quaternion.identity,
                __instance.board.transform
            );

            CherryLittleFly newFly = newFlyObj.GetComponent<CherryLittleFly>();
            if (newFly != null)
            {
                // Thêm vào list không giới hạn
                Core.unlimitedCherryFlies[__instance].Add(newFly);

                newFly.parentPlant = __instance;
                newFly.small = true;

                // Tính damage với bonus: dmg + 300 * số lượng cherry flies hiện tại
                int currentCount = Core.unlimitedCherryFlies[__instance].Count;
                int enhancedDamage = dmg + (300 * currentCount);
                newFly.dmg = enhancedDamage;

                // Random position offset
                newFly.positionOffset = UnityEngine.Random.Range(-0.5f, 0.5f);

                MelonLogger.Msg($"Triệu hồi Cherry Fly #{currentCount} với damage: {enhancedDamage}");
            }

            // Reset fire times
            __instance.fireTimes = 0;

            return false; // Skip original method
        }
    }

    // Patch DieEvent để dọn dẹp dictionary khi plant chết
    [HarmonyPatch(typeof(CherryTorch), "DieEvent")]
    public class Patch_DieEvent
    {
        [HarmonyPostfix]
        static void Postfix(CherryTorch __instance, Plant.DieReason reason)
        {
            // Dọn dẹp entry trong dictionary khi plant chết
            if (Core.unlimitedCherryFlies.ContainsKey(__instance))
            {
                int count = Core.unlimitedCherryFlies[__instance].Count(fly => fly != null);
                MelonLogger.Msg($"Cherry Torch chết, đã có {count} cherry flies được triệu hồi");
                Core.unlimitedCherryFlies.Remove(__instance);
            }
        }
    }

    // Optional: Patch để hiển thị thông tin trong game
    [HarmonyPatch(typeof(Plant), "Update")]
    public class Patch_PlantUpdate
    {
        [HarmonyPostfix]
        static void Postfix(Plant __instance)
        {
            // Chỉ xử lý CherryTorch
            if (__instance is CherryTorch cherryTorch)
            {
                if (Core.unlimitedCherryFlies.ContainsKey(cherryTorch))
                {
                    // Cập nhật damage cho tất cả cherry flies hiện có
                    var flies = Core.unlimitedCherryFlies[cherryTorch];
                    int validCount = flies.Count(fly => fly != null && !fly.WasCollected);

                    for (int i = 0; i < flies.Count; i++)
                    {
                        if (flies[i] != null && !flies[i].WasCollected)
                        {
                            // Cập nhật damage liên tục dựa trên số lượng hiện tại
                            flies[i].dmg = 300 + (300 * validCount);
                        }
                    }
                }
            }
        }
    }
}
