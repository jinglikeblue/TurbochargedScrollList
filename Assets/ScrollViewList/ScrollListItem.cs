using UnityEngine;
namespace Jing.ScrollViewList
{
    /// <summary>
    /// 滚动列表项
    /// </summary>
    [RequireComponent(typeof(RectTransform))]    
    public class ScrollListItem : MonoBehaviour
    {        
        public RectTransform rectTransform { get; private set; }

        /// <summary>
        /// Item的索引位置
        /// </summary>
        public int index { get; internal set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; internal set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public float width
        {
            get
            {
                return rectTransform.rect.width;
            }
            set
            {
                if (rectTransform.sizeDelta.x != value)
                {
                    var sd = rectTransform.sizeDelta;
                    sd.x = value;
                    rectTransform.sizeDelta = sd;
                }
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public float height
        {
            get
            {
                return rectTransform.rect.height;
            }
            set
            {
                if (rectTransform.sizeDelta.y != value)
                {
                    var sd = rectTransform.sizeDelta;
                    sd.y = value;
                    rectTransform.sizeDelta = sd;
                }
            }
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        internal void ChangeSize(float w, float h)
        {
            if (rectTransform.sizeDelta.x != w || rectTransform.sizeDelta.y != h)
            {
                var sd = rectTransform.sizeDelta;
                sd.x = w;
                sd.y = h;
                rectTransform.sizeDelta = sd;
            }
        }
    }
}
