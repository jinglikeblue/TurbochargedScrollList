
using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;
using Zero;

public class Demo : MonoBehaviour
{
    public VerticalScrollListComponent vList;
    public HorizontalScrollListComponent hList;

    public InputField inputCount;

    private void Start()
    {
        //GUIDeviceInfo.Show();
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        var datas = new int[500];

        vList.renderItem += RenderItem;
        vList.AddDatas(datas);

        hList.renderItem += RenderItem;
        hList.AddDatas(datas);
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
