using UnityEngine;

public class CosmeticManager : MonoBehaviour
{
    [HideInInspector] public Transform penisTip;
    [SerializeField] private GameObject cosmeticMenu;
    [SerializeField] private GameObject removeCosmeticButton;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject itemMenu;

    Cosmetic currentCosmetic;

    public void ToggleCosmenticMenu()
    {
        cosmeticMenu.SetActive(!cosmeticMenu.activeInHierarchy);
        if (cosmeticMenu.activeInHierarchy)
        {
            mainMenu.SetActive(false);
            itemMenu.SetActive(false);
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }

    public void RemoveCosmetic()
    {
        Destroy(currentCosmetic.gameObject);
        removeCosmeticButton.SetActive(false);
    }

    public void ApplyCosmetic(GameObject cosmetic)
    {
        if(currentCosmetic != null)
            Destroy(currentCosmetic.gameObject);
        currentCosmetic = Instantiate(cosmetic, penisTip).GetComponent<Cosmetic>();
        removeCosmeticButton.SetActive(true);
    }
}
