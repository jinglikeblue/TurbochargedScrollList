using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public class TurbochargedGridScrollList : MonoBehaviour
    {
        public GameObject itemPrefab;

        public GridLayoutSettings layout;

        GridScrollList _list;

        public GridScrollList GetList()
        {
            if (null == _list)
            {
                _list = new GridScrollList(GetComponent<ScrollRect>(), itemPrefab, layout);
            }
            return _list;
        }
    }
}
