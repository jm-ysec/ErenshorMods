// Minimap
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
	public Camera MapCam;

	public RectTransform MapPar;

	private Vector2 MapHome = new Vector2(700f, 433f);

	private Vector2 MapBig = new Vector2(450f, 125f);

	private bool bigMap;

	private float CamSize = 100f;

	private float MapSize = 250f;

	public RectTransform Mask;

	public RectTransform CircleTrim;

	public Transform Buttons;

	private bool DrawReal;

	public bool LockNorth;

	public DragUI UIDrag;

	private void Start()
	{
		if (!GameData.UseMap)
		{
			MapPar.gameObject.SetActive(value: false);
			MapCam.gameObject.SetActive(value: false);
		}
	}

	public void ZoomOut()
	{
		if (bigMap)
		{
			if (MapCam.orthographicSize < 2250f)
			{
				MapCam.orthographicSize += 10f;
			}
			MapSize = MapCam.orthographicSize;
		}
		else
		{
			if (MapCam.orthographicSize < 250f)
			{
				MapCam.orthographicSize += 10f;
			}
			CamSize = MapCam.orthographicSize;
		}
	}

	public void ZoomIn()
	{
		if (bigMap)
		{
			if (MapCam.orthographicSize > 150f)
			{
				MapCam.orthographicSize -= 10f;
			}
			MapSize = MapCam.orthographicSize;
		}
		else
		{
			if (MapCam.orthographicSize > 60f)
			{
				MapCam.orthographicSize -= 10f;
			}
			CamSize = MapCam.orthographicSize;
		}
	}

	public void ToggleRealTimeMap()
	{
		int num = 1;
		if (!DrawReal)
		{
			MapCam.cullingMask |= num;
			DrawReal = true;
		}
		else
		{
			MapCam.cullingMask &= ~num;
			DrawReal = false;
		}
	}

	private void LateUpdate()
	{
		if (LockNorth && GameData.UseMap)
		{
			if (!(GameData.ZoneAnnounce == null))
			{
				base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, GameData.CurrentZoneAnnounce.transform.eulerAngles.y - 180f, base.transform.eulerAngles.z);
			}
		}
		else if (GameData.PlayerControl != null && !LockNorth && GameData.UseMap && base.transform.eulerAngles != GameData.PlayerControl.transform.eulerAngles)
		{
			base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, GameData.PlayerControl.transform.eulerAngles.y, base.transform.eulerAngles.z);
		}
	}

	public void ToggleNorthLocl()
	{
		LockNorth = !LockNorth;
	}

	private void Update()
	{
		if (GameData.UseMap)
		{
			if (!MapPar.gameObject.activeSelf)
			{
				MapPar.gameObject.SetActive(value: true);
				MapCam.gameObject.SetActive(value: true);
			}
		}
		else if (MapPar.gameObject.activeSelf)
		{
			MapPar.gameObject.SetActive(value: false);
			MapCam.gameObject.SetActive(value: false);
		}
		if (Input.GetKeyDown(InputManager.Map) && !GameData.PlayerTyping && SceneManager.GetActiveScene().name != "LoadScene")
		{
			if (!bigMap)
			{
				MapCam.orthographicSize = MapSize;
				bigMap = true;
				MapPar.localPosition = MapBig;
				Mask.sizeDelta = new Vector2(750f, 750f);
				CircleTrim.sizeDelta = new Vector2(772f, 772f);
				Buttons.transform.localPosition = new Vector2(Buttons.transform.localPosition.x, -360f);
				UIDrag.GetComponent<Image>().enabled = false;
			}
			else
			{
				MapCam.orthographicSize = CamSize;
				bigMap = false;
				MapPar.localPosition = UIDrag.PrefPos;
				Mask.sizeDelta = new Vector2(250f, 250f);
				CircleTrim.sizeDelta = new Vector2(257f, 256f);
				Buttons.transform.localPosition = new Vector2(Buttons.transform.localPosition.x, -100f);
				UIDrag.GetComponent<Image>().enabled = true;
			}
		}
	}
}
