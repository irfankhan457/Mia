using UnityEngine;
using UnityEngine.UI;

namespace MiaFantasyRun.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public sealed class AspectFillImage : MonoBehaviour
    {
        private Image image;
        private RectTransform rectTransform;

        private void Awake()
        {
            CacheComponents();
            ResizeToCover();
        }

        private void OnEnable()
        {
            CacheComponents();
            ResizeToCover();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                ResizeToCover();
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            ResizeToCover();
        }

        private void CacheComponents()
        {
            image ??= GetComponent<Image>();
            rectTransform ??= GetComponent<RectTransform>();
        }

        private void ResizeToCover()
        {
            CacheComponents();
            if (image == null || image.sprite == null || rectTransform == null || rectTransform.parent is not RectTransform parent)
            {
                return;
            }

            var parentRect = parent.rect;
            if (parentRect.width <= 0f || parentRect.height <= 0f)
            {
                return;
            }

            var spriteRect = image.sprite.rect;
            var spriteAspect = spriteRect.width / spriteRect.height;
            var parentAspect = parentRect.width / parentRect.height;

            var width = parentRect.width;
            var height = parentRect.height;
            if (parentAspect > spriteAspect)
            {
                height = width / spriteAspect;
            }
            else
            {
                width = height * spriteAspect;
            }

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
