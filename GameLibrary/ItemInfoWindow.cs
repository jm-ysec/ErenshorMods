// ItemInfoWindow
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoWindow : MonoBehaviour
{
	public TextMeshProUGUI ItemName;

	public TextMeshProUGUI Str;

	public TextMeshProUGUI End;

	public TextMeshProUGUI Dex;

	public TextMeshProUGUI Agi;

	public TextMeshProUGUI Int;

	public TextMeshProUGUI Wis;

	public TextMeshProUGUI Cha;

	public TextMeshProUGUI MR;

	public TextMeshProUGUI PR;

	public TextMeshProUGUI ER;

	public TextMeshProUGUI VR;

	public TextMeshProUGUI HP;

	public TextMeshProUGUI Mana;

	public TextMeshProUGUI AC;

	public TextMeshProUGUI Lore;

	public TextMeshProUGUI DMGtxt;

	public TextMeshProUGUI DMGNum;

	public TextMeshProUGUI DelTXT;

	public TextMeshProUGUI DelNum;

	public TextMeshProUGUI Slot;

	public TextMeshProUGUI Usable;

	public TextMeshProUGUI Res;

	public TextMeshProUGUI RangeNum;

	public TextMeshProUGUI RangeText;

	public GameObject ParentWindow;

	public Image ItemIcon;

	public GameObject StatTextParent;

	public GameObject OtherTextParent;

	public GameObject ItemEffect;

	public TextMeshProUGUI ClickSpell;

	public TextMeshProUGUI ClickDesc;

	public GameObject ReqLvl;

	public TextMeshProUGUI itemPrice;

	[SerializeField]
	private RectTransform RootCanvasRect;

	public Color Normal;

	public Color Blessed;

	public Color Legendary;

	public Color NormalText;

	public Color BlessedText;

	public Color GodlyText;

	public Image Banner;

	public GameObject SpellDetailsWindow;

	public Image SpellDetailsImage;

	public TextMeshProUGUI SpellDetailsName;

	public TextMeshProUGUI SpellDetailsDesc;

	private Spell ItemSpellForDisplay;

	private bool EffectWorn;

	private void Start()
	{
		GameData.ItemInfoWindow = this;
	}

	private void GetItemSpellIfAny(Item _item)
	{
		Spell itemSpellForDisplay = null;
		bool effectWorn = false;
		if (_item.WeaponProcOnHit != null)
		{
			itemSpellForDisplay = _item.WeaponProcOnHit;
		}
		if (_item.WornEffect != null)
		{
			itemSpellForDisplay = _item.WornEffect;
			effectWorn = true;
		}
		if (_item.ItemEffectOnClick != null)
		{
			itemSpellForDisplay = _item.ItemEffectOnClick;
		}
		if (_item.WandEffect != null)
		{
			itemSpellForDisplay = _item.WandEffect;
		}
		if (_item.BowEffect != null)
		{
			itemSpellForDisplay = _item.BowEffect;
		}
		if (_item.Aura != null)
		{
			effectWorn = true;
			itemSpellForDisplay = _item.Aura;
		}
		EffectWorn = effectWorn;
		ItemSpellForDisplay = itemSpellForDisplay;
	}

	private void Update()
	{
		if (GameData.RequireRightClickInfo && ListenForInputChange.KeyboardActive)
		{
			if (SpellDetailsWindow.activeSelf && !Input.GetKey(KeyCode.LeftControl))
			{
				CloseSpellDetailsWindow();
			}
			else if (ItemSpellForDisplay != null && Input.GetKeyDown(KeyCode.LeftControl))
			{
				LoadSpellDetails(ItemSpellForDisplay, EffectWorn);
			}
		}
		else if (ListenForInputChange.KeyboardActive)
		{
			if (SpellDetailsWindow.activeSelf && !Input.GetMouseButton(1))
			{
				CloseSpellDetailsWindow();
			}
			else if (ItemSpellForDisplay != null && Input.GetMouseButton(1))
			{
				LoadSpellDetails(ItemSpellForDisplay, EffectWorn);
			}
		}
		if (!ListenForInputChange.KeyboardActive)
		{
			if (SpellDetailsWindow.activeSelf && Input.GetAxis("DPADY") > -0.4f)
			{
				CloseSpellDetailsWindow();
			}
			else if (ItemSpellForDisplay != null && Input.GetAxis("DPADY") < -0.4f)
			{
				LoadSpellDetails(ItemSpellForDisplay, EffectWorn);
			}
		}
	}

	public void DisplayItem(Item item, Vector2 slotLoc, int _quantity)
	{
		GetItemSpellIfAny(item);
		if (ParentWindow.activeSelf)
		{
			return;
		}
		if (item.TeachSpell == null && item.TeachSkill == null)
		{
			ReqLvl.SetActive(value: false);
		}
		ParentWindow.SetActive(value: true);
		Usable.text = "";
		if (item.ItemValue > 0 && !item.NoTradeNoDestroy)
		{
			itemPrice.text = item.ItemValue.ToString();
		}
		else
		{
			itemPrice.text = "Unsellable";
		}
		if (item.Classes.Count > 0)
		{
			if (item.Classes.Contains(GameData.ClassDB.Arcanist))
			{
				Usable.text += " Arcanist ";
			}
			if (item.Classes.Contains(GameData.ClassDB.Duelist))
			{
				Usable.text += " Windblade ";
			}
			if (item.Classes.Contains(GameData.ClassDB.Druid))
			{
				Usable.text += " Druid ";
			}
			if (item.Classes.Contains(GameData.ClassDB.Paladin))
			{
				Usable.text += " Paladin ";
			}
			if (item.Classes.Contains(GameData.ClassDB.Stormcaller))
			{
				Usable.text += " Stormcaller ";
			}
			if (item.Classes.Contains(GameData.ClassDB.Reaver))
			{
				Usable.text += " Reaver ";
			}
		}
		else
		{
			Usable.text = "";
		}
		_ = slotLoc.y;
		_ = 0f;
		ItemIcon.sprite = item.ItemIcon;
		ItemName.text = item.ItemName;
		if (!item.Template)
		{
			Lore.verticalAlignment = VerticalAlignmentOptions.Middle;
			Lore.text = item.Lore.ToString();
		}
		else
		{
			Lore.verticalAlignment = VerticalAlignmentOptions.Middle;
			Lore.text = "Ingredients:\n\n";
			foreach (Item templateIngredient in item.TemplateIngredients)
			{
				TextMeshProUGUI lore = Lore;
				lore.text = lore.text + templateIngredient.ItemName + "\n";
			}
			Lore.text += "\n<color=grey>Note: Ingredients MUST be exact quantities\n\nUse CTRL + CLICK to separate stacks.</color>";
		}
		PositionInspectWindow(ParentWindow.GetComponent<RectTransform>(), RootCanvasRect, Input.mousePosition);
		if (item.RequiredSlot == Item.SlotType.General)
		{
			ItemName.color = NormalText;
		}
		if ((bool)item.Aura)
		{
			ItemName.color = NormalText;
		}
		if (item.RequiredSlot == Item.SlotType.Charm)
		{
			string text = "";
			if (item.StrScaling > 0f)
			{
				text = text + "Physicality: +" + item.StrScaling + " / 40 \n";
			}
			if (item.EndScaling > 0f)
			{
				text = text + "Hardiness: +" + item.EndScaling + " / 40\n";
			}
			if (item.DexScaling > 0f)
			{
				text = text + "Finesse: +" + item.DexScaling + " / 40\n";
			}
			if (item.AgiScaling > 0f)
			{
				text = text + "Defense: +" + item.AgiScaling + " / 40\n";
			}
			if (item.IntScaling > 0f)
			{
				text = text + "Arcanism: +" + item.IntScaling + " / 40\n";
			}
			if (item.WisScaling > 0f)
			{
				text = text + "Restoration: +" + item.WisScaling + " / 40\n";
			}
			if (item.ChaScaling > 0f)
			{
				text = text + "Mind: +" + item.ChaScaling + " / 40\n";
			}
			if (item.ResistScaling > 0f)
			{
				text = text + "Resist Scaling: " + item.ResistScaling + " / 40\n";
			}
			if (item.MitigationScaling > 0f)
			{
				text = text + "Mitigation Scaling: +" + item.MitigationScaling + "%\n";
			}
			Lore.verticalAlignment = VerticalAlignmentOptions.Bottom;
			ReqLvl.SetActive(value: false);
			GameData.ItemInfoWindow.StatTextParent.SetActive(value: false);
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			Lore.verticalAlignment = VerticalAlignmentOptions.Middle;
			Lore.text = "<color=#16EC00>Charm Item\n\n<color=#16EC00><color=white>Class modifiers:\n\n" + text + "</color>\n <color=yellow>Charms do not increase stats, they modify how effectively your character utilizes stats.</color>";
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
		}
		else if (item.Aura == null && item.TeachSpell == null && item.TeachSkill == null && item.RequiredSlot != 0)
		{
			if (_quantity <= 1)
			{
				ItemName.color = NormalText;
			}
			if (_quantity == 2)
			{
				ItemName.color = BlessedText;
			}
			if (_quantity == 3)
			{
				ItemName.color = GodlyText;
			}
			Lore.verticalAlignment = VerticalAlignmentOptions.Bottom;
			ReqLvl.SetActive(value: false);
			GameData.ItemInfoWindow.StatTextParent.SetActive(value: true);
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: false);
			if (item.WeaponDmg == 0 && item.WeaponDly == 0f)
			{
				DMGtxt.gameObject.SetActive(value: false);
				DelTXT.gameObject.SetActive(value: false);
				DMGNum.gameObject.SetActive(value: false);
				DelNum.gameObject.SetActive(value: false);
				RangeNum.gameObject.SetActive(value: false);
				RangeText.gameObject.SetActive(value: false);
			}
			else
			{
				DMGtxt.gameObject.SetActive(value: true);
				DelTXT.gameObject.SetActive(value: true);
				DMGNum.gameObject.SetActive(value: true);
				DelNum.gameObject.SetActive(value: true);
				RangeNum.gameObject.SetActive(value: true);
				RangeText.gameObject.SetActive(value: true);
				if (item.IsWand)
				{
					RangeNum.text = item.WandRange.ToString();
				}
				else if (item.IsBow)
				{
					RangeNum.text = item.BowRange.ToString();
				}
				else
				{
					RangeNum.text = "1";
				}
			}
			Str.text = item.CalcStat(item.Str, _quantity).ToString();
			End.text = item.CalcStat(item.End, _quantity).ToString();
			Dex.text = item.CalcStat(item.Dex, _quantity).ToString();
			Agi.text = item.CalcStat(item.Agi, _quantity).ToString();
			Int.text = item.CalcStat(item.Int, _quantity).ToString();
			Wis.text = item.CalcStat(item.Wis, _quantity).ToString();
			Cha.text = item.CalcStat(item.Cha, _quantity).ToString();
			Res.text = item.CalcRes(item.Res, _quantity).ToString();
			MR.text = "+" + item.CalcStat(item.MR, _quantity) + "%";
			PR.text = "+" + item.CalcStat(item.PR, _quantity) + "%";
			VR.text = "+" + item.CalcStat(item.VR, _quantity) + "%";
			ER.text = "+" + item.CalcStat(item.ER, _quantity) + "%";
			AC.text = item.CalcACHPMC(item.AC, _quantity).ToString();
			HP.text = item.CalcACHPMC(item.HP, _quantity).ToString();
			Mana.text = item.CalcACHPMC(item.Mana, _quantity).ToString();
			DMGNum.text = item.CalcDmg(item.WeaponDmg, _quantity).ToString();
			DelNum.text = item.WeaponDly + " sec";
			Slot.text = "Slot: " + item.RequiredSlot;
			if (item.RequiredSlot == Item.SlotType.PrimaryOrSecondary)
			{
				Slot.text = "Primary or Secondary";
			}
			if (item.ThisWeaponType == Item.WeaponType.TwoHandMelee || item.ThisWeaponType == Item.WeaponType.TwoHandStaff)
			{
				Slot.text += " - 2-Handed";
			}
			if (item.Relic)
			{
				Slot.text += " - Relic Item";
			}
		}
		else if (item.RequiredSlot == Item.SlotType.General)
		{
			GameData.ItemInfoWindow.StatTextParent.SetActive(value: false);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
		}
		else
		{
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
		}
		if (item.ItemEffectOnClick != null)
		{
			Lore.verticalAlignment = VerticalAlignmentOptions.Bottom;
			ReqLvl.SetActive(value: false);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
			ItemEffect.SetActive(value: true);
			ClickSpell.text = "Activatable: " + item.ItemEffectOnClick.SpellName;
			if (!GameData.RequireRightClickInfo && ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold RIGHT MOUSE for details)";
			}
			else if (ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold CONTROL for details)";
			}
			else if (!ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold DPAD DOWN for details)";
			}
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			if (item.Disposable)
			{
				OtherTextParent.GetComponent<TextMeshProUGUI>().text += "Item Consumed Upon Use.\n\n";
			}
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "Right click or assign to hotkey to use.";
		}
		else if (item.WornEffect != null)
		{
			Lore.verticalAlignment = VerticalAlignmentOptions.Bottom;
			ReqLvl.SetActive(value: false);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
			ItemEffect.SetActive(value: true);
			ClickSpell.text = "Worn Effect: " + item.WornEffect.SpellName;
			if (!GameData.RequireRightClickInfo && ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold RIGHT MOUSE for details)";
			}
			else if (ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold CONTROL for details)";
			}
			else if (!ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold DPAD DOWN for details)";
			}
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "Effect applied once item is equipped.";
		}
		else if (item.WeaponProcOnHit != null)
		{
			ItemEffect.SetActive(value: true);
			if (!item.Shield && (item.RequiredSlot == Item.SlotType.Primary || item.RequiredSlot == Item.SlotType.Secondary || item.RequiredSlot == Item.SlotType.PrimaryOrSecondary))
			{
				ClickSpell.text = item.WeaponProcChance + "% chance on ATTACK: \n" + item.WeaponProcOnHit.SpellName;
			}
			if (item.Shield)
			{
				ClickSpell.text = item.WeaponProcChance + "% chance on BASH: \n" + item.WeaponProcOnHit.SpellName;
			}
			if (item.RequiredSlot == Item.SlotType.Bracer)
			{
				ClickSpell.text = item.WeaponProcChance + "% chance on CAST: \n" + item.WeaponProcOnHit.SpellName;
			}
			if (!GameData.RequireRightClickInfo && ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold RIGHT MOUSE for details)";
			}
			else if (ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold CONTROL for details)";
			}
			else if (!ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold DPAD DOWN for details)";
			}
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
		}
		else if (item.WandEffect != null)
		{
			ItemEffect.SetActive(value: true);
			if (!item.Shield && (item.RequiredSlot == Item.SlotType.Primary || item.RequiredSlot == Item.SlotType.Secondary || item.RequiredSlot == Item.SlotType.PrimaryOrSecondary))
			{
				ClickSpell.text = item.WandProcChance + "% chance on ATTACK: \n" + item.WandEffect.SpellName;
			}
			if (!GameData.RequireRightClickInfo && ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold RIGHT MOUSE for details)";
			}
			else if (ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold CONTROL for details)";
			}
			else if (!ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold DPAD DOWN for details)";
			}
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
		}
		else if (item.BowEffect != null)
		{
			ItemEffect.SetActive(value: true);
			if (!item.Shield && (item.RequiredSlot == Item.SlotType.Primary || item.RequiredSlot == Item.SlotType.Secondary || item.RequiredSlot == Item.SlotType.PrimaryOrSecondary))
			{
				ClickSpell.text = item.BowProcChance + "% chance on ATTACK: \n" + item.BowEffect.SpellName;
			}
			if (!GameData.RequireRightClickInfo && ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold RIGHT MOUSE for details)";
			}
			else if (ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold CONTROL for details)";
			}
			else if (!ListenForInputChange.KeyboardActive)
			{
				ClickDesc.text = "(Hold DPAD DOWN for details)";
			}
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "";
		}
		else
		{
			ItemEffect.SetActive(value: false);
			ClickSpell.text = "";
			ClickDesc.text = "";
		}
		if (item.WeaponDmg != 0 && (item.RequiredSlot == Item.SlotType.Primary || item.RequiredSlot == Item.SlotType.Secondary || item.RequiredSlot == Item.SlotType.PrimaryOrSecondary))
		{
			float num = 0f;
			if (item.ThisWeaponType != Item.WeaponType.TwoHandMelee)
			{
				_ = item.ThisWeaponType;
				_ = 4;
			}
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			OtherTextParent.GetComponent<TextMeshProUGUI>().faceColor = Color.green;
			num = ((item.ThisWeaponType != Item.WeaponType.TwoHandBow) ? CalcDPSMelee(item) : CalcDPSBow(item));
			OtherTextParent.GetComponent<TextMeshProUGUI>().text = "Base DPS: " + Mathf.RoundToInt(num);
		}
		else
		{
			OtherTextParent.GetComponent<TextMeshProUGUI>().faceColor = Color.white;
		}
		if (item.TeachSpell != null)
		{
			Lore.verticalAlignment = VerticalAlignmentOptions.Bottom;
			GameData.ItemInfoWindow.StatTextParent.SetActive(value: false);
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			Spell teachSpell = item.TeachSpell;
			GameData.ItemInfoWindow.ReqLvl.SetActive(value: true);
			if (GameData.PlayerControl.Myself.MySpells.KnownSpells.Contains(teachSpell))
			{
				ReqLvl.GetComponent<TextMeshProUGUI>().text = "<color=red>YOU ALREADY KNOW THIS SPELL</color>\n\nMana Cost: " + teachSpell.ManaCost + "\nSpell Type: " + teachSpell.Type.ToString() + "\n\n" + teachSpell.SpellDesc;
			}
			else
			{
				ReqLvl.GetComponent<TextMeshProUGUI>().text = "Required Level: " + teachSpell.RequiredLevel + "\n\nMana Cost: " + teachSpell.ManaCost + "\nSpell Type: " + teachSpell.Type.ToString() + "\n\n" + teachSpell.SpellDesc;
			}
		}
		else if (item.Aura != null)
		{
			GameData.ItemInfoWindow.StatTextParent.SetActive(value: false);
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			Lore.verticalAlignment = VerticalAlignmentOptions.Middle;
			Lore.text = "<color=#16EC00>Aura Item</color>\nAuras effect entire party\nAuras of same type do not stack\n\n<color=#16EC00>" + item.Aura.SpellName + "</color>\n" + item.Aura.SpellDesc;
		}
		if (item.TeachSkill != null)
		{
			Lore.verticalAlignment = VerticalAlignmentOptions.Bottom;
			GameData.ItemInfoWindow.StatTextParent.SetActive(value: false);
			GameData.ItemInfoWindow.OtherTextParent.SetActive(value: true);
			GameData.ItemInfoWindow.ReqLvl.SetActive(value: true);
			Skill teachSkill = item.TeachSkill;
			string text2 = "Required Level: \n";
			if (teachSkill.DuelistRequiredLevel != 0)
			{
				text2 = text2 + "Windblade: " + teachSkill.DuelistRequiredLevel + "\n";
			}
			if (teachSkill.DruidRequiredLevel != 0)
			{
				text2 = text2 + "Druid: " + teachSkill.DruidRequiredLevel + "\n";
			}
			if (teachSkill.ArcanistRequiredLevel != 0)
			{
				text2 = text2 + "Arcanist: " + teachSkill.ArcanistRequiredLevel + "\n";
			}
			if (teachSkill.PaladinRequiredLevel != 0)
			{
				text2 = text2 + "Paladin: " + teachSkill.PaladinRequiredLevel + "\n";
			}
			if (teachSkill.StormcallerRequiredLevel != 0)
			{
				text2 = text2 + "Stormcaller: " + teachSkill.StormcallerRequiredLevel + "\n";
			}
			if (teachSkill.ReaverRequiredLevel != 0)
			{
				text2 = text2 + "Reaver: " + teachSkill.ReaverRequiredLevel + "\n";
			}
			string text3 = "";
			text3 = ((!(teachSkill.StanceToUse == null)) ? ("CHANGE STANCE\n" + teachSkill.StanceToUse.DisplayName + "\n" + teachSkill.StanceToUse.StanceDesc) : teachSkill.SkillDesc);
			if (GameData.PlayerControl.Myself.MySkills.KnownSkills.Contains(teachSkill))
			{
				ReqLvl.GetComponent<TextMeshProUGUI>().text = "<color=red>YOU ALREADY KNOW THIS SKILL</color>\n\nSkill Type: " + teachSkill.TypeOfSkill.ToString() + "\n\n" + text3;
			}
			else
			{
				ReqLvl.GetComponent<TextMeshProUGUI>().text = text2 + "\n\nSkill Type: " + teachSkill.TypeOfSkill.ToString() + "\n\n" + text3;
			}
			if (!teachSkill.SimPlayersAutolearn)
			{
				Lore.text = "<color=yellow>SimPlayers DO NOT automatically learn this skill!\nHand this book to them to allow them to use it.</color>";
			}
		}
		if (item.RequiredSlot == Item.SlotType.General && item.ItemEffectOnClick == null)
		{
			StatTextParent.SetActive(value: false);
		}
		_ = item.Template;
	}

	private float CalcDPSMelee(Item item)
	{
		float num = 0.8f;
		float num2 = (float)GameData.PlayerStats.StrScaleMod / 100f;
		float num3 = (float)GameData.PlayerStats.DexScaleMod / 100f;
		float num4 = (float)GameData.PlayerStats.GetCurrentStr() * num2 * 2f + (float)GameData.PlayerStats.GetCurrentDex() * num3 / 2f;
		float num5 = 1f + (float)(item.WeaponDmg - 1) * 0.4f;
		float num6 = 0.25f * (float)GameData.PlayerStats.Level;
		float num7 = Mathf.RoundToInt((num4 + num6) * num5 * num / item.WeaponDly);
		if (item.ThisWeaponType == Item.WeaponType.TwoHandMelee || item.ThisWeaponType == Item.WeaponType.TwoHandStaff)
		{
			num7 *= 2f;
		}
		return (float)Math.Round(num7, 2);
	}

	private float CalcDPSBow(Item item)
	{
		float num = 0.8f;
		float num2 = (float)GameData.PlayerStats.StrScaleMod / 100f;
		_ = (float)GameData.PlayerStats.DexScaleMod / 100f;
		float num3 = (float)GameData.PlayerStats.GetCurrentStr() * num2 * 0.8f + (float)GameData.PlayerStats.GetCurrentDex() * 1.1f / 2f;
		float num4 = 1f + (float)(item.WeaponDmg - 1) * 0.4f;
		float num5 = 0.25f * (float)GameData.PlayerStats.Level;
		return (float)Math.Round((float)Mathf.RoundToInt((num3 + num5) * num4 * num / item.WeaponDly), 2);
	}

	public void CloseItemWindow()
	{
		SpellDetailsWindow.SetActive(value: false);
		if (ParentWindow.activeSelf)
		{
			ParentWindow.SetActive(value: false);
		}
	}

	public bool isWindowActive()
	{
		return ParentWindow.activeSelf;
	}

	public void LoadSpellDetails(Spell AssignedSpell, bool _worn)
	{
		if (!GameData.ItemInfoWindow.SpellDetailsWindow.activeSelf)
		{
			GameData.ItemInfoWindow.SpellDetailsWindow.SetActive(value: true);
			GameData.ItemInfoWindow.SpellDetailsName.text = AssignedSpell.SpellName;
			GameData.ItemInfoWindow.SpellDetailsImage.sprite = AssignedSpell.SpellIcon;
			string text = "";
			text = text + "<color=green>Spell Level: " + AssignedSpell.RequiredLevel + "</color>\n";
			text = ((AssignedSpell.SpellDurationInTicks <= 0) ? (text + "<color=yellow>Instant Effect</color>\n") : ((AssignedSpell.TargetDamage <= 0) ? (text + "<color=yellow>Effect Duration: " + AssignedSpell.SpellDurationInTicks * 3 + " sec</color>\n") : (text + "<color=yellow>Damage over time: " + AssignedSpell.SpellDurationInTicks * 3 + " sec</color>\n")));
			text = text + "Spell Type: " + AssignedSpell.Type.ToString() + "\n";
			text = text + "Spell Line: " + AssignedSpell.Line.ToString() + "\n\n";
			if (AssignedSpell.TargetDamage > 0)
			{
				text = text + "<color=red>Damage: " + AssignedSpell.TargetDamage + "</color>";
				text = ((AssignedSpell.SpellDurationInTicks <= 0) ? (text + "\n") : (text + " / tick\n"));
			}
			if (!_worn)
			{
				text = text + "Cast Time: " + (AssignedSpell.SpellChargeTime / 60f).ToString("F1") + " sec\n";
			}
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
				text = text + "Haste " + FormatMod(AssignedSpell.Haste) + "%\n";
			}
			if (AssignedSpell.percentLifesteal != 0f)
			{
				text = text + "Lifesteal " + FormatMod(AssignedSpell.percentLifesteal) + "%\n";
			}
			if (AssignedSpell.AtkRollModifier != 0)
			{
				text = text + "Attack Roll Modifier " + FormatMod(AssignedSpell.AtkRollModifier) + "\n";
			}
			if (AssignedSpell.ResonateChance != 0)
			{
				text = text + "Resonance " + FormatMod(AssignedSpell.ResonateChance) + "\n";
			}
			if (AssignedSpell.AddProc != null)
			{
				text = text + AssignedSpell.AddProcChance + "% chance to proc " + AssignedSpell.AddProc.SpellName + "\n";
			}
			if (!string.IsNullOrEmpty(AssignedSpell.SpecialDescriptor))
			{
				text += AssignedSpell.SpecialDescriptor;
			}
			GameData.ItemInfoWindow.SpellDetailsDesc.text = text;
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

	public void CloseSpellDetailsWindow()
	{
		SpellDetailsWindow.SetActive(value: false);
	}

	private void PositionInspectWindow(RectTransform window, RectTransform canvasRect, Vector2 mouseScreen)
	{
		if (!(window == null) && !(canvasRect == null))
		{
			Canvas componentInParent = canvasRect.GetComponentInParent<Canvas>();
			Camera cam = ((componentInParent != null && componentInParent.renderMode != 0) ? componentInParent.worldCamera : null);
			float num = ((mouseScreen.x < (float)Screen.width * 0.5f) ? 1f : (-1f));
			float num2 = ((mouseScreen.y < (float)Screen.height * 0.5f) ? 1f : (-1f));
			float num3 = 100f;
			float num4 = 16f;
			Vector2 vector = ScreenToCanvasLocal(canvasRect, mouseScreen, cam);
			float num5 = ((componentInParent != null && componentInParent.scaleFactor > 0f) ? componentInParent.scaleFactor : 1f);
			float num6 = Mathf.Max(1f, num5);
			Vector2 size = window.rect.size;
			Vector3 localScale = window.localScale;
			size = new Vector2(size.x * Mathf.Abs(localScale.x), size.y * Mathf.Abs(localScale.y));
			Vector2 vector2 = size * 0.5f;
			Rect rect = canvasRect.rect;
			float num7 = ((num > 0f) ? (rect.xMax - (vector.x + vector2.x)) : (vector.x - vector2.x - rect.xMin));
			float num8 = 6f;
			float value = (235f + num4) * (num6 / num5);
			float num9 = num3 * (num6 / num5);
			float num10 = Mathf.Clamp(value, 0f, Mathf.Max(0f, num7 - num8));
			Vector2 vector3 = new Vector2(num10 * num, num9 * num2);
			Vector2 anchoredPos = vector + vector3;
			anchoredPos = (window.anchoredPosition = ClampWindowInsideCanvas(window, canvasRect, anchoredPos, num8));
		}
	}

	private Vector2 ScreenToCanvasLocal(RectTransform canvasRect, Vector2 screenPos, Camera cam)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out var localPoint);
		return localPoint;
	}

	private Vector2 ClampWindowInsideCanvas(RectTransform window, RectTransform canvasRect, Vector2 anchoredPos, float padding)
	{
		Vector2 size = window.rect.size;
		Vector3 localScale = window.localScale;
		size = new Vector2(size.x * Mathf.Abs(localScale.x), size.y * Mathf.Abs(localScale.y));
		Vector2 vector = size * 0.5f;
		Rect rect = canvasRect.rect;
		float min = rect.xMin + vector.x + padding;
		float max = rect.xMax - vector.x - padding;
		float min2 = rect.yMin + vector.y + padding;
		float max2 = rect.yMax - vector.y - padding;
		anchoredPos.x = Mathf.Clamp(anchoredPos.x, min, max);
		anchoredPos.y = Mathf.Clamp(anchoredPos.y, min2, max2);
		return anchoredPos;
	}
}
