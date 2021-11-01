using UnityEngine;

public class ImagePicker : MonoBehaviour
{
    [SerializeField]
    private Material picture;
	[SerializeField]
	private Vector3 imageScale;
	[SerializeField]
	private Texture defaultTexture;
	[SerializeField]
	private Transform picturePlane;
	[SerializeField]
	private GameObject takePicButton;

	WebCamTexture camText;
	bool camAvailable;
	bool isDeafault = true;

    private void Start()
    {
		picture.mainTexture = defaultTexture;

		WebCamDevice[] devices = WebCamTexture.devices;

		if(devices.Length == 0)
        {
			camAvailable = false;
			return;
		}

		for(int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
				camText = new WebCamTexture();
            }
        }

		if (camText == null)
			return;

		camText.Play();
		camAvailable = true;
		camText.wrapMode = TextureWrapMode.Repeat;
	}

    private void Update()
    {
		if (!camAvailable)
			return;

		int orient = -camText.videoRotationAngle;
		if(!isDeafault)
			picturePlane.localEulerAngles = new Vector3(0f, 0f, orient);
	}

	/*
    public void SetImage()
    {
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			if (path != null)
			{
				Texture texture = NativeGallery.LoadImageAtPath(path);

				text = texture;
			}
		});
	}
	*/

	public void TakeImage()
    {
		Texture2D snap = new Texture2D(camText.width, camText.height);
		snap.SetPixels(camText.GetPixels());
		snap.Apply();
		
		isDeafault = false;
		picture.mainTexture = snap;
		picturePlane.transform.localScale = imageScale;
	}
}
