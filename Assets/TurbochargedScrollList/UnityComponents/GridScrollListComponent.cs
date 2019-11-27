using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class GridScrollListComponent : BaseScrollListComponent
    {
        [Header("item gap")]
        public Vector2 gap;

        private void Awake()
        {
            list = new GridScrollList<object>(gameObject, itemPrefab, ItemRender, gap);
        }
    }
}
