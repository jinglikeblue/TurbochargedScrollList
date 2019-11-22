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
        /// 最后检测变化时的矩形数据
        /// </summary>
        Rect _lastRect;

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
            _lastRect = rectTransform.rect;
        }

        /// <summary>
        /// 检查尺寸是否有改变
        /// </summary>
        /// <returns></returns>
        internal bool CheckSizeChanged()
        {
            if(_lastRect.width != rectTransform.rect.width || _lastRect.height != rectTransform.rect.height)
            {
                _lastRect = rectTransform.rect;
                return true;
            }
            return false;
        }
    }
}
