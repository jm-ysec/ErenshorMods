// SkillDB
using System.Linq;
using UnityEngine;

public class SkillDB : MonoBehaviour
{
	public Skill[] SkillDatabase;

	public Ascension[] AscensionDatabase;

	public Stance NormalStance;

	public Stance AggressiveStance;

	public Stance RecklessStance;

	public Stance TauntingStance;

	public Stance DefensiveStance;

	private void Start()
	{
		SkillDatabase = Resources.LoadAll("Skills", typeof(Skill)).Cast<Skill>().ToArray();
		AscensionDatabase = Resources.LoadAll("Ascensions", typeof(Ascension)).Cast<Ascension>().ToArray();
		GameData.SkillDatabase = this;
	}

	public Skill GetSkillByID(string _ID)
	{
		Skill[] skillDatabase = SkillDatabase;
		foreach (Skill skill in skillDatabase)
		{
			if (skill.Id == _ID)
			{
				return skill;
			}
		}
		return null;
	}

	public Skill GetSkillByName(string _name)
	{
		Skill[] skillDatabase = SkillDatabase;
		foreach (Skill skill in skillDatabase)
		{
			if (skill.SkillName == _name)
			{
				return skill;
			}
		}
		return null;
	}

	public Ascension GetAscensionByID(string _ID)
	{
		Ascension[] ascensionDatabase = AscensionDatabase;
		foreach (Ascension ascension in ascensionDatabase)
		{
			if (ascension.Id == _ID)
			{
				return ascension;
			}
		}
		return null;
	}

	public Ascension GetAscensionByName(string _name)
	{
		Ascension[] ascensionDatabase = AscensionDatabase;
		foreach (Ascension ascension in ascensionDatabase)
		{
			if (ascension.SkillName == _name)
			{
				return ascension;
			}
		}
		return null;
	}
}
