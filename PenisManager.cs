using TMPro;
using UnityEngine;

public class PenisManager : MonoBehaviour
{
    [SerializeField]
    private float targetHardness;
    [SerializeField]
    private TextMeshProUGUI hardnessTxt;
    [SerializeField]
    private PenisGrabber grabber;
    [SerializeField]
    private ConfigurableJoint[] penisJoints;
    [SerializeField]
    private Transform[] animatedJoints;
    [SerializeField]
    private ParticleSystem cum;
    [SerializeField]
    private GameObject cumSplat;
    [SerializeField]
    private GameObject cumButton;
    [SerializeField]
    private Transform penisTip;
    [SerializeField]
    private Transform picture;
    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private float velocityThresh;
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

    [HideInInspector]
    public float hardness;
    bool isHard = false;
    float velocity;

    Quaternion[] ogLocRots = new Quaternion[100];

    private void Start()
    {
        hardness = 1;

        int i = 0;
        foreach (ConfigurableJoint joint in penisJoints)
        {
            joint.GetComponent<Rigidbody>().mass = penisMass;
            joint.GetComponent<Rigidbody>().drag = penisDrag;
            joint.GetComponent<Rigidbody>().angularDrag = penisAngularDrag;
            joint.GetComponent<Rigidbody>().useGravity = gravity;
            joint.massScale = penisMassScale;

            ogLocRots[i] = joint.transform.localRotation;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        velocity = 0f;

        int i = 0;
        foreach (ConfigurableJoint joint in penisJoints)
        {
            JointDrive driveX = new JointDrive();
            driveX = joint.angularXDrive;
            driveX.positionSpring = (hardness / 100) * targetHardness;
            joint.angularXDrive = driveX;

            JointDrive driveYZ = new JointDrive();
            driveYZ = joint.angularXDrive;
            driveYZ.positionSpring = (hardness / 100) * targetHardness;
            joint.angularYZDrive = driveYZ;

            velocity += joint.GetComponent<Rigidbody>().velocity.magnitude;

            ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedJoints[i].localRotation, ogLocRots[i]);

            i++;
        }

        velocity /= penisJoints.Length;

        if (velocity >= velocityThresh && grabber.isGrabbing)
        {
            hardness += 0.1f;
        }

        if(!isHard)
        {
            foreach (ConfigurableJoint joint in penisJoints)
            {
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;
            }
        }
        if(hardness > 100f)
        {
            foreach (ConfigurableJoint joint in penisJoints)
            {
                joint.angularXMotion = ConfigurableJointMotion.Locked;
                joint.angularYMotion = ConfigurableJointMotion.Locked;
                joint.angularZMotion = ConfigurableJointMotion.Locked;
            }

            isHard = true;
            cumButton.SetActive(true);
        }

        hardnessTxt.text = "Hardness: " + hardness.ToString("#0.0") + "%";
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
        Quaternion ogRot = penisTip.rotation;
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
