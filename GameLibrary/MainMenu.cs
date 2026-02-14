// MainMenu
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Image Blackout;

	private Color32 Black;

	public Image Logo;

	private bool GameOn;

	public TextMeshProUGUI Loading;

	public TextMeshProUGUI flav;

	public GameObject OptionsMenu;

	private float funDelay = 360f;

	private float flDly = 35f;

	public List<string> Flavor;

	private int flIndex;

	private int colBkp;

	public InputField NameBox;

	public GameObject Credits;

	private float mouseX;

	private float mouseY;

	private float maxMouse;

	public GameObject ResetWindow;

	public TMP_Dropdown BackupSelector;

	public GameObject BackupConfirm;

	public GameObject ServerMenu;

	public Toggle Minimap;

	public Toggle QuestMarkers;

	public Slider HPMod;

	public Slider XPMod;

	public Slider LootMod;

	public Slider DmgMod;

	public Toggle TargetLevel;

	public Toggle XPPenalty;

	public Slider GlobalRunSpeed;

	public Slider ServerPop;

	public Toggle Jail;

	public Toggle Flee;

	public Toggle GamepadToggle;

	public GameObject FirstTime;

	public Button LoginButton;

	public PostProcessProfile PPFX;

	public CamGetPPFXProfile PPFXGetter;

	public Slider RespawnTime;

	public Font FontAwesome;

	private void Awake()
	{
		DebugManager.instance.enableRuntimeUI = false;
		GameData.LoadFromBackup = null;
		if (GameData.DDOLOBJS.Count > 0)
		{
			for (int num = GameData.DDOLOBJS.Count - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(GameData.DDOLOBJS[num]);
			}
			GameData.DDOLOBJS.Clear();
		}
	}

	public void ChangeContrast()
	{
		float @float = PlayerPrefs.GetFloat("CONTRAST", 30f);
		if (PPFX.TryGetSettings<ColorGrading>(out var outSetting))
		{
			outSetting.contrast.value = @float;
		}
	}

	public void ChangeSat()
	{
		float @float = PlayerPrefs.GetFloat("SATURATION", 82f);
		if (PPFX.TryGetSettings<ColorGrading>(out var outSetting))
		{
			outSetting.saturation.value = @float;
		}
	}

	public void ChangeTemp()
	{
		float @float = PlayerPrefs.GetFloat("COLORTEMP", 9f);
		if (PPFX.TryGetSettings<ColorGrading>(out var outSetting))
		{
			outSetting.temperature.value = @float;
		}
	}

	public void ChangeBloom()
	{
		float @float = PlayerPrefs.GetFloat("BLOOM", 1f);
		if (PPFX.TryGetSettings<Bloom>(out var outSetting))
		{
			outSetting.intensity.value = @float;
		}
	}

	public void ChangeDiffusion()
	{
		float @float = PlayerPrefs.GetFloat("DIFFUSION", 5f);
		if (PPFX.TryGetSettings<Bloom>(out var outSetting))
		{
			outSetting.diffusion.value = @float;
		}
	}

	private void Start()
	{
		PPFX = GameData.CamGetPPFX.GetLivePPFX();
		InputManager.LoadScheme();
		ApplyOptions.ApplyAll();
		GetComponent<PostProcessVolume>().profile = GameData.CamGetPPFX.GetLivePPFX();
		ChangeContrast();
		ChangeBloom();
		ChangeSat();
		ChangeTemp();
		ChangeDiffusion();
		Black = new Color32(0, 0, 0, 0);
		GameData.ServerXPMod = PlayerPrefs.GetFloat("SERVERXP", 1f);
		GameData.ServerHPMod = PlayerPrefs.GetFloat("SERVERHP", 1f);
		GameData.ServerDMGMod = PlayerPrefs.GetFloat("SERVERDMG", 1f);
		GameData.ServerLootRate = PlayerPrefs.GetFloat("SERVERLOOT", 1f);
		GameData.RunSpeedMod = PlayerPrefs.GetFloat("RUNSPEEDMOD", 1f);
		GameData.ServerPop = PlayerPrefs.GetInt("SERVERPOP", 140);
		if (PlayerPrefs.GetInt("MINIMAP", 0) == 1)
		{
			GameData.UseMap = true;
		}
		else
		{
			GameData.UseMap = false;
		}
		if (PlayerPrefs.GetInt("QUESTMARKERS", 0) == 1)
		{
			GameData.UseMarkers = true;
		}
		else
		{
			GameData.UseMarkers = false;
		}
		if (PlayerPrefs.GetInt("TARGLEVEL", 0) == 1)
		{
			GameData.ShowTargetLevel = true;
		}
		else
		{
			GameData.ShowTargetLevel = false;
		}
		if (PlayerPrefs.GetInt("XPPENALTY", 0) == 1)
		{
			GameData.XPLossOnDeath = true;
		}
		else
		{
			GameData.XPLossOnDeath = false;
		}
		if (PlayerPrefs.GetInt("JAIL", 1) == 0)
		{
			GameData.Jail = false;
		}
		else
		{
			GameData.Jail = true;
		}
		if (PlayerPrefs.GetInt("FLEE", 1) == 0)
		{
			GameData.NPCFlee = false;
		}
		else
		{
			GameData.NPCFlee = true;
		}
		GameData.RespawnTimeMod = PlayerPrefs.GetFloat("RESPAWNMOD", 1f);
		if (GameData.UsingSteam)
		{
			NameBox.text = SteamFriends.GetPersonaName();
		}
		else
		{
			NameBox.text = "Player";
		}
		if (PlayerPrefs.GetInt("FIRSTTIME", 0) == 0)
		{
			FirstTime.SetActive(value: true);
			LoginButton.interactable = false;
			PlayerPrefs.SetInt("V03WARN", 1);
		}
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		GameData.CreateBackupFolders();
		PopulateBackupDropdown();
	}

	public void ContinueClassic()
	{
		LoginButton.interactable = true;
		PlayerPrefs.SetInt("FIRSTTIME", 1);
		FirstTime.SetActive(value: false);
	}

	public void ViewSettings()
	{
		LoginButton.interactable = true;
		PlayerPrefs.SetInt("FIRSTTIME", 1);
		FirstTime.SetActive(value: false);
		ServerMenu.SetActive(value: true);
	}

	private void Update()
	{
		if (GameData.Gamepad)
		{
			if (Input.GetAxis("DPADX") > 0.6f)
			{
				mouseX = 4f;
			}
			else if (Input.GetAxis("DPADX") < -0.6f)
			{
				mouseX = -4f;
			}
			else
			{
				mouseX = 0f;
			}
			if (Input.GetAxis("DPADY") > 0.6f)
			{
				mouseY = 4f;
			}
			else if (Input.GetAxis("DPADY") < -0.6f)
			{
				mouseY = -4f;
			}
			else
			{
				mouseY = 0f;
			}
			if (Mathf.Abs(Input.GetAxis("DPADX")) > 0.4f || Mathf.Abs(Input.GetAxis("DPADY")) > 0.4f)
			{
				Vector2 vector = new Vector2(mouseX, mouseY);
				Vector2 position = Mouse.current.position.ReadValue() + vector;
				Mouse.current.WarpCursorPosition(position);
			}
		}
		if (!GameData.Gamepad)
		{
			return;
		}
		if (Input.GetAxis("Gamepad X") > 0.1f)
		{
			mouseX = maxMouse * Input.GetAxis("Gamepad X") * 222f * Time.deltaTime;
		}
		else if (Input.GetAxis("Gamepad X") < -0.1f)
		{
			mouseX = maxMouse * Input.GetAxis("Gamepad X") * 222f * Time.deltaTime;
		}
		else
		{
			mouseX = 0f;
		}
		if (Input.GetAxis("Gamepad Y") > 0.1f)
		{
			mouseY = maxMouse * (0f - Input.GetAxis("Gamepad Y") * 222f * Time.deltaTime);
		}
		else if (Input.GetAxis("Gamepad Y") < -0.1f)
		{
			mouseY = maxMouse * (0f - Input.GetAxis("Gamepad Y") * 222f * Time.deltaTime);
		}
		else
		{
			mouseY = 0f;
		}
		if (Mathf.Abs(Input.GetAxis("Gamepad X")) > 0.1f || Mathf.Abs(Input.GetAxis("Gamepad Y")) > 0.1f)
		{
			if (maxMouse < 9f)
			{
				maxMouse += 12f * Time.deltaTime;
			}
			Vector2 vector2 = new Vector2(mouseX, mouseY);
			Vector2 position2 = Mouse.current.position.ReadValue() + vector2 * 60f * Time.deltaTime;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Mouse.current.WarpCursorPosition(position2);
		}
		else if (maxMouse != 3f)
		{
			maxMouse = 3f;
		}
	}

	public void ToggleServerMenu()
	{
		ServerMenu.SetActive(!ServerMenu.activeSelf);
		if (GameData.UseMap)
		{
			Minimap.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			Minimap.SetIsOnWithoutNotify(value: false);
		}
		if (GameData.UseMarkers)
		{
			QuestMarkers.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			QuestMarkers.SetIsOnWithoutNotify(value: false);
		}
		if (GameData.ShowTargetLevel)
		{
			TargetLevel.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			TargetLevel.SetIsOnWithoutNotify(value: false);
		}
		if (GameData.XPLossOnDeath)
		{
			XPPenalty.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			XPPenalty.SetIsOnWithoutNotify(value: false);
		}
		if (GameData.Jail)
		{
			Jail.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			Jail.SetIsOnWithoutNotify(value: false);
		}
		if (GameData.NPCFlee)
		{
			Flee.SetIsOnWithoutNotify(value: true);
		}
		else
		{
			Flee.SetIsOnWithoutNotify(value: false);
		}
		ServerPop.SetValueWithoutNotify(GameData.ServerPop);
		HPMod.SetValueWithoutNotify(GameData.ServerHPMod);
		XPMod.SetValueWithoutNotify(GameData.ServerXPMod);
		LootMod.SetValueWithoutNotify(GameData.ServerLootRate);
		DmgMod.SetValueWithoutNotify(GameData.ServerDMGMod);
		GlobalRunSpeed.SetValueWithoutNotify(GameData.RunSpeedMod);
		RespawnTime.SetValueWithoutNotify(GameData.RespawnTimeMod);
	}

	public void AdjustServerSettings()
	{
		GameData.ServerXPMod = XPMod.value;
		PlayerPrefs.SetFloat("SERVERXP", XPMod.value);
		GameData.ServerHPMod = HPMod.value;
		PlayerPrefs.SetFloat("SERVERHP", HPMod.value);
		GameData.ServerLootRate = LootMod.value;
		PlayerPrefs.SetFloat("SERVERLOOT", LootMod.value);
		GameData.ServerDMGMod = DmgMod.value;
		PlayerPrefs.SetFloat("SERVERDMG", DmgMod.value);
		GameData.RunSpeedMod = GlobalRunSpeed.value;
		PlayerPrefs.SetFloat("RUNSPEEDMOD", GlobalRunSpeed.value);
		GameData.RespawnTimeMod = RespawnTime.value;
		PlayerPrefs.SetFloat("RESPAWNMOD", RespawnTime.value);
		GameData.ServerPop = (int)ServerPop.value;
		PlayerPrefs.SetInt("SERVERPOP", (int)ServerPop.value);
		GameData.UseMap = Minimap.isOn;
		if (Minimap.isOn)
		{
			PlayerPrefs.SetInt("MINIMAP", 1);
		}
		else
		{
			PlayerPrefs.SetInt("MINIMAP", 0);
		}
		GameData.UseMarkers = QuestMarkers.isOn;
		if (QuestMarkers.isOn)
		{
			PlayerPrefs.SetInt("QUESTMARKERS", 1);
		}
		else
		{
			PlayerPrefs.SetInt("QUESTMARKERS", 0);
		}
		if (TargetLevel.isOn)
		{
			PlayerPrefs.SetInt("TARGLEVEL", 1);
		}
		else
		{
			PlayerPrefs.SetInt("TARGLEVEL", 0);
		}
		GameData.ShowTargetLevel = TargetLevel.isOn;
		if (XPPenalty.isOn)
		{
			PlayerPrefs.SetInt("XPPENALTY", 1);
		}
		else
		{
			PlayerPrefs.SetInt("XPPENALTY", 0);
		}
		GameData.XPLossOnDeath = XPPenalty.isOn;
		if (Jail.isOn)
		{
			PlayerPrefs.SetInt("JAIL", 1);
		}
		else
		{
			PlayerPrefs.SetInt("JAIL", 0);
		}
		GameData.Jail = Jail.isOn;
		if (Flee.isOn)
		{
			PlayerPrefs.SetInt("FLEE", 1);
		}
		else
		{
			PlayerPrefs.SetInt("FLEE", 0);
		}
		GameData.NPCFlee = Flee.isOn;
	}

	private void FixedUpdate()
	{
		if (!GameOn)
		{
			return;
		}
		if (funDelay < 0f)
		{
			if (Black.a >= 254 || colBkp > 254)
			{
				GameStart();
				return;
			}
			Black.a += 5;
			colBkp += 5;
			Blackout.color = Black;
			return;
		}
		if (funDelay >= 0f)
		{
			funDelay -= 60f * Time.deltaTime;
		}
		flDly -= 60f * Time.deltaTime;
		if (flDly <= 0f && flIndex < 8)
		{
			flIndex++;
			flDly = UnityEngine.Random.Range(35, 45);
			flav.text = Flavor[flIndex];
			funDelay = 60f;
		}
	}

	public void LaunchGame()
	{
		if (!string.IsNullOrEmpty(GameData.LoadFromBackup))
		{
			string[] files = Directory.GetFiles(GameData.LoadFromBackup);
			string path = Path.Combine(Application.persistentDataPath, "ESSaveData");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] array = files;
			foreach (string obj in array)
			{
				string destFileName = string.Concat(str2: Path.GetFileName(obj), str0: Application.persistentDataPath, str1: "\\ESSaveData\\");
				File.Copy(obj, destFileName, overwrite: true);
			}
			OpenCheckWindow();
		}
		else
		{
			Debug.Log("Main files loaded at login - using current save data: " + DateTime.Now);
			GameOn = true;
			if (!Screen.fullScreen)
			{
				GameData.WindowDimensionsThisSession.x = Screen.width;
				GameData.WindowDimensionsThisSession.y = Screen.height;
				PlayerPrefs.SetFloat("WINDOWDIMENSIONX", GameData.WindowDimensionsThisSession.x);
				PlayerPrefs.SetFloat("WINDOWDIMENSIONY", (int)GameData.WindowDimensionsThisSession.y);
			}
			Blackout.transform.SetAsLastSibling();
		}
	}

	public void OpenCheckWindow()
	{
		BackupConfirm.SetActive(!BackupConfirm.activeSelf);
	}

	public void LaunchGameNoCheck()
	{
		if (!string.IsNullOrEmpty(GameData.LoadFromBackup))
		{
			Debug.Log("Files loaded from backup slot " + GameData.LoadFromBackup + " at login: " + DateTime.Now);
		}
		if (BackupConfirm.activeSelf)
		{
			BackupConfirm.SetActive(value: false);
		}
		GameOn = true;
		Blackout.transform.SetAsLastSibling();
	}

	private void OnApplicationQuit()
	{
		if (SceneManager.GetActiveScene().name != "LoadScene" && SceneManager.GetActiveScene().name != "Menu")
		{
			Debug.Log("Player closed app, saving game...");
			GameData.GM.SaveGameData(wScene: true);
		}
		Debug.Log("Quitting game...");
		StopAllCoroutines();
		SteamAPI.Shutdown();
		Debug.Log("SteamAPI shutdown complete");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	private void GameStart()
	{
		SceneManager.LoadScene("LoadScene");
	}

	public void Options()
	{
		OptionsMenu.SetActive(!OptionsMenu.activeSelf);
		OptionsMenu.GetComponent<OptionsMenu>().DoGFX();
	}

	public void OpenDisc()
	{
		Application.OpenURL("https://Discord.gg/DxqZc5Dc9q");
	}

	public void OpenPatreon()
	{
		Application.OpenURL("https://www.patreon.com/BurgeeMedia");
	}

	public void OpenZemore()
	{
		Application.OpenURL("https://zemoredesign.com/");
	}

	public void OpenSteamworks()
	{
		Application.OpenURL("https://steamworks.github.io/");
	}

	public void OpenMagic()
	{
		Application.OpenURL("https://www.rit.edu/magic/");
	}

	public void OpenCloseCredits()
	{
		Credits.SetActive(!Credits.activeSelf);
	}

	public void OpenWishlist()
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/2382520/Erenshor/");
	}

	public void ToggleResetWindow()
	{
		ResetWindow.SetActive(!ResetWindow.activeSelf);
	}

	private void PopulateBackupDropdown()
	{
		BackupSelector.ClearOptions();
		List<string> list = new List<string> { "Load Most Recent Data" };
		for (int i = 1; i <= 10; i++)
		{
			string text = Path.Combine(Application.persistentDataPath, $"Data{i}");
			string text2 = FindMetaFile(text) ?? Path.Combine(text, "metaData.json");
			bool flag = Directory.Exists(text) && text2 != null && DirectoryHasFilesOtherThanMetadataSafe(text);
			try
			{
				flag = Directory.Exists(text) && File.Exists(text2) && DirectoryHasFilesOtherThanMetadataSafe(text);
			}
			catch
			{
				flag = false;
			}
			if (flag)
			{
				DateTime lastWriteTime = File.GetLastWriteTime(text2);
				list.Add($"Slot {i} - {lastWriteTime:ddd dd HH:mm}");
			}
			else
			{
				list.Add($"Slot {i} - No Backup Found");
			}
		}
		BackupSelector.AddOptions(list);
	}

	private string FindMetaFile(string dir)
	{
		if (!Directory.Exists(dir))
		{
			return null;
		}
		foreach (string item in Directory.EnumerateFiles(dir))
		{
			if (string.Equals(Path.GetFileName(item), "metaData.json", StringComparison.OrdinalIgnoreCase))
			{
				return item;
			}
		}
		return null;
	}

	private bool DirectoryHasFilesOtherThanMetadataSafe(string dir)
	{
		if (!Directory.Exists(dir))
		{
			return false;
		}
		foreach (string item in Directory.EnumerateFiles(dir))
		{
			if (!Path.GetFileName(item).Equals("metaData.json", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public bool DirectoryHasFilesOtherThanMetadata(string directoryPath)
	{
		if (!Directory.Exists(directoryPath))
		{
			return false;
		}
		return Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).Any((string file) => !Path.GetFileName(file).Equals("metaData.json", StringComparison.OrdinalIgnoreCase));
	}

	public void SelectBackupData()
	{
		string text = BackupSelector.options[BackupSelector.value].text;
		int value = BackupSelector.value;
		if (value != 0 && (bool)BackupSelector && !text.Contains("No Backup Found"))
		{
			GameData.LoadFromBackup = Application.persistentDataPath + "\\Data" + value + "\\";
		}
		else
		{
			GameData.LoadFromBackup = "";
		}
	}

	public void RecommendedSettings()
	{
		Minimap.isOn = false;
		QuestMarkers.isOn = false;
		XPMod.value = 1f;
		HPMod.value = 1f;
		DmgMod.value = 1f;
		LootMod.value = 1f;
		XPPenalty.isOn = false;
		GlobalRunSpeed.value = 1f;
		TargetLevel.isOn = false;
		RespawnTime.value = 1f;
		Jail.isOn = true;
		Flee.isOn = true;
	}

	public void ChallengeSettings()
	{
		Minimap.isOn = false;
		QuestMarkers.isOn = false;
		XPMod.value = 1f;
		HPMod.value = 2f;
		DmgMod.value = 1.5f;
		LootMod.value = 1f;
		XPPenalty.isOn = true;
		GlobalRunSpeed.value = 1f;
		TargetLevel.isOn = false;
		RespawnTime.value = 0.8f;
		Jail.isOn = true;
		Flee.isOn = true;
	}

	public void CasualSettings()
	{
		Minimap.isOn = true;
		QuestMarkers.isOn = true;
		XPMod.value = 1.5f;
		HPMod.value = 1f;
		DmgMod.value = 1f;
		LootMod.value = 1.5f;
		XPPenalty.isOn = false;
		GlobalRunSpeed.value = 1.1f;
		TargetLevel.isOn = true;
		RespawnTime.value = 2f;
		Jail.isOn = false;
		Flee.isOn = false;
	}

	public void ExtendedSettings()
	{
		Minimap.isOn = true;
		QuestMarkers.isOn = false;
		XPMod.value = 0.5f;
		HPMod.value = 2.5f;
		DmgMod.value = 1f;
		LootMod.value = 0.8f;
		XPPenalty.isOn = true;
		GlobalRunSpeed.value = 0.8f;
		TargetLevel.isOn = true;
		RespawnTime.value = 1f;
		Jail.isOn = true;
		Flee.isOn = true;
	}
}
