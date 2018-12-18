using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ItemAPI
{
    class ExampleActive : PlayerItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Init()
        {
            string itemName = "Sweating Bullets"; //The name of the item
            string resourceName = "ItemAPI/Resources/sweating_bullets_icon"; //Refers to an embedded png in the project. Make sure to embed your resources!
            
            //Generate a new GameObject with a sprite component
            GameObject spriteObj = ItemBuilder.CreateSpriteObject(itemName, resourceName);

            //Add a PassiveItem component to the object
            ExampleActive item = spriteObj.AddComponent<ExampleActive>(); 

            //Ammonomicon entry variables
            string shortDesc = "Is it Hot in Here?";
            string longDesc = "While active, doubles damage, but reduces health to 1 hit. \n\nDon't get nervous!";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "examplepool");

            //Set the cooldown type and duration of the cooldown
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500);

            //Adds a passive modifier, like curse, coolness, damage, etc. to the item. Works for passives and actives.
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 2);

            //Set some other fields
            item.consumable = false;
            item.quality = ItemQuality.B;
        }

        //Add the item's functionality down here! I stole most of this from the Stuffed Star active item code!
        protected override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_OBJ_power_up_01", base.gameObject);
            HandleStats(user, true);
            StartCoroutine(this.HandleDuration(user));
        }

        //Disable or enable the active whenever you need!
        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        float damageBuff = -1;
        float duration = 10f;
        private IEnumerator HandleDuration(PlayerController user)
        {
            if (this.IsCurrentlyActive)
            {
                Debug.LogError("Using a ActiveBasicStatItem while it is already active!");
                yield break;
            }
            this.IsCurrentlyActive = true;
            this.m_activeElapsed = 0f;
            this.m_activeDuration = this.duration;
            while (this.m_activeElapsed < this.m_activeDuration && this.IsCurrentlyActive)
            {
                yield return null;
            }
            this.IsCurrentlyActive = false;
            HandleStats(user, false);
            yield break;
        }

        private void HandleStats(PlayerController user, bool active)
        {
            user.healthHaver.NextShotKills = active;
            if (active)
            {
                float curDamage = user.stats.GetBaseStatValue(PlayerStats.StatType.Damage);
                float newDamage = curDamage * 2f;
                user.stats.SetBaseStatValue(PlayerStats.StatType.Damage, curDamage * 2, user);
                damageBuff = newDamage - curDamage;
            }
            else if (damageBuff > 0)
            {
                float curDamage = user.stats.GetBaseStatValue(PlayerStats.StatType.Damage);
                float newDamage = curDamage - damageBuff;
                user.stats.SetBaseStatValue(PlayerStats.StatType.Damage, newDamage, user);
                damageBuff = -1;
            }
        }
    }
}
