using UnityEngine;
using UnityEngine.SceneManagement;

public class Readybutton : MonoBehaviour
{
    public void OnReadyButtonClicked()
    {
        SceneManager.LoadScene("Level1");
    }
}