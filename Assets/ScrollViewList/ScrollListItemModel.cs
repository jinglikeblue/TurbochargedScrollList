using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 列表项的模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScrollListItemModel<T>
    {
        /// <summary>
        /// 关联的数据
        /// </summary>
        public readonly T data;

        /// <summary>
        /// 记录的列表项高度
        /// </summary>
        public float height;

        /// <summary>
        /// 记录的列表项宽度
        /// </summary>
        public float width;

        public ScrollListItemModel(T data, Vector2 defaultSize)
        {
            this.data = data;
            height = defaultSize.x;
            width = defaultSize.y;
        }
    }
}
