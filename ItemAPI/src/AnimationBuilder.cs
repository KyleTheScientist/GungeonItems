using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace ItemAPI
{
    public static class AnimationBuilder
    {
        private static tk2dSpriteCollectionData weaponCollection = PickupObjectDatabase.GetByEncounterName("AK-47").sprite.Collection;

        public static Tuple<tk2dSpriteCollectionData, int> RedoAnimator(Gun gun, string folder)
        {
            var animator = gun.GetComponent<tk2dSpriteAnimator>();
            animator.enabled = true;

            var clips = animator.Library.clips as tk2dSpriteAnimationClip[];
            var textures = ResourceExtractor.GetTexturesFromFolder(folder).ToArray();

            List<int> ids = new List<int>();
            for (int i = 0; i < textures.Length; i++)
            {
                tk2dSpriteDefinition def = SpriteBuilder.ConstructDefinition(textures[i]);
                ids.Add(SpriteBuilder.AddSpriteToCollection(def, weaponCollection));
            }

            foreach (var clip in clips)
            {
                var frames = new tk2dSpriteAnimationFrame[textures.Length];
                for (int i = 0; i < textures.Length; i++)
                {
                    frames[i] = new tk2dSpriteAnimationFrame();
                    frames[i].spriteId = ids[i];
                    frames[i].spriteCollection = weaponCollection;
                }
                clip.frames = frames;
            }

            return new Tuple<tk2dSpriteCollectionData, int>(weaponCollection, ids[0]);
        }

    }
}
