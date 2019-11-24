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

    private void OnDisable()
    {
        StopAllCoroutines();
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
                Debug.Log($"[{gameObject.name}] 索引值[{item.index}] 纹理(H:{img.sprite.texture.height})[{img.sprite.name}]");
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
