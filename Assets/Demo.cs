using System;
using Jing.ScrollViewList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalList vList;

    private void Start()
    {
        vList.renderItem += RenderItem;
        vList.SetDatas(new int[500]);
    }

    private void RenderItem(ScrollListItem item, object data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("Index:{0}", item.index);
    }
}
