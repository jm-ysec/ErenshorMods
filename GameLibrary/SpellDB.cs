// SpellDB
using System.Linq;
using UnityEngine;

public class SpellDB : MonoBehaviour
{
	public Spell[] SpellDatabase;

	private void Start()
	{
		SpellDatabase = Resources.LoadAll("Spells", typeof(Spell)).Cast<Spell>().ToArray();
		GameData.SpellDatabase = this;
	}

	public Spell GetSpellByID(string _ID)
	{
		Spell[] spellDatabase = SpellDatabase;
		foreach (Spell spell in spellDatabase)
		{
			if (spell.Id == _ID)
			{
				return spell;
			}
		}
		return null;
	}
}
