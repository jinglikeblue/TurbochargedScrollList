using Jing.ScrollViewList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalList vList;
    public HorizontalList hList;

    public InputField inputCount;

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

    public void Clear()
    {
        vList.Clear();
        hList.Clear();
    }

    public void Add()
    {
        var count = int.Parse(inputCount.text);
        for(int i = 0; i < count; i++)
        {
            vList.Add(0);
            hList.Add(0);
        }
    }
}
