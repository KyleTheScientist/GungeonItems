using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemAPI
{
    class ExampleGun
    {
        public static void Init()
        {
            Gun baseGun = Gungeon.Game.Items["ak47"] as Gun;
            foreach(var c in baseGun.GetComponents<Component>())
            {
                ETGModConsole.Log(c.GetType().Name);
            }
            
            Gun gun = CustomGun.GenerateGun(baseGun, "AK-48").GetComponent<Gun>();
            ETGModConsole.Log("gun made");

            int spriteID = CustomGun.SetupSprite(gun, "ak48", "ak48/idle/AK-48_000");
            ETGModConsole.Log("sprites added");

            CustomGun.SetupProjectiles(gun, baseGun, ProjectileModule.ShootStyle.Automatic);
            ETGModConsole.Log("projectiles added");

            CustomGun.SetupItem(gun, "+1", "It's 1 better", spriteID);
            ETGModConsole.Log("ak-48 added");
        }
    }
}
