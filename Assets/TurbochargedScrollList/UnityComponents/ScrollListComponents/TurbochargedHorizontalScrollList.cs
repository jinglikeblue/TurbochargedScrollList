using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    public class TurbochargedHorizontalScrollList : MonoBehaviour
    {
        public GameObject itemPrefab;

        public HorizontalLayoutSettings layout;

        HorizontalScrollList _list;

        public HorizontalScrollList GetList()
        {
            if (null == _list)
            {
                _list = new HorizontalScrollList(gameObject, itemPrefab, layout);
            }
            return _list;
        }
    }
}