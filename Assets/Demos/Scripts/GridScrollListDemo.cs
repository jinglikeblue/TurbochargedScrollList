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

    private void OnItemRender(ScrollListItem item, object data, bool isFresh)
    {
        if (isFresh)
        {
            item.GetComponent<ListItem>().Refresh(0,0,null);
            //Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
        }
    }
}
