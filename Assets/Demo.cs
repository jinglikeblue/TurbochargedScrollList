using Jing.ScrollViewList;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public VerticalScrollViewList list;

    public GameObject scrollView;

    public GameObject itemPrefab1;

    public GameObject itemPrefab2;

    public GameObject itemPrefab3;

    private void OnEnable()
    {
        list = new VerticalScrollViewList(scrollView, RenderItem, 10);        
        list.SetDatas(new int[50], itemPrefab3);
    }

    private void RenderItem(ScrollListItem item, object data)
    {
        var text = item.transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("Index:{0}", item.index);
    }

    private void LateUpdate()
    {
        list.Update();
    }

    public void Rebuild()
    {
        list.RebuildContent();
    }
}
