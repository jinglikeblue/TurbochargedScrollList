using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class VerticalScrollListComponent : BaseScrollListComponent
    {
        [Header("item gap")]
        public float gap;

        private void Awake()
        {
            list = new VerticalScrollList(gameObject, itemPrefab, ItemRender, gap);
        }
    }
}