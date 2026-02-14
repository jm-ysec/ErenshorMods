// LootWindow
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootWindow : MonoBehaviour
{
	public GameObject WindowParent;

	public List<ItemIcon> LootSlots;

	public TextMeshProUGUI LootSource;

	private Vector3 lootPos;

	private LootTable parent;

	private int items;

	public TextMeshProUGUI LootButtonTxt;

	private float downCD;

	private void Awake()
	{
		GameData.LootWindow = this;
		WindowParent.SetActive(value: false);
	}

	private void Update()
	{
		if ((WindowParent.activeSelf && parent != null && Vector3.Distance(GameData.PlayerControl.transform.position, parent.transform.position) > 5f + parent.transform.localScale.y) || Input.GetKeyDown(KeyCode.Escape))
		{
			CloseWindow();
		}
		if (WindowParent.activeSelf && (GameData.Autoattacking || GameData.PlayerStats.Myself.MySpells.isCasting()))
		{
			CloseWindow();
		}
		if (Input.GetKeyDown(InputManager.Loot) && downCD <= 0f && !GameData.PlayerTyping)
		{
			LootAll();
			downCD = 5f;
		}
		if (downCD > 0f)
		{
			downCD -= 60f * Time.deltaTime;
		}
	}

	public void LootAll()
	{
		foreach (ItemIcon lootSlot in LootSlots)
		{
			if (!(lootSlot.MyItem == GameData.PlayerInv.Empty))
			{
				bool flag = false;
				if (lootSlot.MyItem.RequiredSlot == Item.SlotType.General)
				{
					flag = GameData.PlayerInv.AddItemToInv(lootSlot.MyItem);
				}
				else
				{
					flag = GameData.PlayerInv.AddItemToInv(lootSlot.MyItem, lootSlot.Quantity);
					_ = lootSlot.Quantity;
					_ = 1;
					_ = lootSlot.Quantity;
					_ = 2;
					_ = lootSlot.Quantity;
					_ = 3;
				}
				if (flag)
				{
					UpdateSocialLog.LogAdd("Looted Item: " + lootSlot.MyItem.ItemName, "yellow");
					lootSlot.InformGroupOfLoot(lootSlot.MyItem);
					lootSlot.MyItem = GameData.PlayerInv.Empty;
					lootSlot.UpdateSlotImage();
				}
				else
				{
					UpdateSocialLog.LogAdd("No room for " + lootSlot.MyItem.ItemName, "yellow");
				}
			}
		}
		if (WindowParent.activeSelf)
		{
			GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume / 2f * GameData.UIVolume * GameData.MasterVol);
		}
		CloseWindow();
	}

	public void DestroyAll()
	{
		foreach (ItemIcon lootSlot in LootSlots)
		{
			if (lootSlot.MyItem != GameData.PlayerInv.Empty)
			{
				UpdateSocialLog.LogAdd("Destroyed Item: " + lootSlot.MyItem.ItemName, "yellow");
				lootSlot.MyItem = GameData.PlayerInv.Empty;
				lootSlot.UpdateSlotImage();
			}
		}
		if (WindowParent.activeSelf)
		{
			GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume / 2f * GameData.UIVolume * GameData.MasterVol);
		}
		CloseWindow();
	}

	public void LoadWindow(List<Item> LootItems, LootTable _incoming)
	{
		downCD = 5f;
		foreach (ItemIcon lootSlot in LootSlots)
		{
			lootSlot.Quantity = 1;
		}
		bool flag = false;
		if (_incoming.qualUps.Count == 0 && LootItems.Count > 0 && !_incoming.FromChest())
		{
			flag = true;
		}
		GameData.PlayerControl.LootDelay = 25f;
		items = 0;
		LootSource.text = _incoming.transform.name + "'s Loot";
		WindowParent.SetActive(value: true);
		parent = _incoming;
		GameData.PlayerInv.ForceOpenInv();
		GameData.GM.CloseAscensionWindow();
		LootButtonTxt.text = InputManager.Loot.ToString() + "  - Loot All";
		int num = Mathf.Min(8, LootSlots.Count, LootItems.Count);
		for (int i = 0; i < num; i++)
		{
			if (LootItems[i] != null)
			{
				LootSlots[i].MyItem = LootItems[i];
				if (flag)
				{
					if (LootItems[i].RequiredSlot != 0 && LootItems[i].RequiredSlot != Item.SlotType.Aura)
					{
						if (((i < _incoming.ActualDropsQual.Count) ? _incoming.ActualDropsQual[i] : Random.Range(0, 3)) < 1)
						{
							LootSlots[i].Quantity = 2;
							SetAchievement.Unlock("SPARKLES");
							_incoming.qualUps.Add(2);
						}
						else
						{
							_incoming.qualUps.Add(1);
						}
					}
					else
					{
						_incoming.qualUps.Add(1);
					}
				}
				else if (i < _incoming.qualUps.Count)
				{
					LootSlots[i].Quantity = _incoming.qualUps[i];
				}
				else
				{
					LootSlots[i].Quantity = 1;
				}
				items++;
			}
			else
			{
				LootSlots[i].MyItem = GameData.PlayerInv.Empty;
				LootSlots[i].Quantity = 1;
				_incoming.qualUps.Add(1);
			}
			LootSlots[i].UpdateSlotImage();
		}
		GameData.PlayerControl.GetComponent<Animator>().ResetTrigger("EndLoot");
	}

	public void CloseWindow()
	{
		if (!WindowParent.activeSelf)
		{
			return;
		}
		GameData.PlayerControl.GetComponent<Animator>().SetTrigger("EndLoot");
		GameData.PlayerAud.PlayOneShot(GameData.Misc.CloseWindow, GameData.UIVolume * GameData.MasterVol * 0.05f);
		if (!WindowParent.activeSelf || GameData.InCharSelect)
		{
			return;
		}
		int num = 0;
		List<Item> list = new List<Item>();
		foreach (ItemIcon lootSlot in LootSlots)
		{
			if (lootSlot.MyItem != GameData.PlayerInv.Empty)
			{
				num++;
			}
			list.Add(lootSlot.MyItem);
			lootSlot.MyItem = GameData.PlayerInv.Empty;
			lootSlot.UpdateSlotImage();
			GameData.PlayerInv.ForceCloseInv();
		}
		if (parent != null)
		{
			parent.ReturnLoot(list);
		}
		WindowParent.SetActive(value: false);
		if (parent == null)
		{
			return;
		}
		parent.ReturnLoot(list);
		if (num <= 0)
		{
			if (parent.GetComponent<NPC>() != null)
			{
				parent.GetComponent<NPC>().ExpediteRot();
			}
			if (parent.GetComponent<RotChest>() != null)
			{
				parent.GetComponent<RotChest>().rotTimer = 1f;
			}
		}
		else
		{
			((parent.GetComponent<NPC>()?.GetChar())?.savedCorpse)?.ModifyLoot(list);
		}
	}
}
