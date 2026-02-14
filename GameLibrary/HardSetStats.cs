// HardSetStats
using System.Collections.Generic;
using UnityEngine;

public class HardSetStats : MonoBehaviour
{
	private Stats myStats;

	public List<SpawnPoint> MyWards;

	public int amplitude;

	public int BaseLevel;

	public int BaseAtk;

	public int BaseHP;

	public int ModLevelBy;

	public int ModAtkBy;

	public int ModHPBy;

	private bool IndicatedVulnerable;

	public string IndicateVuln;

	public string IndicateProt;

	private void Start()
	{
		myStats = GetComponent<Stats>();
		if (!(myStats.Myself.MyNPC.GetSpawnPoint() != null) || myStats.Myself.MyNPC.GetSpawnPoint().EssentailSpawnPoints.Count <= 0)
		{
			return;
		}
		foreach (SpawnPoint essentailSpawnPoint in myStats.Myself.MyNPC.GetSpawnPoint().EssentailSpawnPoints)
		{
			MyWards.Add(essentailSpawnPoint);
		}
	}

	private void Update()
	{
		if (myStats.CurrentHP <= 0)
		{
			return;
		}
		int num = 0;
		foreach (SpawnPoint myWard in MyWards)
		{
			if (myWard.MyNPCAlive)
			{
				num++;
			}
		}
		if (num == 0)
		{
			amplitude = 0;
			if (!IndicatedVulnerable)
			{
				UpdateSocialLog.LogAdd(IndicateVuln, "#FF9000");
				IndicatedVulnerable = true;
			}
		}
		else if (IndicatedVulnerable)
		{
			IndicatedVulnerable = false;
			UpdateSocialLog.LogAdd(IndicateProt, "#FF9000");
		}
		amplitude = num;
		myStats.CurrentMaxHP = BaseHP + ModHPBy * amplitude;
		if (myStats.Myself.MyNPC.CurrentAggroTarget == null)
		{
			myStats.CurrentHP = myStats.CurrentMaxHP;
		}
		myStats.Myself.MyNPC.DamageRange.x = 900 + ModAtkBy * amplitude;
		myStats.Myself.MyNPC.DamageRange.y = 1500 + ModAtkBy * amplitude;
		myStats.Level = BaseLevel + ModLevelBy * amplitude;
	}
}
