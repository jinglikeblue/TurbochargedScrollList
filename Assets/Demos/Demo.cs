using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public BaseScrollListComponent list;

    public InputField inputCount;

    public int itemCount = 1;

    int id = 0;

    private void Start()
    {
        //GUIDeviceInfo.Show();
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        var datas = new int[itemCount];
        for(int i = 0; i < datas.Length; i++)
        {
            datas[i] = id++;
        }

        list.renderItem += RenderItem;
        list.AddRange(datas);
    }

    private void RenderItem(ScrollListItem item, object data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("{0}", item.index);
        //Debug.Log($"渲染一个 Item [idx:{item.index} data:{data}]");
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Add()
    {
        var count = int.Parse(inputCount.text);
        for (int i = 0; i < count; i++)
        {
            var data = id++;
            list.Add(data);
        }
    }

    public void Insert()
    {
        var data = id++;
        list.Insert(2, data);
    }

    public void RemoveAt()
    {
        list.RemoveAt(2);
    }

    public void Remove()
    {
        list.Remove(0);
    }
}