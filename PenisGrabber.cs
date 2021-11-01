using UnityEngine;

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
    private float zMax;
    [SerializeField]
    private float zMin;
    [SerializeField]
    private float zMoveRate;
    [SerializeField]
    private float xClamp;
    [SerializeField]
    private float yMin;
    [SerializeField]
    private float yMax;
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
        if (Input.GetMouseButton(0)) // Input.touchCount > 0
        {
            //Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // touch.position
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask);
            float height = Mathf.Clamp(Input.mousePosition.y * heightMultiplyer, 0f, heightClamp); // touch.position.y

            if (hit.transform.gameObject != gameObject && !isGrabbing)
                transform.position = hit.point;
            if (hit.transform.gameObject != gameObject && isGrabbing && isPenis)
                transform.position = new Vector3(Mathf.Clamp(hit.point.x, -xClamp, xClamp), Mathf.Clamp(hit.point.y, yMin, yMax) * heightMultiplyer, transform.position.z); // y: height
            else if(hit.transform.gameObject != gameObject && isGrabbing)
                transform.position = new Vector3(hit.point.x, hit.point.y * heightMultiplyer, transform.position.z); // y: height

            if (item != null && itemJoint != null && item.axis)
                itemJoint.axis = item.sideAxis;
        }
        else // Input.touchCount == 0
        {
            LetGo();
        }

        if (Input.GetMouseButton(1) && isGrabbing) // Input.touchCount > 1
        {
            //Touch touch = Input.GetTouch(1);

            wereTwo = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // touch.position
            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, zMask);

            float delta = (hit.point - lastPoint).normalized.z;
            delta = delta < 0 ? -1f : delta > 0 ? 1f : 0f;
            if (transform.position.z >= zMax)
                delta = -1f;
            else if (transform.position.z <= zMin)
                delta = 1f;
            lastPoint = hit.point;

            if (hit.transform.gameObject != gameObject)
                transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f, 0f, delta * zMoveRate), zMoveSpeed * Time.deltaTime);

            if (item != null && itemJoint != null && item.axis)
                itemJoint.axis = item.forwardAxis;
        }

        /*
        if (wereTwo && Input.touchCount == 1 && isPenis)
        {
            LetGo();
            wereTwo = false;
        }
        */
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
            //item.ToggleAbilityButton();

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
