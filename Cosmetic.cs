using UnityEngine;

public class Cosmetic : MonoBehaviour
{
    [SerializeField] private Vector3 posOffset;
    [SerializeField] private Vector3 rotOffset;
    [SerializeField] private Vector3 scaleOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles = rotOffset;
        transform.localPosition = posOffset;
        transform.localScale = scaleOffset;
    }
}
