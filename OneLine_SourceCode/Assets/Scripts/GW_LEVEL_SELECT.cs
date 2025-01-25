using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GW_LEVEL_SELECT : MonoBehaviour
{
	public TextMeshProUGUI tTitle;
	public static GW_LEVEL_SELECT instance;
	public GridView gridView;

	List<SelectLevelButton> listLevelButton;

	public Color normalColor, passedColor;
	List<int> listLevelPassed;

	void Awake ()
	{
		instance = this;
		listLevelButton = new List<SelectLevelButton> ();
	}

	void Start ()
	{
        /*
		int totalLevel = GameManager.currentPackModule.LevelsCount;
		for (int i = 0; i < totalLevel; i++) {
			SelectLevelButton bt = gridView.AddContent ().GetComponent<SelectLevelButton> ();
			bt.level = i + 1;
			//hieu ung hien ra
			bt.aniShow.tween.SetDelay ((i + 1) * 0.01F);
			bt.aniShow.tween.Play ();
			listLevelButton.Add (bt);
			bt.passed = false;
		}
		GameManager.GetListLevelPassed (GameManager.currentPackType, ref listLevelPassed);
		for (int i = 0; i < listLevelPassed.Count; i++) {
			// phai tru 1 vi list bat dau tu vi tri 0 , level lai bat dau tu 1
			listLevelButton [listLevelPassed [i] - 1].passed = true;
		}

*/
	}

	void OnDisable ()
	{
		gridView.ClearAllContent ();
		listLevelButton.Clear ();
	}

	void OnEnable ()
	{

		int totalLevel = GameManager.currentPackModule.LevelsCount;
		for (int i = 0; i < totalLevel; i++) {
			SelectLevelButton bt = gridView.AddContent ().GetComponent<SelectLevelButton> ();
			bt.level = i + 1;
			//hieu ung hien ra
			bt.aniShow.tween.SetDelay ((i + 1) * 0.01F);
			bt.aniShow.tween.Play ();
			listLevelButton.Add (bt);
			bt.passed = false;
		}

		// doi pack name
		tTitle.text = GameManager.currentPackName;
		// lay mau cho nut
		normalColor = GameDefine.instance.gameColorDefine.ListGameColors [GameManager.currentGameColorModeID].levelButtonNormal;
		passedColor = GameDefine.instance.gameColorDefine.ListGameColors [GameManager.currentGameColorModeID].levelButtonPassed;

		if (listLevelButton != null) {
			// hieu ung hien ra
			for (int i = 0; i < listLevelButton.Count; i++) {
				listLevelButton [i].aniShow.tween.Restart (true);

			}
			// set lai mau cho cac level da pass
			GameManager.GetListLevelPassed (GameManager.currentPackType, ref listLevelPassed);
			for (int i = 0; i < listLevelButton.Count; i++) {
				listLevelButton [i].passed = false;
			}
			for (int i = 0; i < listLevelPassed.Count; i++) {
				listLevelButton [listLevelPassed [i] - 1].passed = true;
			}

		}
	}

	public void HideAllLevelTween ()
	{
		// hieu ung bien mat
		for (int i = 0; i < listLevelButton.Count; i++) {
			listLevelButton [i].aniHide.tween.Restart ();
		}
	}

	public void GotoSelectLevelPack ()
	{
		GameManager.gameState = GameState.SELECT_PACK;
		WindowManager.OpenWindow (WindowName.GW_LEVEL_PACK);
	}
}
