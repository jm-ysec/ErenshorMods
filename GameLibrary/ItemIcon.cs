// ItemIcon
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
	public bool dragging;

	private Vector2 HomePos;

	public Item.SlotType ThisSlotType;

	public Item MyItem;

	private Image myImage;

	private ItemIcon SwapWith;

	public bool VendorSlot;

	public bool PlayerOwned;

	public bool LootSlot;

	public bool MouseSlot;

	public bool BankSlot;

	public Color ParCol;

	private float rightClickDownTime;

	private float leftClickDownTime;

	private bool rButton;

	private bool lButton;

	private bool doHotkey;

	private Hotkeys hotkey;

	public Hotkeys assignedHotkey;

	public int Quantity = 1;

	public GameObject QuantityBox;

	public TextMeshProUGUI QuantityTxt;

	private Vector3 clickLoc;

	private Animator MyAnim;

	public bool TrashSlot;

	private ItemIcon MouseHomeSlot;

	public int ALLSLOTSINDEX = -1;

	public Image MySparkler;

	public bool CanTakeBlessedItem = true;

	public bool NotInInventory;

	private float lastClicked;

	private void Awake()
	{
		myImage = GetComponent<Image>();
		ParCol = base.transform.parent.GetComponent<Image>().color;
		HomePos = base.transform.parent.position;
		if (!LootSlot && !VendorSlot && ThisSlotType == Item.SlotType.General && QuantityBox != null)
		{
			QuantityTxt = QuantityBox.transform.GetComponentInChildren<TextMeshProUGUI>();
		}
	}

	private void Start()
	{
		MySparkler = Object.Instantiate(GameData.Misc.ItemSparkler, base.transform.parent).GetComponent<Image>();
		base.transform.SetAsLastSibling();
		MySparkler.transform.position = base.transform.position;
		MySparkler.gameObject.SetActive(value: false);
		if (MyItem == null)
		{
			MyItem = GameData.PlayerInv.Empty;
		}
		if (!MyItem.Stackable && ThisSlotType == Item.SlotType.General && !VendorSlot && !LootSlot)
		{
			if (QuantityBox != null)
			{
				QuantityBox.transform.SetParent(base.transform, worldPositionStays: false);
				QuantityBox.SetActive(value: false);
			}
		}
		else if (ThisSlotType == Item.SlotType.General && !VendorSlot && !LootSlot && QuantityBox != null)
		{
			QuantityBox.transform.SetParent(base.transform, worldPositionStays: false);
			QuantityBox.SetActive(value: true);
		}
	}

	public void Update()
	{
		if (lastClicked > 0f)
		{
			lastClicked -= 60f * Time.deltaTime;
		}
		if (dragging)
		{
			base.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f);
		}
		else
		{
			base.transform.position = base.transform.parent.position;
		}
		if (base.transform.localScale != Vector3.one)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, 10f * Time.deltaTime);
		}
		if (rButton)
		{
			rightClickDownTime += 60f * Time.deltaTime;
			if (rightClickDownTime > 10f && MyItem != GameData.PlayerInv.Empty)
			{
				int num = 0;
				num = ((!(Input.mousePosition.y > (float)(Screen.height / 2))) ? 200 : (-200));
				if (ThisSlotType == Item.SlotType.General && !VendorSlot && !LootSlot)
				{
					GameData.ItemInfoWindow.DisplayItem(MyItem, base.transform.position + new Vector3(-200f, num, 0f), Quantity);
				}
				else
				{
					GameData.ItemInfoWindow.DisplayItem(MyItem, base.transform.parent.position + new Vector3(-200f, num, 0f), Quantity);
				}
				if (MyItem.AssignQuestOnRead != null && (MyItem.AssignQuestOnRead.repeatable || !GameData.CompletedQuests.Contains(MyItem.AssignQuestOnRead.DBName)))
				{
					GameData.AssignQuest(MyItem.AssignQuestOnRead.DBName);
				}
				if (MyItem.CompleteOnRead != null)
				{
					GameData.FinishQuest(MyItem.CompleteOnRead.DBName);
				}
				if (MyItem.WeaponProcOnHit != null)
				{
					_ = MyItem.WeaponProcOnHit;
				}
				if (MyItem.WornEffect != null)
				{
					_ = MyItem.WornEffect;
				}
				if (MyItem.ItemEffectOnClick != null)
				{
					_ = MyItem.ItemEffectOnClick;
				}
				if (MyItem.WandEffect != null)
				{
					_ = MyItem.WandEffect;
				}
				if (MyItem.Aura != null)
				{
					_ = MyItem.Aura;
				}
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			leftClickDownTime = 0f;
			_ = doHotkey;
		}
		_ = lButton;
		if (GameData.PlayerControl.usingGamepad && Input.GetKeyDown(KeyCode.JoystickButton2) && GameData.HighlightedItem == this)
		{
			GamepadConsumeOrLoot();
		}
		if (MyItem.RequiredSlot != 0 && MyItem.RequiredSlot != Item.SlotType.Aura)
		{
			if (Quantity <= 1 && MySparkler.gameObject.activeSelf)
			{
				MySparkler.gameObject.SetActive(value: false);
			}
			if (Quantity == 2)
			{
				if (!MySparkler.gameObject.activeSelf)
				{
					MySparkler.gameObject.SetActive(value: true);
				}
				if (MySparkler.color != Color.cyan)
				{
					MySparkler.color = Color.cyan;
				}
			}
			if (Quantity == 3)
			{
				if (!MySparkler.gameObject.activeSelf)
				{
					MySparkler.gameObject.SetActive(value: true);
				}
				if (MySparkler.color != Color.magenta)
				{
					MySparkler.color = Color.magenta;
				}
			}
		}
		else if (MySparkler.gameObject.activeSelf)
		{
			MySparkler.gameObject.SetActive(value: false);
		}
	}

	public void ForceInitInv()
	{
		myImage = GetComponent<Image>();
		if (MyItem == null)
		{
			MyItem = GameData.PlayerInv.Empty;
		}
	}

	public void ForceOffCursor()
	{
		GameData.MouseSlot.dragging = false;
		if (GameData.MouseSlot.MouseHomeSlot != null)
		{
			GameData.MouseSlot.MouseHomeSlot.MyItem = MyItem;
			GameData.MouseSlot.MouseHomeSlot.Quantity = Quantity;
			MyItem = GameData.PlayerInv.Empty;
		}
		else if (MyItem != GameData.PlayerInv.Empty)
		{
			GameData.PlayerInv.AddItemToInv(MyItem);
		}
		GameData.MouseSlot.MyItem = GameData.PlayerInv.Empty;
		GameData.ItemOnCursor = null;
		UpdateSlotImage();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		GameData.Misc.IDStrip.transform.position = new Vector2(-1000f, -1000f);
		clickLoc = Input.mousePosition;
		lButton = false;
		doHotkey = false;
		leftClickDownTime = 0f;
		if (!VendorSlot && eventData.button == PointerEventData.InputButton.Left)
		{
			lButton = true;
		}
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			rButton = true;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (GameData.HighlightedItem != this && GameData.ItemOnCursor != this)
		{
			return;
		}
		if (!doHotkey)
		{
			ClickOnItemSlot(eventData);
		}
		doHotkey = false;
		lButton = false;
		leftClickDownTime = 0f;
		lastClicked = 20f;
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (rightClickDownTime < 20f && rButton && !dragging)
			{
				if (string.IsNullOrEmpty(MyItem.BookTitle))
				{
					if (MyItem.Classes.Contains(GameData.PlayerStats.CharacterClass) || MyItem.Classes.Count == 0)
					{
						if (MyItem.TeachSpell != null && !VendorSlot && !LootSlot)
						{
							if (!GameData.PlayerControl.GetComponent<CastSpell>().KnownSpells.Contains(MyItem.TeachSpell))
							{
								if (MyItem.TeachSpell.RequiredLevel <= GameData.PlayerControl.Myself.MyStats.Level)
								{
									GameData.PlayerControl.GetComponent<CastSpell>().KnownSpells.Add(MyItem.TeachSpell);
									UpdateSocialLog.LogAdd("Learned spell: " + MyItem.TeachSpell.SpellName, "lightblue");
									GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSpell, 0.6f * GameData.SFXVol * GameData.MasterVol);
									GameData.PlayerStats.GetComponent<CastSpell>().LearnSpell.Play();
									if (GameData.PlayerSpellBook.Spellbook.activeSelf)
									{
										GameData.PlayerSpellBook.UpdateSpellList(GameData.PlayerSpellBook.GetPage());
									}
									if (GameData.HKMngr.FindBlankHotkey() != null)
									{
										GameData.HKMngr.AssignNewSpellToHK(MyItem.TeachSpell, GameData.HKMngr.FindBlankHotkey());
									}
									GameData.PlayerInv.RemoveItemFromInv(this);
								}
								else
								{
									UpdateSocialLog.LogAdd("You are not experienced enough to learn this spell yet...", "yellow");
								}
							}
							else
							{
								UpdateSocialLog.LogAdd("You already know this spell!", "yellow");
							}
						}
						if (MyItem.TeachSkill != null && !VendorSlot && !LootSlot)
						{
							if (!GameData.PlayerControl.GetComponent<UseSkill>().KnownSkills.Contains(MyItem.TeachSkill))
							{
								bool flag = false;
								if (MyItem.TeachSkill.ArcanistRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Arcanist)
								{
									if (GameData.PlayerStats.Level >= MyItem.TeachSkill.ArcanistRequiredLevel)
									{
										flag = true;
										GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
										UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
										GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
										GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
										if (GameData.PlayerSkillBook.Skillbook.activeSelf)
										{
											GameData.PlayerSkillBook.UpdateSkillList(GameData.PlayerSkillBook.GetPage());
										}
										if (GameData.HKMngr.FindBlankHotkey() != null)
										{
											GameData.HKMngr.AssignNewSkillToHK(MyItem.TeachSkill, GameData.HKMngr.FindBlankHotkey());
										}
										GameData.PlayerInv.RemoveItemFromInv(this);
									}
									else
									{
										UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet...", "yellow");
									}
								}
								if (!flag && MyItem.TeachSkill.DruidRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Druid)
								{
									if (GameData.PlayerStats.Level >= MyItem.TeachSkill.DruidRequiredLevel)
									{
										flag = true;
										GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
										UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
										GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
										if (GameData.PlayerSkillBook.Skillbook.activeSelf)
										{
											GameData.PlayerSkillBook.UpdateSkillList(GameData.PlayerSkillBook.GetPage());
										}
										if (GameData.HKMngr.FindBlankHotkey() != null)
										{
											GameData.HKMngr.AssignNewSkillToHK(MyItem.TeachSkill, GameData.HKMngr.FindBlankHotkey());
										}
										GameData.PlayerInv.RemoveItemFromInv(this);
										GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
									}
									else
									{
										UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet...", "yellow");
									}
								}
								if (!flag && MyItem.TeachSkill.DuelistRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Duelist)
								{
									if (GameData.PlayerStats.Level >= MyItem.TeachSkill.DuelistRequiredLevel)
									{
										flag = true;
										GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
										UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
										GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
										GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
										if (GameData.PlayerSkillBook.Skillbook.activeSelf)
										{
											GameData.PlayerSkillBook.UpdateSkillList(GameData.PlayerSkillBook.GetPage());
										}
										if (GameData.HKMngr.FindBlankHotkey() != null)
										{
											GameData.HKMngr.AssignNewSkillToHK(MyItem.TeachSkill, GameData.HKMngr.FindBlankHotkey());
										}
										GameData.PlayerInv.RemoveItemFromInv(this);
									}
									else
									{
										UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet...", "yellow");
									}
								}
								if (!flag && MyItem.TeachSkill.PaladinRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Paladin)
								{
									if (GameData.PlayerStats.Level >= MyItem.TeachSkill.PaladinRequiredLevel)
									{
										flag = true;
										GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
										UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
										GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
										GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
										if (GameData.PlayerSkillBook.Skillbook.activeSelf)
										{
											GameData.PlayerSkillBook.UpdateSkillList(GameData.PlayerSkillBook.GetPage());
										}
										if (GameData.HKMngr.FindBlankHotkey() != null)
										{
											GameData.HKMngr.AssignNewSkillToHK(MyItem.TeachSkill, GameData.HKMngr.FindBlankHotkey());
										}
										GameData.PlayerInv.RemoveItemFromInv(this);
									}
									else
									{
										UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet...", "yellow");
									}
								}
								if (!flag && MyItem.TeachSkill.StormcallerRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Stormcaller)
								{
									if (GameData.PlayerStats.Level >= MyItem.TeachSkill.StormcallerRequiredLevel)
									{
										flag = true;
										GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
										UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
										GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
										GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
										if (GameData.PlayerSkillBook.Skillbook.activeSelf)
										{
											GameData.PlayerSkillBook.UpdateSkillList(GameData.PlayerSkillBook.GetPage());
										}
										if (GameData.HKMngr.FindBlankHotkey() != null)
										{
											GameData.HKMngr.AssignNewSkillToHK(MyItem.TeachSkill, GameData.HKMngr.FindBlankHotkey());
										}
										GameData.PlayerInv.RemoveItemFromInv(this);
									}
									else
									{
										UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet...", "yellow");
									}
								}
								if (!flag && MyItem.TeachSkill.ReaverRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Reaver)
								{
									if (GameData.PlayerStats.Level >= MyItem.TeachSkill.ReaverRequiredLevel)
									{
										flag = true;
										GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
										UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
										GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
										GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
										if (GameData.PlayerSkillBook.Skillbook.activeSelf)
										{
											GameData.PlayerSkillBook.UpdateSkillList(GameData.PlayerSkillBook.GetPage());
										}
										if (GameData.HKMngr.FindBlankHotkey() != null)
										{
											GameData.HKMngr.AssignNewSkillToHK(MyItem.TeachSkill, GameData.HKMngr.FindBlankHotkey());
										}
										GameData.PlayerInv.RemoveItemFromInv(this);
									}
									else
									{
										UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet...", "yellow");
									}
								}
							}
							else
							{
								UpdateSocialLog.LogAdd("You already know this skill!", "yellow");
							}
						}
						if (MyItem.RequiredSlot != 0 && !VendorSlot && !LootSlot && MyItem.ItemEffectOnClick == null && (!GameData.PlayerInv.EquipmentSlots.Contains(this) || MyItem.RequiredSlot == Item.SlotType.Aura))
						{
							bool flag2 = false;
							foreach (ItemIcon equipmentSlot in GameData.PlayerInv.EquipmentSlots)
							{
								if ((equipmentSlot.ThisSlotType == MyItem.RequiredSlot || ((equipmentSlot.ThisSlotType == Item.SlotType.Primary || equipmentSlot.ThisSlotType == Item.SlotType.Secondary) && (MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary || MyItem.RequiredSlot == Item.SlotType.Primary || MyItem.RequiredSlot == Item.SlotType.Secondary))) && equipmentSlot.MyItem == GameData.PlayerInv.Empty && DoInitialChecks(MyItem, equipmentSlot))
								{
									equipmentSlot.MyItem = MyItem;
									equipmentSlot.Quantity = Quantity;
									equipmentSlot.UpdateSlotImage();
									MyItem = GameData.PlayerInv.Empty;
									Quantity = 1;
									UpdateSlotImage();
									flag2 = true;
									UpdateSocialLog.LogAdd("Equipped " + equipmentSlot.MyItem.ItemName, "yellow");
									GameData.PlayerInv.UpdatePlayerInventory();
									break;
								}
							}
							if (!flag2 && MyItem.RequiredSlot == Item.SlotType.Aura)
							{
								Item myItem = GameData.PlayerInv.AuraSlot.MyItem;
								int quantity = GameData.PlayerInv.AuraSlot.Quantity;
								GameData.PlayerInv.AuraSlot.MyItem = MyItem;
								GameData.PlayerInv.AuraSlot.Quantity = Quantity;
								GameData.PlayerInv.AuraSlot.UpdateSlotImage();
								MyItem = myItem;
								Quantity = quantity;
								UpdateSlotImage();
								flag2 = true;
								UpdateSocialLog.LogAdd("Equipped " + GameData.PlayerInv.AuraSlot.MyItem.ItemName, "yellow");
								UpdateSocialLog.LogAdd("Unequipped " + MyItem.ItemName, "yellow");
								GameData.PlayerInv.UpdatePlayerInventory();
							}
							if (!flag2)
							{
								foreach (ItemIcon equipmentSlot2 in GameData.PlayerInv.EquipmentSlots)
								{
									if ((equipmentSlot2.ThisSlotType == MyItem.RequiredSlot || ((equipmentSlot2.ThisSlotType == Item.SlotType.Primary || equipmentSlot2.ThisSlotType == Item.SlotType.Secondary) && (MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary || MyItem.RequiredSlot == Item.SlotType.Primary || MyItem.RequiredSlot == Item.SlotType.Secondary))) && equipmentSlot2.MyItem != GameData.PlayerInv.Empty && DoInitialChecks(MyItem, equipmentSlot2))
									{
										Item myItem2 = equipmentSlot2.MyItem;
										int quantity2 = equipmentSlot2.Quantity;
										equipmentSlot2.MyItem = MyItem;
										equipmentSlot2.Quantity = Quantity;
										equipmentSlot2.UpdateSlotImage();
										MyItem = myItem2;
										Quantity = quantity2;
										UpdateSlotImage();
										flag2 = true;
										UpdateSocialLog.LogAdd("Equipped " + equipmentSlot2.MyItem.ItemName, "yellow");
										UpdateSocialLog.LogAdd("Unequipped " + MyItem.ItemName, "yellow");
										GameData.PlayerInv.UpdatePlayerInventory();
										break;
									}
								}
							}
							if (flag2)
							{
								GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume / 2f * GameData.UIVolume * GameData.MasterVol);
							}
						}
					}
					else if (!LootSlot && MyItem != GameData.PlayerInv.Empty && MyItem.ItemEffectOnClick != null)
					{
						UpdateSocialLog.LogAdd("Your class cannot use this item!", "yellow");
					}
					if (MyItem.ItemEffectOnClick != null && !VendorSlot && !LootSlot && rightClickDownTime < 10f && rButton)
					{
						if (MyItem.ItemEffectOnClick.Cooldown <= 1f || MyItem.SpellCastTime <= 0.1f)
						{
							UseConsumable();
						}
						else
						{
							UpdateSocialLog.LogAdd("This item can only be used from a hotkey.", "yellow");
						}
					}
					else if (MyItem.ItemSkillUse != null && !VendorSlot && !LootSlot && rightClickDownTime < 10f && rButton)
					{
						UseSkill();
					}
					else if (VendorSlot)
					{
						UpdateSocialLog.LogAdd("You must buy this item before you can use it that way!", "yellow");
					}
				}
				else if (!VendorSlot && !LootSlot)
				{
					GameData.GM.OpenBook(MyItem.BookTitle);
					SetAchievement.Unlock("BOOK");
				}
				else if (VendorSlot)
				{
					UpdateSocialLog.LogAdd("You must buy this item before you can use it that way!", "yellow");
				}
			}
			if (LootSlot && MyItem != GameData.PlayerInv.Empty && rButton && rightClickDownTime < 20f)
			{
				bool flag3 = false;
				string colorAsString = "yellow";
				if (MyItem.RequiredSlot == Item.SlotType.General || MyItem.RequiredSlot == Item.SlotType.Aura)
				{
					colorAsString = "yellow";
					flag3 = GameData.PlayerInv.AddItemToInv(MyItem);
				}
				else
				{
					flag3 = GameData.PlayerInv.AddItemToInv(MyItem, Quantity);
					if (Quantity <= 1)
					{
						colorAsString = "yellow";
					}
					if (Quantity == 2)
					{
						colorAsString = GameData.ReadableBlue;
					}
					if (Quantity == 3)
					{
						colorAsString = "magenta";
					}
				}
				if (flag3)
				{
					UpdateSocialLog.LogAdd("Looted Item: " + MyItem.ItemName, colorAsString);
					InformGroupOfLoot(MyItem);
					MyItem = GameData.PlayerInv.Empty;
					UpdateSlotImage();
					GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume / 2f * GameData.UIVolume * GameData.MasterVol);
				}
				else
				{
					UpdateSocialLog.LogAdd("Your inventory is full!", "yellow");
				}
			}
		}
		CloseSpellDetailsWindow();
		if (GameData.RequireRightClickInfo)
		{
			GameData.ItemInfoWindow.CloseItemWindow();
		}
		rButton = false;
		rightClickDownTime = 0f;
	}

	public void UpdateSlotImage()
	{
		if (MyAnim == null)
		{
			MyAnim = GameData.PlayerControl.GetComponent<Animator>();
		}
		if (MyItem != GameData.PlayerInv.Empty && MyItem != null)
		{
			if (myImage != null)
			{
				myImage.enabled = true;
				myImage.sprite = MyItem.ItemIcon;
				if (MyItem.Stackable && MyItem.RequiredSlot == Item.SlotType.General && QuantityBox != null)
				{
					QuantityBox.SetActive(value: true);
					QuantityTxt.text = Quantity.ToString();
				}
				else if (QuantityBox != null)
				{
					QuantityBox.SetActive(value: false);
				}
			}
			else
			{
				base.transform.name = "ItemSlot" + Random.Range(0, 1000);
			}
			if (ThisSlotType == Item.SlotType.Primary && MyItem != null)
			{
				if (MyItem.ThisWeaponType == Item.WeaponType.OneHandMelee)
				{
					MyAnim.SetBool("1HSmall", value: true);
					MyAnim.SetBool("1HDagger", value: false);
					MyAnim.SetBool("2HMelee", value: false);
					MyAnim.SetBool("2HStaff", value: false);
				}
				if (MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee)
				{
					MyAnim.SetBool("1HSmall", value: false);
					MyAnim.SetBool("1HDagger", value: false);
					MyAnim.SetBool("2HMelee", value: true);
					MyAnim.SetBool("2HStaff", value: false);
				}
				if (MyItem.ThisWeaponType == Item.WeaponType.OneHandDagger)
				{
					MyAnim.SetBool("1HSmall", value: false);
					MyAnim.SetBool("1HDagger", value: true);
					MyAnim.SetBool("2HMelee", value: false);
					MyAnim.SetBool("2HStaff", value: false);
				}
				if (MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff)
				{
					MyAnim.SetBool("1HSmall", value: false);
					MyAnim.SetBool("1HDagger", value: false);
					MyAnim.SetBool("2HMelee", value: false);
					MyAnim.SetBool("2HStaff", value: true);
				}
			}
			if (ThisSlotType == Item.SlotType.Secondary)
			{
				if (MyItem.ThisWeaponType != 0)
				{
					GameData.PlayerStats.DualWield = true;
				}
				else
				{
					GameData.PlayerStats.DualWield = false;
				}
			}
			if (ThisSlotType == Item.SlotType.Aura)
			{
				GameData.PlayerStats.CheckAuras();
			}
		}
		else if (myImage != null)
		{
			myImage.sprite = null;
			myImage.enabled = false;
			if (MyItem == GameData.PlayerInv.Empty && QuantityBox != null)
			{
				QuantityBox.SetActive(value: false);
				Quantity = 1;
			}
		}
		if (assignedHotkey != null)
		{
			if (assignedHotkey.AssignedItem == this)
			{
				assignedHotkey.MyImage.sprite = myImage.sprite;
			}
			else
			{
				assignedHotkey.UpdateImg();
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (dragging)
		{
			if (collision.transform.tag == "ItemSlot")
			{
				SwapWith = collision.GetComponent<ItemIcon>();
			}
			if (collision.transform.tag == "Hotkey")
			{
				hotkey = collision.GetComponent<Hotkeys>();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (dragging)
		{
			if (collision.transform.tag == "ItemSlot")
			{
				SwapWith = null;
			}
			if (collision.transform.tag == "Hotkey")
			{
				hotkey = null;
			}
		}
	}

	public void SendToTrade()
	{
		GameData.MouseSlot.MyItem = GameData.PlayerInv.Empty;
		GameData.MouseSlot.dragging = false;
		UpdateSlotImage();
		GameData.PlayerInv.UpdatePlayerInventory();
		GameData.ItemOnCursor = null;
		GameData.MouseSlot.MouseHomeSlot = null;
		SwapWith = null;
		dragging = false;
	}

	private void ClickOnItemSlot(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			InteractItemSlot();
		}
	}

	private bool DoInitialChecks()
	{
		if (SwapWith.TrashSlot && MyItem.NoTradeNoDestroy)
		{
			UpdateSocialLog.LogAdd("This item cannot destroyed.", "yellow");
			return false;
		}
		if (!SwapWith.CanTakeBlessedItem && MyItem.RequiredSlot != 0 && Quantity > 1)
		{
			UpdateSocialLog.LogAdd("This person cannot accept a blessed item. Find a way to remove the blessing if you want to trade it.", "yellow");
			return false;
		}
		if (MyItem.Relic && SwapWith.ThisSlotType != 0)
		{
			foreach (Item equippedItem in GameData.PlayerInv.EquippedItems)
			{
				if (equippedItem.Id == MyItem.Id && SwapWith.MyItem != MyItem)
				{
					UpdateSocialLog.LogAdd("You cannot equip two of the same RELIC ITEMS.", "yellow");
					return false;
				}
			}
		}
		if (!MyItem.Classes.Contains(GameData.PlayerStats.CharacterClass) && MyItem.Classes.Count > 0 && SwapWith.ThisSlotType != 0)
		{
			UpdateSocialLog.LogAdd("Your class cannot use this item!", "yellow");
			return false;
		}
		if ((SwapWith.ThisSlotType != Item.SlotType.Secondary || (MyItem.ThisWeaponType != Item.WeaponType.OneHandMelee && MyItem.ThisWeaponType != Item.WeaponType.OneHandDagger)) && GameData.PlayerStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")) && ThisSlotType != 0)
		{
			UpdateSocialLog.LogAdd("You need the DUAL WIELD skill to put a weapon in your secondary hand.", "yellow");
			return false;
		}
		if ((SwapWith.ThisSlotType == Item.SlotType.Primary || SwapWith.ThisSlotType == Item.SlotType.Secondary) && MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary)
		{
			if ((SwapWith.ThisSlotType != Item.SlotType.Secondary || (MyItem.ThisWeaponType != Item.WeaponType.OneHandMelee && MyItem.ThisWeaponType != Item.WeaponType.OneHandDagger)) && GameData.PlayerStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")) && ThisSlotType != 0)
			{
				UpdateSocialLog.LogAdd("You need the DUAL WIELD skill to put a weapon in your secondary hand.", "yellow");
				return false;
			}
			ItemIcon itemIcon = null;
			foreach (ItemIcon equipmentSlot in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot.ThisSlotType == Item.SlotType.Primary)
				{
					itemIcon = equipmentSlot;
					break;
				}
			}
			if (SwapWith.ThisSlotType != Item.SlotType.Primary && (itemIcon.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || itemIcon.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || itemIcon.MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow))
			{
				UpdateSocialLog.LogAdd("Primary Slot Items requires two hands!", "yellow");
				return false;
			}
			return true;
		}
		if (SwapWith.ThisSlotType != 0 && SwapWith.ThisSlotType != MyItem.RequiredSlot)
		{
			if (SwapWith.ThisSlotType == Item.SlotType.Secondary && (MyItem.ThisWeaponType == Item.WeaponType.OneHandMelee || MyItem.ThisWeaponType == Item.WeaponType.OneHandDagger) && GameData.PlayerStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")))
			{
				return true;
			}
			UpdateSocialLog.LogAdd("This item cannot go in this slot.", "yellow");
			return false;
		}
		if (SwapWith.ThisSlotType == Item.SlotType.Primary && (MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow))
		{
			ItemIcon itemIcon2 = null;
			foreach (ItemIcon equipmentSlot2 in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot2.ThisSlotType == Item.SlotType.Secondary)
				{
					itemIcon2 = equipmentSlot2;
					break;
				}
			}
			if (itemIcon2.MyItem != GameData.PlayerInv.Empty)
			{
				UpdateSocialLog.LogAdd("Second hand must be empty to equip a 2-handed weapon!", "yellow");
				return false;
			}
		}
		if (SwapWith.ThisSlotType == Item.SlotType.Secondary && (MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow))
		{
			UpdateSocialLog.LogAdd("Two handed weapons must be equipped in the primary slot with an empty secondary hand.", "yellow");
			return false;
		}
		if (SwapWith.ThisSlotType == Item.SlotType.Secondary && MyItem != GameData.PlayerInv.Empty)
		{
			ItemIcon itemIcon3 = null;
			foreach (ItemIcon equipmentSlot3 in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot3.ThisSlotType == Item.SlotType.Primary)
				{
					itemIcon3 = equipmentSlot3;
					break;
				}
			}
			if (itemIcon3.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || itemIcon3.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || itemIcon3.MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow)
			{
				UpdateSocialLog.LogAdd("Primary Slot Items requires two hands!", "yellow");
				return false;
			}
		}
		if (SwapWith.LootSlot)
		{
			UpdateSocialLog.LogAdd("Cannot store or rearrange items on corpses.", "yellow");
			return false;
		}
		return true;
	}

	private bool DoInitialChecks(Item MyItem, ItemIcon SwapWith)
	{
		if (!SwapWith.CanTakeBlessedItem && MyItem.RequiredSlot != 0 && Quantity > 1)
		{
			UpdateSocialLog.LogAdd("This person cannot accept a blessed item. Find a way to remove the blessing if you want to trade it.", "yellow");
			return false;
		}
		if (MyItem.Relic && SwapWith.ThisSlotType != 0)
		{
			foreach (Item equippedItem in GameData.PlayerInv.EquippedItems)
			{
				if (equippedItem.Id == MyItem.Id && SwapWith.MyItem != MyItem)
				{
					UpdateSocialLog.LogAdd("You cannot equip two of the same RELIC ITEMS.", "yellow");
					return false;
				}
			}
		}
		if (!MyItem.Classes.Contains(GameData.PlayerStats.CharacterClass) && MyItem.Classes.Count > 0 && SwapWith.ThisSlotType != 0)
		{
			UpdateSocialLog.LogAdd("Your class cannot use this item!", "yellow");
			return false;
		}
		if ((SwapWith.ThisSlotType != Item.SlotType.Secondary || (MyItem.ThisWeaponType != Item.WeaponType.OneHandMelee && MyItem.ThisWeaponType != Item.WeaponType.OneHandDagger)) && GameData.PlayerStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")) && ThisSlotType != 0)
		{
			UpdateSocialLog.LogAdd("You need the DUAL WIELD skill to put a weapon in your secondary hand.", "yellow");
			return false;
		}
		if ((SwapWith.ThisSlotType == Item.SlotType.Primary || SwapWith.ThisSlotType == Item.SlotType.Secondary) && MyItem.RequiredSlot == Item.SlotType.PrimaryOrSecondary)
		{
			if ((SwapWith.ThisSlotType != Item.SlotType.Secondary || (MyItem.ThisWeaponType != Item.WeaponType.OneHandMelee && MyItem.ThisWeaponType != Item.WeaponType.OneHandDagger)) && GameData.PlayerStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")) && ThisSlotType != 0)
			{
				UpdateSocialLog.LogAdd("You need the DUAL WIELD skill to put a weapon in your secondary hand.", "yellow");
				return false;
			}
			ItemIcon itemIcon = null;
			foreach (ItemIcon equipmentSlot in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot.ThisSlotType == Item.SlotType.Primary)
				{
					itemIcon = equipmentSlot;
					break;
				}
			}
			if (SwapWith.ThisSlotType != Item.SlotType.Primary && (itemIcon.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || itemIcon.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || itemIcon.MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow))
			{
				UpdateSocialLog.LogAdd("Primary Slot Items requires two hands!", "yellow");
				return false;
			}
			return true;
		}
		if (SwapWith.ThisSlotType != 0 && SwapWith.ThisSlotType != MyItem.RequiredSlot)
		{
			if (SwapWith.ThisSlotType == Item.SlotType.Secondary && (MyItem.ThisWeaponType == Item.WeaponType.OneHandMelee || MyItem.ThisWeaponType == Item.WeaponType.OneHandDagger) && GameData.PlayerStats.Myself.MySkills.KnownSkills.Contains(GameData.SkillDatabase.GetSkillByName("Dual Wield")))
			{
				return true;
			}
			UpdateSocialLog.LogAdd("This item cannot go in this slot.", "yellow");
			return false;
		}
		if (SwapWith.ThisSlotType == Item.SlotType.Primary && (MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow))
		{
			ItemIcon itemIcon2 = null;
			foreach (ItemIcon equipmentSlot2 in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot2.ThisSlotType == Item.SlotType.Secondary)
				{
					itemIcon2 = equipmentSlot2;
					break;
				}
			}
			if (itemIcon2.MyItem != GameData.PlayerInv.Empty)
			{
				UpdateSocialLog.LogAdd("Second hand must be empty to equip a 2-handed weapon!", "yellow");
				return false;
			}
		}
		if (SwapWith.ThisSlotType == Item.SlotType.Secondary && (MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow))
		{
			UpdateSocialLog.LogAdd("Two handed weapons must be equipped in the primary slot with an empty secondary hand.", "yellow");
			return false;
		}
		if (SwapWith.ThisSlotType == Item.SlotType.Secondary && MyItem != GameData.PlayerInv.Empty)
		{
			ItemIcon itemIcon3 = null;
			foreach (ItemIcon equipmentSlot3 in GameData.PlayerInv.EquipmentSlots)
			{
				if (equipmentSlot3.ThisSlotType == Item.SlotType.Primary)
				{
					itemIcon3 = equipmentSlot3;
					break;
				}
			}
			if (itemIcon3.MyItem.ThisWeaponType == Item.WeaponType.TwoHandMelee || itemIcon3.MyItem.ThisWeaponType == Item.WeaponType.TwoHandStaff || itemIcon3.MyItem.ThisWeaponType == Item.WeaponType.TwoHandBow)
			{
				UpdateSocialLog.LogAdd("Primary Slot Items requires two hands!", "yellow");
				return false;
			}
		}
		if (SwapWith.LootSlot)
		{
			UpdateSocialLog.LogAdd("Cannot store or rearrange items on corpses.", "yellow");
			return false;
		}
		return true;
	}

	public void UseConsumable()
	{
		if (GameData.PlayerControl.Myself.MySpells.isCasting())
		{
			UpdateSocialLog.LogAdd("You cannot use this item while casting a spell.", "yellow");
		}
		else if (GameData.PlayerControl.CurrentTarget != null || MyItem.ItemEffectOnClick.SelfOnly || (MyItem.ItemEffectOnClick.Type == Spell.SpellType.Misc && MyItem.ItemEffectOnClick.SpellRange <= 0f))
		{
			if (MyItem.ItemEffectOnClick.Type == Spell.SpellType.Misc && MyItem.ItemEffectOnClick.SpellRange > 0f && GameData.PlayerControl.CurrentTarget != null && MyItem.ItemEffectOnClick.SpellRange < Vector3.Distance(GameData.PlayerControl.transform.position, GameData.PlayerControl.CurrentTarget.transform.position) && !MyItem.ItemEffectOnClick.SelfOnly)
			{
				UpdateSocialLog.LogAdd("Target is out of range, get closer!", "yellow");
			}
			else if (GameData.PlayerControl.Myself.Alive)
			{
				if (GameData.PlayerControl.CurrentTarget != null && !MyItem.ItemEffectOnClick.SelfOnly)
				{
					GameData.PlayerControl.GetComponent<CastSpell>().StartSpell(MyItem.ItemEffectOnClick, GameData.PlayerControl.CurrentTarget.MyStats, MyItem.SpellCastTime);
				}
				else
				{
					GameData.PlayerControl.GetComponent<CastSpell>().StartSpell(MyItem.ItemEffectOnClick, GameData.PlayerControl.Myself.MyStats, MyItem.SpellCastTime);
				}
				UpdateSocialLog.CombatLogAdd("You use your " + MyItem.ItemName + "...", "lightblue");
				if (MyItem.Disposable)
				{
					Quantity--;
				}
				if (Quantity <= 0)
				{
					MyItem = GameData.PlayerInv.Empty;
				}
				UpdateSlotImage();
			}
			else
			{
				UpdateSocialLog.LogAdd("Can't do that while dead...", "yellow");
			}
		}
		else
		{
			UpdateSocialLog.CombatLogAdd("No target selected...", "lightblue");
		}
	}

	public void UseSkill()
	{
		GameData.PlayerControl.GetComponent<UseSkill>().MyFishing.StartFishing();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GameData.HighlightedItem = null;
		rButton = false;
		lastClicked = 0f;
		rightClickDownTime = 0f;
		GameData.Misc.IDStrip.transform.position = new Vector2(-1000f, -1000f);
		GameData.ItemInfoWindow.CloseItemWindow();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameData.HighlightedItem = this;
		base.transform.localScale = Vector3.one * 1.33f;
		if (!(MyItem != GameData.PlayerInv.Empty) || dragging)
		{
			return;
		}
		if (!GameData.PlayerControl.usingGamepad)
		{
			GameData.PlayerAud.PlayOneShot(GameData.Misc.Click, 0.05f * GameData.UIVolume * GameData.MasterVol);
			if (GameData.RequireRightClickInfo)
			{
				GameData.Misc.IDStrip.transform.SetAsLastSibling();
				GameData.Misc.IDStrip.transform.position = base.transform.position + Vector3.up * 75f;
				GameData.Misc.IDTitle.text = MyItem.ItemName;
				GameData.Misc.IDSubtext.text = "Hold 'RIGHT CLICK' to view info";
				if ((MyItem.RequiredSlot != 0 && Quantity <= 1) || MyItem.RequiredSlot == Item.SlotType.General)
				{
					GameData.Misc.IDTitle.color = Color.white;
				}
				else if (MyItem.RequiredSlot != 0 && Quantity == 2)
				{
					GameData.Misc.IDTitle.color = Color.cyan;
				}
				else if (MyItem.RequiredSlot != 0 && Quantity == 3)
				{
					GameData.Misc.IDTitle.color = Color.magenta;
				}
			}
			else
			{
				ShowInfoWindow();
			}
		}
		else
		{
			ShowInfoWindow();
		}
	}

	public void ShowInfoWindow()
	{
		int num = 0;
		int num2 = 0;
		num2 = ((!GameData.SteamDeck) ? 600 : (-150));
		num = ((!(base.transform.position.y > (float)(Screen.height / 2))) ? 150 : (-100));
		if (base.transform.position.x > (float)(Screen.width / 2))
		{
			num2 = -150;
		}
		if (GameData.SteamDeck)
		{
			num = 0;
		}
		if (ThisSlotType == Item.SlotType.General && !VendorSlot && !LootSlot)
		{
			GameData.ItemInfoWindow.DisplayItem(MyItem, base.transform.position + new Vector3(num2, num, 0f), Quantity);
		}
		else
		{
			GameData.ItemInfoWindow.DisplayItem(MyItem, base.transform.parent.position + new Vector3(num2, num, 0f), Quantity);
		}
		if (MyItem.CompleteOnRead != null)
		{
			GameData.FinishQuest(MyItem.CompleteOnRead.DBName);
		}
		if (MyItem.AssignQuestOnRead != null)
		{
			GameData.AssignQuest(MyItem.AssignQuestOnRead.DBName);
		}
	}

	public void InteractItemSlot()
	{
		if (Input.GetKey(InputManager.SplitOne) && Input.GetKey(InputManager.SplitTen))
		{
			return;
		}
		GameData.ItemInfoWindow.CloseItemWindow();
		if (!GameData.VendorWindowOpen && !GameData.AuctionWindowOpen)
		{
			if (!GameData.MouseSlot.dragging && MyItem != GameData.PlayerInv.Empty)
			{
				GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().MoveItem, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
				lButton = false;
				GameData.MouseSlot.dragging = true;
				GameData.ItemOnCursor = this;
				if (MyItem.RequiredSlot != 0 || (!Input.GetKey(InputManager.SplitOne) && !Input.GetKey(InputManager.SplitTen) && (!GameData.PlayerControl.usingGamepad || !Input.GetKey(KeyCode.JoystickButton1))))
				{
					GameData.MouseSlot.MyItem = MyItem;
					GameData.MouseSlot.Quantity = Quantity;
					GameData.MouseSlot.MouseHomeSlot = this;
					if (LootSlot)
					{
						UpdateSocialLog.LogAdd("Looted Item: " + MyItem.ItemName, "yellow");
						InformGroupOfLoot(MyItem);
					}
					MyItem = GameData.PlayerInv.Empty;
					Quantity = 1;
				}
				else
				{
					if ((MyItem.RequiredSlot == Item.SlotType.General && Input.GetKey(InputManager.SplitOne) && !Input.GetKey(InputManager.SplitTen)) || (GameData.PlayerControl.usingGamepad && Input.GetKey(KeyCode.JoystickButton1)))
					{
						GameData.MouseSlot.MyItem = MyItem;
						GameData.MouseSlot.Quantity = 1;
						GameData.MouseSlot.MouseHomeSlot = this;
						GameData.ItemOnCursor = GameData.MouseSlot;
						if (LootSlot)
						{
							UpdateSocialLog.LogAdd("Looted Item: " + MyItem.ItemName, "yellow");
							InformGroupOfLoot(MyItem);
						}
						if (Quantity > 1)
						{
							Quantity--;
						}
						else
						{
							MyItem = GameData.PlayerInv.Empty;
							Quantity = 1;
						}
						if (GameData.PlayerInv.CosmeticSlots.Contains(this))
						{
							GameData.PlayerInv.SetHalloweenMask();
						}
					}
					if (MyItem.RequiredSlot == Item.SlotType.General && !Input.GetKey(InputManager.SplitOne) && Input.GetKey(InputManager.SplitTen))
					{
						if (Quantity >= 10)
						{
							GameData.MouseSlot.MyItem = MyItem;
							GameData.MouseSlot.Quantity = 10;
							Quantity -= 10;
							if (Quantity == 0)
							{
								MyItem = GameData.PlayerInv.Empty;
								Quantity = 1;
							}
						}
						else
						{
							GameData.MouseSlot.MyItem = MyItem;
							GameData.MouseSlot.Quantity = Quantity;
							MyItem = GameData.PlayerInv.Empty;
							Quantity = 1;
						}
						if (GameData.PlayerInv.CosmeticSlots.Contains(this))
						{
							GameData.PlayerInv.SetHalloweenMask();
						}
						GameData.MouseSlot.MouseHomeSlot = this;
					}
				}
				GameData.MouseSlot.transform.parent.SetAsLastSibling();
				GameData.MouseSlot.transform.parent.parent.SetAsLastSibling();
				UpdateSlotImage();
				GameData.MouseSlot.UpdateSlotImage();
				GameData.PlayerInv.UpdatePlayerInventory();
				if (assignedHotkey != null)
				{
					assignedHotkey.ClearMe();
					assignedHotkey = null;
				}
			}
			else if (SwapWith != null)
			{
				if (!DoInitialChecks())
				{
					return;
				}
				if (!SwapWith.TrashSlot)
				{
					if (SwapWith.MyItem == GameData.PlayerInv.Empty)
					{
						GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().MoveItem, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
						SwapWith.MyItem = MyItem;
						SwapWith.Quantity = Quantity;
						MyItem = GameData.PlayerInv.Empty;
						Quantity = 1;
						if (QuantityBox != null)
						{
							QuantityBox.SetActive(value: false);
						}
						GameData.ItemOnCursor = null;
						GameData.MouseSlot.MouseHomeSlot = null;
						GameData.MouseSlot.dragging = false;
						GameData.HighlightedItem = SwapWith;
						if (GameData.PlayerInv.CosmeticSlots.Contains(SwapWith))
						{
							GameData.PlayerInv.SetHalloweenMask();
						}
					}
					else
					{
						GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
						if (SwapWith.MyItem == MyItem && MyItem.Stackable)
						{
							SwapWith.Quantity += Quantity;
							MyItem = GameData.PlayerInv.Empty;
							if (QuantityBox != null)
							{
								QuantityBox.SetActive(value: false);
							}
							Quantity = 1;
							dragging = false;
							SwapWith.dragging = false;
							GameData.ItemOnCursor = null;
							GameData.MouseSlot.MouseHomeSlot = null;
							GameData.HighlightedItem = SwapWith;
							if (GameData.PlayerInv.CosmeticSlots.Contains(SwapWith))
							{
								GameData.PlayerInv.SetHalloweenMask();
							}
						}
						else
						{
							int quantity = SwapWith.Quantity;
							Item myItem = SwapWith.MyItem;
							SwapWith.MyItem = MyItem;
							SwapWith.Quantity = Quantity;
							Quantity = quantity;
							MyItem = myItem;
							GameData.ItemOnCursor = this;
							GameData.MouseSlot.MouseHomeSlot = this;
							GameData.HighlightedItem = SwapWith;
							if (LootSlot)
							{
								UpdateSocialLog.LogAdd("Looted Item: " + GameData.ItemOnCursor.MyItem, "yellow");
								InformGroupOfLoot(GameData.ItemOnCursor.MyItem);
							}
							if (GameData.PlayerInv.CosmeticSlots.Contains(SwapWith))
							{
								GameData.PlayerInv.SetHalloweenMask();
							}
						}
					}
				}
				else if (MyItem != GameData.PlayerInv.Empty)
				{
					GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
					SwapWith.MyItem = MyItem;
					SwapWith.Quantity = Quantity;
					MyItem = GameData.PlayerInv.Empty;
					Quantity = 1;
					GameData.ItemOnCursor = null;
					GameData.MouseSlot.MouseHomeSlot = null;
					GameData.MouseSlot.dragging = false;
					GameData.HighlightedItem = SwapWith;
					if (GameData.PlayerInv.CosmeticSlots.Contains(SwapWith))
					{
						GameData.PlayerInv.SetHalloweenMask();
					}
				}
				else
				{
					GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume * GameData.UIVolume * GameData.MasterVol);
					MyItem = SwapWith.MyItem;
					Quantity = SwapWith.Quantity;
					SwapWith.MyItem = GameData.PlayerInv.Empty;
					SwapWith.Quantity = 1;
					GameData.ItemOnCursor = this;
					GameData.MouseSlot.MouseHomeSlot = this;
					GameData.HighlightedItem = SwapWith;
					if (LootSlot)
					{
						UpdateSocialLog.LogAdd("Looted Item: " + GameData.ItemOnCursor.MyItem, "yellow");
						InformGroupOfLoot(GameData.ItemOnCursor.MyItem);
					}
					if (GameData.PlayerInv.CosmeticSlots.Contains(SwapWith))
					{
						GameData.PlayerInv.SetHalloweenMask();
					}
				}
				SwapWith.UpdateSlotImage();
				UpdateSlotImage();
				GameData.PlayerInv.UpdatePlayerInventory();
			}
			else if (hotkey != null)
			{
				if (MouseHomeSlot.BankSlot)
				{
					UpdateSocialLog.LogAdd("Cannot hotkey items from bank.", "yellow");
				}
				else if (MyItem.ItemEffectOnClick == null && MyItem.ItemSkillUse == null)
				{
					UpdateSocialLog.LogAdd("WARNING: This item does not have an activatable ability and cannot be hotkeyed.", "yellow");
				}
				else if (NotInInventory)
				{
					UpdateSocialLog.LogAdd("WARNING: Cannot hotkey items that are not in your direct inventory.", "yellow");
				}
				else if (MouseHomeSlot.MyItem == GameData.PlayerInv.Empty)
				{
					MouseHomeSlot.assignedHotkey = hotkey;
					_ = MouseHomeSlot;
					ForceOffCursor();
					UpdateSlotImage();
					GameData.PlayerInv.UpdatePlayerInventory();
					hotkey.AssignItemFrominv(MouseHomeSlot);
					hotkey = null;
				}
				else
				{
					UpdateSocialLog.LogAdd("WARNING: Cannot hotkey item - place item in an EMPTY inventory slot first, and then drag it to a hotkey.", "yellow");
				}
			}
		}
		else if (GameData.VendorWindowOpen)
		{
			if (lastClicked <= 0f)
			{
				lastClicked = 20f;
				GameData.PlayerAud.PlayOneShot(GameData.Misc.Click, GameData.UIVolume * GameData.MasterVol);
				GameData.ActivateSlotForVendor(this);
			}
			else if (!VendorSlot)
			{
				if (Quantity > 0)
				{
					GameData.VendorWindow.Transaction();
				}
				else
				{
					GameData.VendorWindow.DoSellStack();
				}
			}
		}
		else if (GameData.AuctionWindowOpen && MyItem != GameData.PlayerInv.Empty)
		{
			GameData.PlayerAud.PlayOneShot(GameData.Misc.Click, GameData.UIVolume * GameData.MasterVol);
			GameData.ActivateSlotForAuction(this);
		}
	}

	private void GamepadConsumeOrLoot()
	{
		if (MyItem.Classes.Contains(GameData.PlayerStats.CharacterClass) || MyItem.Classes.Count == 0)
		{
			if (MyItem.TeachSpell != null && !VendorSlot && !LootSlot && !MouseSlot)
			{
				if (!GameData.PlayerControl.GetComponent<CastSpell>().KnownSpells.Contains(MyItem.TeachSpell))
				{
					if (MyItem.TeachSpell.RequiredLevel <= GameData.PlayerControl.Myself.MyStats.Level)
					{
						GameData.PlayerControl.GetComponent<CastSpell>().KnownSpells.Add(MyItem.TeachSpell);
						UpdateSocialLog.LogAdd("Learned spell: " + MyItem.TeachSpell.SpellName, "lightblue");
						GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSpell, 0.6f * GameData.SFXVol * GameData.MasterVol);
						GameData.PlayerStats.GetComponent<CastSpell>().LearnSpell.Play();
						GameData.PlayerInv.RemoveItemFromInv(this);
					}
					else
					{
						UpdateSocialLog.LogAdd("You are not experienced enough to learn this spell yet...", "yellow");
					}
				}
				else
				{
					UpdateSocialLog.LogAdd("You already know this spell!", "yellow");
				}
			}
			if (MyItem.TeachSkill != null && !VendorSlot && !LootSlot && !MouseSlot)
			{
				if (!GameData.PlayerControl.GetComponent<UseSkill>().KnownSkills.Contains(MyItem.TeachSkill))
				{
					bool flag = false;
					if (MyItem.TeachSkill.ArcanistRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Arcanist)
					{
						if (GameData.PlayerStats.Level >= MyItem.TeachSkill.ArcanistRequiredLevel)
						{
							flag = true;
							GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
							UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
							GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
							GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
							GameData.PlayerInv.RemoveItemFromInv(this);
						}
						else
						{
							UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet", "yellow");
						}
					}
					if (!flag && MyItem.TeachSkill.DruidRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Druid)
					{
						if (GameData.PlayerStats.Level >= MyItem.TeachSkill.DruidRequiredLevel)
						{
							flag = true;
							GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
							UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
							GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
							GameData.PlayerInv.RemoveItemFromInv(this);
							GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
						}
						else
						{
							UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet", "yellow");
						}
					}
					if (!flag && MyItem.TeachSkill.DuelistRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Duelist)
					{
						if (GameData.PlayerStats.Level >= MyItem.TeachSkill.DuelistRequiredLevel)
						{
							flag = true;
							GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
							UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
							GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
							GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
							GameData.PlayerInv.RemoveItemFromInv(this);
						}
						else
						{
							UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet", "yellow");
						}
					}
					if (!flag && MyItem.TeachSkill.PaladinRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Paladin)
					{
						if (GameData.PlayerStats.Level >= MyItem.TeachSkill.PaladinRequiredLevel)
						{
							flag = true;
							GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
							UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
							GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
							GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
							GameData.PlayerInv.RemoveItemFromInv(this);
						}
						else
						{
							UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet", "yellow");
						}
					}
					if (!flag && MyItem.TeachSkill.StormcallerRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Stormcaller)
					{
						if (GameData.PlayerStats.Level >= MyItem.TeachSkill.StormcallerRequiredLevel)
						{
							flag = true;
							GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
							UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
							GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
							GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
							GameData.PlayerInv.RemoveItemFromInv(this);
						}
						else
						{
							UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet", "yellow");
						}
					}
					if (!flag && MyItem.TeachSkill.ReaverRequiredLevel > 0 && GameData.PlayerStats.CharacterClass == GameData.ClassDB.Reaver)
					{
						if (GameData.PlayerStats.Level >= MyItem.TeachSkill.ReaverRequiredLevel)
						{
							flag = true;
							GameData.PlayerStats.Myself.MySkills.KnownSkills.Add(MyItem.TeachSkill);
							UpdateSocialLog.LogAdd("Learned skill: " + MyItem.TeachSkill.SkillName, "lightblue");
							GameData.PlayerStats.GetComponent<UseSkill>().LearnSkill.Play();
							GameData.PlayerAud.PlayOneShot(GameData.Misc.NewSkill, 0.6f * GameData.SFXVol * GameData.MasterVol);
							GameData.PlayerInv.RemoveItemFromInv(this);
						}
						else
						{
							UpdateSocialLog.LogAdd("You are not experienced enough to learn this skill yet", "yellow");
						}
					}
				}
				else
				{
					UpdateSocialLog.LogAdd("You already know this skill!", "yellow");
				}
			}
		}
		else if (!LootSlot && MyItem != GameData.PlayerInv.Empty && MyItem.ItemEffectOnClick != null)
		{
			UpdateSocialLog.LogAdd("Your class cannot use this item!", "yellow");
		}
		if (MyItem.ItemEffectOnClick != null && !VendorSlot && !LootSlot)
		{
			UseConsumable();
		}
		else if (MyItem.ItemSkillUse != null && !VendorSlot && !LootSlot)
		{
			UseSkill();
		}
		else if (VendorSlot)
		{
			UpdateSocialLog.LogAdd("You must buy this item before you can use it that way!", "yellow");
		}
		if (LootSlot && MyItem != GameData.PlayerInv.Empty)
		{
			if (GameData.PlayerInv.AddItemToInv(MyItem))
			{
				UpdateSocialLog.LogAdd("Looted Item: " + MyItem.ItemName, "yellow");
				InformGroupOfLoot(MyItem);
				MyItem = GameData.PlayerInv.Empty;
				UpdateSlotImage();
				GameData.PlayerAud.PlayOneShot(GameData.GM.GetComponent<Misc>().DropItem, GameData.PlayerAud.volume / 2f * GameData.UIVolume * GameData.MasterVol);
			}
			else
			{
				UpdateSocialLog.LogAdd("Your inventory is full!", "yellow");
			}
		}
		GameData.ItemInfoWindow.CloseItemWindow();
	}

	public void InformGroupOfLoot(Item _item)
	{
		int num;
		if (!string.IsNullOrEmpty(GameData.PlayerControl?.MyGuild))
		{
			GuildManager guildManager = GameData.GuildManager;
			num = (((object)guildManager != null && guildManager.GetGuildDataByID(GameData.PlayerControl.MyGuild)?.OngoingGuildQuests?.Count > 0) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		if (GameData.GroupMembers[0] != null)
		{
			if (GameData.GroupMembers[0].MyAvatar.IsThatAnUpgrade(_item))
			{
				string incoming = GameData.GroupMembers[0].MyAvatar.MyDialog.GetLootReq().Replace("II", _item.ItemName);
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.SimPlayerGrouping.PlayerOneName.text + " tells the group: " + GameData.SimMngr.PersonalizeString(incoming, GameData.GroupMembers[0].MyAvatar), "#00B2B7");
				GameData.GroupMembers[0].OpinionOfPlayer -= 0.01f;
			}
			if (GameData.GuildManager.CheckTradeAgainstQuests(GameData.GroupMembers[0], _item, _justDropped: true))
			{
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.GroupMembers[0].SimName + " tells the group: " + GameData.SimMngr.PersonalizeString("Oh! That's the item I mentioned I was after in guild chat!", GameData.GroupMembers[0].MyAvatar), "#00B2B7");
			}
		}
		if (GameData.GroupMembers[1] != null)
		{
			if (GameData.GroupMembers[1].MyAvatar.IsThatAnUpgrade(_item))
			{
				string incoming2 = GameData.GroupMembers[1].MyAvatar.MyDialog.GetLootReq().Replace("II", _item.ItemName);
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.SimPlayerGrouping.PlayerTwoName.text + " tells the group: " + GameData.SimMngr.PersonalizeString(incoming2, GameData.GroupMembers[1].MyAvatar), "#00B2B7");
				GameData.GroupMembers[1].OpinionOfPlayer -= 0.01f;
			}
			if (GameData.GuildManager.CheckTradeAgainstQuests(GameData.GroupMembers[1], _item, _justDropped: true))
			{
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.GroupMembers[1].SimName + " tells the group: " + GameData.SimMngr.PersonalizeString("Oh! That's the item I mentioned I was after in guild chat!", GameData.GroupMembers[1].MyAvatar), "#00B2B7");
			}
		}
		if (GameData.GroupMembers[2] != null)
		{
			if (GameData.GroupMembers[2].MyAvatar.IsThatAnUpgrade(_item))
			{
				string incoming3 = GameData.GroupMembers[2].MyAvatar.MyDialog.GetLootReq().Replace("II", _item.ItemName);
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.SimPlayerGrouping.PlayerThreeName.text + " tells the group: " + GameData.SimMngr.PersonalizeString(incoming3, GameData.GroupMembers[2].MyAvatar), "#00B2B7");
				GameData.GroupMembers[2].OpinionOfPlayer -= 0.01f;
			}
			if (GameData.GuildManager.CheckTradeAgainstQuests(GameData.GroupMembers[2], _item, _justDropped: true))
			{
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.GroupMembers[2].SimName + " tells the group: " + GameData.SimMngr.PersonalizeString("Oh! That's the item I mentioned I was after in guild chat!", GameData.GroupMembers[2].MyAvatar), "#00B2B7");
			}
		}
		if (GameData.GroupMembers[3] != null)
		{
			if (GameData.GroupMembers[3].MyAvatar.IsThatAnUpgrade(_item))
			{
				string incoming4 = GameData.GroupMembers[3].MyAvatar.MyDialog.GetLootReq().Replace("II", _item.ItemName);
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.SimPlayerGrouping.PlayerFourName.text + " tells the group: " + GameData.SimMngr.PersonalizeString(incoming4, GameData.GroupMembers[3].MyAvatar), "#00B2B7");
				GameData.GroupMembers[3].OpinionOfPlayer -= 0.01f;
			}
			if (GameData.GuildManager.CheckTradeAgainstQuests(GameData.GroupMembers[3], _item, _justDropped: true))
			{
				GameData.SimPlayerGrouping.AddStringForDisplay(GameData.GroupMembers[3].SimName + " tells the group: " + GameData.SimMngr.PersonalizeString("Oh! That's the item I mentioned I was after in guild chat!", GameData.GroupMembers[3].MyAvatar), "#00B2B7");
			}
		}
	}

	public void CloseSpellDetailsWindow()
	{
		GameData.ItemInfoWindow.SpellDetailsWindow.SetActive(value: false);
	}
}
