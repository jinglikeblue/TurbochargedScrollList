using System;
using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 布局设置
    /// </summary>    
    public abstract class BaseLayoutSettings
    {
        [Header("Use In Grid|Horizontal")]
        public float paddingLeft = 0;

        [Header("Use In Grid|Horizontal")]
        public float paddingRight = 0;

        [Header("Use In Grid|Vertical")]
        public float paddingTop = 0;

        [Header("Use In Grid|Vertical")]
        public float paddingBottom = 0;

        [Header("Use In Grid|Horizontal")]
        public float gapX = 0;

        [Header("Use In Grid|Vertical")]
        public float gapY = 0;

        public BaseLayoutSettings()
        {

        }

        protected void SetPadding(float left, float right, float top, float bottom)
        {
            paddingLeft = left;
            paddingRight = right;
            paddingTop = top;
            paddingBottom = bottom;
        }
    }
}
