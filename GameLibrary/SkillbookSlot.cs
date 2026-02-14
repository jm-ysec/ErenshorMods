// SkillbookSlot
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillbookSlot : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image SlotGraphic;

	public Image SlotBG;

	public Skill AssignedSkill;

	public TextMeshProUGUI Desc;

	public TextMeshProUGUI Name;

	public SkillBook SB;

	private float mouseDown;

	private bool LClick;

	private bool doHK;

	public bool HKOnly;

	public bool dragMe;

	private Hotkeys hotkey;

	private bool active;

	private void Start()
	{
	}

	private void Update()
	{
		if (LClick)
		{
			mouseDown += 60f * Time.deltaTime;
			if (mouseDown >= 30f)
			{
				LClick = false;
				mouseDown = 0f;
				MakeHotkey();
			}
		}
		if (GameData.PlayerControl.usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			if (!HKOnly)
			{
				if (active)
				{
					foreach (SkillbookSlot slot in SB.Slots)
					{
						slot.ToggleSelect(_selected: false);
					}
					ToggleSelect(_selected: true);
				}
			}
			else
			{
				if (hotkey != null)
				{
					hotkey.AssignSkillFromBook(AssignedSkill);
				}
				dragMe = false;
				AssignSkill(null);
				base.gameObject.SetActive(value: false);
			}
		}
		if (dragMe)
		{
			base.transform.position = Input.mousePosition;
		}
	}

	public void AssignSkill(Skill skill)
	{
		if (skill != null)
		{
			SlotGraphic.enabled = true;
			AssignedSkill = skill;
			SlotGraphic.sprite = skill.SkillIcon;
		}
	}

	public void ClearSkill()
	{
		AssignedSkill = null;
		SlotGraphic.sprite = null;
		SlotGraphic.enabled = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && !HKOnly)
		{
			mouseDown = 0f;
			LClick = true;
			foreach (SkillbookSlot slot in SB.Slots)
			{
				slot.ToggleSelect(_selected: false);
			}
			ToggleSelect(_selected: true);
		}
		else if (eventData.button == PointerEventData.InputButton.Left && HKOnly)
		{
			if (hotkey != null)
			{
				hotkey.AssignSkillFromBook(AssignedSkill);
			}
			dragMe = false;
			AssignSkill(null);
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!HKOnly)
		{
			LClick = false;
			mouseDown = 0f;
		}
	}

	public void MakeHotkey()
	{
		SB.SkillHK.SetActive(value: true);
		SB.SkillHK.GetComponent<SkillbookSlot>().AssignSkill(AssignedSkill);
		SB.SkillHK.GetComponent<SkillbookSlot>().dragMe = true;
		base.transform.parent.SetAsLastSibling();
	}

	public void ToggleSelect(bool _selected)
	{
		if (!_selected)
		{
			SlotBG.color = Color.black;
		}
		else if (AssignedSkill != null)
		{
			string text = ((AssignedSkill.TypeOfSkill != 0) ? "Activatable" : "Passive");
			SlotBG.color = Color.yellow;
			Name.text = AssignedSkill.SkillName + " - " + text;
			if (AssignedSkill.StanceToUse == null)
			{
				Desc.text = AssignedSkill.SkillDesc;
			}
			else
			{
				Desc.text = "Change Stance\n\n" + AssignedSkill.StanceToUse.DisplayName + "\n\n" + AssignedSkill.StanceToUse.StanceDesc;
			}
			SB.CurrentSelection = this;
		}
		else
		{
			SB.Hotkey.gameObject.SetActive(value: false);
			Name.text = "No Skill Selected.";
			Desc.text = "";
			SB.CurrentSelection = null;
		}
	}

	public void ClearDesc()
	{
		Name.text = "No Skill Selected.";
		Desc.text = "";
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.transform.tag == "Hotkey")
		{
			hotkey = collision.GetComponent<Hotkeys>();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.transform.tag == "Hotkey")
		{
			hotkey = null;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		active = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		active = true;
	}
}
