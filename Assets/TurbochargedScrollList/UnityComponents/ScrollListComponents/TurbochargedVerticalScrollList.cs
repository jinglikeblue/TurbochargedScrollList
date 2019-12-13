using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public class TurbochargedVerticalScrollList : MonoBehaviour
    {
        public GameObject itemPrefab;

        public VerticalLayoutSettings layout;

        public VerticalScrollList _list;

        public VerticalScrollList GetList()
        {
            if (null == _list)
            {
                _list = new VerticalScrollList(GetComponent<ScrollRect>(), itemPrefab, layout);
            }
            return _list;
        }
    }
}