using UnityEngine;
using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
using UnityEngine.SceneManagement;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject itemMenu;
    [SerializeField]
    private GameObject mainMenu;

    //[SerializeField]
    //private GameObject x;
    
    List<GameObject> itemsInScene = new List<GameObject>();

    public void SpawnItem(GameObject x)
    {
        GameObject obj = Instantiate(x, transform.position, transform.rotation);
        itemsInScene.Add(obj);
    }

    /*
    private void Start()
    {
        Texture2D t = AssetPreview.GetAssetPreview(x);
        byte[] bytes = t.EncodeToPNG();
        File.WriteAllBytes("C:/Users/brett/Desktop/C#/Unity/PenisSim/Assets/Thumbnail.png", bytes);
    }
    */

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleMenu()
    {
        itemMenu.SetActive(!itemMenu.activeInHierarchy);
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
    }
    public void Clear()
    {
        foreach (GameObject item in itemsInScene)
        {
            Destroy(item);
        }
    }
}
