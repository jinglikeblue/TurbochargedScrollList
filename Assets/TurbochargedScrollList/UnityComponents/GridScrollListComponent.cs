using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class GridScrollListComponent : BaseScrollListComponent
    {
        [Header("item gap")]
        public Vector2 gap = Vector2.zero;

        public EGridConstraint constraint = EGridConstraint.FLEXIBLE;

        [Header("If constraint != EGridConstraint.FLEXIBLE")]
        public int constraintCount = 2;

        public GridScrollList List
        {
            get
            {
                return (GridScrollList)list;
            }
        }

        private void Awake()
        {
            list = new GridScrollList(gameObject, itemPrefab, ItemRender, gap, constraint, constraintCount);            
        }

        public void ScrollToPosition(float x, float y)
        {
            List.ScrollToPosition(x, y);
        }
    }
}
