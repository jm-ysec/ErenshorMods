// SkillBook
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBook : MonoBehaviour
{
	private List<Skill> DisplaySkills = new List<Skill>();

	public List<SkillbookSlot> Slots;

	public GameObject Skillbook;

	public Dropdown Hotkey;

	public SkillbookSlot CurrentSelection;

	public TextMeshProUGUI SBPage;

	private HotkeyManager HKM;

	private int curPage;

	public GameObject SkillHK;

	private void Start()
	{
		DisplaySkills = GameData.PlayerControl.GetComponent<UseSkill>().KnownSkills;
		foreach (SkillbookSlot slot in Slots)
		{
			slot.SB = this;
		}
		HKM = GetComponent<HotkeyManager>();
		GameData.PlayerSkillBook = this;
	}

	private void Update()
	{
		if (Input.GetKeyDown(InputManager.Spellbook) && !GameData.PlayerTyping && !GameData.InCharSelect && GameData.SpellOrSkill == "Skill")
		{
			OpenCloseSkillbook();
		}
	}

	public void OpenCloseSkillbook()
	{
		Skillbook.SetActive(!Skillbook.activeSelf);
		if (Skillbook.activeSelf)
		{
			UpdateSkillList(0);
		}
		else
		{
			GameData.PlayerAud.PlayOneShot(GameData.Misc.CloseWindow, GameData.UIVolume * 0.05f * GameData.MasterVol);
		}
	}

	public void UseSpellsTab()
	{
		Skillbook.SetActive(value: false);
		GameData.SpellOrSkill = "Spell";
		GameData.PlayerSpellBook.OpenCloseSpellbook();
	}

	public void StartHotkey()
	{
		if (CurrentSelection != null)
		{
			CurrentSelection.MakeHotkey();
		}
	}

	public void PageForward()
	{
		curPage++;
		UpdateSkillList(curPage);
	}

	public void PageBack()
	{
		curPage--;
		if (curPage < 0)
		{
			curPage = 0;
		}
		UpdateSkillList(curPage);
	}

	public void UpdateSkillList(int _page)
	{
		if (_page < 0)
		{
			_page = 0;
		}
		SBPage.text = "Page " + (_page + 1);
		curPage = _page;
		foreach (SkillbookSlot slot in Slots)
		{
			slot.ClearSkill();
			slot.ToggleSelect(_selected: false);
			slot.ClearDesc();
		}
		int num = 0;
		for (int i = _page * 12; i <= _page * 12 + 11; i++)
		{
			if (i < DisplaySkills.Count)
			{
				Slots[num].AssignSkill(DisplaySkills[i]);
			}
			num++;
		}
	}

	public void SetSkillToHotkey()
	{
		if (Hotkey.value != 0)
		{
			HKM.AllHotkeys[Hotkey.value - 1].AssignSkillFromBook(CurrentSelection.AssignedSkill);
			Hotkey.SetValueWithoutNotify(0);
		}
	}

	public int GetPage()
	{
		return curPage;
	}
}
