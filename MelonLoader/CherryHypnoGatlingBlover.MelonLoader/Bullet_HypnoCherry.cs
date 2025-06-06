using System;
using System.Runtime.CompilerServices;
using Il2Cpp;
using MelonLoader;

namespace CherryHypnoGatlingBlover.MelonLoader
{
	// Token: 0x02000005 RID: 5
	[RegisterTypeInIl2Cpp]
	public class Bullet_HypnoCherry : Bullet_superCherry
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002120 File Offset: 0x00000320
		public override void HitZombie(Zombie zombie)
		{
            zombie.TakeDamage(0, 300, false);
            CreateParticle.SetParticle(37, zombie.ColliderPosition, zombie.theZombieRow, true).transform.SetParent(GameAPP.board.transform);
            if ((double)((zombie.theHealth + (float)zombie.theSecondArmorHealth + (float)zombie.theFirstArmorHealth) / (float)(zombie.theMaxHealth + zombie.theSecondArmorMaxHealth + zombie.theFirstArmorMaxHealth)) <= 0.9 && new Random().Next(0, 100) < 70)
            {
                zombie.SetMindControl(0);
                if (new Random().Next(0, 100) < 45)
                {
                    CreateParticle.SetParticle(99, zombie.ColliderPosition, zombie.theZombieRow, true).transform.SetParent(GameAPP.board.transform);
                    CreateZombie.Instance.SetZombie(zombie.theZombieRow, (ZombieType)49, zombie.ColliderPosition.x, false).GetComponent<Zombie>().SetMindControl(0);
                    zombie.Die(0);
                }
            }
            base.Die();
        }
	}
}
