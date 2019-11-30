using Jing.TurbochargedScrollList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalScrollListDemo : MonoBehaviour
{
    public int itemCount = 1;

    public GameObject scrollView;

    HorizontalScrollList _list;

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

        _list = new HorizontalScrollList(scrollView, OnItemRender, 10);
        _list.AddRange<int>(datas);
    }

    private void OnItemRender(ScrollListItem item, object data)
    {
        Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }
}
