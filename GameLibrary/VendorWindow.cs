// VendorWindow
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorWindow : MonoBehaviour
{
	public GameObject WindowParent;

	public List<ItemIcon> VendorSlots;

	public TextMeshProUGUI LootSource;

	public TextMeshProUGUI ButtonContext;

	public ItemIcon BuyBack;

	public GameObject NextPage;

	public GameObject PrevPage;

	private List<Item> LoadedItems;

	private VendorInventory parent;

	public int Page;

	public Button SellStack;

	private void Awake()
	{
		GameData.VendorWindow = this;
		WindowParent.SetActive(value: false);
	}

	private void Update()
	{
		if ((WindowParent.activeSelf && Vector3.Distance(GameData.PlayerControl.transform.position, parent.transform.position) > 5f) || (WindowParent.activeSelf && Input.GetKeyDown(KeyCode.Escape)))
		{
			CloseWindow();
		}
	}

	public void LoadWindow(List<Item> VendorItems, VendorInventory _incoming)
	{
		LoadedItems = VendorItems;
		LootSource.text = "Doing business with " + _incoming.transform.name;
		WindowParent.SetActive(value: true);
		BuyBack.MyItem = GameData.PlayerInv.Empty;
		BuyBack.Quantity = 1;
		BuyBack.UpdateSlotImage();
		parent = _incoming;
		if (parent.QuestRewardsForSale.Count > 0)
		{
			foreach (Quest item in parent.QuestRewardsForSale)
			{
				if (GameData.IsQuestDone(item.DBName) && item.UnlockItemForVendor != null && item.UnlockItemForVendor != GameData.PlayerInv.Empty && !VendorItems.Contains(item.UnlockItemForVendor))
				{
					VendorItems.Add(item.UnlockItemForVendor);
				}
			}
		}
		GameData.PlayerInv.ForceOpenInv();
		GameData.VendorWindowOpen = true;
		for (int i = 0; i < 12; i++)
		{
			int num = i + Page * 12;
			if (num < VendorItems.Count && VendorItems[num] != null)
			{
				VendorSlots[i].MyItem = VendorItems[num];
			}
			else
			{
				VendorSlots[i].MyItem = GameData.PlayerInv.Empty;
			}
			VendorSlots[i].UpdateSlotImage();
		}
		if (VendorItems.Count > (Page + 1) * 12)
		{
			NextPage.SetActive(value: true);
		}
		else
		{
			NextPage.SetActive(value: false);
		}
		if (Page > 0)
		{
			PrevPage.SetActive(value: true);
		}
		else
		{
			PrevPage.SetActive(value: false);
		}
	}

	public void GoToNextPage()
	{
		Page++;
		LoadWindow(LoadedItems, parent);
	}

	public void GoToPrevPage()
	{
		Page--;
		LoadWindow(LoadedItems, parent);
	}

	public void Transaction()
	{
		bool flag = false;
		if (!(GameData.SlotActiveForVendor != null))
		{
			return;
		}
		if (GameData.SlotActiveForVendor.VendorSlot)
		{
			int num = GameData.CurSellVal;
			if (GameData.SlotActiveForVendor == BuyBack)
			{
				num = Mathf.RoundToInt((float)num * 0.65f);
				flag = true;
			}
			else
			{
				flag = false;
			}
			if (GameData.PlayerInv.Gold < num)
			{
				return;
			}
			GameData.PlayerAud.PlayOneShot(GameData.Misc.BuyItem, GameData.UIVolume * GameData.MasterVol);
			if (GameData.SlotActiveForVendor.Quantity == 1)
			{
				if (!GameData.PlayerInv.AddItemToInv(GameData.SlotActiveForVendor.MyItem))
				{
					GameData.PlayerInv.ForceItemToInv(GameData.SlotActiveForVendor.MyItem);
				}
			}
			else if (!GameData.PlayerInv.AddItemToInv(GameData.SlotActiveForVendor.MyItem, GameData.SlotActiveForVendor.Quantity))
			{
				GameData.PlayerInv.ForceItemToInv(GameData.SlotActiveForVendor.MyItem, GameData.SlotActiveForVendor.Quantity);
			}
			GameData.PlayerInv.Gold -= num;
			GameData.PlayerInv.GoldTXT.text = GameData.PlayerInv.Gold.ToString();
			UpdateSocialLog.LogAdd("You purchased: " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + num + " gold", "yellow");
			UpdateSocialLog.LocalLogAdd("You purchased: " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + num + " gold", "yellow");
			if (flag)
			{
				GameData.SlotActiveForVendor.MyItem = GameData.PlayerInv.Empty;
				GameData.SlotActiveForVendor.UpdateSlotImage();
				GameData.SlotActiveForVendor = null;
			}
		}
		else if (GameData.SlotActiveForVendor.MyItem.ItemValue > 0)
		{
			if (GameData.SlotActiveForVendor.Quantity > 1 && GameData.SlotActiveForVendor.MyItem.RequiredSlot != 0)
			{
				BuyBack.Quantity = GameData.SlotActiveForVendor.Quantity;
				BuyBack.MyItem = GameData.SlotActiveForVendor.MyItem;
			}
			else
			{
				BuyBack.MyItem = GameData.SlotActiveForVendor.MyItem;
				BuyBack.Quantity = 1;
			}
			BuyBack.UpdateSlotImage();
			GameData.PlayerAud.PlayOneShot(GameData.Misc.BuyItem, GameData.UIVolume * GameData.MasterVol);
			UpdateSocialLog.LogAdd("You sold: " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + " gold", "yellow");
			UpdateSocialLog.LocalLogAdd("You sold: " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + " gold", "yellow");
			GameData.PlayerInv.Gold += Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + 1;
			GameData.PlayerInv.GoldTXT.text = GameData.PlayerInv.Gold.ToString();
			GameData.PlayerInv.RemoveItemFromInv(GameData.SlotActiveForVendor);
		}
	}

	public void DoSellStack()
	{
		if (!(GameData.SlotActiveForVendor != null) || GameData.SlotActiveForVendor.VendorSlot || GameData.SlotActiveForVendor.MyItem.ItemValue <= 0)
		{
			return;
		}
		GameData.PlayerAud.PlayOneShot(GameData.Misc.BuyItem, GameData.UIVolume * GameData.MasterVol);
		if (GameData.SlotActiveForVendor.MyItem != null && GameData.SlotActiveForVendor.MyItem.RequiredSlot == Item.SlotType.General)
		{
			BuyBack.MyItem = GameData.SlotActiveForVendor.MyItem;
			if (GameData.SlotActiveForVendor.Quantity > 1 && GameData.SlotActiveForVendor.MyItem.RequiredSlot != 0)
			{
				BuyBack.Quantity = GameData.SlotActiveForVendor.Quantity;
				BuyBack.MyItem = GameData.SlotActiveForVendor.MyItem;
			}
			else
			{
				BuyBack.MyItem = GameData.SlotActiveForVendor.MyItem;
				BuyBack.Quantity = 1;
			}
			BuyBack.UpdateSlotImage();
			UpdateSocialLog.LogAdd("You sold a stack of " + GameData.SlotActiveForVendor.Quantity + " " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + GameData.SlotActiveForVendor.Quantity * Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + " gold", "yellow");
			UpdateSocialLog.LocalLogAdd("You sold a stack of " + GameData.SlotActiveForVendor.Quantity + " " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + GameData.SlotActiveForVendor.Quantity * Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + " gold", "yellow");
			GameData.PlayerInv.Gold += GameData.SlotActiveForVendor.Quantity * Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f);
		}
		else
		{
			BuyBack.MyItem = GameData.SlotActiveForVendor.MyItem;
			BuyBack.UpdateSlotImage();
			UpdateSocialLog.LogAdd("You sold a " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + " gold", "yellow");
			UpdateSocialLog.LocalLogAdd("You sold a " + GameData.SlotActiveForVendor.MyItem.ItemName + " for " + Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f) + " gold", "yellow");
			GameData.PlayerInv.Gold += Mathf.RoundToInt((float)GameData.SlotActiveForVendor.MyItem.ItemValue * 0.65f);
		}
		GameData.PlayerInv.GoldTXT.text = GameData.PlayerInv.Gold.ToString();
		GameData.PlayerInv.RemoveStackFromInv(GameData.SlotActiveForVendor);
	}

	public string GetVendor()
	{
		return parent.transform.name;
	}

	public void CloseWindow()
	{
		Page = 0;
		GameData.PlayerAud.PlayOneShot(GameData.Misc.CloseWindow, GameData.UIVolume * GameData.MasterVol * 0.05f);
		GameData.DeactivateSlotForVendor();
		GameData.SlotActiveForVendor = null;
		GameData.PlayerInv.ForceCloseInv();
		WindowParent.SetActive(value: false);
		GameData.VendorWindowOpen = false;
	}
}
