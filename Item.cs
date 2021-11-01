using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System;

public class Item : MonoBehaviour
{
    [SerializeField]
    private string ID;
    [Header("Bat")]
    [SerializeField]
    private float swingForce = 80000f;
    [Header("Chainsaw")]
    [SerializeField]
    private Transform bladeCol;
    [SerializeField]
    private float rotRate;
    bool isOn = false;
    
    UnityAction ability;

    [Header("Vars")]
    [SerializeField]
    private Vector3 pickUpRot;
    [SerializeField]
    private float breakForce = 1000f;
    [SerializeField]
    private float breakTorque = 1000f;
    [SerializeField]
    private float spring = 0.33f;
    public float zMoveSpeed;
    [SerializeField]
    private Vector3 anchor;

    public Vector3 sideAxis = Vector3.right;
    public Vector3 forwardAxis = Vector3.up;
    [HideInInspector]
    public bool axis = true;

    Button abiltiyButton;
    HingeJoint joint;

    Transform penisPoint;

    private void Start()
    {
        abiltiyButton = GameObject.FindWithTag("AbilityButton").GetComponent<Button>();
        penisPoint = GameObject.FindWithTag("PenisPoint").transform;

        switch (ID)
        {
            case "Swing":
                ability = () => SwingBat();
                break;
            case "Toggle":
                ability = () =>
                {
                    isOn = !isOn;
                    if (!isOn)
                    {
                        bladeCol.gameObject.SetActive(false);
                        transform.eulerAngles = pickUpRot;
                    }
                    else
                    {
                        bladeCol.gameObject.SetActive(true);
                    }
                };
                break;
        }
    }

    private void Update()
    {
        if (isOn)
        {
            transform.rotation = Quaternion.LookRotation((penisPoint.position - transform.position));
            bladeCol.Rotate(new Vector3(rotRate, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ability();
        }
    }

    public HingeJoint SetUp(Rigidbody connectedBody)
    {
        joint = gameObject.AddComponent<HingeJoint>();
        joint.connectedBody = connectedBody;
        joint.autoConfigureConnectedAnchor = true;
        joint.breakForce = breakForce;
        joint.breakTorque = breakTorque;
        joint.anchor = anchor;
        joint.useSpring = true;
        transform.eulerAngles = pickUpRot;

        JointSpring spr = new JointSpring();
        spr.spring = spring;
        joint.spring = spr;

        /*
        ToggleAbilityButton();
        abiltiyButton.onClick.RemoveAllListeners();
        abiltiyButton.onClick.AddListener(ability);
        abiltiyButton.GetComponentInChildren<TextMeshProUGUI>().text = ID;
        */

        return joint;
    }

    public void ToggleAbilityButton()
    {
        abiltiyButton.enabled = !abiltiyButton.enabled;
        abiltiyButton.GetComponent<Image>().enabled = !abiltiyButton.GetComponent<Image>().enabled;
        abiltiyButton.GetComponentInChildren<TextMeshProUGUI>().enabled = !abiltiyButton.GetComponentInChildren<TextMeshProUGUI>().enabled;
    } 

    //---------------ITEM FUNCTIONS---------------\\
    void SwingBat()
    {
        if (!axis)
            return;
        
        StartCoroutine(SwingBatC(GetComponent<Rigidbody>(), swingForce));
    }
    private IEnumerator SwingBatC(Rigidbody bat, float force)
    {
        axis = false;
        bat.transform.rotation = Quaternion.LookRotation(penisPoint.position - bat.transform.position);
        joint.axis = Vector3.right;
        bat.AddForce(Vector3.right * force * Time.deltaTime, ForceMode.Acceleration);
        yield return new WaitForSeconds(2.1f);
        axis = true;
    }
}
