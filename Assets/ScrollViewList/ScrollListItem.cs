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
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
