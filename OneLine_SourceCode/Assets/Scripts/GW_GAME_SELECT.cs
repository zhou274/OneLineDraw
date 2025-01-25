using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GW_GAME_SELECT : MonoBehaviour
{
	public static GW_GAME_SELECT instance;

	void Awake ()
	{
		instance = this;
	}

	// ícac ínut chon game
	public List<GameSelectButton> listGameSelectButtons;
	// số game nhỏ
	int maxID = 6;

	public void GoToLevelPacksSelect ()
	{
		GameManager.LoadGameData (GameManager.currentGameName);
		GameManager.gameState = GameState.SELECT_PACK;
		WindowManager.OpenWindow (WindowName.GW_LEVEL_PACK);
	}

	void Start ()
	{
		// lấy sốấyấyấyấy game nhỏ 
		maxID = GameDefine.instance.gameTypeConfig.listGames.Count;

		for (int i = 0; i < listGameSelectButtons.Count; i++) {
			//set kểuieu game nhỏ cho nut
			if (i - 1 >= 0) {
				listGameSelectButtons [i].id = i - 1;
			} else {
				listGameSelectButtons [i].id = maxID - 1;
			}

		}

	}

	//
	bool isDraging = false;
	Vector3 mousePos;
	Vector2 mousePos2D;

	// dung de luu tam 1 vector
	Vector2 posTemp;

	// dung de luu tam vector vi tri cuoi cung cua 1 obj truoc khi no thay doi vi tri
	Vector2 lastPosTemp;
	int idTemp;
	Vector2 initMovePos;
	MoveDir mvDir;
	public bool canClickButtonGameType = true;

	void Update ()
	{
        /*
		mousePos = Input.mousePosition;
		mousePos2D.x = mousePos.x;
		mousePos2D.y = mousePos.y;
		if (Input.GetMouseButtonDown (0)) {
			isDraging = true;
			for (int i = 0; i < listGameSelectButtons.Count; i++) {
				listGameSelectButtons [i].offset = mousePos2D - listGameSelectButtons [i].rectTrans.anchoredPosition;
			}
			initMovePos = mousePos2D;

		}


		if (isDraging) {
			if (Vector2.Distance (mousePos, initMovePos) > 10) {
				canClickButtonGameType = false;
			}
			// di chuyen cac nut chon game theo chuot
			for (int i = 0; i < listGameSelectButtons.Count; i++) {
				GameSelectButton bt = listGameSelectButtons [i];
				posTemp = mousePos2D - bt.offset;
				posTemp.y = 0;
				lastPosTemp = bt.rectTrans.anchoredPosition;
				bt.rectTrans.anchoredPosition = posTemp;

				if (bt.rectTrans.anchoredPosition.x > 1080 && lastPosTemp.x < posTemp.x) {
					

					bt.rectTrans.anchoredPosition = new Vector2 (bt.rectTrans.anchoredPosition.x - 2160, bt.rectTrans.anchoredPosition.y);
					bt.offset = mousePos2D - bt.rectTrans.anchoredPosition;
					posTemp = bt.rectTrans.anchoredPosition;
					idTemp = bt.id - 3;
					if (idTemp < 0) {
						bt.id = maxID + idTemp;
					} else {
						bt.id = idTemp;
					}
				}
				if (bt.rectTrans.anchoredPosition.x < -1080 && lastPosTemp.x > posTemp.x) {
					

					bt.rectTrans.anchoredPosition = new Vector2 (bt.rectTrans.anchoredPosition.x + 2160, bt.rectTrans.anchoredPosition.y);
					bt.offset = mousePos2D - bt.rectTrans.anchoredPosition;
					idTemp = bt.id + 3;
					if (idTemp >= maxID) {
						bt.id = idTemp - maxID;
					} else {
						bt.id = idTemp;
					}
				}
						
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			isDraging = false;
			listGameSelectButtons.Sort ((x, y) => x.rectTrans.anchoredPosition.x.CompareTo (y.rectTrans.anchoredPosition.x));
			for (int i = 0; i < listGameSelectButtons.Count; i++) {
				GameSelectButton bt = listGameSelectButtons [i];
				bt.rectTrans.DOLocalMoveX (-720 + 720 * i, 0.5F).SetEase (Ease.OutBack).OnComplete (() => {
					canClickButtonGameType = true;
				});
			}
		}
		*/
	}

	public void NoAds ()
	{
		Debug.Log ("NoAds");
	}

	public void ViewLeaderboard ()
	{
		Debug.Log ("View Leaderboard");
	}
}

enum MoveDir
{
	Left,
	Right
}