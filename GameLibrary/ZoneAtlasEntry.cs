// ZoneAtlasEntry
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Atlas Entry", menuName = "ScriptableObjects/Atlas Entry", order = 5)]
public class ZoneAtlasEntry : BaseScriptableObject
{
	public string ZoneName;

	public int LevelRangeLow;

	public int LevelRangeHigh;

	public int curPop;

	public List<string> NeighboringZones;

	public bool Dungeon;
}
