using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupRewardVideo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Cancel()
    {
        gameObject.SetActive(false);
        GameManager.gameState = GameState.PLAYING;
    }

    public void GetMoreHints()
    {
        GameManager.gameState = GameState.PLAYING;
        AdsControl.Instance.ShowRewardVideo();
        gameObject.SetActive(false);
    }
}
