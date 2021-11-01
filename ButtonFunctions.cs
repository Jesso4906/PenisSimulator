using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    
    public void Play()
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(1);
    }
}
