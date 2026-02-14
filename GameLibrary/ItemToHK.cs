// ItemToHK
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemToHK : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public HotkeyManager HKManager;

	private Hotkeys TarHK;

	public ItemIcon InvSlot;

	public Image myImg;

	private void Start()
	{
		GameData.ItemToHK = base.gameObject;
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		base.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.transform.tag == "Hotkey")
		{
			TarHK = collision.GetComponent<Hotkeys>();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.transform.tag == "Hotkey")
		{
			TarHK = null;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (TarHK != null)
		{
			TarHK.AssignItemFrominv(InvSlot);
			InvSlot.assignedHotkey = TarHK.GetComponent<Hotkeys>();
		}
		base.gameObject.SetActive(value: false);
	}
}
