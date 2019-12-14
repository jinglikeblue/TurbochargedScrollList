using System;
using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    [Serializable]
    public class GridLayoutSettings
    {
        /// <summary>
        /// 距离左边的内边距
        /// </summary>
        public float paddingLeft = 0;

        /// <summary>
        /// 距离右边的内边距
        /// </summary>
        public float paddingRight = 0;

        /// <summary>
        /// 距离顶部的内边距
        /// </summary>
        public float paddingTop = 0;

        /// <summary>
        /// 距离底部的内边距
        /// </summary>
        public float paddingBottom = 0;

        /// <summary>
        /// 列表项横向间距
        /// </summary>
        public float gapX = 0;

        /// <summary>
        /// 列表项垂直间距
        /// </summary>
        public float gapY = 0;

        /// <summary>
        /// 列表项排列方式限定
        /// </summary>
        public EGridConstraint constraint = EGridConstraint.FLEXIBLE;

        /// <summary>
        /// 当constraint不为「FLEXIBLE」，根据该值确定列数或者行数
        /// </summary>
        [Header("Work When [Constraint != EGridConstraint.FLEXIBLE]")]
        public int constraintCount = 2;
    }
}
