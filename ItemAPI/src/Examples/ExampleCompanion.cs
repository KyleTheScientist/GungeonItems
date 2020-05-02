using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.CompanionBuilder.AnimationType;
namespace ItemAPI
{
    public class ExampleCompanion
    {
        public static GameObject prefab;
        private static readonly string guid = "big_slime4206912345690"; //give your companion some unique guid

        public static void Init()
        {
            string itemName = "Big Slime";
            string resourceName = "ItemAPI/Resources/BigSlime/item_sprite";

            GameObject obj = new GameObject();
            var item = obj.AddComponent<CompanionItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Coolest Kid On the Block";
            string longDesc = "This kid is so cool that people are starting to call YOU cool by association.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "kts");
            item.quality = PickupObject.ItemQuality.B;
            item.CompanionGuid = guid; //this will be used by the item later to pull your companion from the enemy database
            item.Synergies = new CompanionTransformSynergy[0]; //this just needs to not be null
            item.AddPassiveStatModifier(PlayerStats.StatType.Coolness, 5f);
            BuildPrefab();
        }

        public static void BuildPrefab()
        {
            if (prefab != null || CompanionBuilder.companionDictionary.ContainsKey(guid))
                return;

            //Create the prefab with a starting sprite and hitbox offset/size
            prefab = CompanionBuilder.BuildPrefab("Big Slime", guid, "ItemAPI/Resources/BigSlime/Idle/son_idle_001", new IntVector2(1, 0), new IntVector2(9, 9));

            //Add a companion component to the prefab (could be a custom class)
            var companion = prefab.AddComponent<CompanionController>();
            companion.aiActor.MovementSpeed = 5f;

            //Add all of the needed animations (most of the animations need to have specific names to be recognized, like idle_right or attack_left)
            prefab.AddAnimation("idle_right", "ItemAPI/Resources/BigSlime/Idle", fps: 5, AnimationType.Idle, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("idle_left", "ItemAPI/Resources/BigSlime/Idle", fps: 5, AnimationType.Idle, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("run_right", "ItemAPI/Resources/BigSlime/MoveRight", fps: 7, AnimationType.Move, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("run_left", "ItemAPI/Resources/BigSlime/MoveLeft", fps: 7, AnimationType.Move, DirectionType.TwoWayHorizontal);

            //Add the behavior here, this too can be a custom class that extends AttackBehaviorBase or something like that
            var bs = prefab.GetComponent<BehaviorSpeculator>();
            bs.MovementBehaviors.Add(new CompanionFollowPlayerBehavior() { IdleAnimations = new string[] { "idle" } });
        }

    }
}
