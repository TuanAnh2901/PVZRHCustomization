# PVZRHCustomization
ֲ���ս��ʬ�ںϰ����ֲ���뽩ʬ by [@Infinite75](https://space.bilibili.com/672619350)    
������Ϸ�汾2.6      
�ѹ����汾��������Release��    

����[MelonLoader](https://github.com/LavaGang/MelonLoader)��[BepInEx](https://github.com/BepInEx/BepInEx)����      

�ںϰ������飺    
[@��ƮƮfly](https://space.bilibili.com/3546619314178489) ���ڴ˴�������Ϸ����  
[@������˾](https://space.bilibili.com/85881762)   
[@����ѽ](https://space.bilibili.com/270840380)    
[@������Starryfly](https://space.bilibili.com/27033629)    

��л[@��Ʒ���](https://space.bilibili.com/237491236)(Github:[@likefengzi](https://github.com/likefengzi))�ļ���֧��  
��ͼ�滻����ʹ����[PvZ-Fusion-Blooms](https://github.com/Dynamixus/PvZ-Fusion-Blooms)�Ĵ��룬��лBlooms������      
��л[@�������ҷ�](https://space.bilibili.com/1117414477)(Github:[@LibraHp](https://github.com/LibraHp/))�ļ���֧��    

PVZ Fusion Customized Plants and Zombies    
Game Version: 2.6    
Please download in Release page    

Based on [MelonLoader](https://github.com/LavaGang/MelonLoader) and [BepInEx](https://github.com/BepInEx/BepInEx)     

PVZ Fusion Dev Team:     
[@��ƮƮfly](https://space.bilibili.com/3546619314178489)     
[@������˾](https://space.bilibili.com/85881762)   
[@����ѽ](https://space.bilibili.com/270840380)    
[@������Starryfly](https://space.bilibili.com/27033629)    


Thanks for [@��Ʒ���](https://space.bilibili.com/237491236)(Github:[@likefengzi](https://github.com/likefengzi))'s help 
Used codes of [PvZ-Fusion-Blooms](https://github.com/Dynamixus/PvZ-Fusion-Blooms) to replace texures, thanks for Blooms       
Thanks for [@�������ҷ�](https://space.bilibili.com/1117414477)(Github:[@LibraHp](https://github.com/LibraHp/))'s help 


### ��ͼ�滻 thanks to BloomsTeam
1.��װ����Ϸ��ǰ�úͶ�����������һ����Ϸ    
2.��ȡ��Ϸ�����Դ��ʹ��[AssetRipper](https://github.com/AssetRipper/AssetRipper/releases)����      
3.�ڽ����Դ���ҵ���Ҫ�޸ĵ���ͼ/����ͼ����PS������޸ģ�Ȼ����ͼ�������   
��Ϸ��Ŀ¼\BepInEx\plugins\Textures(b��) �� 
��Ϸ��Ŀ¼\Mods\Textures(m��)     
Ҫ�󣺲����Ը��ļ����������Ը�ͼƬ��С������ʹ��png��������psd,psb     
4.������Ϸ������滻�Ƿ���Ч     

����֤����ͼƬ������Ч��ĳЩ��ͼ�޸ĺ󳬳�ԭͼ�ǿ�����/��������Ĳ��ֿ��ܻᱻ�ص�

### ����Զ���ֲ�� made by [@likefengzi](https://github.com/likefengzi)

#### �ӵ�����

```C#

// Decompiled with JetBrains decompiler
// Type: Il2Cpp.BulletType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F4BA5295-8D17-4B4A-B7AA-4FDC00AA3B5E
// Assembly location: C:\STUDY\Code\Rider\PlantsVsZombiesRHCustomPlantsMelonLoader\libs\Il2CppAssemblies\Assembly-CSharp.dll

using Il2CppInterop.Common.Attributes;

#nullable disable
namespace Il2Cpp
{
  [OriginalName("Assembly-CSharp.dll", "", "BulletType")]
  public enum BulletType
  {
    Bullet_pea = 0,
    Bullet_cherry = 1,
    Bullet_nut = 2,
    Bullet_superCherry = 3,
    Bullet_zombieBlock = 4,
    Bullet_zombieBlock2 = 5,
    Bullet_zombieBlock3 = 6,
    Bullet_potato = 7,
    Bullet_smallSun = 8,
    Bullet_puff = 9,
    Bullet_puffPea = 10, // 0x0000000A
    Bullet_ironPea = 11, // 0x0000000B
    Bullet_threeSpike = 12, // 0x0000000C
    Bullet_puffRandomColor = 13, // 0x0000000D
    Bullet_puffLove = 14, // 0x0000000E
    Bullet_snowPea = 15, // 0x0000000F
    Bullet_snowPuffPea = 16, // 0x00000010
    Bullet_iceSpark = 17, // 0x00000011
    Bullet_smallIceSpark = 18, // 0x00000012
    Bullet_magicTrack = 20, // 0x00000014
    Bullet_snowPuff = 21, // 0x00000015
    Bullet_blackPuff = 22, // 0x00000016
    Bullet_doom = 23, // 0x00000017
    Bullet_iceDoom = 24, // 0x00000018
    Bullet_firePea_yellow = 25, // 0x00000019
    Bullet_firePea_orange = 26, // 0x0000001A
    Bullet_firePea_red = 27, // 0x0000001B
    Bullet_squash = 28, // 0x0000001C
    Bullet_kelp = 29, // 0x0000001D
    Bullet_fireKelp = 30, // 0x0000001E
    Bullet_squashKelp = 32, // 0x00000020
    Bullet_normalTrack = 33, // 0x00000021
    Bullet_iceTrack = 34, // 0x00000022
    Bullet_fireTrack = 35, // 0x00000023
    Bullet_cherrySquash = 36, // 0x00000024
    Bullet_cactus = 37, // 0x00000025
    Bullet_lanternCactus_glow = 38, // 0x00000026
    Bullet_star = 39, // 0x00000027
    Bullet_lanternStar = 40, // 0x00000028
    Bullet_seaStar = 41, // 0x00000029
    Bullet_cactusStar = 42, // 0x0000002A
    Bullet_jackboxStar = 43, // 0x0000002B
    Bullet_pickaxeStar = 44, // 0x0000002C
    Bullet_magnetStar = 45, // 0x0000002D
    Bullet_ironStar = 46, // 0x0000002E
    Bullet_magnetCactus = 47, // 0x0000002F
    Bullet_superStar = 48, // 0x00000030
    Bullet_ultimateStar = 49, // 0x00000031
    Bullet_firePea_small = 50, // 0x00000032
    Bullet_sword = 51, // 0x00000033
    Bullet_cabbage = 52, // 0x00000034
    Bullet_sunCabbage = 53, // 0x00000035
    Bullet_kernal = 54, // 0x00000036
    Bullet_butter = 55, // 0x00000037
    Bullet_bigKernal = 56, // 0x00000038
    Bullet_bigButter = 57, // 0x00000039
    Bullet_basketball = 58, // 0x0000003A
    Bullet_melon = 59, // 0x0000003B
    Bullet_winterMelon = 60, // 0x0000003C
    Bullet_garlicKernal = 61, // 0x0000003D
    Bullet_garlicButter = 62, // 0x0000003E
    Bullet_garlicCabbage = 63, // 0x0000003F
    Bullet_garlicMelon = 64, // 0x00000040
    Bullet_cannon = 65, // 0x00000041
    Bullet_cornMelon = 66, // 0x00000042
    Bullet_butterMelon = 67, // 0x00000043
    Bullet_fireCannon = 68, // 0x00000044
    Bullet_iceCannon = 69, // 0x00000045
    Bullet_cabbageMelon = 70, // 0x00000046
    Bullet_superMelon = 71, // 0x00000047
    Bullet_ultimateMelon = 72, // 0x00000048
    Bullet_melonCannon = 73, // 0x00000049
    Bullet_silverCabbage = 74, // 0x0000004A
    Bullet_goldCabbage = 75, // 0x0000004B
    Bullet_silverKernal = 76, // 0x0000004C
    Bullet_goldKernal = 77, // 0x0000004D
    Bullet_silverButter = 78, // 0x0000004E
    Bullet_goldButter = 79, // 0x0000004F
    Bullet_smallGoldCannon = 80, // 0x00000050
    Bullet_silverMelon = 81, // 0x00000051
    Bullet_goldMelon = 82, // 0x00000052
    Bullet_goldMelonCannon = 83, // 0x00000053
    Bullet_ultimateCannon = 84, // 0x00000054
    Bullet_ultimateKernal = 85, // 0x00000055
    Bullet_fireMelon = 86, // 0x00000056
    Bullet_firePea_super = 87, // 0x00000057
    Bullet_garlicBomb = 89, // 0x00000059
    Bullet_puffIronPea = 90, // 0x0000005A
    Bullet_lourCactus = 91, // 0x0000005B
    Bullet_puffPotato = 92, // 0x0000005C
    Bullet_steelPea = 93, // 0x0000005D
    Bullet_shulkLeaf = 94, // 0x0000005E
    Bullet_sunSpike = 95, // 0x0000005F
    Bullet_endoSun = 96, // 0x00000060
    Bullet_extremeSnowPea = 97, // 0x00000061
    Bullet_silverCoin = 98, // 0x00000062
    Bullet_goldCoin = 99, // 0x00000063
    Bullet_ultimateCactus = 100, // 0x00000064
    Bullet_snowBall = 101, // 0x00000065
    Bullet_doomCactus = 102, // 0x00000066
    Bullet_bigMelon = 103, // 0x00000067
    Bullet_fireStar = 104, // 0x00000068
    Bullet_fireCherry = 105, // 0x00000069
    Bullet_lanternCactus_dark = 106, // 0x0000006A
    Bullet_firePea_ultimate = 107, // 0x0000006B
    Bullet_redIronPea = 108, // 0x0000006C
    Bullet_normalSun = 109, // 0x0000006D
    Bullet_firePea_purple = 110, // 0x0000006E
    Bullet_springMelon = 111, // 0x0000006F
    Bullet_smallKernal = 112, // 0x00000070
    Bullet_smallButter = 113, // 0x00000071
    Bullet_portalPea = 114, // 0x00000072
    Bullet_goldSquashKelp = 115, // 0x00000073
    Bullet_ironPea_air = 116, // 0x00000074
    Bullet_kernal_magnet = 117, // 0x00000075
    Bullet_butter_magnet = 118, // 0x00000076
    Bullet_kernal_portal = 119, // 0x00000077
    Bullet_butter_portal = 120, // 0x00000078
    Bullet_kernal_iron = 121, // 0x00000079
    Bullet_butter_iron = 122, // 0x0000007A
    Bullet_pea_doom = 123, // 0x0000007B
    Bullet_pea_doom_fire = 124, // 0x0000007C
    Bullet_doom_fire = 125, // 0x0000007D
    Bullet_doom_big = 126, // 0x0000007E
    Bullet_spruce = 127, // 0x0000007F
    Bullet_spruceShulk = 128, // 0x00000080
    Bullet_icePeach = 129, // 0x00000081
    Bullet_passionFruit = 130, // 0x00000082
    Bullet_passionFruit2 = 131, // 0x00000083
    Bullet_water = 132, // 0x00000084
    Bullet_iceBlock = 133, // 0x00000085
    Bullet_water_big = 134, // 0x00000086
    Bullet_iceBlock_big = 135, // 0x00000087
    Bullet_shulkLeaf_water = 136, // 0x00000088
    Bullet_iceBlock_recover = 137, // 0x00000089
    Bullet_shulkLeaf_ice = 138, // 0x0000008A
    Bullet_lotusSpruce = 139, // 0x0000008B
    Bullet_kernal_ultimate = 140, // 0x0000008C
    Bullet_BlackHole_gold = 141, // 0x0000008D
  }
}

```

#### �ӵ��ƶ���ʽ

```C#

// Decompiled with JetBrains decompiler
// Type: Il2Cpp.BulletMoveWay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F4BA5295-8D17-4B4A-B7AA-4FDC00AA3B5E
// Assembly location: C:\STUDY\Code\Rider\PlantsVsZombiesRHCustomPlantsMelonLoader\libs\Il2CppAssemblies\Assembly-CSharp.dll

using Il2CppInterop.Common.Attributes;

#nullable disable
namespace Il2Cpp
{
  [OriginalName("Assembly-CSharp.dll", "", "BulletMoveWay")]
  public enum BulletMoveWay
  {
    //���е��ӵ�������ֲ������
    //��Ծ�����ƶ�BUG
    Jump = -1, // 0xFFFFFFFF
    //Ĭ��
    MoveRight = 0,
    //����
    Roll = 1,
    //û��������Ĭ�ϵ�����
    Free = 2,
    //�����ĸ����ʧ
    Puff = 3,
    //�������֣���·
    Three_up = 4,
    //�������֣���·
    Three_down = 5,
    //׷��
    Track = 6,
    //���У�ֻ�ܻ��п��н�ʬ
    Fly = 7,
    //��֪���������Ǹ���ʲô����
    Fly2 = 8,
    //�����
    Left = 9,
    //�ӵ�ͣ����ԭ��
    Stable = 10, // 0x0000000A
    //�ӵ�ͣ����ԭ�أ�û����������һ��������
    Rounding = 11, // 0x0000000B
    //������ж����Ի���
    LittleFly = 12, // 0x0000000C
    //�����ߣ����޸�
    Throw = 13, // 0x0000000D
    //��ũ�ڣ��޸����֣��Ѿ����Ի���
    Cannon = 14, // 0x0000000E
    //û��������Ĭ�ϵ�����
    Right_free = 15, // 0x0000000F
    //׷�٣�������ж����Ի��У�����û�˺�
    Convolute = 16, // 0x00000010
    //�����ߣ����޸�
    Quick_throw = 17, // 0x00000011
    //������ж����Ի���
    Freefly = 18, // 0x00000012
    //����򣬷���
    Split_left = 19, // 0x00000013
    //�Ե����˺����Կ�����
    Track_air_singleRow = 20, // 0x00000014
    //�ƶ��켣
    Sin = 21, // 0x00000015
    //�ƶ��켣
    Cos = 22, // 0x00000016
    //�ӵ�ͣ����ԭ�أ�û����������һ��������
    Pirouette = 23, // 0x00000017
  }
}

```

### �Զ�����Ƥ���ļ�

- Ƥ���ļ�����"skin_"+id

- Ԥ��������"Prefab"

- Ԥ��ͼ����"Preview"

- ·��Mods/Skin/

- json�ļ�����"skin_"+id+".json"

#### json�ļ���ʽ

json��֧��ע�ͣ���Ҫ��json�ļ���дע��

```C#
//�ļ����м��ID������Ҫ��Ƥ��ֲ��
{
  "CustomPlantData": {
    //ֲ��ID����Ҫ�����ֲ�ʹ�������߼�
    //���Ƥ��������ֲ��ѪԵ��ϵ̫Զ���п��ܳ�BUG
    //����ͨ�㶹���ֺͳ���ӣ���㶹�������֣���������һ����У��߼���û�в�̫��
    //�������ֺ�Ͷ�־Ͳ��̫���ˣ��ӵ��������
    //�����ֲ�ﻻ����������ֲ���Ҫ��������
    "ID": 931,
    //ֲ������
    "PlantData": {
      //������
      //���ܵ�����Ӱ�죬���յ��˺�����ƫ��
      "attackDamage": 100,
      //ֲ��ID
      //�������ͻ�Ƥֲ��һ��
      "field_Public_PlantType_0": 931,
      //�������
      //���ǹ�����ֲ���д0
      "field_Public_Single_0": 0,
      //�������
      //�������տ���ֲ���д0
      "field_Public_Single_1": 0,
      //MaxHP
      "field_Public_Int32_0": 100000,
      //CD
      "field_Public_Single_2": 20,
      //����۸�
      //����ǻ�����ֲ�һ��������ò�����
      "field_Public_Int32_1": 1000
    }
    //��ʵ����໹��������Ա�����Ǳ������գ���Ȼ�޷���ȡ
  },
  //�Զ������ʱ�õ��ӵ�
  //����д�ܶ࣬����CustomAnimShoot�������
  //key���ӵ����ͣ�value���ӵ��ƶ���ʽ
  "CustomBulletType": {
    "0": 0,
    "0": 0
  },
  //�������Щ���ݣ�0��ʾ��ֹ�����ԣ�1��ʾ���ø�����
  //-1��ʾʹ�û�Ƥֲ�������
  //�������е����Ըĳ�1������Ч����Щ���Ի��ǵô������
  "TypeMgrExtraSkin": {
    //����
    "BigNut": -1,
    //��ʬ
    "BigZombie": -1,
    //˫��ֲ��
    "DoubleBoxPlants": -1,
    //���佩ʬ
    "EliteZombie": -1,
    //����ֲ��
    "FlyingPlants": -1,
    //���н�ʬ
    "IsAirZombie": -1,
    //�ش�ֲ��
    "IsCaltrop": -1,
    //�Զ���ֲ��
    "IsCustomPlant": -1,
    //����ֲ��
    "IsFirePlant": -1,
    //����ֲ��
    "IsIcePlant": -1,
    //����ֲ��
    "IsMagnetPlants": -1,
    //���ֲ��
    "IsNut": -1,
    //����ֲ��
    "IsPlantern": -1,
    //����ֲ��
    "IsPot": -1,
    //����ֲ��
    "IsPotatoMine": -1,
    //С�繽��
    //����һ���������ǵͰ�ֲ��
    "IsPuff": -1,
    //�Ϲ�ֲ��
    "IsPumpkin": -1,
    //С����
    "IsSmallRangeLantern": -1,
    //����ֲ��
    "IsSpecialPlant": -1,
    //��ʱ��ֲ��
    "IsSpickRock": -1,
    //�߼��ֲ��
    "IsTallNut": -1,
    //���ƺ���
    "IsTangkelp": -1,
    //ˮ��ֲ��
    "IsWaterPlant": -1,
    //
    "NotRandomBungiZombie": -1,
    //
    "NotRandomZombie": -1,
    //������ʬ
    "UltimateZombie": -1,
    //ɡ��ֲ��
    "UmbrellaPlants": -1,
    //�����Ȼ�Ľ�ʬ
    "UselessHypnoZombie": -1,
    //ˮ����ʬ
    "WaterZombie": -1
  },
  //ͼ������
  "PlantAlmanac": {
    "ID": 3,
    "Name": "���ؿ�¶",
    "Description": "���ؿ�¶"
  }
}
```

#### AnimEvent

```C#
//û�в���
public void CustomAnimShoot(){}
//num�ǻ�Ѫ�ٷֱ�
public void CustomAnimHeal(float num)
```

