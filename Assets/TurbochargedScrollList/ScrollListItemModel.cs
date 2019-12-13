using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 列表项的模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScrollListItemModel
    {
        /// <summary>
        /// 关联的数据
        /// </summary>
        public readonly object data;

        /// <summary>
        /// 记录的列表项高度
        /// </summary>
        public float height;

        /// <summary>
        /// 记录的列表项宽度
        /// </summary>
        public float width;

        public ScrollListItemModel(object data, Vector2 defaultSize)
        {
            this.data = data;
            width = defaultSize.x;
            height = defaultSize.y;
        }
    }
}
