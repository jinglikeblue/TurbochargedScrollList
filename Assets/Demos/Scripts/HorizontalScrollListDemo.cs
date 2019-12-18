using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollListDemo : BaseScrollListDemo
{
    protected override void InitItems()
    {
        var datas = new int[itemCount];
        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = i;
        }

        GameObject itemPrefab = transform.Find("Horizontal Scroll View/Item").gameObject;

        var ls = new HorizontalLayoutSettings();
        ls.gap = 10;
        ls.paddingRight = 500;
        list = new HorizontalScrollList(scrollView.GetComponent<ScrollRect>(), itemPrefab, ls);        
        list.onRenderItem += OnItemRender;
        list.onRebuildContent += OnRebuildContent;
        list.onRefresh += OnListRefresh;
        list.AddRange(datas);
    }

    protected void OnItemRender(ScrollListItem item, object data, bool isRefresh)
    {
        item.GetComponent<ListItem>().Refresh(0,0);
        Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }
}
