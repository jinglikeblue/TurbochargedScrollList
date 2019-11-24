using System;
using Jing.ScrollViewList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalList vList;
    public HorizontalList hList;

    private void Start()
    {
        var datas = new int[500];

        vList.renderItem += RenderItem;
        vList.SetDatas(datas);

        hList.renderItem += RenderItem;
        hList.SetDatas(datas);
    }

    private void RenderItem(ScrollListItem item, object data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("Index:{0}", item.index);
    }
}
