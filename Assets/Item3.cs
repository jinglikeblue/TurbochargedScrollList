using Jing.ScrollViewList;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Item3 : MonoBehaviour
{
    public Image img;

    public Sprite[] sprites;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        StartCoroutine(SpriteRefresh());
    }

    IEnumerator SpriteRefresh()
    {
        while (true)
        {
            var item = gameObject.GetComponent<ScrollListItem>();
            if (null != item)
            {
                img.sprite = sprites[item.index % sprites.Length];
                img.SetNativeSize();
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
