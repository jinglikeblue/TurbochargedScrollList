using Jing.TurbochargedScrollList;
using UnityEngine;

public class GridScrollListDemo : BaseScrollListDemo
{   
    public TurbochargedGridScrollList listComponent;

    protected override void InitList()
    {
        list = listComponent.GetList();        
    }

    protected override (float, float) CalculateItemSize(RectTransform rt, int index)
    {
        return (rt.rect.width, rt.rect.height);
    }
}
