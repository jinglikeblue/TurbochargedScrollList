using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class VerticalScrollListComponent : BaseScrollListComponent
    {
        [Header("item gap")]
        public float gap;

        private void Awake()
        {
            list = new VerticalScrollList<object>(gameObject, itemPrefab, ItemRender, gap);
        }
    }
}