using HarmonyLib;
using System.Text.Json;
using TMPro;
using UnityEngine;

///
///Credit to likefengzi(https://github.com/likefengzi)(https://space.bilibili.com/237491236)
///
namespace CustomizeLib.BepInEx
{
    /// <summary>
    /// ��ʼ��������ʾ������ť������Ƥ��
    /// </summary>
    /// <param name="__instance"></param>
    /// <returns></returns>
    /// <summary>
    /// ֲ��ͼ��
    /// </summary>
    [HarmonyPatch(typeof(AlmanacPlantBank))]
    public static class AlmanacMgrPatch
    {
        /// <summary>
        /// ��ʼ��������ʾ������ť������Ƥ��
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void PostStart(AlmanacPlantBank __instance)
        {
            PlantType plantType = (PlantType)__instance.theSeedType;
            //���μ���Ƥ��
            if (!CustomCore.CustomPlantsSkin.ContainsKey(plantType))
            {
                //�Ƿ���Ƥ���ɹ�
                bool buttonFlag = __instance.skinButton.active;
                //exe��λ��
                string? fullName = Directory.GetParent(Application.dataPath)?.FullName;
                if (fullName != null)
                {
                    //Ѱ��Mods/Skin/
                    string modsPath = Path.Combine(fullName, "BepInEx", "plugins", "Skin");
                    if (Directory.Exists(modsPath))
                    {
                        //ֻҪskin_��ͷ���ļ�
                        string[] files = Directory.GetFiles(modsPath, "skin_*");

                        foreach (string file in files)
                        {
                            try
                            {
                                //����ļ���"Skin_"�����idƥ��
                                if (((int)plantType).ToString() == Path.GetFileName(file)[5..])
                                {
                                    //������Դ�ļ�
                                    AssetBundle ab = AssetBundle.LoadFromFile(file);
                                    //���Լ���json
                                    bool jsonFlag = false;
                                    CustomPlantData plantDataFromJson = default;
                                    CustomPlantAlmanac plantAlmanac = default;
                                    Dictionary<int, int> bulletTypesFormJson = [];
                                    foreach (string jsonFile in files)
                                    {
                                        try
                                        {
                                            if (((int)plantType) + ".json" ==
                                                Path.GetFileName(jsonFile)[5..])
                                            {
                                                // ��ȡ JSON �ļ�����
                                                string jsonContent = File.ReadAllText(jsonFile);

                                                // �����л� JSON ����
                                                var options = new JsonSerializerOptions
                                                {
                                                    PropertyNameCaseInsensitive = true // �������ִ�Сд����������ƥ��
                                                };

                                                JsonSkinObject? root =
                                                    JsonSerializer.Deserialize<JsonSkinObject>(jsonContent, options);

                                                // ��������
                                                if (root != null)
                                                {
                                                    plantDataFromJson = root.CustomPlantData;
                                                    root.TypeMgrExtraSkin.AddValueToTypeMgrExtraSkinBackup(plantType);
                                                    bulletTypesFormJson = root.CustomBulletType;
                                                    plantAlmanac = root.PlantAlmanac;
                                                }

                                                //�ҵ���json�ļ����ɹ�����
                                                jsonFlag = true;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }

                                    //�����Ƥ��Ԥ����
                                    GameObject? newPrefab = null;
                                    try
                                    {
                                        newPrefab = ab.GetAsset<GameObject>("Prefab");
                                        newPrefab.tag = "Plant";
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }

                                    //�����Ƥ��Ԥ��ͼ
                                    GameObject? newPreview = null;
                                    try
                                    {
                                        newPreview = ab.GetAsset<GameObject>("Preview");
                                        newPreview.tag = "Preview";
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }

                                    //�ɹ�����Ԥ����
                                    if (newPrefab != null)
                                    {
                                        //�ɵ�Ԥ����
                                        GameObject prefab;
                                        try
                                        {
                                            prefab = GameAPP.resourcesManager.plantPrefabs[jsonFlag ? (PlantType)plantDataFromJson.ID : plantType];
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                            prefab = GameAPP.resourcesManager.plantPrefabs[plantType];
                                        }

                                        //�õ��ű�
                                        Plant plant = prefab.GetComponent<Plant>();
                                        //��ӵ��µ�Ԥ������
                                        newPrefab.AddComponent(plant.GetIl2CppType());
                                        CustomPlantMonoBehaviour temp =
                                            newPrefab.AddComponent<CustomPlantMonoBehaviour>();
                                        CustomPlantMonoBehaviour.BulletTypes.Add(plantType, bulletTypesFormJson);

                                        Plant newPlant = newPrefab.GetComponent<Plant>();

                                        //ָ��id
                                        newPlant.thePlantType = plantType;

                                        //shoot��Ա�������⣬���
                                        newPlant.shoot = null;
                                        newPlant.shoot2 = null;
                                        //ָ��shoot
                                        try
                                        {
                                            newPlant.FindShoot(newPrefab.transform);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }

                                    CustomPlantData newCustomPlantData = default;
                                    //�ж��Ƿ�ɹ����ض�Ӧ��json
                                    if (jsonFlag)
                                    {
                                        //ʹ��json�е�����
                                        newCustomPlantData = new()
                                        {
                                            ID = (int)plantType,
                                            PlantData = plantDataFromJson.PlantData,
                                            Prefab = GameAPP.resourcesManager.plantPrefabs[plantType],
                                            Preview = GameAPP.resourcesManager.plantPreviews[plantType]
                                        };
                                    }
                                    else
                                    {
                                        //û��json�ļ���ʹ��Ĭ������
                                        //���ݼ��ص��Զ���Ƥ����
                                        newCustomPlantData = new()
                                        {
                                            ID = (int)plantType,
                                            PlantData = PlantDataLoader.plantDatas[plantType],
                                            Prefab = GameAPP.resourcesManager.plantPrefabs[plantType],
                                            Preview = GameAPP.resourcesManager.plantPreviews[plantType]
                                        };
                                    }

                                    //�ɹ���ȡ��˭�ͼ���˭
                                    if (newPrefab != null)
                                    {
                                        newCustomPlantData.Prefab = newPrefab;
                                    }

                                    if (newPreview != null)
                                    {
                                        newCustomPlantData.Preview = newPreview;
                                    }

                                    CustomCore.CustomPlantsSkin.Add(plantType, newCustomPlantData);
                                    //����ͼ��
                                    try
                                    {
                                        CustomCore.PlantsSkinAlmanac.Add(plantType, jsonFlag ?
                                            (plantAlmanac.Name, plantAlmanac.Description) : null);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }

                                    //��Ƥ������ť������ʾ
                                    buttonFlag = true;
                                    CustomCore.CustomPlantsSkinActive[plantType] = false;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                }

                __instance.skinButton.SetActive(buttonFlag);
            }
            else
            {
                //��Ƥ������ť������ʾ
                __instance.skinButton.SetActive(true);
            }

            if (CustomCore.CustomPlants.ContainsKey(plantType))
            {
                //����ֲ���ť������ʾ
                __instance.skinButton.SetActive(CustomCore.CustomPlantsSkin.ContainsKey(plantType));
            }
        }

        /// <summary>
        /// ��json����ֲ����Ϣ
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPatch("InitNameAndInfoFromJson")]
        [HarmonyPrefix]
        public static bool PreInitNameAndInfoFromJson(AlmanacPlantBank __instance)
        {
            //����Զ���ֲ��ͼ����Ϣ����
            if (CustomCore.PlantsAlmanac.ContainsKey((PlantType)__instance.theSeedType))
            {
                //����ͼ���ϵ����
                for (int i = 0; i < __instance.transform.childCount; i++)
                {
                    Transform childTransform = __instance.transform.GetChild(i);
                    if (childTransform == null)
                    {
                        continue;
                    }

                    //ֲ������
                    if (childTransform.name == "Name")
                    {
                        childTransform.GetComponent<TextMeshPro>().text =
                            CustomCore.PlantsAlmanac[(PlantType)__instance.theSeedType].Item1;
                        childTransform.GetChild(0).GetComponent<TextMeshPro>().text =
                            CustomCore.PlantsAlmanac[(PlantType)__instance.theSeedType].Item1;
                    }

                    //ֲ����Ϣ
                    if (childTransform.name == "Info")
                    {
                        TextMeshPro info = childTransform.GetComponent<TextMeshPro>();
                        info.overflowMode = TextOverflowModes.Page;
                        info.fontSize = 40;
                        info.text = CustomCore.PlantsAlmanac[(PlantType)__instance.theSeedType].Item2;
                        __instance.introduce = info;
                    }

                    //ֲ������
                    if (childTransform.name == "Cost")
                    {
                        childTransform.GetComponent<TextMeshPro>().text = "";
                    }
                }

                //���ԭʼ�ļ���
                return false;
            }

            if (CustomCore.CustomPlantsSkinActive.ContainsKey((PlantType)__instance.theSeedType) && CustomCore.PlantsSkinAlmanac.ContainsKey((PlantType)__instance.theSeedType) && CustomCore.CustomPlantsSkinActive[(PlantType)__instance.theSeedType])
            {
                var alm = CustomCore.PlantsSkinAlmanac[(PlantType)__instance.theSeedType];
                if (alm is null) return true;
                var almanac = alm.Value;
                //����ͼ���ϵ����
                for (int i = 0; i < __instance.transform.childCount; i++)
                {
                    Transform childTransform = __instance.transform.GetChild(i);
                    if (childTransform == null)
                    {
                        continue;
                    }

                    //ֲ������
                    if (childTransform.name == "Name")
                    {
                        childTransform.GetComponent<TextMeshPro>().text = almanac.Item1;
                        childTransform.GetChild(0).GetComponent<TextMeshPro>().text = almanac.Item1;
                    }

                    //ֲ����Ϣ
                    if (childTransform.name == "Info")
                    {
                        TextMeshPro info = childTransform.GetComponent<TextMeshPro>();
                        info.overflowMode = TextOverflowModes.Page;
                        info.fontSize = 40;
                        info.text = almanac.Item2;
                        __instance.introduce = info;
                    }

                    //ֲ������
                    if (childTransform.name == "Cost")
                    {
                        childTransform.GetComponent<TextMeshPro>().text = "";
                    }
                }

                //���ԭʼ�ļ���
                return false;
            }

            return true;
        }

        /// <summary>
        /// ͼ������갴�£����ڷ�ҳ
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPatch("OnMouseDown")]
        [HarmonyPrefix]
        public static bool PreOnMouseDown(AlmanacPlantBank __instance)
        {
            //�Ҳ���ʾ
            __instance.introduce =
                __instance.gameObject.transform.FindChild("Info").gameObject.GetComponent<TextMeshPro>();
            //ҳ��
            __instance.pageCount = __instance.introduce.m_pageNumber * 1;
            //��һҳ
            if (__instance.currentPage <= __instance.introduce.m_pageNumber)
            {
                ++__instance.currentPage;
            }
            else
            {
                __instance.currentPage = 1;
            }

            //��ҳ
            __instance.introduce.pageToDisplay = __instance.currentPage;

            //���ԭʼ��ҳ
            return false;
        }
    }

    [HarmonyPatch(typeof(AlmanacMgrZombie))]
    public static class AlmanacMgrZombiePatch
    {
        [HarmonyPatch("InitNameAndInfoFromJson")]
        [HarmonyPrefix]
        public static bool PreInitNameAndInfoFromJson(AlmanacMgrZombie __instance)
        {
            if (CustomCore.ZombiesAlmanac.ContainsKey(__instance.theZombieType))
            {
                for (int i = 0; i < __instance.transform.childCount; i++)
                {
                    Transform childTransform = __instance.transform.GetChild(i);
                    if (childTransform == null)
                        continue;
                    if (childTransform.name == "Name")
                    {
                        childTransform.GetComponent<TextMeshPro>().text = CustomCore.ZombiesAlmanac[__instance.theZombieType].Item1;
                        childTransform.GetChild(0).GetComponent<TextMeshPro>().text = CustomCore.ZombiesAlmanac[__instance.theZombieType].Item1;
                    }
                    if (childTransform.name == "Info")
                    {
                        TextMeshPro info = childTransform.GetComponent<TextMeshPro>();
                        info.overflowMode = TextOverflowModes.Page;
                        info.fontSize = 40;
                        info.text = CustomCore.ZombiesAlmanac[__instance.theZombieType].Item2;
                        __instance.introduce = info;
                    }
                    if (childTransform.name == "Cost")
                        childTransform.GetComponent<TextMeshPro>().text = "";
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CreatePlant), "SetPlant")]
    public static class CreatePlantPatch
    {
        public static void Postfix(ref GameObject __result)
        {
            if (__result is not null && __result.TryGetComponent<Plant>(out var plant) && CustomCore.CustomPlantTypes.Contains(plant.thePlantType))
            {
                TypeMgr.GetPlantTag(plant);
            }
        }
    }

    [HarmonyPatch(typeof(GameAPP))]
    public static class GameAPPPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LoadResources")]
        public static void LoadResources()
        {
            foreach (var plant in CustomCore.CustomPlants)
            {
                GameAPP.resourcesManager.plantPrefabs[plant.Key] = plant.Value.Prefab;
                GameAPP.resourcesManager.plantPrefabs[plant.Key].tag = "Plant";
                if (!GameAPP.resourcesManager.allPlants.Contains(plant.Key)) GameAPP.resourcesManager.allPlants.Add(plant.Key);
                if (plant.Value.PlantData is not null)
                {
                    PlantDataLoader.plantData[(int)plant.Key] = plant.Value.PlantData;//ע��ֲ������
                    PlantDataLoader.plantDatas.Add(plant.Key, plant.Value.PlantData);
                }
                GameAPP.resourcesManager.plantPreviews[plant.Key] = plant.Value.Preview;
                GameAPP.resourcesManager.plantPreviews[plant.Key].tag = "Preview";
            }
            Il2CppSystem.Array array = MixData.data.Cast<Il2CppSystem.Array>();
            foreach (var f in CustomCore.CustomFusions)
            {
                array.SetValue(f.Item1, f.Item2, f.Item3);
            }
            foreach (var z in CustomCore.CustomZombies)
            {
                if (!GameAPP.resourcesManager.allZombieTypes.Contains(z.Key)) GameAPP.resourcesManager.allZombieTypes.Add(z.Key);
                GameAPP.resourcesManager.zombiePrefabs[z.Key] = z.Value.Item1;
                GameAPP.resourcesManager.zombiePrefabs[z.Key].tag = "Zombie";
            }
            foreach (var bullet in CustomCore.CustomBullets)
            {
                GameAPP.resourcesManager.bulletPrefabs[bullet.Key] = bullet.Value;
                if (!GameAPP.resourcesManager.allBullets.Contains(bullet.Key)) GameAPP.resourcesManager.allBullets.Add(bullet.Key);
            }
            foreach (var par in CustomCore.CustomParticles)
            {
                GameAPP.particlePrefab[(int)par.Key] = par.Value;
                GameAPP.resourcesManager.particlePrefabs[par.Key] = par.Value;
                if (!GameAPP.resourcesManager.allParticles.Contains(par.Key)) GameAPP.resourcesManager.allParticles.Add(par.Key);
            }
            foreach (var spr in CustomCore.CustomSprites)
            {
                GameAPP.spritePrefab[spr.Key] = spr.Value;
            }
        }
    }

    [HarmonyPatch(typeof(Money))]
    public static class MoneyPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ReinforcePlant")]
        public static bool PreReinforcePlant(Money __instance, ref Plant plant)
        {
            if (CustomCore.SuperSkills.ContainsKey(plant.thePlantType))
            {
                var cost = CustomCore.SuperSkills[plant.thePlantType].Item1(plant);

                if (Board.Instance.theMoney < cost)
                {
                    InGameText.Instance.ShowText($"������Ҫ{cost}���", 5);
                    return false;
                }
                if (plant.SuperSkill())
                {
                    CustomCore.SuperSkills[plant.thePlantType].Item2(plant);
                    plant.AnimSuperShoot();
                    __instance.UsedEvent(plant.thePlantColumn, plant.thePlantRow, cost);
                    __instance.OtherSuperSkill(plant);
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Mouse))]
    public static class MousePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetPlantsOnMouse")]
        public static void PostGetPlantsOnMouse(ref Il2CppSystem.Collections.Generic.List<Plant> __result)
        {
            for (int i = __result.Count - 1; i >= 0; i--)
            {
                if (__result[i] is not null && TypeMgr.BigNut(__result[i].thePlantType))
                {
                    __result.RemoveAt(i);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("LeftClickWithNothing")]
        public static void PostLeftClickWithNothing()
        {
            foreach (GameObject gameObject in (List<GameObject>)[..from RaycastHit2D raycastHit2D in
                                           (RaycastHit2D[])Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                           Vector2.zero) select raycastHit2D.collider.gameObject])
            {
                if (gameObject.TryGetComponent<Plant>(out var plant) && CustomCore.CustomPlantClicks.ContainsKey(plant.thePlantType))
                {
                    CustomCore.CustomPlantClicks[plant.thePlantType](plant);
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(NoticeMenu), "Awake")]
    public static class NoticeMenuPatch
    {
        public static void Postfix() => CustomCore.Instance.Value.LateInit();
    }

    [HarmonyPatch(typeof(Plant))]
    public static class PlantPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UseItem")]
        public static void PostUseItem(Plant __instance, ref BucketType type, ref Bucket bucket)
        {
            if (CustomCore.CustomUseItems.ContainsKey((__instance.thePlantType, type)))
            {
                CustomCore.CustomUseItems[(__instance.thePlantType, type)](__instance);
                UnityEngine.Object.Destroy(bucket.gameObject);
            }
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    [HarmonyPatch(typeof(SkinButton), nameof(SkinButton.OnMouseUpAsButton))]
    public static class SkinButton_OnMouseUpAsButton
    {
        [HarmonyPrefix]
        public static bool Prefix(SkinButton __instance)
        {
            PlantType plantType = (PlantType)__instance.showPlant.theSeedType;
            if (CustomCore.CustomPlantsSkin.ContainsKey(plantType))
            {
                CustomPlantData customPlantData = CustomCore.CustomPlantsSkin[plantType];
                //����Ԥ��������
                (GameAPP.resourcesManager.plantPrefabs[plantType], customPlantData.Prefab) =
                    (customPlantData.Prefab, GameAPP.resourcesManager.plantPrefabs[plantType]);

                //����Ԥ��ͼ
                (GameAPP.resourcesManager.plantPreviews[plantType], customPlantData.Preview) =
                    (customPlantData.Preview, GameAPP.resourcesManager.plantPreviews[plantType]);

                //����ֲ������
                if (customPlantData.PlantData is not null)
                {
                    (PlantDataLoader.plantData[(int)plantType], customPlantData.PlantData) =
                        (customPlantData.PlantData, PlantDataLoader.plantData[(int)plantType]);
                    PlantDataLoader.plantDatas[plantType] = PlantDataLoader.plantData[(int)plantType];
                }
                CustomCore.CustomPlantsSkin[plantType] = customPlantData;

                //���������б�
                Extensions.SwapTypeMgrExtraSkinAndBackup(plantType);

                //GameObject prefab = GameAPP.resourcesManager.plantPrefabs[(PlantType)__instance.showPlant.theSeedType];

                //Transform transform = AlmanacMenu.Instance.currentShowCtrl.localShowPlant.transform.parent;

                //�ɵģ����������ݾ�����
                GameObject oldGameObject = AlmanacMenu.Instance.currentShowCtrl.localShowPlant;
                oldGameObject.name = "ToDestroy";
                // //ʵ�����µ�
                // AlmanacMenu.Instance.currentShowCtrl.localShowPlant = UnityEngine.Object.Instantiate(prefab, transform);
                // //ͬ��λ��
                // AlmanacMenu.Instance.currentShowCtrl.localShowPlant.transform.position =
                //     oldGameObject.transform.position;
                // AlmanacMenu.Instance.currentShowCtrl.localShowPlant.transform.localPosition =
                //     oldGameObject.transform.localPosition;

                //���پɵ�
                UnityEngine.Object.Destroy(oldGameObject);

                //����Ƿ񻻷�
                CustomCore.CustomPlantsSkinActive[plantType] = !CustomCore.CustomPlantsSkinActive[plantType];
                //__instance.showPlant.gameObject.SetActive(false);
                __instance.showPlant.InitNameAndInfoFromJson();
                AlmanacMenu.Instance.currentShowCtrl.localShowPlant =
                    AlmanacMenu.Instance.currentShowCtrl.SetPlant((int)plantType);

                if (AlmanacMenu.Instance.currentShowCtrl.localShowPlant.GetComponent<CustomPlantMonoBehaviour>() !=
                    null)
                {
                    UnityEngine.Object.Destroy(AlmanacMenu.Instance.currentShowCtrl.localShowPlant
                        .GetComponent<CustomPlantMonoBehaviour>());
                }

                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// ���������ı�Ⱦɫ
    /// </summary>
    [HarmonyPatch(typeof(TravelBuffOptionButton))]
    public static class TravelBuffOptionButtonPatch
    {
        [HarmonyPatch(nameof(TravelBuffOptionButton.SetBuff))]
        public static void PostSetBuff(TravelBuffOptionButton __instance, ref BuffType buffType, ref int buffIndex)
        {
            if (buffType is BuffType.AdvancedBuff && CustomCore.CustomAdvancedBuffs.ContainsKey(buffIndex)
                && CustomCore.CustomAdvancedBuffs[buffIndex].Item5 is not null)
            {
                __instance.introduce.text = $"<color={CustomCore.CustomAdvancedBuffs[buffIndex].Item5}>{__instance.introduce.text}</color>";
            }
        }
    }

    [HarmonyPatch(typeof(TravelBuff))]
    public static class TravelBuffPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ChangeSprite")]
        public static void PreChangeSprite(TravelBuff __instance)
        {
            if (__instance.theBuffType == 1 && CustomCore.CustomAdvancedBuffs.ContainsKey(__instance.theBuffNumber))
            {
                __instance.thePlantType = CustomCore.CustomAdvancedBuffs[__instance.theBuffNumber].Item1;
            }
            if (__instance.theBuffType == 2 && CustomCore.CustomUltimateBuffs.ContainsKey(__instance.theBuffNumber))
            {
                __instance.thePlantType = CustomCore.CustomUltimateBuffs[__instance.theBuffNumber].Item1;
            }
        }
    }

    /// <summary>
    /// ���������ı�Ⱦɫ
    /// </summary>
    [HarmonyPatch(typeof(TravelLookBuff))]
    public static class TravelLookBuffPatch
    {
        [HarmonyPatch(nameof(TravelLookBuff.SetBuff))]
        public static void PostSetBuff(TravelLookBuff __instance, ref BuffType buffType, ref int buffIndex)
        {
            if (buffType is BuffType.AdvancedBuff && CustomCore.CustomAdvancedBuffs.ContainsKey(buffIndex)
                && CustomCore.CustomAdvancedBuffs[buffIndex].Item5 is not null)
            {
                __instance.introduce.text = $"<color={CustomCore.CustomAdvancedBuffs[buffIndex].Item5}>{__instance.introduce.text}</color>";
            }
        }
    }

    [HarmonyPatch(typeof(TravelMgr))]
    public static class TravelMgrPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void PostAwake(TravelMgr __instance)
        {
            if (CustomCore.CustomAdvancedBuffs.Count > 0)
            {
                bool[] newAdv = new bool[__instance.advancedUpgrades.Count + CustomCore.CustomAdvancedBuffs.Count];
                int[] newAdvUnlock = new int[__instance.advancedUnlockRound.Count + CustomCore.CustomAdvancedBuffs.Count];
                Array.Copy(__instance.advancedUpgrades, newAdv, __instance.advancedUpgrades.Length);
                Array.Copy(__instance.advancedUnlockRound, newAdvUnlock, __instance.advancedUnlockRound.Length);
                __instance.advancedUpgrades = newAdv;
                __instance.advancedUnlockRound = newAdvUnlock;
            }
            if (CustomCore.CustomUltimateBuffs.Count > 0)
            {
                int[] newUlti = new int[__instance.ultimateUpgrades.Count + CustomCore.CustomUltimateBuffs.Count];
                Array.Copy(__instance.ultimateUpgrades, newUlti, __instance.ultimateUpgrades.Length);
                __instance.ultimateUpgrades = newUlti;
            }
            if (CustomCore.CustomDebuffs.Count > 0)
            {
                bool[] newdeb = new bool[__instance.debuff.Count + CustomCore.CustomDebuffs.Count];
                Array.Copy(__instance.debuff, newdeb, __instance.debuff.Length);
                __instance.debuff = newdeb;
            }
        }

        [HarmonyPatch("GetAdvancedBuffPool")]
        [HarmonyPostfix]
        public static void PostGetAdvancedBuffPool(ref Il2CppSystem.Collections.Generic.List<int> __result)
        {
            for (int i = __result.Count - 1; i >= 0; i--)
            {
                if (CustomCore.CustomAdvancedBuffs.ContainsKey(__result[i]) && !CustomCore.CustomAdvancedBuffs[__result[i]].Item3())
                {
                    __result.Remove(__result[i]);
                }
            }
        }

        [HarmonyPatch(nameof(TravelMgr.GetAdvancedText))]
        [HarmonyPostfix]
        public static void PostGetAdvancedText(ref int index, ref string __result)
        {
            if (CustomCore.CustomAdvancedBuffs.ContainsKey(index) && CustomCore.CustomAdvancedBuffs[index].Item5 is not null)
            {
                __result = $"<color={CustomCore.CustomAdvancedBuffs[index].Item5}>{__result}</color>";
            }
        }

        [HarmonyPatch(nameof(TravelMgr.GetPlantTypeByAdvBuff))]
        [HarmonyPostfix]
        public static void PostGetPlantTypeByAdvBuff(ref int index, ref PlantType __result)
        {
            if (CustomCore.CustomAdvancedBuffs.ContainsKey(index) && CustomCore.CustomAdvancedBuffs[index].Item1 is not PlantType.Nothing)
            {
                __result = CustomCore.CustomAdvancedBuffs[index].Item1;
            }
        }
    }

    [HarmonyPatch(typeof(TravelStore))]
    public static class TravelStorePatch
    {
        [HarmonyPatch("RefreshBuff")]
        [HarmonyPostfix]
        public static void PostRefreshBuff(TravelStore __instance)
        {
            foreach (var travelBuff in __instance.gameObject.GetComponentsInChildren<TravelBuff>())
            {
                if (travelBuff.theBuffType is (int)BuffType.AdvancedBuff && CustomCore.CustomAdvancedBuffs.ContainsKey(travelBuff.theBuffNumber))
                {
                    travelBuff.cost = CustomCore.CustomAdvancedBuffs[travelBuff.theBuffNumber].Item4;
                    travelBuff.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"��{CustomCore.CustomAdvancedBuffs[travelBuff.theBuffNumber].Item4}";
                }
                if (travelBuff.theBuffType is (int)BuffType.UltimateBuff && CustomCore.CustomUltimateBuffs.ContainsKey(travelBuff.theBuffNumber))
                {
                    travelBuff.cost = CustomCore.CustomUltimateBuffs[travelBuff.theBuffNumber].Item3;
                    travelBuff.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"��{CustomCore.CustomUltimateBuffs[travelBuff.theBuffNumber].Item4}";
                }
            }
        }
    }

    [HarmonyPatch(typeof(TypeMgr))]
    public static class TypeMgrPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("BigNut")]
        public static bool PreBigNut(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.BigNut.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("BigZombie")]
        public static bool PreBigZombie(ref ZombieType theZombieType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.BigZombie.Contains(theZombieType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("DoubleBoxPlants")]
        public static bool PreDoubleBoxPlants(ref PlantType thePlantType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.DoubleBoxPlants.Contains(thePlantType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("EliteZombie")]
        public static bool PreEliteZombie(ref ZombieType theZombieType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.EliteZombie.Contains(theZombieType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("FlyingPlants")]
        public static bool PreFlyingPlants(ref PlantType thePlantType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.FlyingPlants.Contains(thePlantType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetPlantTag")]
        public static bool PreGetPlantTag(ref Plant plant)
        {
            if (CustomCore.CustomPlantTypes.Contains(plant.thePlantType))
            {
                plant.plantTag = new()
                {
                    icePlant = TypeMgr.IsIcePlant(plant.thePlantType),
                    caltropPlant = TypeMgr.IsCaltrop(plant.thePlantType),
                    doubleBoxPlant = TypeMgr.DoubleBoxPlants(plant.thePlantType),
                    firePlant = TypeMgr.IsFirePlant(plant.thePlantType),
                    flyingPlant = TypeMgr.FlyingPlants(plant.thePlantType),
                    lanternPlant = TypeMgr.IsPlantern(plant.thePlantType),
                    smallLanternPlant = TypeMgr.IsSmallRangeLantern(plant.thePlantType),
                    magnetPlant = TypeMgr.IsMagnetPlants(plant.thePlantType),
                    nutPlant = TypeMgr.IsNut(plant.thePlantType),
                    tallNutPlant = TypeMgr.IsTallNut(plant.thePlantType),
                    potatoPlant = TypeMgr.IsPotatoMine(plant.thePlantType),
                    potPlant = TypeMgr.IsPot(plant.thePlantType),
                    puffPlant = TypeMgr.IsPuff(plant.thePlantType),
                    pumpkinPlant = TypeMgr.IsPumpkin(plant.thePlantType),
                    spickRockPlant = TypeMgr.IsSpickRock(plant.thePlantType),
                    tanglekelpPlant = TypeMgr.IsTangkelp(plant.thePlantType),
                    waterPlant = TypeMgr.IsWaterPlant(plant.thePlantType)
                };

                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsCaltrop")]
        public static bool PreIsCaltrop(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsCaltrop.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsFirePlant")]
        public static bool PreIsFirePlant(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsFirePlant.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsIcePlant")]
        public static bool PreIsIcePlant(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsIcePlant.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsMagnetPlants")]
        public static bool PreIsMagnetPlants(ref PlantType thePlantType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsMagnetPlants.Contains(thePlantType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsNut")]
        public static bool PreIsNut(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsNut.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsPlantern")]
        public static bool PreIsPlantern(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsPlantern.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsPot")]
        public static bool PreIsPot(ref PlantType thePlantType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsPot.Contains(thePlantType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsPotatoMine")]
        public static bool PreIsPotatoMine(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsPotatoMine.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsPuff")]
        public static bool PreIsPuff(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsPuff.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsPumpkin")]
        public static bool PreIsPumpkin(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsPumpkin.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsSmallRangeLantern")]
        public static bool PreIsSmallRangeLantern(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsSmallRangeLantern.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsSpecialPlant")]
        public static bool PreIsSpecialPlant(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsSpecialPlant.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsSpickRock")]
        public static bool PreIsSpickRock(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsSpickRock.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsTallNut")]
        public static bool PreIsTallNut(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsTallNut.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsTangkelp")]
        public static bool PreIsTangkelp(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsTangkelp.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IsWaterPlant")]
        public static bool PreIsWaterPlant(ref PlantType theSeedType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.IsWaterPlant.Contains(theSeedType))
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UmbrellaPlants")]
        public static bool PreUmbrellaPlants(ref PlantType thePlantType, ref bool __result)
        {
            if (CustomCore.TypeMgrExtra.UmbrellaPlants.Contains(thePlantType))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ZombieData))]
    public static class ZombieDataPatch
    {
        [HarmonyPatch("InitZombieData")]
        [HarmonyPostfix]
        public static void InitZombieData()
        {
            foreach (var z in CustomCore.CustomZombies)
            {
                ZombieData.zombieData[(int)z.Key] = z.Value.Item3;
            }
        }
    }
}