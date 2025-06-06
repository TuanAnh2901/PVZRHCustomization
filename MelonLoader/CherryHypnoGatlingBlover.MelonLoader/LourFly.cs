using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

//[assembly: MelonInfo(typeof(LourFlyPatcher.Core), "LourFlyPatcher", "1.0.0", "YourName", null)]
//[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace LourFlyPatcher
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("LourFly Patcher đã được load!");
            MelonLogger.Msg("- Bắn 3 viên đạn theo 3 hướng khác nhau");
            MelonLogger.Msg("- Tăng damage và tốc độ");
        }
    }

    // Patch ShootUpdate method để tạo 3 viên đạn theo 3 hướng
    [HarmonyPatch(typeof(LourFly), "ShootUpdate")]
    public class Patch_ShootUpdate
    {
        [HarmonyPrefix]
        static bool Prefix(LourFly __instance)
        {
            if (__instance.shootCount == 0) __instance.shootCount = 0;
            // Cập nhật timer
            __instance.timer += Time.deltaTime;

            // Kiểm tra nếu đã đến lúc bắn (timer > 0.1f * random)
            float shootInterval = UnityEngine.Random.Range(0.95f, 1.05f) * 0.1f;

            if (__instance.timer >= shootInterval)
            {
                // Reset timer
                __instance.timer = 0f;

                // Lấy vị trí bắn
                Transform shootPos = __instance.shootPos;
                if (shootPos == null)
                {
                    MelonLogger.Warning("ShootPos is null!");
                    return false;
                }

                // Lấy CreateBullet instance
                CreateBullet createBullet = CreateBullet.Instance;
                if (createBullet == null)
                {
                    MelonLogger.Warning("CreateBullet.Instance is null!");
                    return false;
                }

                // Tính damage
                int baseDamage = 0;
                if (__instance.lour != null)
                {
                    // Kiểm tra travel advanced
                    if (Lawnf.TravelAdvanced(35)) // Giả sử index 35, có thể cần điều chỉnh
                    {
                        baseDamage = __instance.lour.attackDamage * 5;
                    }
                    else
                    {
                        baseDamage = __instance.lour.attackDamage;
                    }
                }

                // Tăng damage
                int enhancedDamage = baseDamage * 2;

                // Tạo 3 viên đạn với các hướng khác nhau
                Vector3 shootPosition = shootPos.position;

                // Bullet 1: Hướng thẳng (y offset = 0)
                CreateBullet1(__instance, createBullet, shootPosition, 0f, enhancedDamage);

                // Bullet 2: Hướng trên (y offset = +0.5)
                CreateBullet1(__instance, createBullet, shootPosition, 0.5f, enhancedDamage);

                // Bullet 3: Hướng dưới (y offset = -0.5)
                CreateBullet1(__instance, createBullet, shootPosition, -0.5f, enhancedDamage);

                //MelonLogger.Msg($"LourFly bắn 3 viên đạn với damage: {enhancedDamage}");

                __instance.shootCount++;

                // Kiểm tra nếu đã bắn đủ 30 lần thì hủy
                if (__instance.shootCount >= 30)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    return false;
                }
            }

            // Destroy LourFly sau khi bắn (giống code gốc)
            //UnityEngine.Object.Destroy(__instance.gameObject);

            return false; // Skip original method
        }

        private static void CreateBullet1(LourFly lourFly, CreateBullet createBullet, Vector3 basePosition, float yOffset, int damage)
        {
            // Tạo bullet tại vị trí có offset
            Bullet bullet = createBullet.SetBullet(
                basePosition.x,
                basePosition.y + yOffset,
                lourFly.theRow,
                //BulletType.Bullet_lourCactus, // Sử dụng bullet type phù hợp (Default: Bullet_lourCactus)
                BulletType.Bullet_springMelon, // Hoặc Bullet_lourCactus nếu muốn
                BulletMoveWay.Track, // Hoặc Track nếu muốn tracking
                false // isZombieShoot = false
            );

            if (bullet != null)
            {
                // Set damage
                bullet.Damage = damage;

                // Lấy component Bullet_lourCactus nếu có
                Bullet_lourCactus lourCactusComponent = bullet.GetComponent<Bullet_lourCactus>();
                if (lourCactusComponent != null && lourFly.lour != null)
                {
                    lourCactusComponent.lour = lourFly.lour;
                }

                // Tăng tốc độ bullet
                bullet.normalSpeed *= 3f;

                // Nếu có tracking speed thì cũng tăng
                if (bullet.trackSpeed > 0)
                {
                    bullet.trackSpeed *= 3f;
                }

                // Set rigidbody velocity nếu cần (cho hướng khác nhau)
                //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                //if (rb != null && yOffset != 0)
                //{
                //    // Thêm velocity theo trục Y để tạo hướng bắn khác nhau
                //    Vector2 currentVelocity = rb.velocity;
                //    rb.velocity = new Vector2(currentVelocity.x, currentVelocity.y + yOffset * 2f);
                //}
            }
        }
    }

    // Optional: Patch PositionUpdate để tăng tốc độ di chuyển
    //[HarmonyPatch(typeof(LourFly), "PositionUpdate")]
    //public class Patch_PositionUpdate
    //{
    //    [HarmonyPrefix]
    //    static bool Prefix(LourFly __instance)
    //    {
    //        Transform transform = __instance.transform;
    //        Vector3 currentPosition = transform.position;
    //        Vector2 targetPosition = __instance.targetPosition;

    //        // Tăng tốc độ di chuyển từ 3f lên 6f
    //        float moveSpeed = Time.deltaTime * 6f;

    //        // Tính toán vị trí mới
    //        Vector3 newPosition = Vector3.MoveTowards(
    //            currentPosition,
    //            new Vector3(targetPosition.x, targetPosition.y, currentPosition.z),
    //            moveSpeed
    //        );

    //        // Cập nhật vị trí
    //        transform.position = newPosition;

    //        // Kiểm tra đã đến target chưa
    //        float distance = Vector2.Distance(
    //            new Vector2(currentPosition.x, currentPosition.y),
    //            targetPosition
    //        );

    //        if (distance <= 0.1f || moveSpeed > 1f)
    //        {
    //            __instance.arrived = true;
    //            transform.position = new Vector3(targetPosition.x, targetPosition.y, currentPosition.z);
    //        }

    // Điều chỉnh hướng dựa trên vị trí target
    //if (targetPosition.y > currentPosition.y)
    //{
    //    // Di chuyển lên trên - có thể thêm animation hoặc rotation
    //    transform.rotation = Quaternion.Euler(0, 0, 10f);
    //}
    //else if (targetPosition.y < currentPosition.y)
    //{
    //    // Di chuyển xuống dưới
    //    transform.rotation = Quaternion.Euler(0, 0, -10f);
    //}
    //else
    //{
    //    // Di chuyển thẳng
    //    transform.rotation = Quaternion.identity;
    ////}

    //return false; // Skip original method
    //    }
    //}

    // Optional: Patch Update để tối ưu logic
    [HarmonyPatch(typeof(LourFly), "Update")]
    public class Patch_Update
    {
        [HarmonyPrefix]
        static bool Prefix(LourFly __instance)
        {
            // Nếu chưa arrived thì tiếp tục di chuyển
            if (!__instance.arrived)
            {
                __instance.PositionUpdate();
            }
            else
            {
                // Đã arrived thì bắn và tự hủy
                __instance.ShootUpdate();
                // Thêm điều kiện hủy (ví dụ sau 30 lần bắn)
                if (__instance.shootCount >= 30)
                {
                    __instance.shootCount = 0; // Reset count nếu cần
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }

            return false; // Skip original method
        }
    }

    // Optional: Patch constructor để modify initial values
    [HarmonyPatch(typeof(LourFly), MethodType.Constructor)]
    public class Patch_Constructor
    {
        [HarmonyPostfix]
        static void Postfix(LourFly __instance)
        {
            // Reset timer để bắn nhanh hơn
            __instance.timer = 0f;

            MelonLogger.Msg("LourFly được khởi tạo với cài đặt custom!");
        }
    }
}
