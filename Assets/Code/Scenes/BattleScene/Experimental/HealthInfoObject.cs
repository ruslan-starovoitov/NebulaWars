using TMPro;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental
{
    public class HealthInfoObject : MonoBehaviour
    {
        [SerializeField] private Color shieldLineColor;
        [SerializeField] private Color shieldTextColor;
        [SerializeField] private SpriteRenderer topLine;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private TextMeshPro healthPoints;
        private MaterialPropertyBlock propertyBlock;

        public void Initialize()
        {
            propertyBlock = new MaterialPropertyBlock();
            topLine.GetPropertyBlock(propertyBlock);

            var parent = transform.parent;
            var scale = 1f;
            if (parent.GetComponent<HealthInfoObject>() == null)
            {
                var parentRenderer = parent.GetComponent<Renderer>();
                if (parentRenderer != null)
                {
                    var size = parentRenderer.bounds.size;
                    scale = Mathf.Min(size.x, size.y);
                }
            }

            transform.localScale = new Vector3(scale, Mathf.Sqrt(scale), 1f);

            //https://answers.unity.com/questions/1042119/getting-a-sprites-size-in-pixels.html
            var sprite = topLine.sprite;
            var spriteSize = sprite.rect.size;
            var localSpriteSize = spriteSize / sprite.pixelsPerUnit;
            var worldSize = localSpriteSize;
            worldSize.Scale(transform.lossyScale);

            Vector3 screenSize = 0.5f * worldSize / Camera.main.orthographicSize;
            screenSize.y *= Camera.main.aspect;

            var pixelsSize = new Vector2(screenSize.x * Camera.main.pixelWidth, screenSize.y * Camera.main.pixelHeight) * 0.5f;
            //

            propertyBlock.SetVector("_ImageSize", pixelsSize);
        }

        public void SaveChanges()
        {
            topLine.SetPropertyBlock(propertyBlock);
            healthPoints.text = $"{currentHealthPoints:F0}/{maxHealthPoints:F0}";
        }

        //https://answers.unity.com/questions/1463955/how-can-i-make-the-division-of-the-health-bar-like.html
        private const float HealthBarStepsLength = 250f;
        private const float DefaultHealthPoints = 1000f;
        private float maxHealthPoints = DefaultHealthPoints;
        private float currentHealthPoints = DefaultHealthPoints;
        //private float damages;

        public float HealthPoints
        {
            get => currentHealthPoints;
            set
            {
                currentHealthPoints = value;
                propertyBlock.SetFloat("_Percent", currentHealthPoints / maxHealthPoints);

                //if (currentHealthPoints < Mathf.Epsilon)
                //    Damages = 0;
            }
        }

        //public float Damages
        //{
        //    get => damages;
        //    set
        //    {
        //        damages = Mathf.Clamp(value, 0, MaxHealthPoints);
        //        propertyBlock.SetFloat("_DamagesPercent", damages / MaxHealthPoints);
        //    }
        //}

        public float MaxHealthPoints
        {
            get => maxHealthPoints;
            set
            {
                maxHealthPoints = value;
                propertyBlock.SetFloat("_Steps", maxHealthPoints / HealthBarStepsLength);
                propertyBlock.SetFloat("_Percent", currentHealthPoints / maxHealthPoints);
            }
        }

        public void SetShieldStyle()
        {
            //topLine.color = shieldLineColor;
            propertyBlock.SetColor("_Color", shieldLineColor);
            healthPoints.color = shieldTextColor;
        }

        public void SetTransparency(float percentage)
        {
            if (percentage > 0f)
            {
                if(!gameObject.activeSelf) gameObject.SetActive(true);
                var color = healthPoints.color;
                color.a = percentage;
                healthPoints.color = color;
                color = background.color;
                color.a = percentage;
                background.color = color;
                propertyBlock.SetFloat("_Transparency", percentage);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void HideHealthBar()
        {
            gameObject.SetActive(false);
        }
    }
}