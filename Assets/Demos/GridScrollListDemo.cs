using Jing.TurbochargedScrollList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridScrollListDemo : MonoBehaviour
{
    public int itemCount = 100;

    public GridScrollListComponent list;

    void Awake()
    {
        var btnClear = GameObject.Find("BtnClear").GetComponent<Button>();
        btnClear.onClick.AddListener(() => {
            list.Clear();
        });

        var btnAdd = GameObject.Find("BtnAdd").GetComponent<Button>();
        var input = GameObject.Find("InputNumber").GetComponent<InputField>();
        btnAdd.onClick.AddListener(() => {
            int count = 0;
            int.TryParse(input.text, out count);
            var datas = new int[count];
            list.AddRange(datas);
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
            list.ScrollToPosition(list.ContentWidth, list.ContentHeight);
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

        //var list = new GridScrollList(GameObject.Find("Grid Scroll View"), OnRenderItem, new Vector2(10,10), EGridConstraint.FIXED_ROW_COUNT, 3);
        list.renderItem += OnRenderItem;
        list.AddRange(datas);
    }

    private void OnRenderItem(ScrollListItem item, object data, bool isFresh)
    {
        item.GetComponent<Item>().Refresh();
        Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }
}
