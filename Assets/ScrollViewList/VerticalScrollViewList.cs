using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 垂直滚动列表
/// </summary>
public class VerticalScrollViewList<TData> : BaseScrollViewList<TData>
{
    public VerticalScrollViewList(GameObject scrollView, GameObject itemPrefab, float gap = 0) : base(scrollView, itemPrefab, gap)
    {
    }

    protected override void OnSetDatas()
    {
        //计算content高度
        var h = _datas.Length * itemSize.y;
        //加上间距
        h += ((_datas.Length - 1) * gap);

        SetContentSize(itemSize.x, h);
    }

    protected override void Refresh()
    {
        var showStartPos = 1 - scrollPos.y;        

        int startIdx = (int)(_datas.Length * showStartPos);

        var contentHeight = content.GetComponent<RectTransform>().sizeDelta.y;

        var contentPos = showStartPos * (contentHeight - scrollRect.viewport.rect.height);

        Debug.LogFormat("滚动位置:{0}, 起始索引:{1}, Content位置:{2}", showStartPos, startIdx, contentPos);

        int dataIdx = (int)(contentPos / (itemSize.y + gap));

        if(dataIdx < 0)
        {
            dataIdx = 0;
        }

        float startPos = -1 * dataIdx * (itemSize.y + gap);

        Debug.LogFormat("显示的开始索引:{0} 开始的位置:{1}", dataIdx, startPos);

        var items = RecycleItems();
        int usedRecycleItem = 0;
        //显示的内容刚好大于这个值即可           
        float contentHeightLimit = scrollRect.viewport.rect.height;
        float itemY = startPos;
        do
        {
            if(dataIdx >= _datas.Length)
            {
                break;
            }

            var data = _datas[dataIdx];
            GameObject go;
            RectTransform rt;
            if (usedRecycleItem < items.Count)
            {
                go = items[usedRecycleItem++];
                rt = go.GetComponent<RectTransform>();
            }
            else
            {
                go = GameObject.Instantiate(itemPrefab, content);
                rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);                
            }

            go.name = string.Format("item_{0}", dataIdx);

            //先渲染，再定位
            RenderItem(dataIdx, go, data);

            var pos = Vector3.zero;
            pos.y = itemY;
            rt.localPosition = pos;
            //下一个item的Y坐标
            itemY -= (rt.rect.height + gap);
            //下一个item的索引
            dataIdx++;

            if(-contentPos - itemY >= contentHeightLimit)
            {
                //显示区域已满
                break;
            }
        }
        while (true);

        for(int i = usedRecycleItem; i < items.Count; i++)
        {
            GameObject.Destroy(items[i]);
        }
    }

    

    List<GameObject> RecycleItems()
    {
        List<GameObject> recycledItems = new List<GameObject>();
        for(int i = 0; i < content.childCount; i++)
        {
            recycledItems.Add(content.GetChild(i).gameObject);
        }
        return recycledItems;
    }
}
