﻿using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class BaseScrollListDemo : MonoBehaviour
{
    [Header("初始化列表项数量")] public int itemCount = 100;

    public IScrollList list { get; protected set; }

    protected GameObject scrollView;

    /// <summary>
    /// 输入的数量
    /// </summary>
    protected int InputNumber
    {
        get
        {
            var input = GameObject.Find("InputNumber").GetComponent<InputField>();
            int count = 0;
            int.TryParse(input.text, out count);
            return count;
        }
    }

    //protected abstract void Clear();
    //protected abstract void AddRange();
    //protected abstract void Insert();
    //protected abstract void Remove();
    //protected abstract void RemoveAt();
    //protected abstract void ScrollToItem();
    //protected abstract void ScrollToPosition();

    protected int GetInputValue()
    {
        var input = GameObject.Find("InputNumber").GetComponent<InputField>();
        if (string.IsNullOrEmpty(input.text))
        {
            return 0;
        }

        return int.Parse(input.text);
    }

    void Awake()
    {
        scrollView = FindObjectOfType<ScrollRect>().gameObject;

        var btnExit = GameObject.Find("BtnExit").GetComponent<Button>();
        btnExit.onClick.AddListener(() => { SceneManager.LoadScene("DemoMain"); });

        var btnClear = GameObject.Find("BtnClear").GetComponent<Button>();
        btnClear.onClick.AddListener(() => { Clear(); });

        var btnAdd = GameObject.Find("BtnAdd").GetComponent<Button>();
        var input = GameObject.Find("InputNumber").GetComponent<InputField>();
        btnAdd.onClick.AddListener(() => { AddRange(); });

        var btnInsert = GameObject.Find("BtnInsert").GetComponent<Button>();
        btnInsert.onClick.AddListener(() => { Insert(); });

        var btnRemoveAt = GameObject.Find("BtnRemoveAt").GetComponent<Button>();
        btnRemoveAt.onClick.AddListener(() => { RemoveAt(); });

        var btnRemove = GameObject.Find("BtnRemove").GetComponent<Button>();
        btnRemove.onClick.AddListener(() => { Remove(); });

        var btnScroll2Index = GameObject.Find("BtnScroll2Index").GetComponent<Button>();
        btnScroll2Index.onClick.AddListener(() => { ScrollToItem(); });

        var btnScroll2End = GameObject.Find("BtnScroll2End").GetComponent<Button>();
        btnScroll2End.onClick.AddListener(() => { ScrollToPosition(); });
    }


    protected void AddRange()
    {
        var datas = new int[InputNumber];
        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = Random.Range(1, 10000);
        }

        list.AddRange(datas);
    }

    protected void Clear()
    {
        list.Clear();
    }

    protected void Insert()
    {
        list.Insert(GetInputValue(), Random.Range(1, 10000));
    }

    protected void Remove()
    {
        list.Remove(GetInputValue());
    }

    protected void RemoveAt()
    {
        list.RemoveAt(GetInputValue());
    }

    protected void ScrollToItem()
    {
        list.ScrollToItem(GetInputValue());
    }

    protected void ScrollToPosition()
    {
        list.ScrollToPosition(new Vector2(0, list.ContentHeight));
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
        
        InitList();
        
        list.AddRange(datas);
        
        list.onRenderItem += OnItemRender;
        list.onRebuildContent += OnRebuildContent;
        list.onRefresh += OnListRefresh;
        list.onItemBeforeReuse += OnItemBeforeReuse;
    }

    void OnEnable()
    {
    }

    /// <summary>
    /// 初始化列表
    /// </summary>
    protected abstract void InitList();

    /// <summary>
    /// 计算宽高
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    protected abstract (float, float) CalculateItemSize(RectTransform rt, int index);

    protected void OnItemRender(ScrollListItem item, object data, bool isFresh)
    {
        Debug.Log($"渲染列表项: {item.ToString()}, [data:{data}], [isFresh:{isFresh}]");

        if (isFresh)
        {
            var listItem = item.GetComponent<ListItem>();
            var listRT = list.scrollRect.GetComponent<RectTransform>();
            var (w,h) = CalculateItemSize(listRT, item.index);
            var content = string.Format("Index:{0} Data:{1}", item.index, item.data);
            listItem.Refresh(w, h, content);

            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => { Debug.Log($"点击列表项: {item.ToString()}"); });
        }
    }

    protected void OnListRefresh()
    {
        // Debug.Log("列表刷新");
    }

    protected void OnRebuildContent()
    {
        Debug.Log($"列表高度改变: {list.ContentHeight}");
    }

    protected void OnItemBeforeReuse(ScrollListItem item)
    {
        Debug.Log($"列表项被复用: {item.ToString()}");

        #region 清理旧的事件

        var button = item.GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        #endregion
    }
}