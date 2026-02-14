// StatusEffectIcon
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image Icon;

	public string Name;

	public GameObject MyDesc;

	public TextMeshProUGUI MyName;

	private bool mouseOver;

	public int Duration;

	public int SlotIndex;

	public Stats ReadStats;

	private void Start()
	{
		MyName = GetComponentInChildren<TextMeshProUGUI>();
		MyDesc.SetActive(value: false);
		Icon.enabled = false;
	}

	private void Update()
	{
		if (mouseOver)
		{
			if (Icon.enabled)
			{
				MyDesc.SetActive(value: true);
				int num = Mathf.RoundToInt(ReadStats.StatusEffects[SlotIndex].Duration * 3f);
				int num2 = num / 60;
				int num3 = num % 60;
				MyName.text = Name + " : " + num2 + "m:" + num3 + "s";
			}
		}
		else
		{
			if (MyDesc.activeSelf)
			{
				MyDesc.SetActive(value: false);
			}
			if (MyName.text != "")
			{
				MyName.text = "";
			}
		}
	}

	public void ResetSlot()
	{
		Name = "";
		Icon.sprite = null;
		Icon.enabled = false;
		MyDesc.SetActive(value: false);
		Duration = 0;
	}

	public void SetSlot()
	{
		if (ReadStats.StatusEffects[SlotIndex].Effect != null)
		{
			Name = ReadStats.StatusEffects[SlotIndex].Effect.SpellName;
			Icon.enabled = true;
			Icon.sprite = ReadStats.StatusEffects[SlotIndex].Effect.SpellIcon;
			Duration = ReadStats.StatusEffects[SlotIndex].Effect.SpellDurationInTicks * 3;
		}
		else
		{
			ResetSlot();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseOver = false;
	}
}
