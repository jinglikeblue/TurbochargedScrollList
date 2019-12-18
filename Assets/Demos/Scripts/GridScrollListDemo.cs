using Jing.TurbochargedScrollList;
using UnityEngine;

public class GridScrollListDemo : BaseScrollListDemo
{   
    public TurbochargedGridScrollList listComponent;

    protected override void InitItems()
    {
        var datas = new int[itemCount];
        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = i;
        }

        list = listComponent.GetList();        
        list.onRenderItem += OnItemRender;
        list.onRebuildContent += OnRebuildContent;
        list.onRefresh += OnListRefresh;
        list.AddRange(datas);
    }

    private void OnItemRender(ScrollListItem item, object data, bool isRefresh)
    {
        if (isRefresh)
        {
            var listItem = item.GetComponent<ListItem>();
            var listRT = list.scrollRect.GetComponent<RectTransform>();
            var content = string.Format("Index:{0}\nData:{1}", item.index, item.data);
            listItem.Refresh(150 + ((item.index % 15) * 20), listRT.rect.height - 40, content);
        }
    }
}
