using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace ItemAPI
{
    public static class CustomGun
    {
        public static GameObject GenerateGun(Gun baseGun, string name)
        {
            var items = Gungeon.Game.Items;
            string gunID = name.Replace(" ", "_").ToLower();

            GameObject go = UnityEngine.Object.Instantiate(baseGun.gameObject);
            go.name = gunID;

            Gun gun = go.GetComponent<Gun>();
            gun.gunName = name;
            gun.gunSwitchGroup = gunID;

            gun.modifiedVolley = null;
            gun.singleModule = null;

            gun.RawSourceVolley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
            gun.Volley.projectiles = new List<ProjectileModule>();

            gun.SetBaseMaxAmmo(300);
            gun.reloadTime = 1f;

            return gun.gameObject;
        }

        public static void SetupProjectiles(Gun gun, Gun baseGun, ProjectileModule.ShootStyle shootStyle)
        {
            InitModule(gun, baseGun);
            gun.DefaultModule.shootStyle = shootStyle;
        }

        public static int SetupSprite(Gun gun, string spritesFolder, string ammonomiconSpritePath)
        {
            var idlePath = Path.Combine(spritesFolder, "idle");
            var defData = AnimationBuilder.RedoAnimator(gun, idlePath);
            gun.sprite.SetSprite(defData.First, defData.Second);

            var ammonomTexture = ResourceExtractor.GetTextureFromFile(ammonomiconSpritePath);
            tk2dSpriteDefinition def = SpriteBuilder.ConstructDefinition(ammonomTexture);


            int spriteID = SpriteBuilder.AddToAmmonomicon(def);

            return spriteID;
        }

        public static void SetupItem(PickupObject item, string descShort, string descLong, int ammonomiconSpriteID, string idPool = "customitems")
        {
            ETGMod.Databases.Items.SetupItem(item, item.name);

            tk2dSpriteCollectionData ammoCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
            item.encounterTrackable.journalData.AmmonomiconSprite = ammoCollection.spriteDefinitions[ammonomiconSpriteID].name;

            item.SetName(item.name);
            item.SetShortDescription(descShort);
            item.SetLongDescription(descLong);

            Gungeon.Game.Items.Add(idPool + ":" + item.name.ToLower().Replace(" ", "_"), item);
            ETGMod.Databases.Items.Add(item);
        }

        private static void InitModule(Gun gun, Gun orig)
        {
            if (orig.RawSourceVolley != null)
            {
                ProjectileVolleyData gunVolley = ScriptableObject.CreateInstance<ProjectileVolleyData>();
                gunVolley.InitializeFrom(orig.GetComponent<Gun>().RawSourceVolley);
                gun.RawSourceVolley = gunVolley;

                foreach (var module in gun.RawSourceVolley.projectiles)
                {
                    foreach (var proj in module.projectiles)
                    {
                        Projectile p = proj.ClonedPrefab();
                        p.gameObject.SetActive(false);
                        for (int i = 0; i < module.projectiles.Count; i++)
                        {
                            module.projectiles[i] = p;
                        }
                    }
                }
            }
            else
            {
                gun.RawSourceVolley = null;
                ProjectileModule module = ProjectileModule.CreateClone(orig.DefaultModule, false);
                foreach (var proj in module.projectiles)
                {
                    Projectile p = proj.ClonedPrefab();
                    p.gameObject.SetActive(false);
                    for (int i = 0; i < module.projectiles.Count; i++)
                    {
                        module.projectiles[i] = p;
                    }
                }
                gun.singleModule = module;
            }
        }
    }
}
