using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GW_LEVEL_PACK_SELECT : MonoBehaviour
{
	public List<LevelPackItem> listLevelPackItem;
	public TextMeshProUGUI tGameName;

	void OnEnable ()
	{
		//tGameName.text = GameManager.currentGameType.gameNameShow;
		for (int i = 0; i < listLevelPackItem.Count; i++) {
			LevelPackModule lvPack = GameManager.GetGameConfigByName (GameManager.currentGameName).levelPacks [i];
			listLevelPackItem [i].id = i;
			listLevelPackItem [i].levelPack = lvPack;
		}
	}

	public void BackToGameSelect ()
	{
		GameManager.gameState = GameState.SELECT_GAME;
		WindowManager.OpenWindow (WindowName.GW_GAME_SELECT);
	}
}
