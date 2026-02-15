// SimPlayerIndependentGroup
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimPlayerIndependentGroup
{
	public SimPlayerTracking Lead;

	public List<SimPlayerTracking> Members = new List<SimPlayerTracking>();

	public int groupsize;

	public int LevelOfGroup;

	public SimPlayerTracking tank;

	public SimPlayerTracking CC;

	public SimPlayerTracking Heals;

	public bool OnlyForGodTarg;

	public SimPlayerIndependentGroup()
	{
		groupsize = 0;
		Lead = null;
		Members.Clear();
	}

	public bool AddPartyMember(SimPlayerTracking _member)
	{
		if (_member == null)
		{
			return false;
		}
		if (groupsize == 0)
		{
			Lead = _member;
			if (tank == null)
			{
				tank = _member;
			}
		}
		if (_member.ClassName == "Paladin" || _member.ClassName == "Reaver")
		{
			Lead = _member;
			if (tank == null)
			{
				tank = _member;
			}
		}
		if (_member.ClassName == "Arcanist")
		{
			CC = _member;
		}
		if (_member.ClassName == "Druid")
		{
			Heals = _member;
		}
		groupsize++;
		Members.Add(_member);
		return false;
	}

	public void RemovePartyMember(SimPlayerTracking _sim)
	{
		if (Members.Contains(_sim))
		{
			Members.Remove(_sim);
		}
		if (Members.Count <= 0)
		{
			GameData.SimMngr.LiveGroups.Remove(this);
		}
		else if (Lead.SimName == _sim.SimName)
		{
			Lead = Members[UnityEngine.Random.Range(0, Members.Count)];
		}
	}

	public bool DoesGroupContain(SimPlayerTracking _query)
	{
		foreach (SimPlayerTracking member in Members)
		{
			if (member.SimName == _query.SimName)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsGroupInCombat()
	{
		foreach (SimPlayerTracking member in Members)
		{
			if (member.MyAvatar != null && member.MyAvatar.GetComponent<NPC>() != null && member.MyAvatar.GetComponent<NPC>().CurrentAggroTarget != null)
			{
				return true;
			}
		}
		return false;
	}

	public int GroupMembersAlive()
	{
		int num = 0;
		foreach (SimPlayerTracking member in Members)
		{
			if (member.MyAvatar != null && member.MyAvatar.MyStats != null && member.MyAvatar.MyStats.CurrentHP > 0)
			{
				num++;
			}
		}
		return num;
	}

	public void CallForAssist(Character _currentAggroTarg)
	{
		if (Members == null || Members.Count == 0)
		{
			return;
		}
		foreach (SimPlayerTracking member in Members)
		{
			NPC nPC = member?.MyAvatar?.GetThisNPC();
			if ((bool)nPC)
			{
				nPC.CurrentAggroTarget = _currentAggroTarg;
			}
		}
	}

	public float CheckDistanceBetweenAll(Transform _leadPos)
	{
		float num = 0f;
		foreach (SimPlayerTracking member in Members)
		{
			if (member.MyAvatar != null && member.MyAvatar.MyStats.CurrentHP > 0)
			{
				float num2 = Vector3.Distance(_leadPos.position, member.MyAvatar.transform.position);
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public bool DoesGroupNeedRest()
	{
		if (Members == null)
		{
			return false;
		}
		if (Members.Count == 0)
		{
			return false;
		}
		foreach (SimPlayerTracking member in Members)
		{
			if (!(member.MyAvatar == null) && !(member.MyAvatar.MyStats == null))
			{
				if ((float)member.MyAvatar.MyStats.CurrentHP < (float)member.MyAvatar.MyStats.CurrentMaxHP * 0.6f)
				{
					return true;
				}
				if ((float)member.MyAvatar.MyStats.CurrentMana < (float)member.MyAvatar.MyStats.GetCurrentMaxMana() * 0.4f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool AmILeader(SimPlayerTracking sp)
	{
		if (Lead.SimName == sp.SimName)
		{
			return true;
		}
		return false;
	}

	public void DisperseGroup()
	{
		Lead = null;
		Members.Clear();
		GameData.SimMngr.LiveGroups.Remove(this);
	}
}
