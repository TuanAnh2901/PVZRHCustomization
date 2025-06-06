using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

//[assembly: MelonInfo(typeof(CattailLourPatcher.Core), "CattailLourPatcher", "1.0.0", "YourName", null)]
//[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
//[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace CattailLourPatcher
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("CattailLour Patcher đã được load!");
        }
    }

    // Patch FirstMeet method
    [HarmonyPatch(typeof(CattailLour), "FirstMeet")]
    public class Patch_FirstMeet
    {
        [HarmonyPrefix]
        static bool Prefix(CattailLour __instance)
        {
            MelonLogger.Msg("CattailLour FirstMeet được gọi!");

            // Code gốc: Unlock achievement 56
            AchievementManager.UnlockAchievement((Achievement)56);

            // Thêm code custom của bạn ở đây
            // Ví dụ: Unlock thêm achievement khác
            // AchievementManager.UnlockAchievement((Achievement)57);

            return false; // Skip original method
        }

        //// Hoặc dùng Postfix nếu muốn chạy sau method gốc
        //[HarmonyPostfix]
        //static void Postfix(CattailLour __instance)
        //{
        //    // Code chạy sau khi FirstMeet gốc hoàn thành
        //}
    }

    // Patch Supply method
    //[HarmonyPatch(typeof(CattailLour), "Supply")]
    //public class Patch_Supply
    //{
    //    [HarmonyPrefix]
    //    static bool Prefix(CattailLour __instance)
    //    {
    //        MelonLogger.Msg("CattailLour Supply được gọi!");

    //        // Lấy các thuộc tính cần thiết
    //        Transform flyPos = __instance.flyPos;
    //        GameObject flyPrefab = __instance.flyPrefab;
    //        Board board = __instance.board;
    //        int plantRow = __instance.thePlantRow;

    //        if (flyPos == null || flyPrefab == null || board == null)
    //        {
    //            MelonLogger.Warning("Một trong các component cần thiết bị null!");
    //            return false;
    //        }

    //        // Tạo LourFly
    //        GameObject newFly = UnityEngine.Object.Instantiate(
    //            flyPrefab,
    //            flyPos.position,
    //            Quaternion.identity,
    //            board.transform
    //        );

    //        LourFly lourFly = newFly.GetComponent<LourFly>();
    //        if (lourFly != null)
    //        {
    //            lourFly.theRow = plantRow;
    //            lourFly.lour = __instance;

    //            // Thêm code custom
    //            // Ví dụ: Tăng kích thước
    //            // newFly.transform.localScale *= 1.5f;

    //            SpriteRenderer spriteRenderer = lourFly.GetComponent<SpriteRenderer>();
    //            if (spriteRenderer != null)
    //            {
    //                string layerName = string.Format("bullet{0}", plantRow);
    //                spriteRenderer.sortingLayerName = layerName;
    //                spriteRenderer.color = Color.red; // Đặt màu mặc định
    //                // Hoặc dùng spriteRenderer.material để thay đổi material nếu cần
    //                // spriteRenderer.material = someMaterial;


    //                // Thêm code custom cho sprite
    //                // Ví dụ: Đổi màu
    //                // spriteRenderer.color = Color.red;
    //            }

    //            MelonLogger.Msg($"Đã tạo LourFly tại row {plantRow}");
    //        }

    //        return false; // Skip original method
    //    }
    //}

    // Patch Shoot1 method
    [HarmonyPatch(typeof(CattailLour), "Shoot1")]
    public class Patch_Shoot1
    {
        [HarmonyPrefix]
        static bool Prefix(CattailLour __instance, ref Bullet __result)
        {
            //MelonLogger.Msg("CattailLour Shoot1 được gọi!");

            // Tìm shoot position
            Transform shootTransform = __instance.transform.Find("Shoot");
            if (shootTransform == null)
            {
                MelonLogger.Warning("Không tìm thấy Shoot transform!");
                __result = null;
                return false;
            }

            CreateBullet createBullet = CreateBullet.Instance;
            if (createBullet == null)
            {
                MelonLogger.Warning("CreateBullet.Instance là null!");
                __result = null;
                return false;
            }

            // Tính damage
            int damage = __instance.attackDamage;

            // Check travel advanced (nếu cần)
            if (!Lawnf.TravelAdvanced(35))
            {
                // Code xử lý khi chưa advanced
                // damage = damage / 2; // Ví dụ
            }

            // Thêm damage bonus custom
            // damage *= 2; // Ví dụ: x2 damage
            
            // Tạo bullet
            Bullet bullet = createBullet.SetBullet(
                shootTransform.position.x,
                shootTransform.position.y,
                __instance.thePlantRow,
                BulletType.Bullet_springMelon, // Hoặc type khác
                BulletMoveWay.Track, // bulletLayer
                false // isPlantBullet
            );

            bullet.Damage = damage * 2;
            bullet.normalSpeed *= 2; // Tốc độ bullet, có thể tùy chỉnh
            bullet.trackSpeed *= 3; // Tốc độ theo dõi, có thể tùy chỉnh
            bullet.accelerate = true; // Tăng tốc độ theo thời gian, có thể tùy chỉnh
            __instance.Supply(); // Gọi Supply để tạo LourFly nếu cần

            foreach (var z in Board.Instance.zombieArray)
            {
                if (z != null)
                {
                    z.AddfreezeLevel(100); // Thêm freeze level cho zombie
                    //z.freezeSpeed = 0.01f; // Giảm tốc độ di chuyển của zombie
                }
            }
            // Custom bullet properties
            // bullet.Speed *= 1.5f; // Ví dụ: tăng tốc độ

            // Play sound
            //GameAPP.PlaySound(0, 0.5f, 1f);

            // Custom sound
            // GameAPP.PlaySound(1, 0.7f, 1.2f); // Ví dụ: thêm sound

            __result = bullet;
            return false; // Skip original method
        }

        //// Hoặc Postfix để modify kết quả
        //[HarmonyPostfix]
        //static void Postfix(CattailLour __instance, ref Bullet __result)
        //{
        //    if (__result != null)
        //    {
        //        // Modify bullet sau khi được tạo
        //        // __result.Damage *= 2;
        //        __result.normalSpeed *= 2f; // Tăng tốc độ bullet
        //        __result.trackSpeed *= 3f; // Tăng tốc độ theo dõi
        //        __result.accelerate = true; // Bật tăng tốc độ theo thời gian
        //        __result.Damage = __instance.attackDamage * 2; // Tăng gấp đôi damage
        //        __result._damage = __result.Damage; // Cập nhật _damage nếu cần
        //        MelonLogger.Msg($"Bullet được bắn với damage: {__result.Damage}");
        //    }
        //}
    }

    //// Patch DieEvent method
    //[HarmonyPatch(typeof(CattailLour), "DieEvent")]
    //public class Patch_DieEvent
    //{
    //    [HarmonyPrefix]
    //    static void Prefix(CattailLour __instance, Plant.DieReason reason)
    //    {
    //        //MelonLogger.Msg($"CattailLour DieEvent được gọi với lý do: {reason}");

    //        // Thêm code custom trước khi chết
    //        // Ví dụ: Tạo explosion

    //            // CreateParticle.SetParticle(99, __instance.transform.position, __instance.thePlantRow, true);

    //            // Hoặc drop sun
    //            // Board.Instance.sun.CreateSun(__instance.transform.position.x, __instance.transform.position.y, 50, true);
    //        //Board.Instance.extraSun = 999999; // Giả lập drop sun vô hạn
    //        //Board.Instance.theSun = 999999; // Giả lập sun vô hạn
    //        //Board.Instance.thePoints = 999999; // Giả lập điểm vô hạn
    //        //Board.Instance.theMoney = 999999; // Giả lập tiền vô hạn
    //        //MelonLogger.Msg("Đã giả lập drop sun vô hạn và điểm vô hạn khi CattailLour chết.");


    //        // Gọi base DieEvent của CattailPlant
    //        // Vì method gốc throw exception, ta cần tự implement

    //        // Destroy các LourFly liên quan
    //        //LourFly[] allFlies = UnityEngine.Object.FindObjectsOfType<LourFly>();
    //        //foreach (var fly in allFlies)
    //        //{
    //        //    if (fly != null && fly.lour == __instance)
    //        //    {
    //        //        UnityEngine.Object.Destroy(fly.gameObject);
    //        //    }
    //        //}

    //        // Destroy plant
    //        //UnityEngine.Object.Destroy(__instance.gameObject);

    //        //return false; // Skip original method vì nó throw exception
    //    }

        // Hoặc dùng Finalizer để catch exception
        //[HarmonyFinalizer]
        //static Exception Finalizer(Exception __exception)
        //{
        //    if (__exception != null)
        //    {
        //        MelonLogger.Warning($"DieEvent gặp exception: {__exception.Message}");
        //        return null; // Suppress exception
        //    }
        //    return __exception;
        //}
    //}

    // Optional: Patch constructor để modify initial values
    //[HarmonyPatch(typeof(CattailLour), MethodType.Constructor)]
    //public class Patch_Constructor
    //{
    //    [HarmonyPostfix]
    //    static void Postfix(CattailLour __instance)
    //    {
    //        MelonLogger.Msg("CattailLour được khởi tạo!");

    //        // Modify dreamTime hoặc các giá trị khác
    //        __instance.dreamTime = 0.01f; // Giảm từ 0.1f xuống 0.05f
    //        __instance.alwaysLightUp = true; // Luôn sáng
    //        __instance.attributeCountdown = 0.01f; // Giảm thời gian countdown xuống 0.1 giây
    //        __instance.defence = 100000; // Tăng phòng thủ
    //        __instance.GiveSunInIZ(); // Giả lập cho LourFly nhận sun
    //        __instance.thePlantAttackCountDown = 0.01f; // Giảm thời gian tấn công xuống 0.01 giây
    //        __instance.thePlantAttackInterval = 0.01f; // Giảm khoảng thời gian tấn công xuống 0.01 giây
    //        __instance.thePlantMaxHealth = 100000; // Tăng máu tối đa
    //        __instance.theShieldHealth = 100000; // Tăng máu khi có shield


    //        // Thêm các modification khác
    //        // __instance.attackDamage *= 2;
    //        // __instance.hp *= 1.5f;
    //    }
    //}
}
