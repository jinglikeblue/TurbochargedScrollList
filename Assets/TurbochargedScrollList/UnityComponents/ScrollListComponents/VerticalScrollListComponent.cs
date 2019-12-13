using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class VerticalScrollListComponent: MonoBehaviour
    {
        public GameObject itemPrefab;

        public VerticalLayoutSettings layout;

        public VerticalScrollList _list;

        public VerticalScrollList GetList()
        {
            if (null == _list)
            {
                _list = new VerticalScrollList(gameObject, itemPrefab, layout);
            }
            return _list;
        }
    }
}