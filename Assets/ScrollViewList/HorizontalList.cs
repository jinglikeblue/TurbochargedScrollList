using Jing.ScrollViewList;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class HorizontalList : MonoBehaviour
{
    [Header("item gap")]
    public float gap;

    public GameObject itemPrefab;

    HorizontalScrollViewList list;

    public event Action<ScrollListItem, object> renderItem;

    private void Awake()
    {
        list = new HorizontalScrollViewList(gameObject, itemPrefab, ItemRender, gap);
    }

    public void SetDatas<T>(T[] datas)
    {
        list.SetDatas(datas);
    }

    private void ItemRender(ScrollListItem item, object data)
    {
        renderItem?.Invoke(item, data);
    }

    private void Update()
    {
        list.Update();
    }
}
