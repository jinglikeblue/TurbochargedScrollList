using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollListDemo : BaseScrollListDemo
{
    protected override void InitList()
    {
        GameObject itemPrefab = transform.Find("Scroll View/ListItem").gameObject;
        
        var ls = new HorizontalLayoutSettings();
        ls.gap = 10;
        ls.paddingRight = 500;
        list = new HorizontalScrollList(scrollView.GetComponent<ScrollRect>(), itemPrefab, ls);        
    }
    
    protected override (float, float) CalculateItemSize(RectTransform rt, int index)
    {
        return (150 + ((index % 15) * 20), rt.rect.height - 40);
    }
}
