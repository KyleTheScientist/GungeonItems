using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using MonoMod.RuntimeDetour;
namespace ItemAPI
{
    public class CompanionBuilder
    {
        private static GameObject behaviorSpeculatorPrefab;
        public static Dictionary<string, GameObject> companionDictionary = new Dictionary<string, GameObject>();

        public static void Init()
        {
            var dog_guid = Gungeon.Game.Items["dog"].GetComponent<CompanionItem>().CompanionGuid;
            var dog = EnemyDatabase.GetOrLoadByGuid(dog_guid);

            behaviorSpeculatorPrefab = GameObject.Instantiate(dog.gameObject);

            foreach (Transform child in behaviorSpeculatorPrefab.transform)
            {
                if (child != behaviorSpeculatorPrefab.transform)
                    GameObject.DestroyImmediate(child);
            }

            foreach (var comp in behaviorSpeculatorPrefab.GetComponents<Component>())
            {
                if (comp.GetType() != typeof(BehaviorSpeculator))
                {
                    GameObject.DestroyImmediate(comp);
                }
            }

            GameObject.DontDestroyOnLoad(behaviorSpeculatorPrefab);
            FakePrefab.MarkAsFakePrefab(behaviorSpeculatorPrefab);
            behaviorSpeculatorPrefab.SetActive(false);

            Hook companionHook = new Hook(
                typeof(EnemyDatabase).GetMethod("GetOrLoadByGuid", BindingFlags.Public | BindingFlags.Static),
                typeof(CompanionBuilder).GetMethod("GetOrLoadByGuid")
            );
        }

        public static AIActor GetOrLoadByGuid(Func<string, AIActor> orig, string guid)
        {
            foreach (var id in companionDictionary.Keys)
            {
                if (id == guid)
                    return companionDictionary[id].GetComponent<AIActor>();
            }

            return orig(guid);
        }

        public static GameObject BuildPrefab(string name, string guid, string defaultSpritePath, IntVector2 hitboxOffset, IntVector2 hitBoxSize)
        {
            if (CompanionBuilder.companionDictionary.ContainsKey(guid))
            {
                ETGModConsole.Log("CompanionBuilder: Tried to create two companion prefabs with the same GUID!");
                return null;
            }
            var prefab = GameObject.Instantiate(behaviorSpeculatorPrefab);
            prefab.name = name;

            //setup misc components
            var sprite = SpriteBuilder.SpriteFromResource(defaultSpritePath, prefab).GetComponent<tk2dSprite>();

            sprite.SetUpSpeculativeRigidbody(hitboxOffset, hitBoxSize).CollideWithOthers = false;
            prefab.AddComponent<tk2dSpriteAnimator>();
            prefab.AddComponent<AIAnimator>();


            //setup health haver
            var healthHaver = prefab.AddComponent<HealthHaver>();
            healthHaver.RegisterBodySprite(sprite);
            healthHaver.PreventAllDamage = true;
            healthHaver.SetHealthMaximum(15000);
            healthHaver.FullHeal();

            //setup AI Actor
            var aiActor = prefab.AddComponent<AIActor>();
            aiActor.State = AIActor.ActorState.Normal;
            aiActor.EnemyGuid = guid;

            //setup behavior speculator
            var bs = prefab.GetComponent<BehaviorSpeculator>();

            bs.MovementBehaviors = new List<MovementBehaviorBase>();
            bs.AttackBehaviors = new List<AttackBehaviorBase>();
            bs.TargetBehaviors = new List<TargetBehaviorBase>();
            bs.OverrideBehaviors = new List<OverrideBehaviorBase>();
            bs.OtherBehaviors = new List<BehaviorBase>();

            //Add to enemy database
            EnemyDatabaseEntry enemyDatabaseEntry = new EnemyDatabaseEntry()
            {
                myGuid = guid,
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = false
            };
            EnemyDatabase.Instance.Entries.Add(enemyDatabaseEntry);
            CompanionBuilder.companionDictionary.Add(guid, prefab);


            //finalize
            GameObject.DontDestroyOnLoad(prefab);
            FakePrefab.MarkAsFakePrefab(prefab);
            prefab.SetActive(false);

            return prefab;
        }



    }
}
