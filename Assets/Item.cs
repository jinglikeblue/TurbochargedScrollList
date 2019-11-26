using Jing.TurbochargedScrollList;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Image img;

    public Sprite[] sprites;

    ScrollListItem _item;

    int idx;

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
            if(null == _item)
            {
                _item = gameObject.GetComponent<ScrollListItem>();
            }
            
            if (null != _item)
            {                                
                img.sprite = sprites[(int)_item.data % sprites.Length];
                img.SetNativeSize();                
                //Debug.Log($"[{gameObject.name}] 索引值[{item.index}] 纹理(H:{img.sprite.texture.height})[{img.sprite.name}]");
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        if(null != _item)
        {
            if(idx != _item.index)
            {
                idx = _item.index;
                var text = transform.Find("Text").GetComponent<Text>();
                text.text = string.Format("Idx:{0} V:{1}", idx, _item.data);                
            }
        }
    }
}
