using Jing.TurbochargedScrollList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScrollListDemo : MonoBehaviour
{
    public int itemCount = 100;

    public GridScrollListComponent list;

    void Start()
    {
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        var datas = new int[itemCount];
        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = i;
        }

        list.renderItem += OnRenderItem;
        list.AddRange(datas);
    }

    private void OnRenderItem(ScrollListItem item, object data)
    {
        item.GetComponent<Item>().Refresh();
        Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }
}
