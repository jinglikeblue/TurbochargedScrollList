using System;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public abstract class BaseScrollListComponent : MonoBehaviour
    {
        public GameObject itemPrefab;

        protected BaseScrollList<object> list;

        public event Action<ScrollListItem, object> renderItem;

        protected void ItemRender(ScrollListItem item, object data)
        {
            renderItem?.Invoke(item, data);
        }

        public void Clear()
        {
            list.Clear();
        }

        public void AddDatas<T>(T[] datas)
        {
            object[] objs = new object[datas.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = datas[i];
            }
            list.AddDatas(objs);
        }

        public void Add(object data)
        {
            list.AddData(data);
        }
    }


}
