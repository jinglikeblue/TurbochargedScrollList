using System;
using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollListDemo : MonoBehaviour
{
    public int itemCount = 1;

    public GameObject scrollView;

    VerticalScrollList<int> _list;

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

        _list = new VerticalScrollList<int>(scrollView, itemRender);
        _list.AddRange(datas);
    }

    private void itemRender(ScrollListItem item, int data)
    {
        
    }
}
