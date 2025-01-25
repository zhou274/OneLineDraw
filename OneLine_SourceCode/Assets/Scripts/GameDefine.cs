using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameDefine : MonoBehaviour
{
	public static GameDefine instance;
	int _hintCount;
	public Action OnHintCountChange;

	public static int hintCount {
		get { 
			return instance._hintCount;
		}
		set {
			if (value >= 0) {
				instance._hintCount = value;
				PlayerPrefs.SetInt ("HintCount", value);
				PlayerPrefs.Save ();
				if (instance.OnHintCountChange != null) {
					instance.OnHintCountChange ();
				}
			}
		}
	}

	void Awake ()
	{
		instance = this;
		hintCount = PlayerPrefs.GetInt ("HintCount", 20);
        Application.targetFrameRate = 60;

	}
	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < gameTypeConfig.listGames.Count; i++) {
			GameManager.currentGameName = gameTypeConfig.listGames [i].gameName;
			GameManager.LoadDataSave ();
		}
		GameManager.currentGameName = gameTypeConfig.listGames [0].gameName;
		GameManager.gameState = GameState.SELECT_GAME;
		WindowManager.OpenWindow (WindowName.GW_GAME_SELECT);
	}

	public GameTypeConfig gameTypeConfig;
	public GameColorDefine gameColorDefine;

	public Button bSound;
}
