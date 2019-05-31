using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Collections;

namespace ItemAPI
{
    public static class ItemBuilder
    {
        /// <summary>
        /// Sets the base assembly of the ResourceExtractor, so 
        /// resources can be accessed
        /// </summary>
        public static void Init()
        {
            try
            {
                MethodBase method = new StackFrame(1, false).GetMethod();
                var declaringType = method.DeclaringType;
                ResourceExtractor.SetAssembly(declaringType);
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        public enum CooldownType
        {
            Timed, Damage, PerRoom, None
        }

        /// <summary>
        /// Adds a tk2dSprite component to an object and adds that sprite to the 
        /// ammonomicon for later use. If obj is null, returns a new GameObject with the sprite
        /// </summary>
        public static GameObject AddSpriteToObject(string name, string resourcePath, GameObject obj = null, bool copyFromExisting = true)
        {
            GameObject spriteObject = SpriteBuilder.SpriteFromResource(resourcePath, obj, copyFromExisting);
            spriteObject.name = name;
            return spriteObject;
        }

        /// <summary>
        /// Finishes the item setup, adds it to the item databases, adds an encounter trackable 
        /// blah, blah, blah
        /// </summary>
        public static void SetupItem(PickupObject item, string shortDesc, string longDesc, string idPool)
        {
            try
            {
                item.encounterTrackable = null;

                ETGMod.Databases.Items.SetupItem(item, item.name);
                SpriteBuilder.AddToAmmonomicon(item.sprite.GetCurrentSpriteDef());
                item.encounterTrackable.journalData.AmmonomiconSprite = item.sprite.GetCurrentSpriteDef().name;

                item.SetName(item.name);
                item.SetShortDescription(shortDesc);
                item.SetLongDescription(longDesc);

                if (item is PlayerItem)
                    (item as PlayerItem).consumable = false;
                Gungeon.Game.Items.Add(idPool + ":" + item.name.ToLower().Replace(" ", "_"), item);
                ETGMod.Databases.Items.Add(item);
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        /// <summary>
        /// Sets the cooldown type and length of a PlayerItem, and resets all other cooldown types
        /// </summary>
        public static void SetCooldownType(PlayerItem item, CooldownType cooldownType, float value)
        {
            item.damageCooldown = -1;
            item.roomCooldown = -1;
            item.timeCooldown = -1;

            switch (cooldownType)
            {
                case CooldownType.Timed:
                    item.timeCooldown = value;
                    break;
                case CooldownType.Damage:
                    item.damageCooldown = value;
                    break;
                case CooldownType.PerRoom:
                    item.roomCooldown = (int)value;
                    break;
            }
        }

        /// <summary>
        /// Adds a passive player stat modifier to a PlayerItem or PassiveItem
        /// </summary>
        public static void AddPassiveStatModifier(PickupObject po, PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            if (po is PlayerItem)
            {
                var item = (po as PlayerItem);
                if (item.passiveStatModifiers == null)
                    item.passiveStatModifiers = new StatModifier[] { modifier };
                else
                    item.passiveStatModifiers = item.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
            }
            else if (po is PassiveItem)
            {
                var item = (po as PassiveItem);
                if (item.passiveStatModifiers == null)
                    item.passiveStatModifiers = new StatModifier[] { modifier };
                else
                    item.passiveStatModifiers = item.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
            }
            else
            {
                throw new NotSupportedException("Object must be of type PlayerItem or PassiveItem");
            }
        }

        public static IEnumerator HandleDuration(PlayerItem item, float duration, PlayerController user, Action<PlayerController> OnFinish)
        {
            if (item.IsCurrentlyActive)
            {
                yield break;
            }

            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", true);
            SetPrivateType<PlayerItem>(item, "m_activeElapsed", 0f);
            SetPrivateType<PlayerItem>(item, "m_activeDuration", duration);
            item.OnActivationStatusChanged?.Invoke(item);

            float elapsed = GetPrivateType<PlayerItem, float>(item, "m_activeElapsed");
            float dur = GetPrivateType<PlayerItem, float>(item, "m_activeDuration");

            while (GetPrivateType<PlayerItem, float>(item, "m_activeElapsed") < GetPrivateType<PlayerItem, float>(item, "m_activeDuration") && item.IsCurrentlyActive)
            {
                yield return null;
            }
            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", false);
            item.OnActivationStatusChanged?.Invoke(item);

            OnFinish?.Invoke(user);
            yield break;
        }

        private static void SetPrivateType<T>(T obj, string field, bool value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
    }
}
