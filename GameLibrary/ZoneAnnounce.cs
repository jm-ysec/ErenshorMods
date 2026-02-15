// ZoneAnnounce
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneAnnounce : MonoBehaviour
{
	public string ZoneName;

	public bool isDungeon;

	private float saveDel = 10f;

	public List<string> ZoneComments;

	public bool limitCam;

	public int camLimit = 3000;

	public bool CheckID = true;

	public string Achievement;

	public string CompleteQuestOnEnter;

	public string CompleteSecondQuestOnEnter;

	public string AssignQuestOnEnter;

	public int MobsKilledByPlayerParty;

	private float SivakSpectreDly = 10f;

	private void Awake()
	{
		NPCTable.LiveNPCs.Clear();
		GameData.LiveGodTargetPOI = null;
	}

	private void Start()
	{
		if (GameData.Misc.compass.alpha < 1f)
		{
			GameData.Misc.compass.alpha = 1f;
		}
		SivakSpectreDly = 10f;
		GameData.ZoneAnnounce = base.transform;
		GameData.ZoneAnnounceData = this;
		if (GameData.StatUpdater != null)
		{
			GameData.StatUpdater.ForceXPBarUpdate();
		}
		if (CheckID)
		{
			string text = "";
			if (GameData.UsingSteam)
			{
				text = SteamUtils.GetAppID().ToString();
			}
			if (text == "2522260")
			{
				Application.Quit();
			}
		}
		GameData.Misc.ZoneName.text = ZoneName;
		if (!string.IsNullOrEmpty(Achievement))
		{
			SetAchievement.Unlock(Achievement);
		}
		if (!string.IsNullOrEmpty(CompleteQuestOnEnter))
		{
			GameData.FinishQuest(CompleteQuestOnEnter);
		}
		if (!string.IsNullOrEmpty(CompleteSecondQuestOnEnter))
		{
			GameData.FinishQuest(CompleteSecondQuestOnEnter);
		}
		if (!string.IsNullOrEmpty(AssignQuestOnEnter))
		{
			GameData.AssignQuest(AssignQuestOnEnter);
		}
		TreasureHunting component = GameData.GM.GetComponent<TreasureHunting>();
		if (!string.IsNullOrEmpty(component.TreasureZone) && component.TreasureZone == SceneManager.GetActiveScene().name && component.TreasureLoc == Vector3.zero)
		{
			component.SetTreasureLoc();
		}
		GameData.CurrentCamLimit = camLimit;
		if (limitCam)
		{
			GameData.CamControl.GetCam().m_Lens.FarClipPlane = camLimit;
		}
		else
		{
			GameData.CamControl.GetCam().m_Lens.FarClipPlane = 3000f;
		}
		if (GameData.CamDrawDistance < GameData.CamControl.GetCam().m_Lens.FarClipPlane)
		{
			GameData.CamControl.GetCam().m_Lens.FarClipPlane = GameData.CamDrawDistance;
		}
		GameData.Atmos.transform.eulerAngles = new Vector3(GameData.Atmos.transform.eulerAngles.x, base.transform.eulerAngles.y, GameData.Atmos.transform.eulerAngles.z);
		GameData.PlayerControl.HuntingMe.Clear();
		saveDel = 30f;
		UpdateSocialLog.LogAdd("You have entered " + ZoneName + " at " + GameData.Time.GetTime());
		if (GameData.PlayerStats.TotalAvailableProficiencies > 0)
		{
			UpdateSocialLog.LogAdd("You have unspent PROFICIENCY POINTS! Open your inventory to assign them.", "yellow");
		}
		GameData.SceneName = ZoneName;
		GameData.InDungeon = isDungeon;
		GameData.PlayerControl.ToggleSwimming(_swim: false);
		GameData.SimMngr.SpawnSimsInZone();
		_ = GameData.ensureSafeLanding;
		if (Vector3.Distance(GameData.PlayerControl.transform.position, GameData.ensureSafeLanding) > 2f)
		{
			GameData.PlayerControl.transform.position = GameData.ensureSafeLanding;
		}
		GameData.SimMngr.BringPlayerGroupToZone();
		GameData.SceneChange.EnsureLanding(SceneManager.GetActiveScene().name, GameData.SceneChange.GetLandingPos());
		CorpseDataManager.SpawnAllCorpses();
		if (GameData.usingSun)
		{
			GameData.Atmos.ForceColors();
		}
		GameData.CurrentZoneAnnounce = this;
		GameData.SimPlayerGrouping.GroupTargets.Clear();
		GameData.AttackingPlayer.Clear();
		GameData.GM.ShowZoneName(ZoneName);
		AuctionHouse.CheckAHBalance(GameData.PlayerStats.MyName);
		GameData.PlayerControl.Autorun = false;
		PointOfInterest pointOfInterest = null;
		if (POI.POIs.Count > 0)
		{
			foreach (PointOfInterest pOI in POI.POIs)
			{
				if (pOI.Use == PointOfInterest.POIType.zoneIn)
				{
					float num = Vector3.Distance(pOI.transform.position, GameData.PlayerControl.transform.position);
					if (pointOfInterest == null || num < Vector3.Distance(pointOfInterest.transform.position, GameData.PlayerControl.transform.position))
					{
						pointOfInterest = pOI;
					}
				}
			}
			if (pointOfInterest != null)
			{
				GameData.NearestZoneIn = pointOfInterest;
			}
		}
		if (GameData.GM.RocFestMode && !GameData.GM.DemoInit)
		{
			GameData.GM.GetComponent<DemonstrationMngrROC>().SetGroupGear();
			GameData.SimPlayerGrouping.SetRoles();
			GameData.SimPlayerGrouping.UpdateGroupWindow();
			GameData.GM.DemoInit = true;
		}
		if (PlayerPrefs.GetInt("ForceV02UIReset", 0) == 0)
		{
			foreach (DragUI allUIElement in GameData.AllUIElements)
			{
				if (allUIElement != null)
				{
					allUIElement.Restore();
				}
			}
			UpdateSocialLog.LogAdd("Erenshor v0.2 introduces UI changes that require a UI reset upon launch... DONE", "yellow");
			UpdateSocialLog.LogAdd("Windows can be re-arranged freely.", "yellow");
			PlayerPrefs.SetInt("ForceV02UIReset", 1);
		}
		GameData.CamControl.RMBClickLoc = Vector3.zero;
		GameData.CamControl.lockTime = 0f;
		GameData.CamControl.freeClick = false;
		GameData.CamControl.usedMouseLook = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		if (SivakSpectreDly > 0f)
		{
			SivakSpectreDly -= 60f * Time.deltaTime;
			if (SivakSpectreDly < 0f && GameData.PlayerInv.HasCosmetic(GameData.PlayerInv.WatchersLens) && SceneManager.GetActiveScene().name != "Azure" && SceneManager.GetActiveScene().name != "Stowaway" && SceneManager.GetActiveScene().name != "Tutorial" && SceneManager.GetActiveScene().name != "SummerEvent" && SceneManager.GetActiveScene().name != "ShiveringStep" && SceneManager.GetActiveScene().name != "ShiveringTomb" && SceneManager.GetActiveScene().name != "ShiveringTomb2" && !GameData.SivakayanSpawnedZones.Contains(SceneManager.GetActiveScene().name))
			{
				SpawnSivakayanSpecter();
			}
		}
		if (!(saveDel > 0f))
		{
			return;
		}
		saveDel -= 60f * Time.deltaTime;
		if (saveDel <= 0f)
		{
			if (!GameData.InCharSelect)
			{
				GameData.GM.SaveGameData(wScene: true);
			}
			if (GameData.usingSun)
			{
				GameData.Atmos.ForceColors();
			}
		}
	}

	public void SpawnSivakayanSpecter()
	{
		if (Random.Range(0, 10) < 9)
		{
			return;
		}
		if (SpawnPointManager.SpawnPointsInScene.Count > 0)
		{
			SpawnPoint spawnPoint = SpawnPointManager.SpawnPointsInScene[Random.Range(0, SpawnPointManager.SpawnPointsInScene.Count)];
			GameObject gameObject = Object.Instantiate(GameData.Misc.SivakayanSpectres[Random.Range(0, GameData.Misc.SivakayanSpectres.Count)], spawnPoint.transform.position, spawnPoint.transform.rotation);
			List<Transform> list = new List<Transform>();
			foreach (SpawnPoint item in SpawnPointManager.SpawnPointsInScene)
			{
				if (item != null && item.transform != null)
				{
					list.Add(item.transform);
				}
			}
			gameObject.GetComponent<NPC>().InitNewNPC(list);
			Debug.Log("Spectre spawned in " + SceneManager.GetActiveScene().name);
			GameData.AddSivakayanSpawn(SceneManager.GetActiveScene().name);
		}
		else
		{
			Debug.Log("No valid spawn found for spectre - other criteria achieved.");
		}
	}
}
