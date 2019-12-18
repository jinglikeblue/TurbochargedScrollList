using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VDemo()
    {
        SceneManager.LoadScene("VerticalScrollListDemo", LoadSceneMode.Single);
    }

    public void HDemo()
    {
        SceneManager.LoadScene("HorizontalScrollListDemo", LoadSceneMode.Single);
    }

    public void GridDemo()
    {
        SceneManager.LoadScene("HorizontalScrollListDemo", LoadSceneMode.Single);
    }
}
