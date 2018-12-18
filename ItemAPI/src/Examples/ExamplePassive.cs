using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemAPI
{
    class ExamplePassive
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Init()
        {
            string itemName = "Boss Bullets"; //The name of the item
            string resourceName = "ItemAPI/Resources/boss_bullets_icon"; //Refers to an embedded png in the project. Make sure to embed your resources!

            //Generate a new GameObject with a sprite component
            GameObject spriteObj = ItemBuilder.CreateSpriteObject(itemName, resourceName);

            //Add a PassiveItem component to the object
            PassiveItem item = spriteObj.AddComponent<PassiveItem>();

            //Ammonomicon entry variables
            string shortDesc = "Show 'em Who's Boss";
            string longDesc = "Greatly increases damage dealt to bosses.\n\n" +
                "This item was created by a union of Gungeoneers who became fed up with low wages and poor benefits.\n" +
                "Viva la Revolverlucion!";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "examplepool");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DamageToBosses, 3, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.S;
        }
    }
}
