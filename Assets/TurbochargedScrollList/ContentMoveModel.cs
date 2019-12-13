using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class ContentMoveModel
    {
        /// <summary>
        /// 最后位置
        /// </summary>
        public Vector2 lastPosition { get; private set; }

        /// <summary>
        /// 当前位置
        /// </summary>
        public Vector2 currentPosition { get; private set; }

        /// <summary>
        /// 移动距离
        /// </summary>
        public Vector2 movedDistance { get; private set; }

        public bool IsScroll2Start
        {
            get
            {
                return IsMove2Right || IsMove2Bottom;
            }
        }

        public bool IsScroll2End
        {
            get
            {
                return IsMove2Left || IsMove2Top;
            }
        }

        public bool IsMove2Left
        {
            get
            {
                return movedDistance.x < 0 ? true : false;
            }
        }

        public bool IsMove2Right
        {
            get
            {
                return movedDistance.x > 0 ? true : false;
            }
        }

        public bool IsMove2Top
        {
            get
            {
                return movedDistance.y > 0 ? true : false;
            }
        }

        public bool IsMove2Bottom
        {
            get
            {
                return movedDistance.y < 0 ? true : false;
            }
        }

        public ContentMoveModel()
        {
            lastPosition = Vector2.zero;
            currentPosition = Vector2.zero;
            movedDistance = Vector2.zero;
        }

        public void SetPosition(Vector2 position)
        {
            if (currentPosition.Equals(position))
            {
                return;
            }
            lastPosition = currentPosition;
            currentPosition = position;
            movedDistance = currentPosition - lastPosition;
        }
    }
}
