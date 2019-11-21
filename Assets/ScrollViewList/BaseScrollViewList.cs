using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基于UGUI中Scroll View组件的列表工具
/// </summary>
public abstract class BaseScrollViewList<TData>
{
    public readonly ScrollRect scrollRect;
    
    public readonly Transform content;    

    public readonly GameObject gameObject;

    TData[] _datas;

    /// <summary>
    /// 获取数据拷贝
    /// </summary>
    public TData[] GetDatasCopy()
    {
        if(null == _datas)
        {
            return null;
        }

        TData[] datas = new TData[_datas.Length];
        _datas.CopyTo(datas, 0);
        return datas;
    }

    public BaseScrollViewList(GameObject gameObject)
    {
        this.gameObject = gameObject;
        scrollRect = gameObject.GetComponent<ScrollRect>();        
        content = scrollRect.viewport.GetChild(0);

        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnScroll(Vector2 v)
    {
        Debug.LogFormat("滚动列表: {0}", v.ToString());
    }
    
    public void SetDatas(TData[] datas)
    {
        Clear();
        _datas = datas;
    }   
    
    public void Clear()
    {
        int childIdx = content.childCount;
        while(--childIdx > -1)
        {
            GameObject.Destroy(content.GetChild(childIdx));
        }
    }

    public void Destroy()
    {
        scrollRect.onValueChanged.RemoveListener(OnScroll);
    }
}
