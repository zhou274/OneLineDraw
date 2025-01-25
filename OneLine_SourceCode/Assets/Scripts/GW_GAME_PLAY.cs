using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GW_GAME_PLAY : MonoBehaviour
{
	public static GW_GAME_PLAY instance;
	public GameObject OneLineGameplay;
	public PopupWin popupWin;
	public PopupHint popupHint;
	public Button bLight, bMenu, bReplay, bHint;
	public Image iBottomBG;

	public Text tGameName, tGamePackAndLevel;

	void Awake ()
	{
		instance = this;
	}

	public void InitInfo ()
	{
		tGameName.text = GameManager.currentGameType.gameNameShow;
		tGamePackAndLevel.text = GameManager.currentPackType.ToString () + " " + GameManager.currentLevel;
	}

	void OnEnable ()
	{
		
		InitInfo ();
		iBottomBG.enabled = false;
		OneLineGameplay.SetActive (true);
	}

	void OnDisable ()
	{
		iBottomBG.enabled = true;
		if (OneLineGameplay != null) {
			OneLineGameplay.SetActive (false);
			popupWin.gameObject.SetActive (false);
		}
	}

	public void BackToLevelSelect ()
	{
		GameManager.gameState = GameState.SELECT_LEVEL;
		WindowManager.OpenWindow (WindowName.GW_LEVEL_SELECT);
	}

	public void Menu ()
	{
		GameManager.gameState = GameState.SELECT_GAME;
		WindowManager.OpenWindow (WindowName.GW_GAME_SELECT);
	}

	public void SelectPack ()
	{
		GameManager.gameState = GameState.SELECT_PACK;
		WindowManager.OpenWindow (WindowName.GW_LEVEL_PACK);
	}

	public void Restart ()
	{
		if (OneLineGameplayControl.instance.isLevelEditor) {
			OneLineGameplayControl.instance.CreateLevelEditorBoard (OneLineGameplayControl.instance.boardW, OneLineGameplayControl.instance.boardH);
		} else {
			OneLineGameplayControl.instance.StartNewGame ();
		}
	}

	public void ShowHintPoupup ()
	{
		popupHint.gameObject.SetActive (true);
	}

	public void Hint ()
	{
		OneLineGameplayControl.instance.Hint ();
	}

}
