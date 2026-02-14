// Inventory
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public int ItemHP;

	public int ItemAC;

	public int ItemMana;

	public int ItemStr;

	public int ItemEnd;

	public int ItemDex;

	public int ItemAgi;

	public int ItemInt;

	public int ItemWis;

	public int ItemCha;

	public int ItemRes;

	public int ItemMR;

	public int ItemER;

	public int ItemPR;

	public int ItemVR;

	public float MHDelay;

	public int MHDmg;

	public int OHDmg;

	public float OHDelay;

	public int MHLevel;

	public TextMeshProUGUI GoldTXT;

	public GameObject InvWindow;

	public GameObject StatWindow;

	public Item Empty;

	public ItemIcon AuraSlot;

	public ItemIcon CharmSlot;

	public List<Item> EquippedItems = new List<Item>();

	public List<Item> StoredItems = new List<Item>();

	public List<ItemIcon> EquipmentSlots = new List<ItemIcon>();

	public List<ItemIcon> StoredSlots = new List<ItemIcon>();

	public List<ItemIcon> ALLSLOTS;

	public Item AuraItem;

	public Item CharmItem;

	public bool isPlayer;

	public GameObject CharCam;

	public ModularParts Modulars;

	public bool SecondaryWeapon;

	public bool PrimaryWeapon;

	public bool TwoHandPrimary;

	public bool SecondaryShield;

	public bool PrimaryBow;

	public CalcStats PlayerStatDisp;

	public int Gold;

	public bool isMale = true;

	public ItemIcon mouseSlot;

	public ItemIcon MH;

	public ItemIcon OH;

	public SimInvSlot SimOH;

	public SimInvSlot SimMH;

	public TextMeshProUGUI AuraTxt;

	public List<Spell> WornEffects = new List<Spell>();

	public GameObject GhostItem;

	private bool SimPlayer;

	private Character lootable;

	public int StrScaleMod;

	public int EndScaleMod;

	public int DexScaleMod;

	public int AgiScaleMod;

	public int IntScaleMod;

	public int WisScaleMod;

	public int ChaScaleMod;

	public float MitScaleMod;

	public float RstScaleMod;

	public GameObject AllocateButton;

	public GameObject BeltLamp;

	public GameObject FernallanLensObj;

	public Item BeltLampItem;

	public Item ShiveringBeltLamp;

	public Item KeepersLamp;

	public Item WatchersLens;

	public List<ItemIcon> CosmeticSlots;

	public bool HasCosmeticMask;

	private void Awake()
	{
		if (base.transform.GetComponent<PlayerControl>() != null)
		{
			GameData.MouseSlot = mouseSlot;
			GameData.PlayerInv = this;
			foreach (ItemIcon equipmentSlot in EquipmentSlots)
			{
				equipmentSlot.ForceInitInv();
				ALLSLOTS.Add(equipmentSlot);
			}
			foreach (ItemIcon storedSlot in StoredSlots)
			{
				storedSlot.ForceInitInv();
				ALLSLOTS.Add(storedSlot);
			}
			AuraItem = Empty;
			CharmItem = Empty;
			AuraSlot.ForceInitInv();
			CharmSlot.ForceInitInv();
			InvWindow.SetActive(value: false);
			StatWindow.SetActive(value: false);
			isPlayer = true;
		}
		if (base.transform.GetComponent<NPC>() != null && base.transform.GetComponent<NPC>().SimPlayer)
		{
			SimMH = new SimInvSlot(Item.SlotType.Primary);
			SimOH = new SimInvSlot(Item.SlotType.Secondary);
			if (!isMale)
			{
				Modulars = GetComponentInChildren<ModularPar>().Female;
			}
			if (isMale)
			{
				Modulars = GetComponentInChildren<ModularPar>().Male;
			}
			GetComponent<SimPlayer>().Mods = Modulars;
		}
	}

	private void Start()
	{
		if (isPlayer)
		{
			UpdatePlayerInventory();
			for (int i = 0; i < ALLSLOTS.Count; i++)
			{
				ALLSLOTS[i].ALLSLOTSINDEX = i;
			}
		}
		if (GetComponent<SimPlayer>() != null)
		{
			SimPlayer = true;
		}
	}

	private void Update()
	{
		if (isPlayer && Input.GetKeyDown(InputManager.Loot) && !GameData.LootWindow.WindowParent.activeSelf && !GameData.PlayerTyping)
		{
			lootable = GameData.PlayerControl.GetComponent<PlayerCombat>().FindNearestLootable();
			if (lootable != null && lootable.GetComponent<LootTable>() != null && !lootable.MyNPC.SimPlayer)
			{
				GameData.PlayerCombat.ForceAttackOff();
				lootable.GetComponent<LootTable>().LoadLootTable();
				GameData.PlayerControl.Myself.GetComponent<Animator>().SetTrigger("StartLoot");
			}
		}
		if (isPlayer)
		{
			bool hasCosmeticMask = false;
			foreach (ItemIcon cosmeticSlot in CosmeticSlots)
			{
				if (cosmeticSlot.MyItem.ItemLevel == -999)
				{
					hasCosmeticMask = true;
				}
			}
			HasCosmeticMask = hasCosmeticMask;
		}
		if (isPlayer && BeltLamp != null && !BeltLamp.activeSelf && (HasCosmetic(BeltLampItem) || HasCosmetic(ShiveringBeltLamp) || HasCosmetic(KeepersLamp)))
		{
			BeltLamp.SetActive(value: true);
		}
		if (isPlayer && BeltLamp != null && BeltLamp.activeSelf && !HasCosmetic(BeltLampItem) && !HasCosmetic(ShiveringBeltLamp) && !HasCosmetic(KeepersLamp))
		{
			BeltLamp.SetActive(value: false);
		}
		if (isPlayer && FernallanLensObj != null && !FernallanLensObj.activeSelf && HasCosmetic(WatchersLens))
		{
			FernallanLensObj.SetActive(value: true);
		}
		if (isPlayer && FernallanLensObj != null && FernallanLensObj.activeSelf && !HasCosmetic(WatchersLens))
		{
			FernallanLensObj.SetActive(value: false);
		}
		if ((Input.GetKeyDown(InputManager.Inventory) || (Input.GetKeyDown(KeyCode.JoystickButton3) && GameData.Gamepad && !Input.GetKey(KeyCode.JoystickButton4) && Input.GetAxis("RTrigger") < 0.1f)) && isPlayer && !GameData.PlayerTyping && !GameData.InCharSelect)
		{
			InvWindow.SetActive(!InvWindow.activeSelf);
			StatWindow.SetActive(InvWindow.activeSelf);
			UpdatePlayerInventory();
			if (InvWindow.activeSelf)
			{
				GameData.PlayerAud.PlayOneShot(GameData.Misc.CloseWindow, GameData.UIVolume * 0.05f * GameData.MasterVol);
				PlayerStatDisp.UpdateDisplayStats();
				if (!GameData.Autoattacking)
				{
					lootable = GameData.PlayerControl.GetComponent<PlayerCombat>().FindNearestLootable();
					if (lootable != null && lootable.GetComponent<LootTable>() != null && !lootable.MyNPC.SimPlayer)
					{
						lootable.GetComponent<LootTable>().LoadLootTable();
						GameData.PlayerControl.Myself.GetComponent<Animator>().SetTrigger("StartLoot");
					}
				}
			}
			else
			{
				GameData.PlayerAud.PlayOneShot(GameData.Misc.CloseWindow, GameData.UIVolume * 0.05f * GameData.MasterVol);
				GameData.Misc.IDStrip.transform.position = new Vector2(-1000f, -1000f);
				GameData.HighlightedItem = null;
				GameData.BankUI.CloseBank();
				GameData.Misc.IDStrip.transform.position = new Vector2(-1000f, -1000f);
				GameData.ItemInfoWindow.CloseItemWindow();
				GameData.HighlightedItem = null;
				if (GameData.ClassIcon != null)
				{
					GameData.ClassIcon.StatInfoWindow.SetActive(value: false);
				}
				if (!(GameData.ItemOnCursor == GameData.PlayerInv.Empty))
				{
					_ = GameData.ItemOnCursor == null;
				}
			}
		}
		if (InvWindow != null)
		{
			if (!InvWindow.activeSelf)
			{
				if ((GameData.ItemOnCursor == GameData.PlayerInv.Empty || GameData.ItemOnCursor == null) && GhostItem.activeSelf)
				{
					GhostItem.SetActive(value: false);
				}
				if (GameData.ItemOnCursor != GameData.PlayerInv.Empty && GameData.ItemOnCursor != null && !GhostItem.activeSelf)
				{
					GhostItem.SetActive(value: true);
					GhostItem.GetComponent<Image>().sprite = GameData.PlayerInv.mouseSlot.MyItem.ItemIcon;
				}
			}
			else if (GhostItem.activeSelf)
			{
				GhostItem.SetActive(value: false);
			}
			if (GhostItem.activeSelf)
			{
				GhostItem.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f);
			}
		}
		if (InvWindow != null && InvWindow.activeSelf)
		{
			if (AllocateButton.activeSelf && GameData.PlayerStats.TotalAvailableProficiencies <= 0)
			{
				AllocateButton.SetActive(value: false);
			}
			else if (!AllocateButton.activeSelf && GameData.PlayerStats.TotalAvailableProficiencies > 0)
			{
				AllocateButton.SetActive(value: true);
			}
		}
	}

	public void ForceOpenInv()
	{
		InvWindow.SetActive(value: true);
		StatWindow.SetActive(InvWindow.activeSelf);
		UpdatePlayerInventory();
	}

	public void ToggleInv()
	{
		if (!InvWindow.activeSelf)
		{
			InvWindow.SetActive(value: true);
			GameData.GM.CloseAscensionWindow();
			StatWindow.SetActive(InvWindow.activeSelf);
			UpdatePlayerInventory();
		}
		else
		{
			GameData.ItemInfoWindow.CloseItemWindow();
			InvWindow.SetActive(value: false);
			StatWindow.SetActive(InvWindow.activeSelf);
			GameData.Misc.IDStrip.transform.position = new Vector2(-1000f, -1000f);
		}
	}

	public bool HasItem(Item _item, bool _remove)
	{
		bool result = false;
		foreach (ItemIcon storedSlot in StoredSlots)
		{
			if (storedSlot.MyItem == _item)
			{
				result = true;
				if (_remove)
				{
					RemoveItemFromInv(storedSlot);
					break;
				}
			}
		}
		foreach (ItemIcon equipmentSlot in EquipmentSlots)
		{
			if (equipmentSlot.MyItem == _item)
			{
				result = true;
			}
		}
		return result;
	}

	public bool HasCosmetic(Item _item)
	{
		bool result = false;
		if (CosmeticSlots == null || CosmeticSlots.Count <= 0)
		{
			return false;
		}
		foreach (ItemIcon cosmeticSlot in CosmeticSlots)
		{
			if (cosmeticSlot.MyItem == _item)
			{
				result = true;
			}
		}
		return result;
	}

	public void ForceCloseInv()
	{
		GameData.ItemInfoWindow.CloseItemWindow();
		InvWindow.SetActive(value: false);
		StatWindow.SetActive(InvWindow.activeSelf);
		GameData.ItemInfoWindow.CloseItemWindow();
		CharCam.SetActive(value: false);
		GameData.Misc.IDStrip.transform.position = new Vector2(-1000f, -1000f);
		if (GameData.ClassIcon != null)
		{
			GameData.ClassIcon.StatInfoWindow.SetActive(value: false);
		}
		GameData.HighlightedItem = null;
	}

	public void UpdatePlayerInventory()
	{
		EquippedItems.Clear();
		AuraSlot.UpdateSlotImage();
		CharmSlot.UpdateSlotImage();
		WornEffects.Clear();
		foreach (ItemIcon equipmentSlot in EquipmentSlots)
		{
			if (equipmentSlot.MyItem.WornEffect != null)
			{
				WornEffects.Add(equipmentSlot.MyItem.WornEffect);
			}
			EquippedItems.Add(equipmentSlot.MyItem);
			equipmentSlot.UpdateSlotImage();
			if (equipmentSlot.ThisSlotType == Item.SlotType.Primary)
			{
				if (equipmentSlot.MyItem.WeaponDmg != 0)
				{
					PrimaryWeapon = true;
					if (equipmentSlot.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || equipmentSlot.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff)
					{
						TwoHandPrimary = true;
					}
					else
					{
						TwoHandPrimary = false;
					}
					if (equipmentSlot.MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow)
					{
						PrimaryBow = true;
					}
					else
					{
						PrimaryBow = false;
					}
				}
				else
				{
					PrimaryWeapon = false;
				}
			}
			if (equipmentSlot.ThisSlotType != Item.SlotType.Secondary)
			{
				continue;
			}
			if (equipmentSlot.MyItem.WeaponDmg != 0)
			{
				SecondaryWeapon = true;
				SecondaryShield = false;
				continue;
			}
			SecondaryWeapon = false;
			if (equipmentSlot.MyItem.Shield)
			{
				SecondaryShield = true;
			}
			else
			{
				SecondaryShield = false;
			}
		}
		foreach (ItemIcon storedSlot in StoredSlots)
		{
			storedSlot.UpdateSlotImage();
		}
		if (isPlayer)
		{
			GoldTXT.text = Gold.ToString();
			Modulars.UpdatePlayerVisuals(EquipmentSlots);
			UpdateInvStats();
			PlayerStatDisp.UpdateDisplayStats();
			if (AuraSlot.MyItem != null && AuraSlot.MyItem != Empty)
			{
				AuraTxt.text = AuraSlot.MyItem.Aura.SpellName;
			}
			else
			{
				AuraTxt.text = "No Aura Equipped";
			}
		}
	}

	public void ForceItemToInv(Item _item)
	{
		for (int i = 0; i < StoredSlots.Count; i++)
		{
			if (StoredSlots[i].TrashSlot)
			{
				StoredSlots[i].MyItem = _item;
				StoredSlots[i].Quantity = 1;
				UpdateSocialLog.LogAdd("WARNING: Inventory full. Received item in TRASH SLOT!", "red");
				UpdateSocialLog.LogAdd("WARNING: Item will be lost unless its relocated to a safe inventory slot.", "red");
				if (InvWindow.activeSelf)
				{
					UpdatePlayerInventory();
				}
			}
		}
	}

	public void ForceItemToInv(Item _item, int _qual)
	{
		for (int i = 0; i < StoredSlots.Count; i++)
		{
			if (StoredSlots[i].TrashSlot)
			{
				StoredSlots[i].MyItem = _item;
				StoredSlots[i].Quantity = _qual;
				UpdateSocialLog.LogAdd("WARNING: Inventory full. Received item in TRASH SLOT!", "red");
				UpdateSocialLog.LogAdd("WARNING: Item will be lost unless its relocated to a safe inventory slot.", "red");
				if (InvWindow.activeSelf)
				{
					UpdatePlayerInventory();
				}
			}
		}
	}

	public bool AddItemToInv(Item _item)
	{
		bool flag = false;
		if (_item.Stackable)
		{
			for (int i = 0; i < StoredSlots.Count; i++)
			{
				if (StoredSlots[i].MyItem == _item)
				{
					StoredSlots[i].Quantity++;
					if (InvWindow.activeSelf)
					{
						UpdatePlayerInventory();
					}
					flag = true;
					return true;
				}
			}
		}
		if (!flag)
		{
			for (int j = 0; j < StoredSlots.Count; j++)
			{
				if (StoredSlots[j].MyItem == GameData.PlayerInv.Empty && !StoredSlots[j].TrashSlot)
				{
					StoredSlots[j].MyItem = _item;
					if (InvWindow.activeSelf)
					{
						UpdatePlayerInventory();
					}
					flag = true;
					return true;
				}
			}
		}
		return false;
	}

	public bool AddItemToInv(Item _item, int _qual)
	{
		if (0 == 0)
		{
			for (int i = 0; i < StoredSlots.Count; i++)
			{
				if (StoredSlots[i].MyItem == GameData.PlayerInv.Empty && !StoredSlots[i].TrashSlot)
				{
					StoredSlots[i].MyItem = _item;
					StoredSlots[i].Quantity = _qual;
					if (InvWindow.activeSelf)
					{
						UpdatePlayerInventory();
					}
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveItemFromInv(ItemIcon whichSlot)
	{
		if (whichSlot.Quantity > 1 && whichSlot.MyItem.RequiredSlot == Item.SlotType.General)
		{
			whichSlot.Quantity--;
		}
		else if (whichSlot.Quantity > 1 && whichSlot.MyItem.RequiredSlot != 0)
		{
			whichSlot.MyItem = Empty;
		}
		else
		{
			whichSlot.MyItem = Empty;
		}
		UpdatePlayerInventory();
		whichSlot.UpdateSlotImage();
	}

	public void RemoveStackFromInv(ItemIcon whichSlot)
	{
		whichSlot.MyItem = Empty;
		whichSlot.Quantity = 1;
		UpdatePlayerInventory();
		whichSlot.UpdateSlotImage();
	}

	public void RemoveItemFromInv(Item whichItem)
	{
		foreach (ItemIcon storedSlot in StoredSlots)
		{
			if (storedSlot.MyItem == whichItem)
			{
				storedSlot.MyItem = Empty;
				storedSlot.UpdateSlotImage();
				UpdatePlayerInventory();
				break;
			}
		}
	}

	public void UpdateInvStats()
	{
		ItemHP = 0;
		ItemAC = 0;
		ItemMana = 0;
		ItemStr = 0;
		ItemEnd = 0;
		ItemDex = 0;
		ItemAgi = 0;
		ItemInt = 0;
		ItemWis = 0;
		ItemCha = 0;
		ItemRes = 0;
		ItemMR = 0;
		ItemER = 0;
		ItemPR = 0;
		ItemVR = 0;
		MHDelay = 0f;
		OHDelay = 0f;
		MHDmg = 0;
		OHDmg = 0;
		StrScaleMod = 0;
		EndScaleMod = 0;
		DexScaleMod = 0;
		AgiScaleMod = 0;
		IntScaleMod = 0;
		WisScaleMod = 0;
		ChaScaleMod = 0;
		MitScaleMod = 0f;
		if (GetComponent<SimPlayer>() == null)
		{
			foreach (ItemIcon equipmentSlot in EquipmentSlots)
			{
				ItemHP += equipmentSlot.MyItem.CalcACHPMC(equipmentSlot.MyItem.HP, equipmentSlot.Quantity);
				ItemAC += equipmentSlot.MyItem.CalcACHPMC(equipmentSlot.MyItem.AC, equipmentSlot.Quantity);
				ItemMana += equipmentSlot.MyItem.CalcACHPMC(equipmentSlot.MyItem.Mana, equipmentSlot.Quantity);
				ItemStr += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.Str, equipmentSlot.Quantity);
				ItemEnd += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.End, equipmentSlot.Quantity);
				ItemDex += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.Dex, equipmentSlot.Quantity);
				ItemAgi += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.Agi, equipmentSlot.Quantity);
				ItemInt += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.Int, equipmentSlot.Quantity);
				ItemWis += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.Wis, equipmentSlot.Quantity);
				ItemCha += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.Cha, equipmentSlot.Quantity);
				ItemRes += equipmentSlot.MyItem.CalcRes(equipmentSlot.MyItem.Res, equipmentSlot.Quantity);
				ItemMR += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.MR, equipmentSlot.Quantity);
				ItemER += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.ER, equipmentSlot.Quantity);
				ItemPR += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.PR, equipmentSlot.Quantity);
				ItemVR += equipmentSlot.MyItem.CalcStat(equipmentSlot.MyItem.VR, equipmentSlot.Quantity);
				if (equipmentSlot.ThisSlotType == Item.SlotType.Primary)
				{
					MHDelay = equipmentSlot.MyItem.WeaponDly;
					MHDmg = equipmentSlot.MyItem.CalcDmg(equipmentSlot.MyItem.WeaponDmg, equipmentSlot.Quantity);
				}
				if (equipmentSlot.ThisSlotType == Item.SlotType.Secondary)
				{
					OHDelay = equipmentSlot.MyItem.WeaponDly;
					OHDmg = equipmentSlot.MyItem.CalcDmg(equipmentSlot.MyItem.WeaponDmg, equipmentSlot.Quantity);
				}
			}
			StrScaleMod += (int)CharmSlot.MyItem.StrScaling;
			EndScaleMod += (int)CharmSlot.MyItem.EndScaling;
			DexScaleMod += (int)CharmSlot.MyItem.DexScaling;
			AgiScaleMod += (int)CharmSlot.MyItem.AgiScaling;
			IntScaleMod += (int)CharmSlot.MyItem.IntScaling;
			WisScaleMod += (int)CharmSlot.MyItem.WisScaling;
			ChaScaleMod += (int)CharmSlot.MyItem.ChaScaling;
			MitScaleMod += (int)CharmSlot.MyItem.MitigationScaling;
			if (isPlayer && CharmSlot.MyItem != null)
			{
				if (CharmSlot.MyItem.ItemName == "Pupil's Pin")
				{
					GlobalFactionManager.FindFactionData("AncientSchool").Value = 10f;
				}
				else
				{
					GlobalFactionManager.FindFactionData("AncientSchool").Value = -10f;
				}
			}
		}
		else
		{
			foreach (SimInvSlot item in GetComponent<SimPlayer>().MyEquipment)
			{
				ItemHP += item.MyItem.CalcACHPMC(item.MyItem.HP, item.Quant);
				ItemAC += item.MyItem.CalcACHPMC(item.MyItem.AC, item.Quant);
				ItemMana += item.MyItem.CalcACHPMC(item.MyItem.Mana, item.Quant);
				ItemStr += item.MyItem.CalcStat(item.MyItem.Str, item.Quant);
				ItemEnd += item.MyItem.CalcStat(item.MyItem.End, item.Quant);
				ItemDex += item.MyItem.CalcStat(item.MyItem.Dex, item.Quant);
				ItemAgi += item.MyItem.CalcStat(item.MyItem.Agi, item.Quant);
				ItemInt += item.MyItem.CalcStat(item.MyItem.Int, item.Quant);
				ItemWis += item.MyItem.CalcStat(item.MyItem.Wis, item.Quant);
				ItemCha += item.MyItem.CalcStat(item.MyItem.Cha, item.Quant);
				ItemRes += item.MyItem.CalcRes(item.MyItem.Res, item.Quant);
				ItemMR += item.MyItem.CalcStat(item.MyItem.MR, item.Quant);
				ItemER += item.MyItem.CalcStat(item.MyItem.ER, item.Quant);
				ItemPR += item.MyItem.CalcStat(item.MyItem.PR, item.Quant);
				ItemVR += item.MyItem.CalcStat(item.MyItem.VR, item.Quant);
				StrScaleMod += (int)item.MyItem.StrScaling;
				EndScaleMod += (int)item.MyItem.EndScaling;
				DexScaleMod += (int)item.MyItem.DexScaling;
				AgiScaleMod += (int)item.MyItem.AgiScaling;
				IntScaleMod += (int)item.MyItem.IntScaling;
				WisScaleMod += (int)item.MyItem.WisScaling;
				ChaScaleMod += (int)item.MyItem.ChaScaling;
				MitScaleMod += item.MyItem.MitigationScaling;
				if (item.ThisSlotType == Item.SlotType.Primary && (item.MyItem.RequiredSlot == Item.SlotType.Primary || item.MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary))
				{
					MHDelay = item.MyItem.WeaponDly;
					MHDmg = item.MyItem.CalcDmg(item.MyItem.WeaponDmg, item.Quant);
					if (SimMH != null)
					{
						SimMH.MyItem = item.MyItem;
						SimMH.Quant = item.Quant;
						if ((SimMH.MyItem != null && SimMH.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee) || SimMH.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff)
						{
							TwoHandPrimary = true;
						}
						else
						{
							TwoHandPrimary = false;
						}
					}
					else
					{
						SimMH = new SimInvSlot(Item.SlotType.Primary);
						SimMH.MyItem = item.MyItem;
						SimMH.Quant = item.Quant;
						if ((SimMH.MyItem != null && SimMH.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee) || SimMH.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff)
						{
							TwoHandPrimary = true;
						}
						else
						{
							TwoHandPrimary = false;
						}
					}
				}
				if (item.ThisSlotType == Item.SlotType.Secondary && (item.MyItem.RequiredSlot == Item.SlotType.Secondary || item.MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary))
				{
					OHDelay = item.MyItem.WeaponDly;
					OHDmg = item.MyItem.CalcDmg(item.MyItem.WeaponDmg, item.Quant);
					if (SimOH != null)
					{
						SimOH.MyItem = item.MyItem;
						SimOH.Quant = item.Quant;
					}
					else
					{
						SimOH = new SimInvSlot(Item.SlotType.Secondary);
						SimOH.MyItem = item.MyItem;
						SimOH.Quant = item.Quant;
					}
				}
			}
		}
		if (isPlayer)
		{
			GetComponent<Stats>().CalcStats();
			PlayerStatDisp.UpdateDisplayStats();
		}
	}

	public Character NearestLootable()
	{
		return lootable;
	}

	public void SetHalloweenMask()
	{
		HasCosmeticMask = false;
		foreach (GameObject halloweenMask in Modulars.HalloweenMasks)
		{
			halloweenMask.SetActive(value: false);
		}
		foreach (ItemIcon cosmeticSlot in CosmeticSlots)
		{
			foreach (GameObject halloweenMask2 in Modulars.HalloweenMasks)
			{
				if (halloweenMask2.transform.name == cosmeticSlot.MyItem.EquipmentToActivate)
				{
					halloweenMask2.SetActive(value: true);
					Modulars.HalloweenMasks[0].SetActive(value: true);
					HasCosmeticMask = true;
				}
			}
		}
		Modulars.UpdatePlayerVisuals(EquipmentSlots);
	}

	public void InitHalloweenMask()
	{
		HasCosmeticMask = false;
		foreach (GameObject halloweenMask in Modulars.HalloweenMasks)
		{
			halloweenMask.SetActive(value: false);
		}
		foreach (ItemIcon cosmeticSlot in CosmeticSlots)
		{
			foreach (GameObject halloweenMask2 in Modulars.HalloweenMasks)
			{
				if (halloweenMask2.transform.name == cosmeticSlot.MyItem.EquipmentToActivate)
				{
					halloweenMask2.SetActive(value: true);
					Modulars.HalloweenMasks[0].SetActive(value: true);
					HasCosmeticMask = true;
				}
			}
		}
	}
}
