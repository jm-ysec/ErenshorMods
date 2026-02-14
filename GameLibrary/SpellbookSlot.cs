// SpellbookSlot
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellbookSlot : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image SlotGraphic;

	public Image SlotBG;

	public Spell AssignedSpell;

	public TextMeshProUGUI Desc;

	public TextMeshProUGUI Name;

	public SpellBook SB;

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
					foreach (SpellbookSlot slot in SB.Slots)
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
					hotkey.AssignSpellFromBook(AssignedSpell);
				}
				dragMe = false;
				AssignSpell(null);
				base.gameObject.SetActive(value: false);
			}
		}
		if (dragMe)
		{
			base.transform.position = Input.mousePosition;
		}
	}

	public void AssignSpell(Spell spell)
	{
		if (spell != null)
		{
			SlotGraphic.enabled = true;
			AssignedSpell = spell;
			SlotGraphic.sprite = spell.SpellIcon;
		}
	}

	public void ClearSpell()
	{
		AssignedSpell = null;
		SlotGraphic.sprite = null;
		SlotGraphic.enabled = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && !HKOnly)
		{
			mouseDown = 0f;
			LClick = true;
			foreach (SpellbookSlot slot in SB.Slots)
			{
				slot.ToggleSelect(_selected: false);
			}
			ToggleSelect(_selected: true);
		}
		else if (eventData.button == PointerEventData.InputButton.Left && HKOnly)
		{
			if (hotkey != null)
			{
				hotkey.AssignSpellFromBook(AssignedSpell);
			}
			dragMe = false;
			AssignSpell(null);
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
		SB.SpellHKSlot.SetActive(value: true);
		SB.SpellHKSlot.GetComponent<SpellbookSlot>().AssignSpell(AssignedSpell);
		SB.SpellHKSlot.GetComponent<SpellbookSlot>().dragMe = true;
		base.transform.parent.SetAsLastSibling();
	}

	public void ToggleSelect(bool _selected)
	{
		if (HKOnly)
		{
			return;
		}
		if (!_selected)
		{
			SlotBG.color = Color.black;
		}
		else if (AssignedSpell != null)
		{
			string text = "";
			text = ((AssignedSpell.SpellDurationInTicks <= 0) ? (text + "<color=yellow>Instant Effect</color>\n") : ((AssignedSpell.TargetDamage <= 0) ? (text + "<color=yellow>Effect Duration: " + AssignedSpell.SpellDurationInTicks * 3 + " sec</color>\n") : (text + "<color=yellow>Damage over time: " + AssignedSpell.SpellDurationInTicks * 3 + " sec</color>\n")));
			text = text + "Spell Type: " + AssignedSpell.Type.ToString() + "\n";
			text = text + "\nMana Cost: " + AssignedSpell.ManaCost + "\n";
			if (AssignedSpell.TargetDamage > 0)
			{
				text = text + "Damage: " + AssignedSpell.TargetDamage;
				text = ((AssignedSpell.SpellDurationInTicks <= 0) ? (text + "\n") : (text + " / tick\n"));
			}
			text = text + "Cast Time: " + AssignedSpell.SpellChargeTime / 60f + " sec\n";
			text = text + "Cooldown: " + AssignedSpell.Cooldown + " sec\n";
			if (AssignedSpell.TargetDamage > 0 || AssignedSpell.Type == Spell.SpellType.StatusEffect || AssignedSpell.TauntSpell)
			{
				text = text + "Resist Type: " + GetColoredDamageType(AssignedSpell.MyDamageType) + "\n";
			}
			text += "\n";
			if (AssignedSpell.Lifetap)
			{
				text += "<color=#8080FF>Lifetap</color>\n";
			}
			if (AssignedSpell.GroupEffect)
			{
				text += "<color=#8080FF>Group Effect</color>\n";
			}
			if (AssignedSpell.StunTarget)
			{
				text += "<color=#8080FF>Stuns Target</color>\n";
			}
			if (AssignedSpell.CharmTarget)
			{
				text += "<color=#8080FF>Charms Target</color>\n";
			}
			if (AssignedSpell.RootTarget)
			{
				text += "<color=#8080FF>Roots Target</color>\n";
			}
			if (AssignedSpell.TauntSpell)
			{
				text = text + "<color=#8080FF>Taunt:</color> " + AssignedSpell.Aggro + " aggro\n";
			}
			if (AssignedSpell.StatusEffectToApply != null)
			{
				text += "<color=#8080FF>Apply Effects on Target</color>\n";
				text = text + "  -" + AssignedSpell.StatusEffectToApply.SpellName;
			}
			text += "\n";
			if (AssignedSpell.HP != 0)
			{
				text = text + "Hitpoints " + FormatMod(AssignedSpell.HP) + "\n";
			}
			if (AssignedSpell.AC != 0)
			{
				text = text + "Armor Class " + FormatMod(AssignedSpell.AC) + "\n";
			}
			if (AssignedSpell.Mana != 0)
			{
				text = text + "Mana " + FormatMod(AssignedSpell.Mana) + "\n";
			}
			if (AssignedSpell.Str != 0)
			{
				text = text + "Strength " + FormatMod(AssignedSpell.Str) + "\n";
			}
			if (AssignedSpell.Dex != 0)
			{
				text = text + "Dexterity " + FormatMod(AssignedSpell.Dex) + "\n";
			}
			if (AssignedSpell.End != 0)
			{
				text = text + "Endurance " + FormatMod(AssignedSpell.End) + "\n";
			}
			if (AssignedSpell.Agi != 0)
			{
				text = text + "Agility " + FormatMod(AssignedSpell.Agi) + "\n";
			}
			if (AssignedSpell.Wis != 0)
			{
				text = text + "Wisdom " + FormatMod(AssignedSpell.Wis) + "\n";
			}
			if (AssignedSpell.Int != 0)
			{
				text = text + "Intelligence " + FormatMod(AssignedSpell.Int) + "\n";
			}
			if (AssignedSpell.Cha != 0)
			{
				text = text + "Charisma " + FormatMod(AssignedSpell.Cha) + "\n";
			}
			if (AssignedSpell.MR != 0)
			{
				text = text + "Magic Resist " + FormatMod(AssignedSpell.MR) + "\n";
			}
			if (AssignedSpell.ER != 0)
			{
				text = text + "Elemental Resist " + FormatMod(AssignedSpell.ER) + "\n";
			}
			if (AssignedSpell.PR != 0)
			{
				text = text + "Poison Resist " + FormatMod(AssignedSpell.PR) + "\n";
			}
			if (AssignedSpell.VR != 0)
			{
				text = text + "Void Resist " + FormatMod(AssignedSpell.VR) + "\n";
			}
			if (AssignedSpell.MovementSpeed != 0f)
			{
				text = text + "Movement Speed " + FormatMod(AssignedSpell.MovementSpeed) + "\n";
			}
			if (AssignedSpell.DamageShield != 0)
			{
				text = text + "Damage Shield " + FormatMod(AssignedSpell.DamageShield) + "\n";
			}
			if (AssignedSpell.Haste != 0f)
			{
				text = text + "Haste " + FormatMod(AssignedSpell.Haste) + "\n";
			}
			if (AssignedSpell.percentLifesteal != 0f)
			{
				text = text + "Lifesteal " + FormatMod(AssignedSpell.percentLifesteal) + "%\n";
			}
			if (AssignedSpell.AtkRollModifier != 0)
			{
				text = text + "Attack Roll Modifier " + FormatMod(AssignedSpell.AtkRollModifier) + "\n";
			}
			if (!string.IsNullOrEmpty(AssignedSpell.SpecialDescriptor))
			{
				text = text + "\n\n" + AssignedSpell.SpecialDescriptor;
			}
			SlotBG.color = Color.yellow;
			Name.text = AssignedSpell.SpellName;
			Desc.text = text;
			SB.CurrentSelection = this;
		}
		else
		{
			SB.Hotkey.gameObject.SetActive(value: false);
			Name.text = "No Spell Selected.";
			Desc.text = "";
			SB.CurrentSelection = null;
		}
	}

	private string GetColoredDamageType(GameData.DamageType type)
	{
		return type switch
		{
			GameData.DamageType.Physical => "<color=#FFFFFF>Physical</color>", 
			GameData.DamageType.Magic => "<color=#8080FF>Magic</color>", 
			GameData.DamageType.Elemental => "<color=#FFA500>Elemental</color>", 
			GameData.DamageType.Poison => "<color=#50C878>Poison</color>", 
			GameData.DamageType.Void => "<color=#B030B0>Void</color>", 
			_ => type.ToString(), 
		};
	}

	private string FormatMod(int value)
	{
		if (value > 0)
		{
			return "<color=#00FF00>+" + value + "</color>";
		}
		if (value < 0)
		{
			return "<color=#FF0000>" + value + "</color>";
		}
		return "0";
	}

	private string FormatMod(float value)
	{
		if (value > 0f)
		{
			return "<color=#00FF00>+" + value.ToString("0.#") + "</color>";
		}
		if (value < 0f)
		{
			return "<color=#FF0000>" + value.ToString("0.#") + "</color>";
		}
		return "0";
	}

	public void ClearDesc()
	{
		Name.text = "No Spell Selected.";
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
