using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class GridScrollListComponent : MonoBehaviour
    {
        public GameObject itemPrefab;

        public GridLayoutSettings layout;

        public GridScrollList _list;

        public GridScrollList GetList()
        {
            if (null == _list)
            {
                _list = new GridScrollList(gameObject, itemPrefab, layout);
            }
            return _list;
        }
    }
}
