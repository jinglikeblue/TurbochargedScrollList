using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMain : MonoBehaviour
{
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
        SceneManager.LoadScene("GridScrollListDemo", LoadSceneMode.Single);
    }
}
