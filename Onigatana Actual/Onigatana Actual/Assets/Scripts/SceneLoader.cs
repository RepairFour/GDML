using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayClicked()
    {
        SceneManager.LoadScene("Test Level");
    }
    public void QuitClicked()
    {
        Application.Quit();
    }
}
