// SetBind
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetBind : MonoBehaviour
{
	public GameObject BindWindow;

	public Transform BindObject;

	public void OpenBindWindow(Transform bo)
	{
		BindWindow.SetActive(value: true);
		BindObject = bo;
	}

	private void Update()
	{
		if (BindObject != null && BindWindow.activeSelf && Vector3.Distance(GameData.PlayerControl.transform.position, BindObject.position) > 5f)
		{
			BindObject = null;
			CloseBindWindow();
		}
	}

	public void SetBindPoint()
	{
		GameData.BindZone = SceneManager.GetActiveScene().name;
		GameData.BindLoc = GameData.PlayerControl.transform.position;
		GameData.SunInBindZone = GameData.usingSun;
		UpdateSocialLog.LogAdd("New respawn point set!", "yellow");
		GameData.PlayerAud.PlayOneShot(GameData.Misc.BindSFX, GameData.PlayerAud.volume * GameData.SFXVol * GameData.MasterVol);
		Object.Instantiate(GameData.EffectDB.SpellEffects[28], GameData.PlayerControl.transform.position, GameData.PlayerControl.transform.rotation);
		if (GameData.PlayerInv.Gold > 0)
		{
			GameData.PlayerInv.Gold--;
		}
		else
		{
			UpdateSocialLog.LogAdd("You are so broke. Somehow, the wish still works.", "yellow");
		}
		CloseBindWindow();
	}

	public void CloseBindWindow()
	{
		BindWindow.SetActive(value: false);
	}
}
