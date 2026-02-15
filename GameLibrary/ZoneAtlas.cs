// ZoneAtlas
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ZoneAtlas
{
	public static ZoneAtlasEntry[] Atlas;

	private static Dictionary<string, ZoneAtlasEntry> atlasByName;

	public static string FindSuitableSpawnZone(int _myLvl)
	{
		int num = 0;
		string text = "Azure";
		text = Random.Range(0, 8) switch
		{
			0 => "Azure", 
			1 => "Hidden", 
			2 => "Brake", 
			3 => "Vitheo", 
			4 => "Fernalla", 
			5 => "Silkengrass", 
			6 => "Braxonia", 
			7 => "Loomingwood", 
			_ => "Azure", 
		};
		ZoneAtlasEntry zoneAtlasEntry = null;
		do
		{
			zoneAtlasEntry = Atlas[Random.Range(0, Atlas.Length)];
			num++;
		}
		while ((zoneAtlasEntry.LevelRangeLow > _myLvl || zoneAtlasEntry.LevelRangeHigh < _myLvl) && num < 100);
		if (zoneAtlasEntry != null)
		{
			text = zoneAtlasEntry.ZoneName;
		}
		if (Random.Range(0, 100) > 95)
		{
			text = "Azure";
		}
		return text;
	}

	public static void ReportZonePop()
	{
		int num = 0;
		GameData.SimMngr.UpdateZonePopulations();
		ZoneAtlasEntry[] atlas = Atlas;
		foreach (ZoneAtlasEntry zoneAtlasEntry in atlas)
		{
			UpdateSocialLog.LogAdd(zoneAtlasEntry.ZoneName + ": " + zoneAtlasEntry.curPop + " players");
			num += zoneAtlasEntry.curPop;
		}
		UpdateSocialLog.LogAdd(num + " players online.");
	}

	public static string FindNeighboringZone(string _curZone, int _lvl)
	{
		_ = SceneManager.sceneCount;
		string text = "Azure";
		ZoneAtlasEntry[] atlas = Atlas;
		foreach (ZoneAtlasEntry zoneAtlasEntry in atlas)
		{
			if (zoneAtlasEntry.ZoneName == _curZone)
			{
				text = zoneAtlasEntry.NeighboringZones[Random.Range(0, zoneAtlasEntry.NeighboringZones.Count)];
			}
		}
		atlas = Atlas;
		foreach (ZoneAtlasEntry zoneAtlasEntry2 in atlas)
		{
			if (zoneAtlasEntry2.ZoneName == text && (zoneAtlasEntry2.curPop > 35 || zoneAtlasEntry2.LevelRangeLow > _lvl))
			{
				return FindAnyZone(_lvl);
			}
		}
		return text;
	}

	public static string FindAnyZone(int _lvl)
	{
		ZoneAtlasEntry[] atlas = Atlas;
		foreach (ZoneAtlasEntry zoneAtlasEntry in atlas)
		{
			if (zoneAtlasEntry.LevelRangeLow < _lvl && zoneAtlasEntry.curPop < 30)
			{
				return zoneAtlasEntry.ZoneName;
			}
		}
		return "Azure";
	}

	public static ZoneAtlasEntry FindZoneInfo(string _zoneName)
	{
		ZoneAtlasEntry[] atlas = Atlas;
		foreach (ZoneAtlasEntry zoneAtlasEntry in atlas)
		{
			if (zoneAtlasEntry.ZoneName == _zoneName)
			{
				return zoneAtlasEntry;
			}
		}
		return null;
	}

	private static void EnsureLookup()
	{
		if (Atlas == null || (atlasByName != null && atlasByName.Count == Atlas.Length))
		{
			return;
		}
		atlasByName = new Dictionary<string, ZoneAtlasEntry>(Atlas.Length);
		ZoneAtlasEntry[] atlas = Atlas;
		foreach (ZoneAtlasEntry zoneAtlasEntry in atlas)
		{
			if (!(zoneAtlasEntry == null) && !string.IsNullOrEmpty(zoneAtlasEntry.ZoneName))
			{
				atlasByName[zoneAtlasEntry.ZoneName] = zoneAtlasEntry;
			}
		}
	}

	public static void UpdateZonePops()
	{
		EnsureLookup();
		if (atlasByName == null)
		{
			return;
		}
		ZoneAtlasEntry[] atlas = Atlas;
		for (int i = 0; i < atlas.Length; i++)
		{
			atlas[i].curPop = 0;
		}
		foreach (SimPlayerTracking sim in GameData.SimMngr.Sims)
		{
			if (sim != null && !string.IsNullOrEmpty(sim.CurScene) && atlasByName.TryGetValue(sim.CurScene, out var value))
			{
				value.curPop++;
			}
		}
	}
}
