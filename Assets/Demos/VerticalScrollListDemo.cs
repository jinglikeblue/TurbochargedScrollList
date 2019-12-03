using System;
using System.Collections.Generic;
using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollListDemo : MonoBehaviour
{
    public int itemCount = 1;

    public GameObject scrollView;

    IScrollList<int> list;

    void Awake()
    {
        var btnClear = GameObject.Find("BtnClear").GetComponent<Button>();
        btnClear.onClick.AddListener(() =>
        {
            list.Clear();
        });

        var btnAdd = GameObject.Find("BtnAdd").GetComponent<Button>();
        var input = GameObject.Find("InputNumber").GetComponent<InputField>();
        btnAdd.onClick.AddListener(() =>
        {
            int count = 0;
            int.TryParse(input.text, out count);
            var datas = new int[count];
            list.AddRange(datas);
        });

        var btnInsert = GameObject.Find("BtnInsert").GetComponent<Button>();
        btnInsert.onClick.AddListener(() =>
        {
            list.Insert(2, 0);
        });

        var btnRemoveAt = GameObject.Find("BtnRemoveAt").GetComponent<Button>();
        btnRemoveAt.onClick.AddListener(() =>
        {
            list.RemoveAt(2);
        });

        var btnRemove = GameObject.Find("BtnRemove").GetComponent<Button>();
        btnRemove.onClick.AddListener(() =>
        {
            list.Remove(0);
        });

        var btnScroll2Index = GameObject.Find("BtnScroll2Index").GetComponent<Button>();
        btnScroll2Index.onClick.AddListener(() =>
        {
            list.ScrollToItem(20);
        });

        var btnScroll2End = GameObject.Find("BtnScroll2End").GetComponent<Button>();
        btnScroll2End.onClick.AddListener(() =>
        {
            //list.ScrollToItem(list.ItemCount);
            list.ScrollToPosition(new Vector2(0, list.ContentHeight));
        });
    }

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

        list = new VerticalScrollList<int>(scrollView, OnItemRender);
        list.onRebuildContent += MoveToBottom;
        list.onRefresh += OnListRefresh;
        list.AddRange(datas);
    }

    private void OnListRefresh()
    {
        Debug.Log("列表刷新");
    }

    private void MoveToBottom()
    {
        Debug.Log("列表高度改变");       
    }

    private void OnItemRender(ScrollListItem item, int data, bool isRefresh)
    {
        item.GetComponent<Item>().Refresh();
        Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }
}
