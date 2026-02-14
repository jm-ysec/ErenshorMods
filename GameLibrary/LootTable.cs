// LootTable
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
	public List<Item> GuaranteeOneDrop;

	public List<Item> CommonDrop;

	public List<Item> UncommonDrop;

	public List<Item> RareDrop;

	public List<Item> LegendaryDrop;

	public List<Item> UltraRareDrop;

	public List<Item> ActualDrops = new List<Item>();

	public List<int> ActualDropsQual = new List<int>();

	public int MaxNumberDrops;

	public List<Transform> VisiblePieces;

	public int MaxNonCommonDrops;

	private int nonCommonDropDone;

	public int MaxGold;

	public int MinGold;

	public int MyGold;

	public List<int> qualUps = new List<int>();

	public bool special;

	public int seed;

	public string ZoneThisLootIsFrom = "";

	private void Start()
	{
		InitLootTable();
		InitVisiblePieces();
		seed = Random.Range(0, 100);
	}

	private void InitLootTable()
	{
		float num = 0f;
		if (GameData.PlayerControl.MyGuild == GameData.GuildManager.GetTopGuildID())
		{
			num = 0.5f;
		}
		ActualDropsQual.Clear();
		special = false;
		if (GuaranteeOneDrop.Count > 0)
		{
			ActualDrops.Add(GuaranteeOneDrop[Random.Range(0, GuaranteeOneDrop.Count)]);
		}
		for (int i = 0; (float)i <= (float)MaxNumberDrops + 2f * num; i++)
		{
			MyGold = Random.Range(MinGold, MaxGold);
			float num2 = Random.Range(0f, 100f);
			num2 /= GameData.ServerLootRate;
			if (num2 < 0.33f && UltraRareDrop.Count > 0 && nonCommonDropDone < MaxNonCommonDrops)
			{
				ActualDrops.Add(UltraRareDrop[Random.Range(0, UltraRareDrop.Count)]);
				nonCommonDropDone++;
			}
			else if (num2 <= 2.3f && LegendaryDrop.Count > 0 && nonCommonDropDone < MaxNonCommonDrops)
			{
				ActualDrops.Add(LegendaryDrop[Random.Range(0, LegendaryDrop.Count)]);
				nonCommonDropDone++;
			}
			else if (num2 <= 7f && RareDrop.Count > 0 && nonCommonDropDone < MaxNonCommonDrops)
			{
				ActualDrops.Add(RareDrop[Random.Range(0, RareDrop.Count)]);
				nonCommonDropDone++;
			}
			else if (num2 <= 15f && UncommonDrop.Count > 0 && nonCommonDropDone < MaxNonCommonDrops)
			{
				ActualDrops.Add(UncommonDrop[Random.Range(0, UncommonDrop.Count)]);
				nonCommonDropDone++;
			}
			else if (num2 <= 70f && CommonDrop.Count > 0)
			{
				if (Random.Range(0, 10) > 8)
				{
					ActualDrops.Add(GameData.GM.CommonWorldItems[Random.Range(0, GameData.GM.CommonWorldItems.Count)]);
				}
				else
				{
					ActualDrops.Add(CommonDrop[Random.Range(0, CommonDrop.Count)]);
				}
			}
		}
		Stats component = GetComponent<Stats>();
		float num3 = GameData.ServerLootRate + num;
		if (component != null && component.Level > 15 && Random.value < 0.001f * num3)
		{
			ActualDrops.Add(GameData.GM.Sivak);
			special = true;
		}
		if (Random.value < 0.005f * num3)
		{
			ActualDrops.Add(GameData.GM.WorldDropMolds[Random.Range(0, GameData.GM.WorldDropMolds.Count)]);
			special = true;
		}
		if (Random.value < 0.0125f * num3)
		{
			ActualDrops.Add(GameData.GM.Maps[Random.Range(0, GameData.GM.Maps.Count)]);
			special = true;
		}
		if (component != null && component.Level > 12 && Random.value < 0.002f * num3)
		{
			ActualDrops.Add(GameData.GM.XPPot);
			special = true;
		}
		if (component != null && component.Level > 20 && Random.value < 0.008f * num3)
		{
			ActualDrops.Add(GameData.GM.InertDiamond);
			special = true;
		}
		if (component != null && component.Level > 15 && Random.value < 0.001f * num3)
		{
			ActualDrops.Add(GameData.GM.PlanarShard);
			special = true;
		}
		if (Random.value < 0.001f && GameData.GM.DropMasks)
		{
			if (Random.Range(0, 100) > 1)
			{
				ActualDrops.Add(GameData.Misc.Masks[Random.Range(0, GameData.Misc.Masks.Count)]);
			}
			else if (GameData.Misc.MoloraiMask != null)
			{
				ActualDrops.Add(GameData.Misc.MoloraiMask);
			}
			special = true;
		}
		if (GameData.GM.DemoBuild && Random.value < 0.001f * num3)
		{
			ActualDrops.Add(GameData.GM.Empty2);
			special = true;
		}
		if (component != null && component.Level > 30 && Random.value < 0.0005f * num3)
		{
			ActualDrops.Add(GameData.GM.CrystallizedBalance);
			special = true;
		}
		if (component != null && component.Level > 30 && Random.value < 0.0005f * num3)
		{
			ActualDrops.Add(GameData.GM.Planar);
			special = true;
		}
		if (ActualDrops.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < ActualDrops.Count; j++)
		{
			int num4 = Random.Range(0, 100);
			if (ActualDrops[j] != null && ActualDrops[j].RequiredSlot != 0 && ActualDrops[j].RequiredSlot != Item.SlotType.Aura)
			{
				ActualDropsQual.Add(num4);
			}
			else
			{
				ActualDropsQual.Add(1);
			}
			if (num4 < 1 && ActualDrops[j] != null && ActualDrops[j].RequiredSlot != 0 && ActualDrops[j].RequiredSlot != Item.SlotType.Aura)
			{
				special = true;
			}
		}
		for (int k = 0; k < ActualDrops.Count; k++)
		{
			if (ActualDrops[k] != null && ActualDrops[k].Unique && GameData.PlayerInv.HasItem(ActualDrops[k], _remove: false))
			{
				ActualDrops[k] = GameData.GM.CommonWorldItems[Random.Range(0, GameData.GM.CommonWorldItems.Count)];
				special = false;
			}
		}
		special = false;
		for (int l = 0; l < ActualDrops.Count; l++)
		{
			if (!(ActualDrops[l] == null) && (ActualDrops[l] == GameData.GM.Sivak || ActualDrops[l] == GameData.GM.PlanarShard || ActualDrops[l] == GameData.GM.XPPot || ActualDrops[l] == GameData.GM.InertDiamond || GameData.GM.Maps.Contains(ActualDrops[l]) || GameData.GM.WorldDropMolds.Contains(ActualDrops[l]) || ActualDrops[l] == GameData.GM.Planar || (ActualDropsQual.Count > l && ActualDropsQual[l] == 0)))
			{
				special = true;
				break;
			}
		}
	}

	private void InitVisiblePieces()
	{
		if (VisiblePieces.Count <= 0)
		{
			return;
		}
		foreach (Item actualDrop in ActualDrops)
		{
			foreach (Transform visiblePiece in VisiblePieces)
			{
				if (actualDrop.EquipmentToActivate == visiblePiece.name)
				{
					visiblePiece.gameObject.SetActive(value: true);
				}
			}
		}
	}

	public void LoadLootTable()
	{
		for (int i = 0; i <= 7; i++)
		{
			if (i >= ActualDrops.Count)
			{
				ActualDrops.Add(GameData.PlayerInv.Empty);
			}
		}
		GameData.LootWindow.CloseWindow();
		GameData.LootWindow.LoadWindow(ActualDrops, this);
		if (MyGold != 0)
		{
			GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().Coin, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
			GameData.PlayerInv.Gold += MyGold;
			UpdateSocialLog.LogAdd("Found " + MyGold + " Gold!");
			MyGold = 0;
		}
	}

	public void ReturnLoot(List<Item> _remainingLoot)
	{
		ActualDrops.Clear();
		MyGold = 0;
		foreach (Item item in _remainingLoot)
		{
			ActualDrops.Add(item);
		}
	}

	public bool FromChest()
	{
		if (GetComponent<RotChest>() != null)
		{
			return true;
		}
		return false;
	}

	private void Example()
	{
		if (ActualDrops.Count > 0)
		{
			if (RareDrop.Count <= 0)
			{
				Example();
			}
			else
			{
				Example();
			}
		}
	}
}
