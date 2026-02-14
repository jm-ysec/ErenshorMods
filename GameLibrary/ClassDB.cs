// ClassDB
using UnityEngine;

public class ClassDB : MonoBehaviour
{
	public Class Arcanist;

	public Class Paladin;

	public Class Duelist;

	public Class Druid;

	public Class Default;

	public Class Stormcaller;

	public Class Reaver;

	private void Awake()
	{
		GameData.ClassDB = this;
	}
}
