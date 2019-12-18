using Jing.TurbochargedScrollList;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Image img;

    public Sprite[] sprites;

    public void Refresh()
    {
        var item = gameObject.GetComponent<ScrollListItem>();
        img.sprite = sprites[(int)item.data % sprites.Length];
        img.SetNativeSize();
        var text = transform.Find("Text").GetComponent<Text>();
        text.text = string.Format("{0}", item.index);
    }
}
