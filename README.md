# GungeonItems
An API for creating custom active/passive items with Mod The Gungeon: Classic

Allows you to create new items with custom sprites and effects

Because of the way Mod the Gungeon loads external libraries, for now, you'll just have to add these cs files to your project:

- ItemBuilder.cs
- SpriteBuilder.cs
- ResourceExtractor.cs
- CustomSynergies.cs
- FakePrefab.cs
- FakePrefabHooks.cs

### Prerequisites

.NET framework 2.0 or higher
A reference to Mod The Gungeon, UnityEngine, and Enter The Gungeon dll's

## Usage

The solution contains an example active and passive.

Here's an example of a class that implements a custom active 

```csharp
//using ItemAPI;

class BloodShield : PlayerItem
{
    public static void Init()
    {
        //The name of the item
        string itemName = "Blood Shield";

        //Refers to an embedded png in the project. Make sure to embed your resources!
        string resourceName = "CustomItems/Resources/armor_shield_heart_idle_001";

        //Create new GameObject
        GameObject obj = new GameObject();

        //Add a ActiveItem component to the object
        var item = obj.AddComponent<BloodShield>();

        //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
        ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

        //Ammonomicon entry variables
        string shortDesc = "Iron from Blood";
        string longDesc = "Trades hearts for armor.\n\n" +
            "For carbon-based species, blood naturally contains hemoglobin, a molecule composed of " +
            "iron and heme groups. This item collects that iron from your blood and forges it into armor.\n\n" +
            "Approved by 100% of all doctors everywhere!";

        //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
        //"example_pool" here is the item pool. In the console you'd type "give example_pool:sweating_bullets"
        ItemBuilder.SetupItem(item, shortDesc, longDesc, "example_pool");

        //Set the cooldown type and duration of the cooldown
        ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1.5f);

        //Adds a passive modifier, like curse, coolness, damage, etc. to the item. Works for passives and actives.
        //ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);

        //Set some other fields
        item.consumable = false;
        item.quality = ItemQuality.B;
    }

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
   FakePrefabHooks.Init() //This prevents 
   ItemBuilder.Init(); //This needs to be called to be able to use embedded resources
   BloodyShield.Init(); //Builds the item
}
```
