using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PenisManager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField]
    private float targetHardness;
    [SerializeField]
    private float hardnessThresh;
    [SerializeField]
    private float penisMass;
    [SerializeField]
    private float penisMassScale;
    [SerializeField]
    private float penisDrag;
    [SerializeField]
    private float penisAngularDrag;
    [SerializeField]
    private bool gravity;
    [SerializeField]
    private LayerMask mask;

    [Header("References")]
    [SerializeField] private PenisManager horseCockManager;
    [SerializeField] private CosmeticManager cosmeticManager;
    [SerializeField]
    private TextMeshProUGUI hardnessTxt;
    [SerializeField]
    private PenisGrabber grabber;
    [SerializeField]
    private ConfigurableJoint[] penisJoints;
    [SerializeField]
    private ParticleSystem cum;
    [SerializeField]
    private Transform[] animatedJoints;
    [SerializeField]
    private Transform penisTip;
    [SerializeField]
    private Transform picture;
    [SerializeField]
    private GameObject cumSplat;
    [SerializeField]
    private GameObject cumButton;
    [SerializeField]
    private GameObject cumAAButton;
    [SerializeField] private GameObject horseCockButton;
    [SerializeField] private GameObject horseCock;
    [SerializeField] private GameObject penis;

    [HideInInspector]
    public float hardness;
    float velocity;
    
    bool isHard = false;

    public bool isHorseCock = false;

    Quaternion[] ogLocRots = new Quaternion[100];

    private void Start()
    {
        // Penis setup
        foreach (ConfigurableJoint joint in penisJoints)
        {
            joint.GetComponent<Rigidbody>().mass = penisMass;
            joint.GetComponent<Rigidbody>().drag = penisDrag;
            joint.GetComponent<Rigidbody>().angularDrag = penisAngularDrag;
            joint.GetComponent<Rigidbody>().useGravity = gravity;
            joint.massScale = penisMassScale;
        }

        cosmeticManager.penisTip = penisTip;

        if (isHorseCock)
        {
            cumButton.SetActive(true);

            cumButton.GetComponent<Button>().onClick.AddListener(Cum);
            cumAAButton.GetComponent<Button>().onClick.AddListener(CumAA);

            horseCockButton.SetActive(false);
            cumButton.SetActive(false);
        }
        
        hardness = 1;

        int i = 0;
        foreach (ConfigurableJoint joint in penisJoints)
        {
            ogLocRots[i] = joint.transform.localRotation;
            i++;
        }
    }
    bool hasSetFree = false;
    void Update()
    {
        #region Physics
        velocity = 0f;

        int i = 0;
        foreach (ConfigurableJoint joint in penisJoints)
        {
            //Set spring on x
            JointDrive driveX = new JointDrive();
            driveX = joint.angularXDrive;
            driveX.positionSpring = (hardness / 100) * targetHardness;
            joint.angularXDrive = driveX;

            //Set spring on x and y
            JointDrive driveYZ = new JointDrive();
            driveYZ = joint.angularXDrive;
            driveYZ.positionSpring = (hardness / 100) * targetHardness;
            joint.angularYZDrive = driveYZ;

            velocity += joint.GetComponent<Rigidbody>().velocity.magnitude;

            ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedJoints[i].localRotation, ogLocRots[i]);

            i++;
        }

        velocity /= penisJoints.Length; // Average velocity amongst all penis joint rigids

        if (velocity >= hardnessThresh && grabber.isGrabbing)
        {
            hardness += 0.1f;
        }
        #endregion

        #region Queries
        if (!isHard && !hasSetFree)
        {
            foreach (ConfigurableJoint joint in penisJoints)
            {
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;
            }

            hasSetFree = true;
        }
        if(hardness >= 100f && !isHard)
        {
            foreach (ConfigurableJoint joint in penisJoints)
            {
                joint.angularXMotion = ConfigurableJointMotion.Locked;
                joint.angularYMotion = ConfigurableJointMotion.Locked;
                joint.angularZMotion = ConfigurableJointMotion.Locked;
            }

            isHard = true;
            hasSetFree = false;
            cumButton.SetActive(true);
        }

        if (hardness >= 200f && !isHorseCock)
        {
            horseCockButton.SetActive(true);
        }

        #endregion
        
        hardnessTxt.text = "Hardness: " + hardness.ToString("#0.0") + "%";
    }

    public void HorseCock()
    {
        cumButton.GetComponent<Button>().onClick.RemoveListener(Cum);
        cumAAButton.GetComponent<Button>().onClick.RemoveListener(CumAA);
        penis.SetActive(false);
        horseCock.SetActive(true);
        horseCockManager.enabled = true;

        //END
        Destroy(this);
    }

    public void Cum()
    {
        penisTip.localEulerAngles = new Vector3(-90f, 0f, -90f);
        
        cum.Play();
        hardness -= 100f;
        if (hardness < 100)
        {
            isHard = false;
            cumButton.SetActive(false);
            hardness = 1f;
        }

        Physics.Raycast(cum.transform.position, cum.transform.forward, out RaycastHit hit, mask);
        GameObject splat = Instantiate(cumSplat, hit.point, Quaternion.LookRotation(hit.normal));
        splat.transform.position += splat.transform.forward * 0.05f;
    }

    public void CumAA()
    {
        penisTip.rotation = Quaternion.LookRotation(picture.position - penisTip.position);
        
        cum.Play();
        hardness -= 100f;
        if(hardness < 100f)
        {
            isHard = false;
            cumButton.SetActive(false);
            if(hardness < 1f)
                hardness = 1f;
        }

        Physics.Raycast(cum.transform.position, cum.transform.forward, out RaycastHit hit, mask);
        GameObject splat = Instantiate(cumSplat, hit.point, Quaternion.LookRotation(hit.normal));
        splat.transform.position += splat.transform.forward * 0.05f;
    }
}
