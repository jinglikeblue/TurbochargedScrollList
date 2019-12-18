using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public Text textLabel;      

    public void Refresh(float w, float h)
    {
        var item = gameObject.GetComponent<ScrollListItem>();

        textLabel = transform.Find("Text").GetComponent<Text>();
        textLabel.text = string.Format("Index:{0} Data:{1}", item.index, item.data);

        var rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
}
