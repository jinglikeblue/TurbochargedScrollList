using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public Text textLabel;      

    public void Refresh(float w, float h, string content)
    {
        var item = gameObject.GetComponent<ScrollListItem>();

        textLabel = transform.Find("Text").GetComponent<Text>();
        textLabel.text = content;

        var rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
}
