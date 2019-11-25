using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public abstract class BaseScrollListComponent : MonoBehaviour,IScrollList<object>
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

        public void AddRange<T>(IEnumerable<T> collection)
        {
            List<object> list = new List<object>();
            foreach(var temp in collection)
            {
                list.Add(temp);
            }
            AddRange(list);
        }

        public void Add(object data)
        {
            list.Add(data);
        }

        public void AddRange(IEnumerable<object> collection)
        {            
            list.AddRange(collection);
        }

        public void Insert(int index, object data)
        {
            list.Insert(index, data);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public bool Remove(object data)
        {
            return list.Remove(data);
        }
    }


}
