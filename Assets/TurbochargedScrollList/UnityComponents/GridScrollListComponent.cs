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

        private void Awake()
        {
            list = new GridScrollList(gameObject, itemPrefab, ItemRender, gap, constraint , constraintCount);
        }
    }
}
