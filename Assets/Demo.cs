using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalScrollListComponent vList;
    public HorizontalScrollListComponent hList;

    public InputField inputCount;

    int id = 0;

    private void Start()
    {
        //GUIDeviceInfo.Show();
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        var datas = new int[500];
        for(int i = 0; i < datas.Length; i++)
        {
            datas[i] = id++;
        }

        vList.renderItem += RenderItem;
        vList.AddRange(datas);

        hList.renderItem += RenderItem;
        hList.AddRange(datas);
    }

    private void RenderItem(ScrollListItem item, object data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("Idx:{0} V:{1}", item.index, data);
        Debug.Log($"渲染一个 Item [idx:{item.index} data:{data}]");
    }

    public void Clear()
    {
        vList.Clear();
        hList.Clear();
    }

    public void Add()
    {
        var count = int.Parse(inputCount.text);
        for (int i = 0; i < count; i++)
        {
            var data = id++;
            vList.Add(data);
            hList.Add(data);
        }
    }

    public void Insert()
    {
        var data = id++;
        vList.Insert(2, data);
        hList.Insert(2, data);
    }

    public void RemoveAt()
    {
        vList.RemoveAt(2);
        hList.RemoveAt(2);
    }

    public void Remove()
    {
        vList.Remove(0);
        hList.Remove(0);
    }
}