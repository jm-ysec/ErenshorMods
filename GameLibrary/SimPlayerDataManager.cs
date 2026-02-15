// SimPlayerDataManager
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SimPlayerDataManager
{
	public static List<SimPlayerSaveData> SimPlayerData = new List<SimPlayerSaveData>();

	public static void SaveAllSimData()
	{
		GameData.SimMngr.CollectActiveSimData();
	}

	public static void DeleteSimSaveData(string npcName)
	{
		if (string.IsNullOrEmpty(npcName))
		{
			return;
		}
		string path = "Sims" + npcName;
		string text = Path.Combine(Path.Combine(Application.persistentDataPath, "ESSaveData"), path);
		try
		{
			if (File.Exists(text))
			{
				File.Delete(text);
				Debug.Log("[SIMPLAYER] Deleted save data for " + npcName + " at " + text + " during RENAME PROCESS");
			}
			else
			{
				Debug.LogWarning("[SIMPLAYER] Tried to delete " + npcName + " but no file found at " + text);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[SIMPLAYER] Error deleting " + npcName + ": " + ex.Message);
		}
	}

	public static void SaveSimData(SimPlayerSaveData _data)
	{
		if (_data == null || string.IsNullOrEmpty(_data.NPCName))
		{
			return;
		}
		string text = "Sims" + _data.NPCName;
		string text2 = Path.Combine(Application.persistentDataPath, "ESSaveData");
		string text3 = Path.Combine(text2, text);
		string text4 = text3 + ".tmp";
		string destinationBackupFileName = Path.Combine(Application.persistentDataPath, "backups", text + ".bak");
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "backups")))
		{
			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "backups"));
		}
		string value = JsonUtility.ToJson(_data, prettyPrint: true);
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		try
		{
			using (FileStream fileStream = new FileStream(text4, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(value);
				streamWriter.Flush();
				fileStream.Flush(flushToDisk: true);
			}
			if (File.Exists(text3))
			{
				File.Replace(text4, text3, destinationBackupFileName);
			}
			else
			{
				File.Move(text4, text3);
			}
		}
		catch
		{
			Debug.Log("SimPlayer save failed: " + _data.NPCName);
		}
	}

	public static SimPlayerSaveData GetMyData(string _name)
	{
		string path = Application.persistentDataPath + "Sims" + _name;
		string path2 = Application.persistentDataPath + "\\ESSaveData\\Sims" + _name;
		if (!File.Exists(path2))
		{
			if (File.Exists(path))
			{
				SimPlayerSaveData simPlayerSaveData = JsonUtility.FromJson<SimPlayerSaveData>(File.ReadAllText(path));
				SimPlayerData.Add(simPlayerSaveData);
				return simPlayerSaveData;
			}
			UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Data Found... SimPlayer reset to default: " + _name, "yellow");
			return null;
		}
		if (File.Exists(path2))
		{
			SimPlayerSaveData simPlayerSaveData2 = JsonUtility.FromJson<SimPlayerSaveData>(File.ReadAllText(path2));
			SimPlayerData.Add(simPlayerSaveData2);
			return simPlayerSaveData2;
		}
		UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Data Found... SimPlayer reset to default: " + _name, "yellow");
		return null;
	}

	public static SimPlayerSaveData CheckLoadData(string _name, List<Item> _inv, int _level, float _skill)
	{
		string text = Path.Combine(Application.persistentDataPath, "Sims" + _name);
		string text2 = Path.Combine(Application.persistentDataPath, "ESSaveData", "Sims" + _name);
		string text3 = null;
		if (File.Exists(text2))
		{
			text3 = text2;
		}
		else if (File.Exists(text))
		{
			text3 = text;
		}
		if (!string.IsNullOrEmpty(text3))
		{
			try
			{
				using FileStream stream = new FileStream(text3, FileMode.Open, FileAccess.Read, FileShare.Read);
				using StreamReader streamReader = new StreamReader(stream);
				string text4 = streamReader.ReadToEnd();
				if (!string.IsNullOrWhiteSpace(text4))
				{
					SimPlayerSaveData simPlayerSaveData = JsonUtility.FromJson<SimPlayerSaveData>(text4);
					SimPlayerData.Add(simPlayerSaveData);
					return simPlayerSaveData;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[SIMPLAYER] Failed to load data for " + _name + ": " + ex.Message);
			}
		}
		SimPlayerSaveData simPlayerSaveData2 = new SimPlayerSaveData(_name, _level, _inv, _skill)
		{
			Year = DateTime.Now.Year - 1,
			Day = DateTime.Now.Day,
			Hour = 1,
			Min = 1,
			FriendedBy = -1
		};
		SimPlayerData.Add(simPlayerSaveData2);
		SaveSimData(simPlayerSaveData2);
		UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Load Data Found - Creating new data for: " + _name, "yellow");
		return simPlayerSaveData2;
	}

	public static SimPlayerSaveData LoadFromFile(string fullPath)
	{
		try
		{
			SimPlayerSaveData simPlayerSaveData = JsonUtility.FromJson<SimPlayerSaveData>(File.ReadAllText(fullPath));
			if (simPlayerSaveData == null)
			{
				throw new Exception("FromJson returned null (malformed JSON?)");
			}
			SimPlayerData.Add(simPlayerSaveData);
			SaveSimData(simPlayerSaveData);
			return simPlayerSaveData;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[SIMPLAYER] Failed to load '" + fullPath + "': " + ex.Message);
			return null;
		}
	}

	public static SimPlayerSaveData FindMyDataInList(string _name)
	{
		foreach (SimPlayerSaveData simPlayerDatum in SimPlayerData)
		{
			if (simPlayerDatum.NPCName == _name)
			{
				return simPlayerDatum;
			}
		}
		return null;
	}

	public static SimPlayerSaveData FixFriendData(string _name)
	{
		string path = Application.persistentDataPath + "Sims" + _name;
		string path2 = Application.persistentDataPath + "\\ESSaveData\\Sims" + _name;
		string.IsNullOrEmpty(GameData.LoadFromBackup);
		if (!File.Exists(path2))
		{
			if (File.Exists(path))
			{
				return JsonUtility.FromJson<SimPlayerSaveData>(File.ReadAllText(path));
			}
			UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Load Data found for: " + _name + ", skipping SimPlayer", "yellow");
			return null;
		}
		if (File.Exists(path2))
		{
			string text = File.ReadAllText(path2);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					return JsonUtility.FromJson<SimPlayerSaveData>(text);
				}
				catch (Exception)
				{
					UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Load Data found for: " + _name + ", skipping SimPlayer", "yellow");
					return null;
				}
			}
			UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Load Data found for: " + _name + ", skipping SimPlayer", "yellow");
			return null;
		}
		UpdateSocialLog.LogAdd("[SIMPLAYER] -> No Load Data found for: " + _name + ", skipping SimPlayer", "yellow");
		return null;
	}

	public static void SaveFriendFix(SimPlayerSaveData _data)
	{
		_data.FriendedBy = -1;
		SaveSimData(_data);
	}
}
