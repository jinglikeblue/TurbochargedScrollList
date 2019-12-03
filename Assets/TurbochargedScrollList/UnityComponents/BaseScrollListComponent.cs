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

        BaseScrollList<object> _list;

        protected BaseScrollList<object> list
        {
            get
            {
                return _list;
            }

            set
            {
                _list = value;
                _list.onRebuildContent += OnRebuildContent;
                _list.onRefresh += OnRefresh;
            }
        }

        private void OnRefresh()
        {
            onRefresh?.Invoke();
        }

        private void OnRebuildContent()
        {
            onRebuildContent?.Invoke();
        }

        public float ContentWidth
        {
            get
            {
                return list.ContentWidth;
            }
        }

        public float ContentHeight
        {
            get
            {
                return list.ContentHeight;
            }
        }

        public int ItemCount
        {
            get
            {
                return list.ItemCount;
            }
        }

        public event Action<ScrollListItem, object, bool> renderItem;
        public event Action onRebuildContent;
        public event Action onRefresh;
        public event OnItemBeforeReuse onItemBeforeReuse;

        protected void ItemRender(ScrollListItem item, object data, bool isFresh)
        {
            renderItem?.Invoke(item, data, isFresh);
        }

        public void Clear()
        {
            list.Clear();
        }

        public void AddRange<T>(IEnumerable<T> collection)
        {
            List<object> datas = new List<object>();
            foreach(var temp in collection)
            {
                datas.Add(temp);
            }
            AddRange(datas);
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

        public void ScrollToItem(int index)
        {
            list.ScrollToItem(index);
        }

        public void ScrollToPosition(Vector2 position)
        {
            list.ScrollToPosition(position);
        }

        public bool CheckItemShowing(int index)
        {
            return list.CheckItemShowing(index);
        }

        public Vector2 GetDistanceToEnd()
        {
            return list.GetDistanceToEnd();
        }
    }


}
