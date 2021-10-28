using UnityEngine;
using System;

public class PenisGrabber : MonoBehaviour
{
    [SerializeField]
    private PenisManager manager;

    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private int penisLayer;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float zMoveSpeed;
    [SerializeField]
    private float zMaxPoint;
    [SerializeField]
    private LayerMask zMask;
    [SerializeField]
    private float grabbedMass;
    [SerializeField]
    private float massScale;
    [SerializeField]
    private float heightMultiplyer;
    [SerializeField]
    private float heightClamp;
    [SerializeField]
    private float connectedMassScale;

    [HideInInspector]
    public bool isGrabbing = false;
    FixedJoint joint;
    HingeJoint itemJoint;

    float ogMass;
    Rigidbody currentRigidbody;
    bool wereTwo = false;
    Vector3 lastPoint = Vector3.zero;
    bool isPenis = false;
    float ogZMoveSpeed;
    Item item;

    private void Start()
    {
        ogZMoveSpeed = zMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask);
            float height = Mathf.Clamp(touch.position.y * heightMultiplyer, 0f, heightClamp);

            if (hit.transform.gameObject != gameObject && !isGrabbing)
                transform.position = hit.point;
            if (hit.transform.gameObject != gameObject && isGrabbing)
                transform.position = new Vector3(hit.point.x, height, transform.position.z);

            if(item != null && itemJoint != null && item.axis)
                itemJoint.axis = item.sideAxis;
        }
        else if(Input.touchCount == 0)
        {
            LetGo();
        }

        if (Input.touchCount > 1 && isGrabbing)
        {
            Touch touch = Input.GetTouch(1);

            wereTwo = true;

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, zMask);

            float delta = (hit.point - lastPoint).normalized.z;
            delta = delta < 0 ? -1f : delta > 0 ? 1f : 0f;
            if (transform.position.z >= zMaxPoint)
                delta = -1f;
            lastPoint = hit.point;

            if (hit.transform.gameObject != gameObject)
                transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f, 0f, delta), zMoveSpeed * Time.deltaTime);

            if (item != null && itemJoint != null && item.axis)
                itemJoint.axis = item.forwardAxis;
        }

        if (wereTwo && Input.touchCount == 1 && isPenis)
        {
            LetGo();
            wereTwo = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.CompareTag("Grababble") || other.CompareTag("PenisPoint")) && other.GetComponent<Rigidbody>() && !isGrabbing)
        {
            if(other.gameObject.layer == penisLayer)
            {
                isPenis = true;

                joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = other.GetComponent<Rigidbody>();
                joint.massScale = massScale;
                joint.connectedMassScale = connectedMassScale;

                currentRigidbody = other.GetComponent<Rigidbody>();

                currentRigidbody.useGravity = false;
                ogMass = currentRigidbody.mass;
                currentRigidbody.mass = grabbedMass;
                manager.hardness += 1f;

                isGrabbing = true;
            }
            else if(other.GetComponent<Item>())
            {
                item = other.GetComponent<Item>();
                itemJoint = item.SetUp(GetComponent<Rigidbody>());

                isGrabbing = true;
                isPenis = false;
                zMoveSpeed = item.zMoveSpeed;
            }
        }
    }

    void LetGo()
    {
        if (currentRigidbody != null)
        {
            currentRigidbody.useGravity = true;
            currentRigidbody.mass = ogMass;
        }

        zMoveSpeed = ogZMoveSpeed;

        isGrabbing = false;

        if (itemJoint != null)
        {
            item.ToggleAbilityButton();

            Rigidbody rb = itemJoint.GetComponent<Rigidbody>();
            Destroy(itemJoint);

            //Update Physics
            rb.useGravity = false;
            rb.useGravity = true;
        }
        if (joint != null)
            Destroy(joint);
    }
}
