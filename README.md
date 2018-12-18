# GungeonItems
An API for creating custom active/passive items with Mod The Gungeon: Classic

Allows you to create new items with custom sprites and effects

### Prerequisites

.NET framework 2.0 or higher

## Usage

The solution contains an example active and passive
Here's an example of a class that implements a custom active 

```csharp
//using ItemAPI;

class BloodyShield : PlayerItem
{
    public static void Init()
    {
        //The name of the item
        string itemName = "Bloody Shield"; 
            
        //Refers to an embedded png in the project. Make sure to embed your resources!
        string resourceName = "CustomItems/Resources/armor_shield_heart_idle_001";

        //Generate a new GameObject with a sprite component
        GameObject spriteObj = ItemBuilder.CreateSpriteObject(itemName, resourceName);

        //Add a ActiveItem component to the object. This can be another class, or you can implement it here.
        BloodyShield item = spriteObj.AddComponent<BloodyShield>();

        //Ammonomicon entry variables
        string shortDesc = "Iron from Blood";
        string longDesc = "Trades hearts for armor.";

        //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
        ItemBuilder.SetupItem(item, shortDesc, longDesc, "kts");

        //Set the cooldown type and duration of the cooldown
        ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1.5f);

        //Adds a passive modifier, like curse, coolness, damage, etc. to the item. Works for passives and actives.
        ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);

        //Set some other fields
        item.consumable = false;
        item.quality = ItemQuality.B;
    }
    
    //Item Functionality (This can go in another class, or you can just do it here.)
    
    //Removes one heart from the player, gives them 1 armor
    protected override void DoEffect(PlayerController user)
    {
        float curHealth = user.healthHaver.GetCurrentHealth();
        if (curHealth > 1)
        {
            AkSoundEngine.PostEvent("Play_OBJ_dead_again_01", base.gameObject);
            user.healthHaver.ForceSetCurrentHealth(curHealth - 1);
            user.healthHaver.Armor += 1;
        }
    }

    //Disables the item if the player's health is less than or equal to 1 heart
    public override bool CanBeUsed(PlayerController user)
    {
        return user.healthHaver.GetCurrentHealth() > 1;
    }
}

```

and in the main module call
```csharp
void Start() {
   ItemBuilder.Init(); //This needs to be called to be able to use embedded resources
   BloodyShield.Init(); //Builds the item
}
```
