using Jing.TurbochargedScrollList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollListDemo : MonoBehaviour
{
    public int itemCount = 1;

    public GameObject scrollView;

    HorizontalScrollList list;

    void Awake()
    {
        var btnClear = GameObject.Find("BtnClear").GetComponent<Button>();
        btnClear.onClick.AddListener(() => {
            list.Clear();
        });

        var btnAdd = GameObject.Find("BtnAdd").GetComponent<Button>();
        btnAdd.onClick.AddListener(() => {
            list.Add(0);
        });

        var btnInsert = GameObject.Find("BtnInsert").GetComponent<Button>();
        btnInsert.onClick.AddListener(() => {
            list.Insert(2, 0);
        });

        var btnRemoveAt = GameObject.Find("BtnRemoveAt").GetComponent<Button>();
        btnRemoveAt.onClick.AddListener(() => {
            list.RemoveAt(2);
        });

        var btnRemove = GameObject.Find("BtnRemove").GetComponent<Button>();
        btnRemove.onClick.AddListener(() => {
            list.Remove(0);
        });

        var btnScroll2Index = GameObject.Find("BtnScroll2Index").GetComponent<Button>();
        btnScroll2Index.onClick.AddListener(() => {
            list.ScrollToItem(20);
        });

        var btnScroll2End = GameObject.Find("BtnScroll2End").GetComponent<Button>();
        btnScroll2End.onClick.AddListener(() => {
            //list.ScrollToItem(list.ItemCount);
            list.ScrollToPosition(list.ContentHeight);
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

        list = new HorizontalScrollList(scrollView, OnItemRender, 10);
        list.onRebuildContent += MoveToBottom;
        list.AddRange<int>(datas);
    }

    private void MoveToBottom()
    {
        list.ScrollToPosition(list.ContentWidth);
    }

    private void OnItemRender(ScrollListItem item, object data)
    {
        item.GetComponent<Item>().Refresh();
        Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }
}
