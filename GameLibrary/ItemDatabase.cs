// ItemDatabase
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
	public Item[] ItemDB;

	public List<Item> ItemDBList;

	public List<Item> GenericItems;

	private Dictionary<string, Item> itemDict;

	private void Awake()
	{
	}

	private void Start()
	{
		GameData.ItemDB = this;
		ItemDB = Resources.LoadAll("Items", typeof(Item)).Cast<Item>().ToArray();
		Item[] itemDB = ItemDB;
		foreach (Item item in itemDB)
		{
			if (!item.Unique && item.RequiredSlot == Item.SlotType.General && item.ItemValue > 0 && item.Id != "46289586" && item.Id != "23431650")
			{
				GenericItems.Add(item);
			}
		}
		itemDict = new Dictionary<string, Item>(ItemDB.Length);
		for (int j = 0; j < ItemDB.Length; j++)
		{
			Item item2 = ItemDB[j];
			if (!string.IsNullOrEmpty(item2.Id))
			{
				itemDict[item2.Id] = item2;
			}
		}
		GetComponent<KnowledgeDatabase>().BuildItemSearchDB();
	}

	public string GetItemListAndLocation()
	{
		string text = "";
		for (int i = 0; i < ItemDB.Length; i++)
		{
			text = text + i + ": " + ItemDB[i].name + "\n";
		}
		return text;
	}

	public string GetItemListAndLocation(int _start, int _end)
	{
		string text = "";
		if (_end >= ItemDB.Length)
		{
			_end = ItemDB.Length - 1;
		}
		for (int i = _start; i < _end; i++)
		{
			if (i < ItemDB.Length)
			{
				text = text + i + ": " + ItemDB[i].ItemName + "\n";
			}
		}
		return text;
	}

	public Item GetItemFromDB(int _index)
	{
		return ItemDB[_index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Item GetItemByID(string id)
	{
		if (id != null && itemDict.TryGetValue(id, out var value))
		{
			return value;
		}
		return GameData.PlayerInv.Empty;
	}

	public Item GetRandomGeneric()
	{
		return GenericItems[Random.Range(0, GenericItems.Count)];
	}
}
