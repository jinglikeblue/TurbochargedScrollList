using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseScrollListDemo : MonoBehaviour
{
    [Header("初始化列表项数量")]
    public int itemCount = 100;

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

    protected abstract void Clear();
    protected abstract void AddRange();
    protected abstract void Insert();
    protected abstract void Remove();
    protected abstract void RemoveAt();
    protected abstract void ScrollToItem();
    protected abstract void ScrollToPosition();

    void Awake()
    {
        scrollView = FindObjectOfType<ScrollRect>().gameObject;

        var btnClear = GameObject.Find("BtnClear").GetComponent<Button>();
        btnClear.onClick.AddListener(() =>
        {
            Clear();
        });

        var btnAdd = GameObject.Find("BtnAdd").GetComponent<Button>();
        var input = GameObject.Find("InputNumber").GetComponent<InputField>();
        btnAdd.onClick.AddListener(() =>
        {
            AddRange();
        });

        var btnInsert = GameObject.Find("BtnInsert").GetComponent<Button>();
        btnInsert.onClick.AddListener(() =>
        {
            Insert();
        });

        var btnRemoveAt = GameObject.Find("BtnRemoveAt").GetComponent<Button>();
        btnRemoveAt.onClick.AddListener(() =>
        {
            RemoveAt();
        });

        var btnRemove = GameObject.Find("BtnRemove").GetComponent<Button>();
        btnRemove.onClick.AddListener(() =>
        {
            Remove();
        });

        var btnScroll2Index = GameObject.Find("BtnScroll2Index").GetComponent<Button>();
        btnScroll2Index.onClick.AddListener(() =>
        {
            ScrollToItem();
        });

        var btnScroll2End = GameObject.Find("BtnScroll2End").GetComponent<Button>();
        btnScroll2End.onClick.AddListener(() =>
        {            
            ScrollToPosition();
        });
    }

    void Start()
    {
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        InitItems();
    }

    void OnEnable()
    {
        
    }

    protected abstract void InitItems();

    protected void OnListRefresh()
    {
        Debug.Log("列表刷新");
    }

    protected void OnRebuildContent()
    {
        Debug.Log("列表高度改变");
    }
}
