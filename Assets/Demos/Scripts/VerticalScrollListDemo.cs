using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollListDemo : BaseScrollListDemo
{
    public GameObject itemPrefab;

    protected override void InitList()
    {
        var layout = new VerticalLayoutSettings();
        layout.gap = 10;
        layout.paddingTop = 50;
        list = new VerticalScrollList(scrollView.GetComponent<ScrollRect>(), itemPrefab, layout);
    }
    
    protected override (float, float) CalculateItemSize(RectTransform rt, int index)
    {
        return (rt.rect.width - 40, 100 + ((index % 15) * 20));
    }
    
}