using UnityEngine;
using UnityEngine.UI;

public class Item3 : MonoBehaviour
{
    public Image img;

    public Sprite[] sprites;

    private void Awake()
    {
        img.sprite = sprites[Random.Range(0, sprites.Length)];
        img.SetNativeSize();
    }

    private void OnEnable()
    {
        
    }
}
