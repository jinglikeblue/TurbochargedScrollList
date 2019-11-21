using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalScrollViewList<int> list;

    public GameObject scrollView;

    public GameObject itemPrefab1;

    public GameObject itemPrefab2;

    private void OnEnable()
    {
        list = new VerticalScrollViewList<int>(scrollView, itemPrefab2, 10);
        list.onRenderItem += OnRenderItem;
        list.SetDatas(new int[100]);
    }

    private void OnRenderItem(int index, GameObject item, int data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("Index:{0}", index);
    }
}
