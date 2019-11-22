using Jing.ScrollViewList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalScrollViewList<int> list;

    public GameObject scrollView;

    public GameObject itemPrefab1;

    public GameObject itemPrefab2;

    public GameObject itemPrefab3;

    private void OnEnable()
    {
        list = new VerticalScrollViewList<int>(scrollView, itemPrefab3, RenderItem, 10);        
        list.SetDatas(new int[10000]);
    }

    private void RenderItem(ScrollListItem item, int data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("Index:{0}", item.index);
    }
}
