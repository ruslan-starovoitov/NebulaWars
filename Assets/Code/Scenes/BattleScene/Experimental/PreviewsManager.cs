using System.Collections.Generic;
using Code.Scenes.BattleScene.ScriptableObjects;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental
{
    public static class PreviewsManager
    {
        private static readonly ILog Log = LogManager.CreateLogger(typeof(PreviewsManager));
        private static readonly Dictionary<ViewKey, Sprite> Sprites = new Dictionary<ViewKey, Sprite>();

        public static Sprite GetSprite(ViewTypeEnum typeId, int width = 48, int height = 48)
        {
            var dictKey = new ViewKey(typeId, width, height);
            if (Sprites.TryGetValue(dictKey, out var sprite))
            {
                return sprite;
            }
            
            ViewObject viewObject = ViewObjectsBase.Instance.GetViewObject(typeId);
            if (viewObject is SpriteObject spriteObject)
            {
                return Sprites[dictKey] = spriteObject.sprite;
            }
            
            Texture2D texture;
            if (viewObject is AbstractLineObject lineObject)
            {
                texture = (Texture2D)lineObject.material.mainTexture;
                if (texture == null)
                {
                    texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    texture.SetPixel(0, 0, lineObject.material.color);
                    texture.Apply();
                }
            }
            else
            {
                RuntimePreviewGenerator.PreviewDirection = Vector3.forward;
                RuntimePreviewGenerator.BackgroundColor = Color.clear;
                RuntimePreviewGenerator.OrthographicMode = true;

                var prefabObject = (PrefabObject)viewObject;
                var withoutParticles = Object.Instantiate(prefabObject.gameObject);
                var particleSystems = withoutParticles.GetComponentsInChildren<ParticleSystem>();
                foreach (var particleSystem in particleSystems) Object.DestroyImmediate(particleSystem);
                texture = RuntimePreviewGenerator.GenerateModelPreview(withoutParticles.transform, width, height);
                Object.DestroyImmediate(withoutParticles);
            }

            Rect rect = new Rect(0, 0, texture.width, texture.height);
            return Sprites[dictKey] = Sprite.Create(texture, rect, Vector2.zero, 100);
        }
    }
}
