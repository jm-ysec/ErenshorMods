// PlayerControl
using System.Collections.Generic;
using System.Text;
using Cinemachine;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
	public List<NPC> HuntingMe;

	private Transform myTransform;

	public CharacterController myControl;

	private Animator myAnim;

	private float speed = 3f;

	public float actualSpeed;

	private bool moving;

	private bool backing;

	private bool sprinting;

	public bool CanMove = true;

	private bool viewLock;

	public Character CurrentTarget;

	private bool TargetLock;

	public Camera camera;

	public Character Myself;

	public GameObject TargetWindow;

	public Image TargetLifebar;

	public Image TarOfTarLifebar;

	private Vector2 TarHPScale = new Vector2(1f, 20f);

	private Vector2 TarofTarHPScale = new Vector2(1f, 13f);

	public TextMeshProUGUI TargetName;

	public TextMeshProUGUI TarLifePerc;

	public TextMeshProUGUI TarofTarName;

	public TextMeshProUGUI TarLvl;

	public Image ColorIndicatorPar;

	public Image ColorIndicator;

	public GameObject LifeNumBG;

	private readonly StringBuilder _sb = new StringBuilder(64);

	public Vector3 gravityOn = new Vector3(0f, -1f, 0f);

	public float grav = -1f;

	private Vector3 gravityOff = new Vector3(0f, 0f, 0f);

	private Vector3 gravity;

	public Transform Pethome;

	private float rotSpd = 2f;

	private bool isOverUI;

	public float MouseSensitivity = 2f;

	public float GamepadSensitivity = 1f;

	public bool Swimming;

	public bool Surfaced = true;

	private Vector3 NormalizeMovement;

	public AnimatorOverrideController AnimOverride;

	public AnimationClip SwimAhead;

	public AnimationClip SwimIdle;

	public AnimationClip RelaxedIdle;

	public AnimationClip Sprint;

	public AnimationClip Jog;

	public AnimationClip JogArmed;

	public JumpPrevent CheckJump;

	private TargetTracker Targeting;

	public GameObject GroupFunctions;

	public GameObject InvOne;

	public GameObject InvTwo;

	public GameObject InvThree;

	public GameObject InvFour;

	public GameObject GroupIcons;

	public GameObject Dismiss;

	private float jumpCD;

	private float gravMod = 1f;

	public bool DroneMode;

	private Transform NamePlate;

	public bool dev;

	private bool armedRun;

	public bool wiki;

	public bool Patron;

	public bool GameRant;

	public GameObject DroneCam;

	private float rbDT;

	private Vector3 MoveSol;

	public Camera TPV;

	public Camera FPV;

	private bool LTinUse;

	private bool RTinUse;

	private Vector2 MouseVector;

	private float mouseX;

	private float mouseY;

	private float DPADDelay;

	private float maxMouse = 5f;

	public bool usingGamepad;

	private float kb;

	private float gp;

	public GameObject CircleCenter;

	public float LootDelay;

	public TextMeshProUGUI TarDPS;

	public TextMeshProUGUI TarHigh;

	public TextMeshProUGUI TarLow;

	public TextMeshProUGUI TarAvg;

	public bool Autorun;

	private CastSpell MySpellCast;

	private float waterJumpCD;

	private Vector3 MousePos;

	private PlayerCombat MyCombat;

	public float? tarDirY;

	public bool ForceToY;

	public float CursorVisCooldown;

	private float DPSUpdateRate;

	private CinemachineOrbitalTransposer transposer;

	private bool switchpage;

	private float clickCooldown;

	public bool CanRespec;

	public Color DeepRed;

	public Color Red;

	public Color Orange;

	public Color Yellow;

	public Color Even;

	public Color CloseLow;

	public Color EasyLow;

	public Color Green;

	public Color Gray;

	private string nm = "";

	public bool Sitting;

	public string MyGuild;

	public int OffNavAbuse;

	public int OffNavWarnings;

	public float offNavWarnCD;

	private float airTime;

	private bool nameShowsInvis;

	private void Awake()
	{
		GameData.GroupMembers[0] = null;
		GameData.GroupMembers[1] = null;
		GameData.GroupMembers[2] = null;
		GameData.GroupMembers[3] = null;
		GameData.PlayerAud = GetComponent<AudioSource>();
		GameData.PlayerControl = this;
		MySpellCast = GetComponent<CastSpell>();
		myAnim = GetComponent<Animator>();
		AnimOverride = new AnimatorOverrideController(myAnim.runtimeAnimatorController);
		myAnim.runtimeAnimatorController = AnimOverride;
		if (!Myself.isNPC)
		{
			MyCombat = GetComponent<PlayerCombat>();
		}
	}

	public void UpdateGuildName()
	{
		NamePlate.GetComponent<TextMeshPro>().text = GameData.PlayerStats.MyName;
		string text = "";
		text = GameData.GuildManager.GetGuildNameByID(MyGuild);
		if (text != "")
		{
			TextMeshPro component = NamePlate.GetComponent<TextMeshPro>();
			component.text = component.text + "\n<" + text + ">";
		}
		else
		{
			MyGuild = null;
		}
	}

	public void UpdateNamePlate()
	{
		string text = GameData.PlayerStats.MyName;
		if (Myself.MyStats.Invisible)
		{
			text = "(" + text + ")";
		}
		NamePlate.GetComponent<TextMeshPro>().text = text;
		string text2 = "";
		text2 = GameData.GuildManager.GetGuildNameByID(MyGuild);
		if (text2 != "")
		{
			TextMeshPro component = NamePlate.GetComponent<TextMeshPro>();
			component.text = component.text + "\n<" + text2 + ">";
		}
	}

	private void Start()
	{
		transposer = GameData.CamControl.GetCam().GetCinemachineComponent<CinemachineOrbitalTransposer>();
		try
		{
			if (GameData.GM.DevTeam.Contains(SteamFriends.GetPersonaName().ToLower()))
			{
				dev = true;
			}
			if (GameData.GM.WikiTeam.Contains(SteamFriends.GetPersonaName().ToLower()))
			{
				wiki = true;
			}
			if (GameData.GM.Patron.Contains(SteamFriends.GetPersonaName().ToLower()))
			{
				Patron = true;
			}
			if (GameData.GM.GameRant.Contains(SteamFriends.GetPersonaName().ToLower()))
			{
				GameRant = true;
			}
		}
		catch
		{
			Debug.Log("Steamworks not initialized");
		}
		NamePlate = Object.Instantiate(GameData.GM.GetComponent<Misc>().NamePlate, base.transform.position, base.transform.rotation).transform;
		NamePlate.position = base.transform.position + new Vector3(0f, 2.2f, 0f);
		NamePlate.GetComponent<TextMeshPro>().text = GameData.PlayerStats.MyName;
		NamePlate.transform.SetParent(base.transform);
		UpdateGuildName();
		Targeting = GetComponentInChildren<TargetTracker>();
		myControl = GetComponent<CharacterController>();
		myTransform = GetComponent<Transform>();
		Myself = GetComponent<Character>();
		if (Myself.MyStats.MyInv.MH.MyItem.WeaponDmg != 0 && Myself.MyStats.MyInv.MH.MyItem != GameData.PlayerInv.Empty && !armedRun)
		{
			armedRun = false;
		}
		CheckJump = GetComponentInChildren<JumpPrevent>();
		CheckJump.cached = true;
		Myself.MyStats.RunSpeed *= GameData.RunSpeedMod;
		Myself.MyStats.actualRunSpeed *= GameData.RunSpeedMod;
		GetComponent<Animator>().SetFloat("RunSpeedModifier", Mathf.Clamp(GameData.RunSpeedMod, 0.7f, 1.3f));
		speed = Myself.MyStats.actualRunSpeed;
		if (PlayerPrefs.GetInt("MODERNCONTROL", 0) == 0)
		{
			InputManager.Modern = false;
			GameData.CamControl.SetToStandard();
		}
		else
		{
			InputManager.Modern = true;
			GameData.CamControl.SetToModern();
		}
		if (MyGuild != "")
		{
			UpdateSocialLog.LogAdd("Connecting to guild chat for " + GameData.GuildManager.GetGuildNameByID(MyGuild) + "... COMPLETE", "green");
		}
	}

	private void Update()
	{
		if (offNavWarnCD > 0f)
		{
			offNavWarnCD -= 60f * Time.deltaTime;
		}
		if (dev)
		{
			Input.GetKeyDown(KeyCode.PageUp);
		}
		if (CursorVisCooldown > 0f)
		{
			CursorVisCooldown -= Time.deltaTime;
			if (CursorVisCooldown <= 0f)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.None;
				if (GameData.mEventSystem != null && GameData.mEventSystem.currentSelectedGameObject != null)
				{
					GameData.mEventSystem.SetSelectedGameObject(null);
				}
			}
		}
		if (Myself.MyStats.Invisible && !nameShowsInvis)
		{
			UpdateNamePlate();
			nameShowsInvis = true;
		}
		if (!Myself.MyStats.Invisible && nameShowsInvis)
		{
			UpdateNamePlate();
			nameShowsInvis = false;
		}
		if (LootDelay > 0f)
		{
			LootDelay -= 60f * Time.deltaTime;
		}
		if (!GameData.ShowPlayerName)
		{
			if (NamePlate.gameObject.activeSelf)
			{
				NamePlate.gameObject.SetActive(value: false);
			}
		}
		else if (!NamePlate.gameObject.activeSelf)
		{
			NamePlate.gameObject.SetActive(value: true);
		}
		if (Input.GetKeyDown(InputManager.AutoRun) && !GameData.PlayerTyping)
		{
			Autorun = !Autorun;
		}
		if (HuntingMe.Count > 0)
		{
			for (int num = HuntingMe.Count - 1; num >= 0; num--)
			{
				if (HuntingMe[num] == null || !HuntingMe[num].GetChar().Alive || HuntingMe[num].CurrentAggroTarget == null || HuntingMe[num].CurrentAggroTarget.transform.name != "Player")
				{
					HuntingMe.RemoveAt(num);
				}
			}
		}
		if (OffNavAbuse >= 5)
		{
			OffNavAbuse = 0;
			WarnOffNav();
		}
		if (Input.GetKeyDown(InputManager.Map) && !GameData.PlayerTyping && !GameData.UseMap)
		{
			GameData.GM.HKManager.OpenCloseMap();
		}
		Input.GetKey(KeyCode.F10);
		if (InputManager.Gamepad)
		{
			ListenForInputChange.Listen();
			usingGamepad = !ListenForInputChange.KeyboardActive;
			if (usingGamepad && !CircleCenter.activeSelf)
			{
				CircleCenter.SetActive(value: true);
			}
		}
		else if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton6) || Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.JoystickButton8))
		{
			InputManager.Gamepad = true;
			usingGamepad = !ListenForInputChange.KeyboardActive;
			if (usingGamepad && !CircleCenter.activeSelf)
			{
				CircleCenter.SetActive(value: true);
			}
		}
		if ((Myself.MyStats.RecentDmg <= 0f && HuntingMe.Count <= 0) || !Myself.Alive)
		{
			Myself.Relax = true;
		}
		else if (Myself.Alive)
		{
			Myself.Relax = false;
		}
		NormalizeMovement = Vector3.zero;
		UpdateAnimRun();
		if (!GroupFunctions.activeSelf && (GameData.GroupMembers[0] != null || GameData.GroupMembers[1] != null || GameData.GroupMembers[2] != null || GameData.GroupMembers[3] != null))
		{
			if (!GroupFunctions.activeSelf)
			{
				GroupFunctions.SetActive(value: true);
			}
		}
		else if (GameData.GroupMembers[0] == null && GameData.GroupMembers[1] == null && GameData.GroupMembers[2] == null && GameData.GroupMembers[3] == null && GroupFunctions.activeSelf)
		{
			GroupFunctions.SetActive(value: false);
		}
		if (CurrentTarget != null && CurrentTarget.MyNPC != null && CurrentTarget.MyNPC.SimPlayer)
		{
			bool flag = false;
			for (int i = 0; i < 4; i++)
			{
				SimPlayerTracking simPlayerTracking = GameData.GroupMembers[i];
				bool flag2 = simPlayerTracking != null && simPlayerTracking.MyAvatar != null && CurrentTarget == simPlayerTracking.MyAvatar.MyStats.Myself;
				if (simPlayerTracking == null && !flag)
				{
					switch (i)
					{
					case 0:
						if (!InvOne.activeSelf)
						{
							InvOne.SetActive(value: true);
						}
						break;
					case 1:
						if (!InvTwo.activeSelf)
						{
							InvTwo.SetActive(value: true);
						}
						break;
					case 2:
						if (!InvThree.activeSelf)
						{
							InvThree.SetActive(value: true);
						}
						break;
					case 3:
						if (!InvFour.activeSelf)
						{
							InvFour.SetActive(value: true);
						}
						break;
					}
					continue;
				}
				switch (i)
				{
				case 0:
					if (InvOne.activeSelf)
					{
						InvOne.SetActive(value: false);
					}
					break;
				case 1:
					if (InvTwo.activeSelf)
					{
						InvTwo.SetActive(value: false);
					}
					break;
				case 2:
					if (InvThree.activeSelf)
					{
						InvThree.SetActive(value: false);
					}
					break;
				case 3:
					if (InvFour.activeSelf)
					{
						InvFour.SetActive(value: false);
					}
					break;
				}
				if (flag2)
				{
					flag = true;
					if (!Dismiss.activeSelf)
					{
						Dismiss.SetActive(value: true);
					}
				}
			}
		}
		else
		{
			if (InvOne.activeSelf)
			{
				InvOne.SetActive(value: false);
			}
			if (InvTwo.activeSelf)
			{
				InvTwo.SetActive(value: false);
			}
			if (InvThree.activeSelf)
			{
				InvThree.SetActive(value: false);
			}
			if (InvFour.activeSelf)
			{
				InvFour.SetActive(value: false);
			}
			if (Dismiss.activeSelf)
			{
				Dismiss.SetActive(value: false);
			}
		}
		if (GameData.GroupMembers[0] != null || GameData.GroupMembers[1] != null || GameData.GroupMembers[2] != null || GameData.GroupMembers[3] != null || (CurrentTarget != null && CurrentTarget.MyNPC != null && CurrentTarget.MyNPC.SimPlayer))
		{
			if (!GroupIcons.activeSelf)
			{
				GroupIcons.SetActive(value: true);
			}
		}
		else if (GroupIcons.activeSelf)
		{
			GroupIcons.SetActive(value: false);
		}
		if (!DroneMode)
		{
			if (!GameData.PlayerInv.Modulars.enabled)
			{
				GameData.PlayerInv.Modulars.enabled = true;
			}
			if (!Swimming)
			{
				if (!myControl.isGrounded)
				{
					airTime += 60f * Time.deltaTime;
					gravity = new Vector3(0f, grav, 0f);
					grav -= 32f * gravMod * Time.deltaTime;
					if (grav < -7f && !Myself.MyStats.Rooted && airTime > 30f)
					{
						myAnim.SetBool("Falling", value: true);
					}
				}
				else
				{
					if (myAnim.GetBool("Falling"))
					{
						Myself.MyAudio.PlayOneShot(GameData.Misc.JumpLand, Myself.MyAudio.volume * GameData.FootVol * GameData.MasterVol);
					}
					airTime = 0f;
					myAnim.SetBool("Falling", value: false);
					myAnim.SetBool("Jumped", value: false);
					grav = -1f;
					gravity = gravityOff;
				}
				if (!Surfaced)
				{
					ToggleSwimming(_swim: true);
				}
			}
			else
			{
				gravity = new Vector3(0f, grav, 0f);
				if (grav > -3f)
				{
					grav -= 1f * Time.deltaTime;
				}
				if (myControl.isGrounded && Surfaced)
				{
					ToggleSwimming(_swim: false);
				}
			}
			if (Surfaced && Swimming && grav > 0f)
			{
				grav = 0f;
			}
			UpdateMoveStatus();
		}
		if (clickCooldown > 0f)
		{
			clickCooldown -= Time.deltaTime;
		}
		if ((Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1)) || (usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton0) && Input.GetAxis("RTrigger") < 0.1f))
		{
			LeftClick();
			if (InputManager.Modern && (CurrentTarget == null || CurrentTarget.GetComponent<SimPlayer>() == null))
			{
				RightClick();
			}
		}
		if ((!InputManager.Modern && Input.GetMouseButtonUp(1) && rbDT < 25f) || (usingGamepad && Input.GetKeyUp(KeyCode.JoystickButton0) && Input.GetAxis("RTrigger") < 0.1f))
		{
			RightClick();
		}
		if (InputManager.Modern && Input.GetMouseButtonUp(1) && CurrentTarget != null && CurrentTarget.GetComponent<SimPlayer>() != null)
		{
			RightClick();
		}
		if (Input.GetMouseButton(1) && rbDT < 60f)
		{
			rbDT += 60f * Time.deltaTime;
		}
		else if (!Input.GetMouseButton(1))
		{
			rbDT = 0f;
		}
		TargetHotkeys();
		if (usingGamepad && Input.GetAxis("RTrigger") <= 0.5f)
		{
			if (Input.GetKeyDown(KeyCode.JoystickButton0))
			{
				if (GameData.ItemOnCursor != null && GameData.ItemOnCursor.MyItem != GameData.PlayerInv.Empty)
				{
					GameData.ItemOnCursor.InteractItemSlot();
				}
				else if (GameData.HighlightedItem != null && GameData.HighlightedItem.MyItem != GameData.PlayerInv.Empty)
				{
					GameData.HighlightedItem.InteractItemSlot();
				}
			}
			if (NamePlate.gameObject.activeSelf)
			{
				NamePlate.gameObject.SetActive(value: false);
			}
			if (Input.GetAxis("LTrigger") < 0.5f && !Input.GetKey(KeyCode.JoystickButton4) && !Input.GetKey(KeyCode.JoystickButton1))
			{
				if (Input.GetAxis("DPADX") > 0.4f && !Input.GetKey(KeyCode.JoystickButton5))
				{
					if (!switchpage)
					{
						switchpage = true;
						GameData.HKMngr.SwitchPage();
					}
				}
				else
				{
					switchpage = false;
				}
			}
			if (MousePos != Input.mousePosition)
			{
				MousePos = Input.mousePosition;
				DPADDelay = 300f;
				if (CursorVisCooldown <= 0f)
				{
					Cursor.visible = true;
				}
			}
			if (Input.GetAxis("LTrigger") > 0.3f)
			{
				DPADDelay = 180f;
				if (Input.GetAxis("Gamepad X") > 0.1f)
				{
					mouseX = maxMouse * Input.GetAxis("Gamepad X") * 220f * Time.deltaTime;
				}
				else if (Input.GetAxis("Gamepad X") < -0.1f)
				{
					mouseX = maxMouse * Input.GetAxis("Gamepad X") * 220f * Time.deltaTime;
				}
				else
				{
					mouseX = 0f;
				}
				if (Input.GetAxis("Gamepad Y") > 0.1f)
				{
					mouseY = maxMouse * (0f - Input.GetAxis("Gamepad Y") * 220f * Time.deltaTime);
				}
				else if (Input.GetAxis("Gamepad Y") < -0.1f)
				{
					mouseY = maxMouse * (0f - Input.GetAxis("Gamepad Y") * 220f * Time.deltaTime);
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
					Vector2 vector = new Vector2(mouseX, mouseY);
					Vector2 position = Mouse.current.position.ReadValue() + vector * 60f * Time.deltaTime;
					DPADDelay = 300f;
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					Mouse.current.WarpCursorPosition(position);
				}
				else if (!GameData.PlayerInv.InvWindow.activeSelf && maxMouse != 3f)
				{
					maxMouse = 3f;
				}
			}
			else if (!GameData.PlayerInv.InvWindow.activeSelf && maxMouse != 3f)
			{
				maxMouse = 3f;
			}
			if (DPADDelay > 0f && !GameData.PlayerInv.InvWindow.activeSelf)
			{
				DPADDelay -= 60f * Time.deltaTime;
				if (DPADDelay <= 0f)
				{
					Vector2 position2 = new Vector2(Screen.width / 2, Screen.height / 2);
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					Mouse.current.WarpCursorPosition(position2);
					MousePos = Input.mousePosition;
					CursorVisCooldown = 0.05f;
				}
			}
		}
		if (Input.GetKeyDown(InputManager.Cycle) || (usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton8) && Input.GetAxis("RTrigger") < 0.1f))
		{
			Character currentTarget = CurrentTarget;
			CurrentTarget = Targeting.TabTarget(CurrentTarget, CurrentTarget, Input.GetKey(KeyCode.LeftShift));
			if (CurrentTarget != null && CurrentTarget != currentTarget)
			{
				CurrentTarget.TargetMe();
			}
			if (currentTarget != CurrentTarget && currentTarget != null)
			{
				currentTarget.UntargetMe();
			}
		}
		if (Input.GetKeyDown(InputManager.Consider) && !GameData.PlayerTyping)
		{
			ConsiderOpponent(CurrentTarget, usedMouse: false);
		}
		if (usingGamepad)
		{
			Input.GetKeyDown(KeyCode.JoystickButton7);
		}
		if (CurrentTarget != null)
		{
			if (!CurrentTarget.Alive || !CurrentTarget.enabled)
			{
				CurrentTarget.UntargetMe(_attackoff: true);
				CurrentTarget = null;
				TargetWindow.SetActive(value: false);
				TarDPS.text = "-";
				TarAvg.text = "-";
				TarLow.text = "-";
				TarHigh.text = "-";
			}
			else if (!TargetWindow.activeSelf)
			{
				TargetWindow.SetActive(value: true);
			}
			else
			{
				if (DPSUpdateRate > 0f)
				{
					DPSUpdateRate -= 60f * Time.deltaTime;
					if (DPSUpdateRate <= 0f)
					{
						TarDPS.text = CurrentTarget.GetDPS().ToString();
						DPSUpdateRate = 30f;
					}
				}
				TarDPS.text = CurrentTarget.GetDPS().ToString();
				TarAvg.text = CurrentTarget.GetAvgHit().ToString();
				TarLow.text = CurrentTarget.GetLowestHit().ToString();
				TarHigh.text = CurrentTarget.GetHightHit().ToString();
			}
		}
		else if (TargetWindow.activeSelf)
		{
			TargetWindow.SetActive(value: false);
			TarDPS.text = "-";
			TarAvg.text = "-";
			TarLow.text = "-";
			TarHigh.text = "-";
		}
		if (!DroneMode)
		{
			if (CanMove && Myself.Alive && !Myself.MyStats.Rooted && !Myself.MyStats.Stunned && !Myself.MyStats.Feared && LootDelay <= 0f)
			{
				if (!Swimming)
				{
					LandMovement();
				}
				else
				{
					WaterMovement();
				}
				myAnim.SetBool("Backing", backing);
				myAnim.SetBool("Walking", moving);
				myAnim.SetBool("Running", sprinting);
				myAnim.SetBool("Stunned", value: false);
			}
			else if (!CanMove || Myself.MyStats.Stunned || Myself.MyStats.Feared)
			{
				if (actualSpeed > 0f)
				{
					actualSpeed -= 60f * Time.deltaTime;
				}
				if (actualSpeed < 0f)
				{
					actualSpeed = 0f;
				}
				myAnim.SetBool("Backing", value: false);
				myAnim.SetBool("Walking", value: false);
				myAnim.SetBool("Running", value: false);
				if (Myself.MyStats.Stunned)
				{
					myAnim.SetBool("Stunned", value: true);
				}
				else
				{
					myAnim.SetBool("Stunned", value: false);
				}
				if (!myControl.isGrounded)
				{
					gravity = new Vector3(0f, grav, 0f);
					grav -= 16f * gravMod * Time.deltaTime;
					if (grav < -7f && !Myself.MyStats.Rooted)
					{
						myAnim.SetBool("Falling", value: true);
					}
				}
				else
				{
					myAnim.SetBool("Falling", value: false);
					grav = -1f;
					gravity = gravityOff;
				}
				myControl.Move(gravity * Time.deltaTime);
			}
		}
		_ = DroneMode;
		if (Myself.Alive && !Myself.MyStats.Rooted && !Myself.MyStats.Stunned && !Myself.MyStats.Feared && !usingGamepad && !InputManager.Modern)
		{
			MouseLook();
		}
		if (Myself.Alive && usingGamepad)
		{
			GamepadControls();
		}
		if (Myself.Alive && InputManager.Modern)
		{
			ModernStrafe();
		}
		if (jumpCD > 0f)
		{
			jumpCD -= 60f * Time.deltaTime;
		}
		if (Myself.MyStats.Rooted)
		{
			myAnim.SetBool("Falling", value: false);
		}
		if (Myself?.MyCharmedNPC != null && GameData.PetAssistGroupMA)
		{
			CheckPetAssist();
		}
		if (Sitting && (NormalizeMovement != Vector3.zero || Myself.MySpells.isCasting() || Swimming || GameData.Autoattacking || Input.GetKey(InputManager.Jump) || Input.GetKey(InputManager.Left) || Input.GetKey(InputManager.Right) || (Input.GetMouseButton(1) && Input.GetAxis("Mouse X") != 0f)))
		{
			Sitting = false;
			myAnim.SetTrigger("StandUp");
			GameData.PlayerAud.PlayOneShot(GameData.Misc.StandSound, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
		}
		myControl.Move((gravity + NormalizeMovement * actualSpeed) * Time.deltaTime);
	}

	private void CheckPetAssist()
	{
		Character character = (GameData.SimPlayerGrouping.MainAssist?.MyAvatar?.MyStats?.Myself?.MyNPC)?.CurrentAggroTarget;
		if (character != null && Myself?.MyCharmedNPC != null)
		{
			Myself.MyCharmedNPC.CurrentAggroTarget = character;
		}
	}

	private void UpdateTargetWindowData()
	{
		if (CurrentTarget == null)
		{
			TarOfTarLifebar.gameObject.SetActive(value: false);
			TarofTarName.text = string.Empty;
			TarLvl.text = string.Empty;
			TarLifePerc.text = string.Empty;
			LifeNumBG.SetActive(value: false);
			TarofTarName.ForceMeshUpdate();
			ColorIndicatorPar.gameObject.SetActive(value: false);
			return;
		}
		Character currentTarget = CurrentTarget;
		Stats myStats = currentTarget.MyStats;
		if (!LifeNumBG.activeSelf)
		{
			LifeNumBG.SetActive(value: true);
		}
		string text = ((currentTarget.transform.name != "Player") ? currentTarget.transform.name : myStats.MyName);
		TargetName.text = text;
		if (GameData.ShowTargetLevel)
		{
			TarLvl.SetText("Lv. {0}", myStats.Level);
			if (ColorIndicatorPar.gameObject.activeSelf)
			{
				ColorIndicatorPar.gameObject.SetActive(value: false);
			}
		}
		else
		{
			if (TarLvl.text != string.Empty)
			{
				TarLvl.text = string.Empty;
			}
			if (!ColorIndicatorPar.gameObject.activeSelf)
			{
				ColorIndicatorPar.gameObject.SetActive(value: true);
			}
			if (ColorIndicator.color != GetTarColor(currentTarget))
			{
				ColorIndicator.color = GetTarColor(currentTarget);
			}
		}
		int num = ((myStats.CurrentMaxHP > 0) ? Mathf.RoundToInt((float)myStats.CurrentHP / (float)myStats.CurrentMaxHP * 100f) : 0);
		TarLifePerc.SetText("{0}%", num);
		TarHPScale.x = ((myStats.CurrentMaxHP > 0) ? ((float)myStats.CurrentHP / (float)myStats.CurrentMaxHP * 185f) : 0f);
		Vector2 sizeDelta = TargetLifebar.rectTransform.sizeDelta;
		if (sizeDelta.x != TarHPScale.x || sizeDelta.y != TarHPScale.y)
		{
			TargetLifebar.rectTransform.sizeDelta = TarHPScale;
		}
		Character character = ((currentTarget.MyNPC != null) ? currentTarget.MyNPC.CurrentAggroTarget : null);
		if (character != null && character.gameObject.activeInHierarchy && character.Alive && character.MyStats != null && character.MyStats.CurrentHP > 0)
		{
			if (!TarOfTarLifebar.gameObject.activeSelf)
			{
				TarOfTarLifebar.gameObject.SetActive(value: true);
			}
			string text2 = ((character.transform.name != "Player") ? character.transform.name : GameData.PlayerStats.MyName);
			TarofTarName.text = "Targeting: " + text2;
			TarofTarName.ForceMeshUpdate();
			TarofTarHPScale.x = ((character.MyStats.CurrentMaxHP > 0) ? ((float)character.MyStats.CurrentHP / (float)character.MyStats.CurrentMaxHP * 185f) : 0f);
			Vector2 sizeDelta2 = TarOfTarLifebar.rectTransform.sizeDelta;
			if (sizeDelta2.x != TarofTarHPScale.x || sizeDelta2.y != TarofTarHPScale.y)
			{
				TarOfTarLifebar.rectTransform.sizeDelta = TarofTarHPScale;
			}
		}
		else
		{
			TarofTarName.text = "No target";
			TarofTarName.ForceMeshUpdate();
			TarofTarHPScale.x = 0f;
			if (TarOfTarLifebar.gameObject.activeSelf)
			{
				TarOfTarLifebar.gameObject.SetActive(value: false);
			}
			if (TarOfTarLifebar.rectTransform.sizeDelta.x != 0f || TarOfTarLifebar.rectTransform.sizeDelta.y != TarofTarHPScale.y)
			{
				TarOfTarLifebar.rectTransform.sizeDelta = TarofTarHPScale;
			}
		}
	}

	private Color GetTarColor(Character _tar)
	{
		if (_tar == null || _tar.MyStats == null)
		{
			return Color.white;
		}
		int num = _tar.MyStats.Level - GameData.PlayerStats.Level;
		if (num >= 7)
		{
			return GameData.PlayerControl.DeepRed;
		}
		if (num >= 5)
		{
			return GameData.PlayerControl.Red;
		}
		if (num >= 3)
		{
			return GameData.PlayerControl.Orange;
		}
		if (num >= 1)
		{
			return GameData.PlayerControl.Yellow;
		}
		if (num == 0)
		{
			return Color.white;
		}
		if (num >= -2)
		{
			return GameData.PlayerControl.CloseLow;
		}
		if (num >= -4)
		{
			return GameData.PlayerControl.EasyLow;
		}
		return GameData.PlayerControl.Green;
	}

	public void TargetTargetsTarget()
	{
		if (!(CurrentTarget == null) && !(CurrentTarget.MyNPC == null) && !(CurrentTarget.MyNPC.CurrentAggroTarget == null))
		{
			if (CurrentTarget != null)
			{
				CurrentTarget.UntargetMe();
			}
			CurrentTarget = CurrentTarget.MyNPC.CurrentAggroTarget;
			CurrentTarget.TargetMe();
		}
	}

	public void TargetPet()
	{
		if (Myself.MyCharmedNPC != null)
		{
			if (CurrentTarget != null)
			{
				CurrentTarget.UntargetMe();
			}
			CurrentTarget = Myself.MyCharmedNPC.GetComponent<Character>();
			CurrentTarget.TargetMe();
		}
	}

	public bool CheckLOS(Character _visible)
	{
		if (_visible == null)
		{
			return false;
		}
		bool result = true;
		Vector3 direction = _visible.transform.position + Vector3.up - base.transform.position + Vector3.up;
		RaycastHit[] array = Physics.RaycastAll(base.transform.position + Vector3.up, direction, direction.magnitude);
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.transform.GetComponent<Character>() == null)
			{
				result = false;
			}
		}
		return result;
	}

	public void ToggleSwimming(bool _swim)
	{
		if (!(AnimOverride != null))
		{
			return;
		}
		if (_swim)
		{
			AnimOverride["Idle"] = SwimIdle;
			AnimOverride["Unarmed-Sprint"] = SwimAhead;
			AnimOverride["Armed-Run-Forward"] = SwimAhead;
			AnimOverride["Unarmed-Run-Forward"] = SwimAhead;
			gravMod = 0.1f;
			if (grav < 3f)
			{
				grav = -1f;
			}
			myAnim.SetTrigger("LandInWater");
			myAnim.SetBool("Falling", value: false);
			Swimming = true;
		}
		else
		{
			AnimOverride["Idle"] = RelaxedIdle;
			AnimOverride["Unarmed-Sprint"] = Sprint;
			if (Myself.MyStats.MyInv.MH.MyItem.WeaponDmg > 0 && Myself.MyStats.MyInv.MH.MyItem != GameData.PlayerInv.Empty)
			{
				AnimOverride["Armed-Run-Forward"] = JogArmed;
				AnimOverride["Unarmed-Run-Forward"] = JogArmed;
			}
			else
			{
				AnimOverride["Unarmed-Run-Forward"] = Jog;
				AnimOverride["Armed-Run-Forward"] = Jog;
			}
			gravMod = 1f;
			Swimming = false;
		}
	}

	private void TargetHotkeys()
	{
		if (Input.GetKeyDown(InputManager.TargetSelf))
		{
			if (CurrentTarget != null)
			{
				CurrentTarget.UntargetMe();
			}
			CurrentTarget = Myself;
			CurrentTarget.TargetMe();
		}
		if (GameData.GroupMembers[0] != null && (Input.GetKeyDown(InputManager.TargetG1) || (usingGamepad && Input.GetKey(KeyCode.JoystickButton4) && Input.GetKeyDown(KeyCode.JoystickButton2))))
		{
			if (CurrentTarget != GameData.GroupMembers[0].MyStats.Myself)
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				CurrentTarget = GameData.GroupMembers[0].MyStats.Myself;
			}
			else if (GameData.GroupMembers[0].MyStats.Myself.MyNPC.CurrentAggroTarget != null)
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				CurrentTarget = GameData.GroupMembers[0].MyStats.Myself.MyNPC.CurrentAggroTarget;
			}
			CurrentTarget.TargetMe();
		}
		if (GameData.GroupMembers[1] != null && (Input.GetKeyDown(InputManager.TargetG2) || (usingGamepad && Input.GetKey(KeyCode.JoystickButton4) && Input.GetKeyDown(KeyCode.JoystickButton3))))
		{
			if (CurrentTarget != GameData.GroupMembers[1].MyStats.Myself)
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				CurrentTarget = GameData.GroupMembers[1].MyStats.Myself;
			}
			else if (GameData.GroupMembers[1].MyStats.Myself.MyNPC.CurrentAggroTarget != null)
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				CurrentTarget = GameData.GroupMembers[1].MyStats.Myself.MyNPC.CurrentAggroTarget;
			}
			CurrentTarget.TargetMe();
		}
		if (GameData.GroupMembers[2] != null && (Input.GetKeyDown(InputManager.TargetG3) || (usingGamepad && Input.GetKey(KeyCode.JoystickButton4) && Input.GetKeyDown(KeyCode.JoystickButton1))))
		{
			if (CurrentTarget != GameData.GroupMembers[2].MyStats.Myself)
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				CurrentTarget = GameData.GroupMembers[2].MyStats.Myself;
			}
			else if (GameData.GroupMembers[2].MyStats.Myself.MyNPC.CurrentAggroTarget != null)
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				CurrentTarget = GameData.GroupMembers[2].MyStats.Myself.MyNPC.CurrentAggroTarget;
			}
			CurrentTarget.TargetMe();
		}
		if (GameData.GroupMembers[3] == null || !Input.GetKeyDown(InputManager.TargetG4))
		{
			return;
		}
		if (CurrentTarget != GameData.GroupMembers[3].MyStats.Myself)
		{
			if (CurrentTarget != null)
			{
				CurrentTarget.UntargetMe();
			}
			CurrentTarget = GameData.GroupMembers[3].MyStats.Myself;
		}
		else if (GameData.GroupMembers[3].MyStats.Myself.MyNPC.CurrentAggroTarget != null)
		{
			if (CurrentTarget != null)
			{
				CurrentTarget.UntargetMe();
			}
			CurrentTarget = GameData.GroupMembers[3].MyStats.Myself.MyNPC.CurrentAggroTarget;
		}
		CurrentTarget.TargetMe();
	}

	private void UpdateMoveStatus()
	{
		if (GameData.PlayerTyping)
		{
			CanMove = false;
		}
		else
		{
			CanMove = true;
		}
	}

	private void LeftClick()
	{
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		bool flag = EventSystem.current.IsPointerOverGameObject();
		if (!Physics.Raycast(ray, out var hitInfo) || hitInfo.transform.name == "Terrain" || hitInfo.transform.name == "Terrain (1)")
		{
			return;
		}
		if (hitInfo.transform.GetComponent<Character>() != null)
		{
			Character component = hitInfo.transform.GetComponent<Character>();
			if (component.Alive && (!flag || (GameData.ItemOnCursor != null && GameData.ItemOnCursor != GameData.PlayerInv.Empty)))
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				component.TargetMe();
				CurrentTarget = component;
				if (GameData.MouseSlot.MyItem != null)
				{
					List<RaycastResult> list = new List<RaycastResult>();
					bool flag2 = false;
					PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
					pointerEventData.position = Input.mousePosition;
					EventSystem.current.RaycastAll(pointerEventData, list);
					if (list.Count > 0)
					{
						foreach (RaycastResult item in list)
						{
							if (item.gameObject.layer == LayerMask.NameToLayer("UI"))
							{
								flag2 = true;
							}
						}
					}
					if (!flag2)
					{
						if (GameData.MouseSlot.MyItem != GameData.PlayerInv.Empty && Vector3.Distance(base.transform.position, CurrentTarget.transform.position) < 4f && !GameData.Trading && CurrentTarget.isNPC && !CurrentTarget.MyNPC.MiningNode)
						{
							if (CurrentTarget.GetComponent<SimPlayer>() == null && (GameData.MouseSlot.MyItem.RequiredSlot == Item.SlotType.General || GameData.MouseSlot.Quantity <= 1))
							{
								GameData.TradeWindow.LoadWindow(GameData.MouseSlot.MyItem, GameData.MouseSlot.Quantity, CurrentTarget.transform);
								GameData.MouseSlot.SendToTrade();
							}
							else if (CurrentTarget.GetComponent<SimPlayer>() != null)
							{
								if (GameData.MouseSlot.MyItem != null && GameData.MouseSlot.MyItem.NoTradeNoDestroy)
								{
									UpdateSocialLog.LogAdd("This item refuses to leave your grasp, as though it has another, unknown use.", "yellow");
								}
								if (CurrentTarget.GetComponent<SimPlayer>().IsGMCharacter)
								{
									UpdateSocialLog.LogAdd("Cannot trade with Burgee Media Staff", "yellow");
								}
								if (CurrentTarget.GetComponent<SimPlayer>() != null && GameData.SimMngr.FindSimplayerByName(CurrentTarget.transform.name) != null && GameData.SimMngr.FindSimplayerByName(CurrentTarget.transform.name).Rival)
								{
									UpdateSocialLog.LogAdd(CurrentTarget.transform.name + " says: I don't want your garbage. Take it somewhere else.");
								}
								else
								{
									GameData.SimTrade.LoadWindow(GameData.MouseSlot.MyItem, GameData.MouseSlot.Quantity, CurrentTarget.transform);
									GameData.MouseSlot.SendToTrade();
								}
							}
						}
						if (GameData.MouseSlot.MyItem != GameData.PlayerInv.Empty && Vector3.Distance(base.transform.position, CurrentTarget.transform.position) < 4f && GameData.Trading && (GameData.MouseSlot.MyItem.RequiredSlot == Item.SlotType.General || GameData.MouseSlot.Quantity <= 1) && CurrentTarget.GetComponent<SimPlayer>() == null && GameData.TradeWindow.GetTradee() == CurrentTarget.transform && GameData.TradeWindow.FindNextEmpty() != null)
						{
							foreach (ItemIcon lootSlot in GameData.TradeWindow.LootSlots)
							{
								if (lootSlot.MyItem == GameData.PlayerInv.Empty)
								{
									lootSlot.MyItem = GameData.MouseSlot.MyItem;
									lootSlot.Quantity = GameData.MouseSlot.Quantity;
									lootSlot.UpdateSlotImage();
									GameData.MouseSlot.SendToTrade();
									break;
								}
							}
						}
						if (GameData.MouseSlot.MyItem != GameData.PlayerInv.Empty && Vector3.Distance(base.transform.position, CurrentTarget.transform.position) < 4f && GameData.MouseSlot.MyItem.RequiredSlot != 0 && GameData.MouseSlot.Quantity > 1 && CurrentTarget.GetComponent<SimPlayer>() == null)
						{
							UpdateSocialLog.LogAdd(CurrentTarget.transform.name + " says: That item is special... I can't take that. Have someone remove the blessing first.");
						}
					}
				}
			}
		}
		if (hitInfo.transform.GetComponent<Door>() != null && !flag && (Vector3.Distance(base.transform.position, hitInfo.transform.position) < 7f || (hitInfo.transform.GetComponent<Door>().upAmt != Vector3.zero && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 10f)))
		{
			hitInfo.transform.GetComponent<Door>().OpenOrShut();
		}
		if (hitInfo.transform.tag == "Bind" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
		{
			GameData.PlayerAud.PlayOneShot(GameData.Misc.TutorialPop, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
			GameData.GM.GetComponent<SetBind>().OpenBindWindow(hitInfo.transform);
		}
		if (hitInfo.transform.tag == "Forge" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
		{
			GameData.PlayerAud.PlayOneShot(GameData.Misc.SmithingOpen, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
			GameData.Smithing.OpenWindow(base.transform.position);
			if (hitInfo.transform.GetComponent<ForgeEffect>() != null)
			{
				GameData.Smithing.Success = hitInfo.transform.GetComponent<ForgeEffect>().Success;
			}
		}
		if (hitInfo.transform.tag == "PlanningDesk" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
		{
			GameData.GM.OpenPlanningWindow();
		}
		if (hitInfo.transform.tag == "ItemBag" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f && hitInfo.transform.GetComponent<ItemBag>() != null)
		{
			hitInfo.transform.GetComponent<ItemBag>().PickUp();
		}
		if (hitInfo.transform.tag == "EventLantern" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f && hitInfo.transform.GetComponent<IslandTorches>() != null)
		{
			if (GameData.PlayerInv.HasItem(GameData.ItemDB.GetItemByID("47164594"), _remove: false))
			{
				hitInfo.transform.GetComponent<IslandTorches>().Ignite();
			}
			else
			{
				UpdateSocialLog.LogAdd("You need something to light this with...", "yellow");
			}
		}
		if (hitInfo.transform.tag == "EventBell" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f && hitInfo.transform.GetComponent<IslandBells>() != null)
		{
			hitInfo.transform.GetComponent<IslandBells>().Ring();
		}
		if (hitInfo.transform.tag == "FernallaTree" && !flag && Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(hitInfo.transform.position.x, hitInfo.transform.position.z)) < 15f && GameData.CompletedQuests.Contains("WILLOWSEED"))
		{
			UpdateSocialLog.LogAdd("You run your fingers along the bark and find that it is not what it seems... [DUNGEON COMING IN THE FUTURE]");
		}
		if (hitInfo.transform.tag == "Treasure" && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
		{
			GameObject original = GameData.Misc.TreasureChest0_10;
			if (Myself.MyStats.Level >= 10 && Myself.MyStats.Level < 20)
			{
				original = GameData.Misc.TreasureChest10_20;
			}
			if (Myself.MyStats.Level >= 20 && Myself.MyStats.Level < 30)
			{
				original = GameData.Misc.TreasureChest20_30;
			}
			if (Myself.MyStats.Level >= 30)
			{
				original = GameData.Misc.TreasureChest30_35;
			}
			Object.Instantiate(original, hitInfo.transform.position, hitInfo.transform.rotation);
			Object.Instantiate(GameData.Misc.DigFX, hitInfo.transform.position, hitInfo.transform.rotation);
			GameData.PlayerAud.PlayOneShot(GameData.Misc.DigSFX, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
			GameData.GM.GetComponent<TreasureHunting>().ResetTreasureHunt();
			hitInfo.transform.gameObject.SetActive(value: false);
		}
	}

	private void RightClick()
	{
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		bool flag = EventSystem.current.IsPointerOverGameObject();
		if (Physics.Raycast(ray, out var hitInfo) && rbDT < 55f)
		{
			if (hitInfo.transform.GetComponent<NPC>() != null && !hitInfo.transform.GetComponent<NPC>().noClick && !flag)
			{
				Character component = hitInfo.transform.GetComponent<Character>();
				if (!component.Alive)
				{
					if (component.GetComponent<LootTable>() != null && Vector3.Distance(base.transform.position, component.transform.position) < 3f + component.transform.localScale.y && Myself.Alive)
					{
						if (component.GetComponent<SimPlayer>() == null)
						{
							if (!GameData.Autoattacking && !GameData.PlayerControl.Myself.MySpells.isCasting())
							{
								component.GetComponent<LootTable>().LoadLootTable();
								myAnim.SetTrigger("StartLoot");
							}
							else
							{
								UpdateSocialLog.LogAdd("Cannot loot while attacking.");
							}
						}
						else
						{
							UpdateSocialLog.LogAdd("You can't take other players' loot.");
						}
					}
					else if (Myself.Alive)
					{
						UpdateSocialLog.LogAdd("You can't reach that, get closer!");
					}
					else
					{
						UpdateSocialLog.LogAdd("You're dead. That's not how this works.", "yellow");
					}
				}
				else
				{
					if (component != null)
					{
						ConsiderOpponent(component, usedMouse: true);
					}
					if (component.MyNPC != null && component.MyNPC.CurrentAggroTarget == null && Vector3.Distance(base.transform.position, component.transform.position) < 4f && !component.MyNPC.SimPlayer)
					{
						if (GameData.ItemOnCursor == null || GameData.ItemOnCursor == GameData.PlayerInv.Empty)
						{
							if (component.GetComponent<NPCDialogManager>() != null)
							{
								if (CurrentTarget != null)
								{
									CurrentTarget.UntargetMe();
								}
								component.TargetMe();
								CurrentTarget = component;
								UpdateSocialLog.LogAdd("You say: Hail " + component.transform.name);
								UpdateSocialLog.LocalLogAdd("You say: Hail " + component.transform.name);
								component.GetComponent<NPCDialogManager>().GenericHail();
							}
						}
						else
						{
							UpdateSocialLog.LogAdd("Remove item from cursor before interacting with a vendor.", "yellow");
						}
					}
					else if (component.isVendor && component.MyNPC.CurrentAggroTarget == null)
					{
						UpdateSocialLog.LogAdd("Get closer to shop!", "yellow");
					}
					else if (component.isVendor && component.MyNPC.CurrentAggroTarget != null)
					{
						UpdateSocialLog.LogAdd("This NPC is busy right now.", "yellow");
					}
					if (component.MyNPC.SimPlayer && Vector3.Distance(base.transform.position, component.transform.position) < 5f && !GameData.Autoattacking)
					{
						GameData.InspectSim.InspectSim(component.GetComponent<SimPlayer>());
					}
					else if (component.MyNPC.SimPlayer && !GameData.Autoattacking)
					{
						UpdateSocialLog.LogAdd("Get closer to inspect this player!");
					}
				}
			}
			if (hitInfo.transform.GetComponent<RotChest>() != null && !flag)
			{
				Transform transform = hitInfo.transform;
				if (transform.GetComponent<LootTable>() != null && Vector3.Distance(base.transform.position, transform.transform.position) < 6f)
				{
					transform.GetComponent<LootTable>().LoadLootTable();
				}
				else
				{
					UpdateSocialLog.LogAdd("You can't reach that, get closer!");
				}
			}
			if (hitInfo.transform.GetComponent<Door>() != null && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
			{
				hitInfo.transform.GetComponent<Door>().OpenOrShut();
			}
			if ((hitInfo.transform.name == "Prestigio Valusha" || hitInfo.transform.name == "Validus Greencent" || hitInfo.transform.name == "Comstock Retalio" || hitInfo.transform.name == "Summoned: Pocket Bank" || hitInfo.transform.name == "Wealthen Giallara") && !flag && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
			{
				Character component2 = hitInfo.transform.GetComponent<Character>();
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				component2.TargetMe();
				CurrentTarget = component2;
				GameData.BankUI.OpenBank(base.transform.position);
			}
			if ((hitInfo.transform.name == "Thella Steepleton" || hitInfo.transform.name == "Goldie Retalio" || hitInfo.transform.name == "Summoned: Pocket Auctions") && Vector3.Distance(base.transform.position, hitInfo.transform.position) < 5f)
			{
				if (GameData.ItemOnCursor == null || GameData.ItemOnCursor == GameData.PlayerInv.Empty)
				{
					Character component3 = hitInfo.transform.GetComponent<Character>();
					if (CurrentTarget != null)
					{
						CurrentTarget.UntargetMe();
					}
					component3.TargetMe();
					CurrentTarget = component3;
					UpdateSocialLog.LogAdd("You say: Hail " + hitInfo.transform.name);
					UpdateSocialLog.LocalLogAdd("You say: Hail " + hitInfo.transform.name);
					hitInfo.transform.GetComponent<NPCDialogManager>().GenericHail();
				}
				else
				{
					UpdateSocialLog.LogAdd("Remove item from cursor before interacting with a vendor.", "yellow");
				}
			}
		}
		rbDT = 0f;
	}

	private string ColorToHex(Color color)
	{
		Color32 color2 = color;
		return $"#{color2.r:X2}{color2.g:X2}{color2.b:X2}";
	}

	private void ConsiderOpponent(Character newTar, bool usedMouse)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		if (newTar != null)
		{
			if (!newTar.MiningNode)
			{
				int num = newTar.MyStats.Level - Myself.MyStats.Level;
				if (num >= 7)
				{
					text = ColorToHex(DeepRed);
					text2 = "Even with a strong group, you are not likely to win this battle.";
				}
				else if (num >= 5)
				{
					text = ColorToHex(Red);
					text2 = "You will need a powerful group for this battle.";
				}
				else if (num >= 3)
				{
					text = ColorToHex(Orange);
					text2 = "This is a battle you shouldn't enter lightly.";
				}
				else if (num >= 1)
				{
					text = ColorToHex(Yellow);
					text2 = "This opponent will be a challenge.";
				}
				else if (num == 0)
				{
					text = "#FFFFFF";
					text2 = "You'd be an even match for this opponent.";
				}
				else if (num >= -2)
				{
					text = ColorToHex(CloseLow);
					text2 = "You're more experienced, but this opponent could pose a challenge.";
				}
				else if (num >= -4)
				{
					text = ColorToHex(EasyLow);
					text2 = "This opponent should fall easily.";
				}
				else
				{
					text = ColorToHex(Color.green);
					text2 = "This opponent is trivial - you won't learn anything.";
				}
				if (newTar.MyNPC != null && newTar.BossXp > 1f)
				{
					text2 += " This opponent looks unique, and may have special abilities.";
				}
				float num2 = ((!(newTar.MyWorldFaction != null)) ? 100f : GlobalFactionManager.FindFactionData(newTar.MyWorldFaction.REFNAME).Value);
				if (num2 > 600f)
				{
					text3 = " regards you as a hero... ";
				}
				if (num2 <= 600f && num2 > 400f)
				{
					text3 = " looks upon you kindly... ";
				}
				if (num2 <= 400f && num2 > 150f)
				{
					text3 = " is indifferent to you... ";
				}
				if (num2 <= 150f && num2 > 0f)
				{
					text3 = " looks tense in your presence... ";
				}
				if (num2 <= 0f && num2 > -200f)
				{
					text3 = " looks threateningly in your direction... ";
				}
				if (num2 <= -200f && num2 > -400f)
				{
					text3 = " postures, ready to fight you... ";
				}
				if (num2 <= -400f)
				{
					text3 = " scowls and braces to attack... ";
				}
				if (GameData.PlayerStats.Invisible && !newTar.SeeInvisible)
				{
					text3 = " is UNAWARE of your presence...";
				}
				if (newTar.AggressiveTowards.Contains(Character.Faction.Player))
				{
					UpdateSocialLog.LogAdd(newTar.transform.name + text3 + text2, text);
				}
				else
				{
					UpdateSocialLog.LogAdd(newTar.transform.name + text3 + text2, text);
				}
				if (usedMouse && newTar.MyNPC != null && (num2 <= 0f || newTar.AggressiveTowards.Contains(Character.Faction.Player)) && MyCombat.CheckTargetInMeleeRange(newTar) && newTar.MyNPC.CurrentAggroTarget != null)
				{
					if (CurrentTarget != null)
					{
						CurrentTarget.UntargetMe();
					}
					newTar.TargetMe();
					CurrentTarget = newTar;
					MyCombat.ForceAttackOn();
				}
			}
			else if (usedMouse && newTar.MyNPC != null && MyCombat.CheckTargetInMeleeRange(newTar))
			{
				if (CurrentTarget != null)
				{
					CurrentTarget.UntargetMe();
				}
				newTar.TargetMe();
				CurrentTarget = newTar;
				MyCombat.ForceAttackOn();
			}
			else
			{
				UpdateSocialLog.LogAdd("You can mine Nodes by attacking them as long as you have a pick in your inventory. Find a pick, or get closer first.");
			}
		}
		else
		{
			UpdateSocialLog.LogAdd("Consider who...?");
		}
	}

	private void LateUpdate()
	{
		if ((Input.GetKey(InputManager.Forward) || Input.GetKey(InputManager.Backward)) && InputManager.Modern)
		{
			float value = transposer.m_XAxis.Value;
			Quaternion quaternion = Quaternion.Euler(0f, value, 0f);
			float num = Quaternion.Angle(base.transform.rotation, quaternion);
			if (num < 1f)
			{
				base.transform.rotation = quaternion;
			}
			else if (num > 20f)
			{
				float t = 7.5f * Time.deltaTime;
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, t);
			}
			else
			{
				float t2 = 1f - Mathf.Exp(-15f * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, t2);
			}
		}
		if (TargetWindow.activeSelf)
		{
			UpdateTargetWindowData();
		}
	}

	public void SitDown()
	{
		if (!Sitting)
		{
			GetAnim().ResetTrigger("StandUp");
			GetAnim().SetTrigger("SitDown");
			GameData.PlayerCombat.ForceAttackOff();
			GameData.PlayerAud.PlayOneShot(GameData.Misc.SitSound, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
			Sitting = true;
		}
		else
		{
			Sitting = false;
			myAnim.SetTrigger("StandUp");
			GameData.PlayerAud.PlayOneShot(GameData.Misc.StandSound, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
		}
	}

	private void LandMovement()
	{
		if (Myself?.MyStats != null)
		{
			speed = Myself.MyStats.actualRunSpeed;
		}
		if (Input.GetKeyDown(InputManager.Sit))
		{
			SitDown();
		}
		if (Input.GetKey(InputManager.Forward))
		{
			Autorun = false;
		}
		if (ForceToY)
		{
			if (Mathf.Abs(base.transform.eulerAngles.y - tarDirY.Value) < 0.25f && Mathf.Abs(GameData.CamControl.GetCam().transform.eulerAngles.y - tarDirY.Value) < 0.25f)
			{
				ForceToY = false;
			}
			if (Input.GetMouseButton(1) || Input.GetKey(InputManager.Left) || Input.GetKey(InputManager.Right) || Input.GetKey(InputManager.Backward) || Input.GetKey(InputManager.StrafeL) || Input.GetKey(InputManager.StrafeR))
			{
				ForceToY = false;
			}
		}
		if ((Input.GetKey(InputManager.Forward) && !Input.GetKey(InputManager.Backward)) || (Input.GetMouseButton(0) && Input.GetMouseButton(1) && !Input.GetKey(InputManager.Backward) && !EventSystem.current.IsPointerOverGameObject()) || (usingGamepad && Input.GetAxis("Gamepad Y") < -0.3f && Input.GetAxis("LTrigger") <= 0.2f) || Autorun)
		{
			if (actualSpeed < speed)
			{
				actualSpeed += 4f * speed * Time.deltaTime;
			}
			if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !InputManager.Modern && !FPV.gameObject.activeSelf)
			{
				if (!ForceToY)
				{
					tarDirY = GameData.CamControl.GetCam().transform.eulerAngles.y;
				}
				float y = Mathf.LerpAngle(base.transform.eulerAngles.y, tarDirY.Value, 5.5f * Time.deltaTime);
				base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, y, base.transform.eulerAngles.z);
			}
			NormalizeMovement += base.transform.forward;
			moving = true;
		}
		else
		{
			if (!backing)
			{
				if (actualSpeed > 0f)
				{
					actualSpeed -= 5f * speed * Time.deltaTime;
				}
				if (actualSpeed < 0f)
				{
					actualSpeed = 0f;
				}
			}
			moving = false;
			sprinting = false;
		}
		if (Input.GetKey(InputManager.Forward) || Autorun)
		{
			speed = Myself.MyStats.actualRunSpeed;
			if (actualSpeed > speed)
			{
				actualSpeed = speed;
			}
		}
		if ((Input.GetKey(InputManager.Jump) || (usingGamepad && Input.GetKey(KeyCode.JoystickButton2) && Input.GetAxis("RTrigger") <= 0.5f && Input.GetAxis("LTrigger") <= 0.2f && GameData.HighlightedItem == null && !Input.GetKey(KeyCode.JoystickButton4) && grav == -1f)) && gravity == gravityOff && jumpCD <= 0f)
		{
			jumpCD = 60f;
			myAnim.SetTrigger("Jump");
			myAnim.SetBool("Jumped", value: true);
			grav = 10f;
			gravity = new Vector3(0f, grav, 0f);
			Myself.MyAudio.PlayOneShot(GameData.Misc.JumpUp, Myself.MyAudio.volume * GameData.SFXVol * GameData.MasterVol);
			MySpellCast.isCasting();
		}
		if ((Input.GetKey(InputManager.Backward) || (usingGamepad && Input.GetAxis("Gamepad Y") > 0.3f && Input.GetAxis("LTrigger") <= 0.2f)) && !Input.GetKey(InputManager.Forward))
		{
			Autorun = false;
			if (actualSpeed < speed / 2f)
			{
				actualSpeed += 4f * speed * Time.deltaTime;
			}
			if (actualSpeed > speed / 2f)
			{
				actualSpeed -= 4f * speed * Time.deltaTime;
			}
			NormalizeMovement -= base.transform.forward;
			if (!InputManager.Modern)
			{
				ResetCam();
			}
			myAnim.SetBool("Dancing", value: false);
			backing = true;
		}
		else
		{
			backing = false;
		}
		if (CanMove && Input.GetKey(InputManager.StrafeR) && CanMove)
		{
			if (actualSpeed < speed / 2f)
			{
				actualSpeed = speed / 2f;
			}
			NormalizeMovement += base.transform.right / 2f;
			if (InputManager.Modern)
			{
				float num = 360f;
				Quaternion b = Quaternion.Euler(0f, GameData.CamControl.GetCam().transform.eulerAngles.y, 0f);
				if (Quaternion.Angle(base.transform.rotation, b) > 20f)
				{
					num = 7.5f;
				}
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, num * Time.deltaTime);
			}
			if (!Swimming)
			{
				myAnim.SetBool("strafeRight", value: true);
			}
			else
			{
				myAnim.SetBool("Walking", value: true);
			}
			myAnim.SetBool("strafeLeft", value: false);
			myAnim.SetBool("pivotRight", value: false);
			myAnim.SetBool("Dancing", value: false);
			if (Input.GetKey(InputManager.Forward))
			{
				if (!Swimming)
				{
					myAnim.SetBool("ForwardRight", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
					myAnim.SetBool("ForwardRight", value: false);
				}
				myAnim.SetBool("ForwardLeft", value: false);
			}
			else
			{
				myAnim.SetBool("ForwardRight", value: false);
			}
		}
		if (CanMove && Input.GetKey(InputManager.StrafeL))
		{
			if (actualSpeed < speed / 2f)
			{
				actualSpeed = speed / 2f;
			}
			NormalizeMovement -= base.transform.right / 2f;
			if (InputManager.Modern)
			{
				float num2 = 360f;
				Quaternion b2 = Quaternion.Euler(0f, GameData.CamControl.GetCam().transform.eulerAngles.y, 0f);
				if (Quaternion.Angle(base.transform.rotation, b2) > 20f)
				{
					num2 = 7.5f;
				}
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b2, num2 * Time.deltaTime);
			}
			myAnim.SetBool("strafeRight", value: false);
			if (!Swimming)
			{
				myAnim.SetBool("strafeLeft", value: true);
			}
			else
			{
				myAnim.SetBool("Walking", value: true);
			}
			myAnim.SetBool("pivotLeft", value: false);
			myAnim.SetBool("Dancing", value: false);
			if (Input.GetKey(InputManager.Forward))
			{
				if (!Swimming)
				{
					myAnim.SetBool("ForwardLeft", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
					myAnim.SetBool("ForwardLeft", value: false);
				}
				myAnim.SetBool("ForwardRight", value: false);
			}
			else
			{
				myAnim.SetBool("ForwardLeft", value: false);
			}
		}
		if ((Input.GetKey(InputManager.Left) && !Input.GetMouseButton(1) && !usingGamepad && !InputManager.Modern) || (usingGamepad && Input.GetAxis("Horizontal") < -0.15f))
		{
			if (!usingGamepad)
			{
				myTransform.Rotate(0f, (0f - rotSpd) * 60f * Time.deltaTime, 0f);
			}
			else
			{
				myTransform.Rotate(0f, (0f - rotSpd) * (0f - Input.GetAxis("Horizontal")) * 60f * Time.deltaTime, 0f);
			}
			myAnim.SetBool("pivotLeft", value: true);
			myAnim.SetBool("Dancing", value: false);
		}
		else
		{
			myAnim.SetBool("pivotLeft", value: false);
			if (!Input.GetKey(InputManager.StrafeL))
			{
				myAnim.SetBool("strafeLeft", value: false);
			}
		}
		if ((Input.GetKey(InputManager.Right) && !Input.GetMouseButton(1) && !usingGamepad && !InputManager.Modern) || (usingGamepad && Input.GetAxis("Horizontal") > 0.15f))
		{
			if (!usingGamepad)
			{
				myTransform.Rotate(0f, rotSpd * 60f * Time.deltaTime, 0f);
			}
			else
			{
				myTransform.Rotate(0f, rotSpd * Input.GetAxis("Horizontal") * 60f * Time.deltaTime, 0f);
			}
			myAnim.SetBool("pivotRight", value: true);
			myAnim.SetBool("Dancing", value: false);
		}
		else
		{
			myAnim.SetBool("pivotRight", value: false);
			if (!Input.GetKey(InputManager.StrafeR))
			{
				myAnim.SetBool("strafeRight", value: false);
			}
		}
		if (backing && moving)
		{
			backing = false;
		}
		if ((backing || moving) && GameData.LootWindow.WindowParent.activeSelf)
		{
			GameData.LootWindow.CloseWindow();
		}
	}

	private void WaterMovement()
	{
		if (waterJumpCD > 0f)
		{
			waterJumpCD -= 60f * Time.deltaTime;
		}
		sprinting = false;
		speed = Myself.MyStats.actualRunSpeed;
		if (((Input.GetKey(InputManager.Forward) || (Input.GetMouseButton(0) && Input.GetMouseButton(1))) && !Input.GetKey(InputManager.Backward)) || (usingGamepad && Input.GetAxis("Gamepad Y") < -0.3f && Input.GetAxis("LTrigger") <= 0.2f))
		{
			if (!InputManager.Modern)
			{
				ResetCam();
			}
			else
			{
				float num = 360f;
				Quaternion b = Quaternion.Euler(0f, GameData.CamControl.GetCam().transform.eulerAngles.y, 0f);
				if (Quaternion.Angle(base.transform.rotation, b) > 20f)
				{
					num = 7.5f;
				}
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, num * Time.deltaTime);
			}
			myAnim.SetBool("Dancing", value: false);
			if (actualSpeed < speed * 0.85f)
			{
				actualSpeed += 4f * speed * Time.deltaTime;
			}
			myControl.Move((gravity + base.transform.forward * actualSpeed) * Time.deltaTime);
			moving = true;
		}
		else
		{
			if (actualSpeed > 0f)
			{
				actualSpeed -= 5f * speed * Time.deltaTime;
			}
			if (actualSpeed < 0f)
			{
				actualSpeed = 0f;
			}
			moving = false;
			myControl.Move((gravity + base.transform.forward * actualSpeed) * Time.deltaTime);
		}
		if ((Input.GetKey(InputManager.Jump) || (usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton2))) && Swimming && Input.GetAxis("RTrigger") <= 0.5f && Input.GetAxis("LTrigger") <= 0.2f && waterJumpCD <= 0f)
		{
			waterJumpCD = 10f;
			if (!Surfaced)
			{
				grav = 3f;
			}
			else
			{
				grav = 0f;
			}
		}
		if ((Input.GetKey(InputManager.Left) && (!Input.GetMouseButton(1) || EventSystem.current.IsPointerOverGameObject())) || (usingGamepad && Input.GetAxis("Horizontal") < -0.3f))
		{
			myTransform.Rotate(0f, (0f - rotSpd) * 60f * Time.deltaTime, 0f);
		}
		if ((Input.GetKey(InputManager.Right) && (!Input.GetMouseButton(1) || EventSystem.current.IsPointerOverGameObject())) || (usingGamepad && Input.GetAxis("Horizontal") > 0.3f))
		{
			myTransform.Rotate(0f, rotSpd * 60f * Time.deltaTime, 0f);
		}
		myAnim.SetBool("pivotLeft", value: false);
		myAnim.SetBool("strafeLeft", value: false);
		myAnim.SetBool("pivotRight", value: false);
		myAnim.SetBool("strafeRight", value: false);
		NormalizeMovement = NormalizeMovement.normalized;
	}

	public bool DraggingObjectWithoutInvOpen()
	{
		if (!GameData.PlayerInv.InvWindow.activeSelf && GameData.ItemOnCursor != null && GameData.ItemOnCursor != GameData.PlayerInv.Empty)
		{
			return true;
		}
		return false;
	}

	private void MouseLook()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && Myself.Alive && Input.GetMouseButton(1) && !isOverUI)
		{
			if (Input.GetAxis("Mouse X") > 0.02f)
			{
				if (Input.GetAxis("Mouse X") > 0.02f)
				{
					myTransform.eulerAngles += new Vector3(0f, Input.GetAxis("Mouse X") * MouseSensitivity * InputManager.MouseLookX, 0f);
				}
				if (GameData.Gamepad && Input.GetAxis("Gamepad X") > 0.1f && Input.GetAxis("LTrigger") <= 0.2f)
				{
					myTransform.eulerAngles += new Vector3(0f, Input.GetAxis("Mouse X") * MouseSensitivity * InputManager.MouseLookX, 0f);
				}
				myAnim.SetBool("pivotRight", value: true);
				myAnim.SetBool("pivotLeft", value: false);
				ResetCam();
			}
			if (Input.GetAxis("Mouse X") < -0.02f)
			{
				if (Input.GetAxis("Mouse X") < -0.02f)
				{
					myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, myTransform.eulerAngles + new Vector3(0f, Input.GetAxis("Mouse X") * MouseSensitivity * InputManager.MouseLookX, 0f), 360f * Time.deltaTime);
				}
				if (GameData.Gamepad && Input.GetAxis("Gamepad X") < -0.1f && Input.GetAxis("LTrigger") <= 0.2f)
				{
					myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, myTransform.eulerAngles + new Vector3(0f, Input.GetAxis("Gamepad X") * GamepadSensitivity, 0f), 360f * Time.deltaTime);
				}
				myAnim.SetBool("pivotLeft", value: true);
				myAnim.SetBool("pivotRight", value: false);
				ResetCam();
			}
			if (CanMove && Input.GetKey(InputManager.Right) && CanMove)
			{
				if (actualSpeed < speed / 2f)
				{
					actualSpeed = speed / 2f;
				}
				NormalizeMovement += base.transform.right / 2f;
				if (!Swimming)
				{
					myAnim.SetBool("strafeRight", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
				}
				myAnim.SetBool("strafeLeft", value: false);
				myAnim.SetBool("pivotRight", value: false);
				myAnim.SetBool("Dancing", value: false);
				ResetCam();
				if (Input.GetKey(InputManager.Forward))
				{
					if (!Swimming)
					{
						myAnim.SetBool("ForwardRight", value: true);
					}
					else
					{
						myAnim.SetBool("Walking", value: true);
						myAnim.SetBool("ForwardRight", value: false);
					}
					myAnim.SetBool("ForwardLeft", value: false);
				}
				else
				{
					myAnim.SetBool("ForwardRight", value: false);
				}
			}
			if (CanMove && Input.GetKey(InputManager.Left))
			{
				if (actualSpeed < speed / 2f)
				{
					actualSpeed = speed / 2f;
				}
				NormalizeMovement -= base.transform.right / 2f;
				myAnim.SetBool("strafeRight", value: false);
				if (!Swimming)
				{
					myAnim.SetBool("strafeLeft", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
				}
				myAnim.SetBool("pivotLeft", value: false);
				myAnim.SetBool("Dancing", value: false);
				ResetCam();
				if (Input.GetKey(InputManager.Forward))
				{
					if (!Swimming)
					{
						myAnim.SetBool("ForwardLeft", value: true);
					}
					else
					{
						myAnim.SetBool("Walking", value: true);
						myAnim.SetBool("ForwardLeft", value: false);
					}
					myAnim.SetBool("ForwardRight", value: false);
				}
				else
				{
					myAnim.SetBool("ForwardLeft", value: false);
				}
			}
		}
		if ((!Input.GetKey(InputManager.Left) && Input.GetAxis("Mouse X") > -0.02f) || !Myself.Alive)
		{
			myAnim.SetBool("ForwardLeft", value: false);
			myAnim.SetBool("strafeLeft", value: false);
			myAnim.SetBool("pivotLeft", value: false);
			if (Myself.Alive && Input.GetKey(InputManager.StrafeL))
			{
				myAnim.SetBool("strafeLeft", value: true);
			}
		}
		if ((!Input.GetKey(InputManager.Right) && Input.GetAxis("Mouse X") < 0.02f) || !Myself.Alive)
		{
			myAnim.SetBool("strafeRight", value: false);
			myAnim.SetBool("pivotRight", value: false);
			myAnim.SetBool("ForwardRight", value: false);
			if (Myself.Alive && Input.GetKey(InputManager.StrafeR))
			{
				myAnim.SetBool("strafeRight", value: true);
			}
		}
		NormalizeMovement.Normalize();
	}

	private void GamepadControls()
	{
		if (Myself.Alive)
		{
			if (!Myself.MyStats.Rooted && !Myself.MyStats.Stunned && !Myself.MyStats.Feared && CanMove && usingGamepad && Input.GetAxis("Gamepad X") > 0.4f && Input.GetAxis("LTrigger") <= 0.2f)
			{
				if (actualSpeed < speed)
				{
					actualSpeed = speed;
				}
				NormalizeMovement += base.transform.right / 2f;
				if (!Swimming)
				{
					myAnim.SetBool("strafeRight", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
				}
				myAnim.SetBool("strafeLeft", value: false);
				myAnim.SetBool("pivotRight", value: false);
				myAnim.SetBool("Dancing", value: false);
				if (usingGamepad && Input.GetAxis("Gamepad Y") < -0.4f && Input.GetAxis("LTrigger") <= 0.2f)
				{
					if (!Swimming)
					{
						myAnim.SetBool("ForwardRight", value: true);
					}
					else
					{
						myAnim.SetBool("Walking", value: true);
						myAnim.SetBool("ForwardRight", value: false);
					}
					myAnim.SetBool("ForwardLeft", value: false);
				}
				else
				{
					myAnim.SetBool("ForwardRight", value: false);
				}
			}
			if (!Myself.MyStats.Rooted && !Myself.MyStats.Stunned && !Myself.MyStats.Feared && CanMove && usingGamepad && Input.GetAxis("Gamepad X") < -0.4f && Input.GetAxis("LTrigger") <= 0.2f)
			{
				if (actualSpeed < speed)
				{
					actualSpeed = speed;
				}
				NormalizeMovement -= base.transform.right / 2f;
				myAnim.SetBool("strafeRight", value: false);
				if (!Swimming)
				{
					myAnim.SetBool("strafeLeft", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
				}
				myAnim.SetBool("pivotLeft", value: false);
				myAnim.SetBool("Dancing", value: false);
				if (usingGamepad && Input.GetAxis("Gamepad Y") < -0.4f && Input.GetAxis("LTrigger") <= 0.2f)
				{
					if (!Swimming)
					{
						myAnim.SetBool("ForwardLeft", value: true);
					}
					else
					{
						myAnim.SetBool("Walking", value: true);
						myAnim.SetBool("ForwardLeft", value: false);
					}
					myAnim.SetBool("ForwardRight", value: false);
				}
				else
				{
					myAnim.SetBool("ForwardLeft", value: false);
				}
			}
		}
		if (Input.GetAxis("Gamepad X") > -0.8f || !Myself.Alive)
		{
			myAnim.SetBool("ForwardLeft", value: false);
			myAnim.SetBool("strafeLeft", value: false);
			myAnim.SetBool("pivotLeft", value: false);
		}
		if (Input.GetAxis("Gamepad X") < 0.8f || !Myself.Alive)
		{
			myAnim.SetBool("strafeRight", value: false);
			myAnim.SetBool("pivotRight", value: false);
			myAnim.SetBool("ForwardRight", value: false);
		}
	}

	private void ResetCam()
	{
		_ = InputManager.Modern;
	}

	private void DroneMove()
	{
		if (Input.GetKey(InputManager.Jump))
		{
			base.transform.Translate(Vector3.Normalize(new Vector3(0f, 1f, 0f)) * Time.deltaTime * 20f);
		}
		if (Input.GetKey(InputManager.Forward))
		{
			base.transform.Translate(Vector3.Normalize(new Vector3(GameData.CamControl.transform.localEulerAngles.x, 0f, GameData.CamControl.transform.localEulerAngles.z + 90f)) * Time.deltaTime * 20f);
		}
		if (Input.GetKey(InputManager.Right))
		{
			base.transform.Translate(Vector3.Normalize(new Vector3(GameData.CamControl.transform.localEulerAngles.x, 0f, GameData.CamControl.transform.localEulerAngles.z)) * Time.deltaTime * 20f);
		}
		if (Input.GetKey(InputManager.Left))
		{
			base.transform.Translate(Vector3.Normalize(new Vector3(0f - GameData.CamControl.transform.localEulerAngles.x, 0f, 0f - GameData.CamControl.transform.localEulerAngles.z)) * Time.deltaTime * 20f);
		}
	}

	public void UpdateAnimRun()
	{
		if (Myself.MyStats.MyInv.MH.MyItem.WeaponDmg != 0 && Myself.MyStats.MyInv.MH.MyItem != GameData.PlayerInv.Empty && !armedRun)
		{
			armedRun = true;
			AnimOverride["Unarmed-Run-Forward"] = JogArmed;
			AnimOverride["Armed-Run-Forward"] = JogArmed;
		}
		else if ((Myself.MyStats.MyInv.MH.MyItem.WeaponDmg == 0 || Myself.MyStats.MyInv.MH.MyItem == GameData.PlayerInv.Empty) && armedRun)
		{
			armedRun = false;
			AnimOverride["Unarmed-Run-Forward"] = Jog;
			AnimOverride["Armed-Run-Forward"] = Jog;
		}
	}

	public void OnJoin(InputAction.CallbackContext ctx)
	{
		if (ctx.control.device is Keyboard)
		{
			usingGamepad = false;
		}
		else if (ctx.control.device is Gamepad)
		{
			usingGamepad = true;
		}
	}

	public Animator GetAnim()
	{
		return myAnim;
	}

	private void ModernStrafe()
	{
		if ((Input.GetKey(InputManager.Right) || Input.GetKey(InputManager.Left)) && InputManager.Modern)
		{
			float num = 360f;
			Quaternion b = Quaternion.Euler(0f, GameData.CamControl.GetCam().transform.eulerAngles.y, 0f);
			if (Quaternion.Angle(base.transform.rotation, b) > 20f)
			{
				num = 7.5f;
			}
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, num * Time.deltaTime);
		}
		if (CanMove && Input.GetKey(InputManager.Right) && CanMove)
		{
			if (InputManager.Modern)
			{
				float num2 = 360f;
				Quaternion b2 = Quaternion.Euler(0f, GameData.CamControl.GetCam().transform.eulerAngles.y, 0f);
				if (Quaternion.Angle(base.transform.rotation, b2) > 20f)
				{
					num2 = 7.5f;
				}
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b2, num2 * Time.deltaTime);
			}
			if (actualSpeed < speed)
			{
				actualSpeed = speed;
			}
			NormalizeMovement += base.transform.right / 2f;
			if (!Swimming)
			{
				myAnim.SetBool("strafeRight", value: true);
			}
			else
			{
				myAnim.SetBool("Walking", value: true);
			}
			myAnim.SetBool("strafeLeft", value: false);
			myAnim.SetBool("pivotRight", value: false);
			myAnim.SetBool("Dancing", value: false);
			if (Input.GetKey(InputManager.Forward))
			{
				if (!Swimming)
				{
					myAnim.SetBool("ForwardRight", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
					myAnim.SetBool("ForwardRight", value: false);
				}
				myAnim.SetBool("ForwardLeft", value: false);
			}
			else
			{
				myAnim.SetBool("ForwardRight", value: false);
			}
		}
		if (CanMove && Input.GetKey(InputManager.Left))
		{
			if (InputManager.Modern)
			{
				base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, new Vector3(base.transform.eulerAngles.x, GameData.CamControl.GetCam().transform.eulerAngles.y, base.transform.eulerAngles.z), 5f * Time.deltaTime);
			}
			if (actualSpeed < speed)
			{
				actualSpeed = speed;
			}
			NormalizeMovement -= base.transform.right / 2f;
			myAnim.SetBool("strafeRight", value: false);
			if (!Swimming)
			{
				myAnim.SetBool("strafeLeft", value: true);
			}
			else
			{
				myAnim.SetBool("Walking", value: true);
			}
			myAnim.SetBool("pivotLeft", value: false);
			myAnim.SetBool("Dancing", value: false);
			if (Input.GetKey(InputManager.Forward))
			{
				if (!Swimming)
				{
					myAnim.SetBool("ForwardLeft", value: true);
				}
				else
				{
					myAnim.SetBool("Walking", value: true);
					myAnim.SetBool("ForwardLeft", value: false);
				}
				myAnim.SetBool("ForwardRight", value: false);
			}
			else
			{
				myAnim.SetBool("ForwardLeft", value: false);
			}
		}
		if (!Input.GetKey(InputManager.Left) || !Myself.Alive)
		{
			myAnim.SetBool("ForwardLeft", value: false);
			myAnim.SetBool("strafeLeft", value: false);
			myAnim.SetBool("pivotLeft", value: false);
			if (Myself.Alive && Input.GetKey(InputManager.StrafeL))
			{
				myAnim.SetBool("strafeLeft", value: true);
			}
		}
		if (!Input.GetKey(InputManager.Right) || !Myself.Alive)
		{
			myAnim.SetBool("strafeRight", value: false);
			myAnim.SetBool("pivotRight", value: false);
			myAnim.SetBool("ForwardRight", value: false);
			if (Myself.Alive && Input.GetKey(InputManager.StrafeR))
			{
				myAnim.SetBool("strafeRight", value: true);
			}
		}
	}

	private void WarnOffNav()
	{
		if (SceneManager.GetActiveScene().name == "Stowaway" || SceneManager.GetActiveScene().name == "Tutorial")
		{
			UpdateSocialLog.LogAdd("[WHISPER FROM] GM-Burgee: Hello! The server indicates that you may be abusing NPC pathing. Please refrain from this activity and refer to the non-existent TOS for any further info. These infractions are not subject to punishment within the tutorial zones.", "yellow");
		}
		else
		{
			if (!GameData.Jail)
			{
				return;
			}
			if (OffNavWarnings == 0)
			{
				UpdateSocialLog.LogAdd("[WHISPER FROM] GM-Burgee: Hello! The server indicates that you may be abusing NPC pathing. Please refrain from this activity and refer to the non-existent TOS for any further info. Subsequent warnings will come with GM action.", "yellow");
			}
			if (OffNavWarnings == 1)
			{
				UpdateSocialLog.LogAdd("[WHISPER FROM] GM-Burgee: This is your second warning to refrain from abusing NPC pathing in order to trivialize gameplay. Thank you.", "yellow");
			}
			if (OffNavWarnings == 2)
			{
				UpdateSocialLog.LogAdd("[WHISPER FROM] GM-Burgee: This is your final warning to refrain from abusing NPC pathing in order to trivialize gameplay. Your next infraction will result in being removed from your group and jailed.", "yellow");
			}
			if (OffNavWarnings == 3)
			{
				if (GameData.GroupMembers[0] != null)
				{
					UpdateSocialLog.LogAdd("Your party member has decided not to follow you to this place...", "yellow");
					GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[0].MyAvatar.GetComponent<Character>());
				}
				if (GameData.GroupMembers[1] != null)
				{
					GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[1].MyAvatar.GetComponent<Character>());
					UpdateSocialLog.LogAdd("Your party member has decided not to follow you to this place...", "yellow");
				}
				if (GameData.GroupMembers[2] != null)
				{
					GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[2].MyAvatar.GetComponent<Character>());
					UpdateSocialLog.LogAdd("Your party member has decided not to follow you to this place...", "yellow");
				}
				if (GameData.GroupMembers[3] != null)
				{
					GameData.SimPlayerGrouping.ForceDismissFromGroup(GameData.GroupMembers[3].MyAvatar.GetComponent<Character>());
					UpdateSocialLog.LogAdd("Your party member has decided not to follow you to this place...", "yellow");
				}
				if (SceneManager.GetActiveScene().name == "Stowaway" || SceneManager.GetActiveScene().name == "Tutorial")
				{
					GameData.GM.JailedFromTutorial = true;
				}
				else
				{
					GameData.GM.JailedFromTutorial = false;
				}
				SimPlayerDataManager.SaveAllSimData();
				GameData.SceneChange.ChangeScene("Detention", new Vector3(-7.4f, 1f, 7.86f), _useSun: false, 180f);
			}
			OffNavWarnings++;
		}
	}
}
