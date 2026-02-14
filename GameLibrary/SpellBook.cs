// SpellBook
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
	private List<Spell> DisplaySpells = new List<Spell>();

	public List<SpellbookSlot> Slots;

	public GameObject Spellbook;

	public Dropdown Hotkey;

	public SpellbookSlot CurrentSelection;

	public TextMeshProUGUI SBPage;

	public GameObject SpellHKSlot;

	private HotkeyManager HKM;

	private int curPage;

	private void Start()
	{
		DisplaySpells = GameData.PlayerControl.GetComponent<CastSpell>().KnownSpells;
		foreach (SpellbookSlot slot in Slots)
		{
			slot.SB = this;
		}
		HKM = GetComponent<HotkeyManager>();
		GameData.PlayerSpellBook = this;
	}

	private void Update()
	{
		if (Input.GetKeyDown(InputManager.Spellbook) && !GameData.PlayerTyping && !GameData.InCharSelect && GameData.SpellOrSkill == "Spell")
		{
			OpenCloseSpellbook();
		}
	}

	public void OpenCloseSpellbook()
	{
		if (GameData.SpellOrSkill == "Skill")
		{
			GameData.PlayerSkillBook.OpenCloseSkillbook();
			return;
		}
		Spellbook.SetActive(!Spellbook.activeSelf);
		if (Spellbook.activeSelf)
		{
			UpdateSpellList(0);
		}
		else
		{
			GameData.PlayerAud.PlayOneShot(GameData.Misc.CloseWindow, GameData.UIVolume * GameData.MasterVol * 0.05f);
		}
	}

	public void UseSkillssTab()
	{
		Spellbook.SetActive(value: false);
		GameData.SpellOrSkill = "Skill";
		GameData.PlayerSkillBook.OpenCloseSkillbook();
	}

	public void PageForward()
	{
		curPage++;
		UpdateSpellList(curPage);
	}

	public void PageBack()
	{
		curPage--;
		if (curPage < 0)
		{
			curPage = 0;
		}
		UpdateSpellList(curPage);
	}

	public void StartHotkey()
	{
		if (CurrentSelection != null)
		{
			CurrentSelection.MakeHotkey();
		}
	}

	public void UpdateSpellList(int _page)
	{
		if (_page < 0)
		{
			_page = 0;
		}
		SBPage.text = "Page " + (_page + 1);
		curPage = _page;
		foreach (SpellbookSlot slot in Slots)
		{
			slot.ClearSpell();
			slot.ToggleSelect(_selected: false);
			slot.ClearDesc();
		}
		int num = 0;
		for (int i = _page * 12; i <= _page * 12 + 11; i++)
		{
			if (i < DisplaySpells.Count)
			{
				Slots[num].AssignSpell(DisplaySpells[i]);
			}
			num++;
		}
	}

	public void SetSpellToHotkey()
	{
		if (Hotkey.value != 0)
		{
			HKM.AllHotkeys[Hotkey.value - 1].AssignSpellFromBook(CurrentSelection.AssignedSpell);
			Hotkey.SetValueWithoutNotify(0);
		}
	}

	public int GetPage()
	{
		return curPage;
	}
}
