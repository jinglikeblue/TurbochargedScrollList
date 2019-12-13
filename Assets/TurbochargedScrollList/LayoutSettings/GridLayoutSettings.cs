using System;
using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    [Serializable]
    public class GridLayoutSettings
    {        
        public float paddingLeft = 0;
     
        public float paddingRight = 0;
        
        public float paddingTop = 0;
        
        public float paddingBottom = 0;
        
        public float gapX = 0;
        
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
