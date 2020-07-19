using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LootboxScene.PrefabScripts
{
    /// <summary>
    /// Находится на префабе премиум-валюты. Управляет анимацией при появлении префаба.
    /// </summary>
    public class HardCurrencyAccrual : MonoBehaviour
    {
        private int amount;
        private bool start;
        private GameObject headerGo;
        private GameObject imageGo;
        private GameObject amountGo;
    
        public void SetData(int amountArg)
        {
            amount = amountArg;
            start = true;
        }
    
        private void Awake()
        {
            headerGo = transform.Find("Canvas/Text_Header").gameObject
                       ?? throw new NullReferenceException("Text_Header");
            imageGo = transform.Find("Canvas/Image").gameObject
                      ?? throw new NullReferenceException("Image");
            amountGo = transform.Find("Canvas/Text_Amount").gameObject
                       ?? throw new NullReferenceException("Text_Amount");
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            yield return new WaitUntil(()=>start);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            StartCoroutine(HeaderAnimation());
            StartCoroutine(ImageAnimation());
            StartCoroutine(AmountAnimation());
        }
    
        private IEnumerator HeaderAnimation()
        {
            Text text = headerGo.GetComponent<Text>();
            text.resizeTextMaxSize = 5;
        
            while (text.resizeTextMaxSize<80)
            {
                text.resizeTextMaxSize += 5;    
                yield return null;   
            }

            while (text.resizeTextMaxSize>60)
            {
                text.resizeTextMaxSize -= 4;    
                yield return null;   
            }
        }
    
        private IEnumerator ImageAnimation()
        {
            RectTransform rectTransform = imageGo.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(0.2f, 0.2f);
            const float xScale1 = 1;
            const float yScale1 = 1.2f;
            while (rectTransform.localScale.x < xScale1 || rectTransform.localScale.y < yScale1)
            {
                float xScaleDelta = 0;
                float yScaleDelta = 0;
                if (rectTransform.localScale.x < xScale1)
                {
                    xScaleDelta = 0.07f;
                }
            
                if (rectTransform.localScale.y < yScale1)
                {
                    yScaleDelta = 0.07f;
                }
        
                Vector3 tmp = rectTransform.localScale;
                tmp = new Vector3(tmp.x+xScaleDelta, tmp.y+yScaleDelta);
                rectTransform.localScale = tmp;
                yield return null;
            }
        
            const float xScale2 = 1;
            const float yScale2 = 1;
            while (rectTransform.localScale.x > xScale2 || rectTransform.localScale.y > yScale2)
            {
                float xScaleDelta = 0;
                float yScaleDelta = 0;
                if (rectTransform.localScale.x > xScale2)
                {
                    xScaleDelta = -0.07f;
                }
            
                if (rectTransform.localScale.y > yScale2)
                {
                    yScaleDelta = -0.07f;
                }

                Vector3 tmp = rectTransform.localScale;
                tmp = new Vector3(tmp.x+xScaleDelta, tmp.y+yScaleDelta);
                rectTransform.localScale = tmp;
                yield return null;
            }
        
        }
    
        private IEnumerator AmountAnimation()
        {
            RectTransform rectTransform = amountGo.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(0.2f, 0.2f);
            const float xScale1 = 1;
            const float yScale1 = 1.4f;
            while (rectTransform.localScale.x < xScale1 || rectTransform.localScale.y < yScale1)
            {
                float xScaleDelta = 0;
                float yScaleDelta = 0;
                if (rectTransform.localScale.x < xScale1)
                {
                    xScaleDelta = 0.07f;
                }
            
                if (rectTransform.localScale.y < yScale1)
                {
                    yScaleDelta = 0.07f;
                }
        
                Vector3 tmp = rectTransform.localScale;
                tmp = new Vector3(tmp.x+xScaleDelta, tmp.y+yScaleDelta);
                rectTransform.localScale = tmp;
                yield return null;
            }

            const float xScale2 = 1;
            const float yScale2 = 1;
            while (rectTransform.localScale.x > xScale2 || rectTransform.localScale.y > yScale2)
            {
                float xScaleDelta = 0;
                float yScaleDelta = 0;
                if (rectTransform.localScale.x > xScale2)
                {
                    xScaleDelta = -0.07f;
                }
            
                if (rectTransform.localScale.y > yScale2)
                {
                    yScaleDelta = -0.07f;
                }

                Vector3 tmp = rectTransform.localScale;
                tmp = new Vector3(tmp.x+xScaleDelta, tmp.y+yScaleDelta);
                rectTransform.localScale = tmp;
                yield return null;
            }
        }
    }
}
