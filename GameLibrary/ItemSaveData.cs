// ItemSaveData
using System;

[Serializable]
public class ItemSaveData
{
	public int Quality = 1;

	public string ID;

	public ItemSaveData(string _itemID, int _quality)
	{
		Quality = _quality;
		ID = _itemID;
	}
}
