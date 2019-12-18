using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollListDemo : BaseScrollListDemo
{
    IScrollList list;

    public GameObject itemPrefab;

    protected override void InitItems()
    {
        var datas = new int[itemCount];
        for (int i = 0; i < datas.Length; i++)
        {
            datas[i] = i;
        }

        var layout = new VerticalLayoutSettings();
        layout.gap = 10;
        layout.paddingTop = 50;
        list = new VerticalScrollList(scrollView.GetComponent<ScrollRect>(), itemPrefab, layout);
        list.onRenderItem += OnItemRender;
        list.onRebuildContent += OnRebuildContent;
        list.onRefresh += OnListRefresh;
        list.AddRange(datas);
    }
    protected void OnItemRender(ScrollListItem item, object data, bool isRefresh)
    {
        if (isRefresh)
        {
            var listItem = item.GetComponent<ListItem>();
            var listRT = list.scrollRect.GetComponent<RectTransform>();
            listItem.Refresh(listRT.rect.width - 40, 100 + ((item.index % 15) * 20));
        }

        //Debug.LogFormat("渲染Item [idx:{0}, value:{1}]", item.index, data);
    }


    protected override void AddRange()
    {
        var datas = new int[InputNumber];
        list.AddRange(datas);
    }

    protected override void Clear()
    {
        list.Clear();
    }

    protected override void Insert()
    {
        list.Insert(GetInputValue(), Random.Range(1,10000));
    }

    protected override void Remove()
    {
        list.Remove(GetInputValue());
    }

    protected override void RemoveAt()
    {
        list.RemoveAt(GetInputValue());
    }

    protected override void ScrollToItem()
    {
        list.ScrollToItem(GetInputValue());
    }

    protected override void ScrollToPosition()
    {
        list.ScrollToPosition(new Vector2(0, list.ContentHeight));
    }
}
