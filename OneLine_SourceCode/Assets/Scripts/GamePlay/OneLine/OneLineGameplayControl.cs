using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.IO;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;

#if UNITY_EDITOR
using UnityEditor;

#endif
public class OneLineGameplayControl : MonoBehaviour
{
	// list các khối
	public List<OneLineBlock> listBlocks;
	//
	public GameObject blockPrefap;
	// khối đang được chọn
	public OneLineBlock currentBlock;
	//
	public static OneLineGameplayControl instance;
	public bool isDraging = false;
	public GameObject finger;
	// list các color của khối
	public List<GameElementColorType> listBlockColorType;
	// color hiện tại của khối
	public GameElementColorType currentBlockColorType;
	[Space (10)]
	[Header ("Use to create new levels")]
	// for create level
	[Tooltip ("Set true để bật chế độ tạo level cho level đang chọn")]
	public bool isLevelEditor = false;
	// độ lớn của board
	[Header ("Board size in LevelEditor Mode")]
	[Range (4, 10)]
	public int boardW;
	[Range (4, 10)]
	public int boardH;
    private StarkAdManager starkAdManager;

    public string clickid;

    void Awake ()
	{
		// tạo 100 block để pools 
		GameObject blockPools = new GameObject ();	
		SimplePool.Preload (blockPrefap, 100, blockPools.transform);
		instance = this;

		hintList = new List<OneLineBlock> ();
		// tạo danh sách các màu cho khối 
		listBlockColorType = new List<GameElementColorType> ();
		listBlockColorType.Add (GameElementColorType.BEGINNER_COLOR);
		listBlockColorType.Add (GameElementColorType.MEDIUM_COLOR);
		listBlockColorType.Add (GameElementColorType.EXPERT_COLOR);
		listBlockColorType.Add (GameElementColorType.MASTER_COLOR);
	}
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
    void OnDisable ()
	{
		// ẩn finger đi nếu đang bật
		if (finger != null && finger.activeInHierarchy) {
			HideFinger ();
		}
		// xóa các block 
		if (listBlocks != null) {
			foreach (OneLineBlock bl in listBlocks) {
				if (bl != null) {
					//Destroy (bl.gameObject);
					SimplePool.Despawn (bl.gameObject);
				}
			}
			listBlocks.Clear ();
		}
		CancelInvoke ();
	}
	// list thứ tự các khối hint
	List<OneLineBlock> hintList;

	public void StartNewGame ()
	{
		// lay ngau nhien mau cua block
		listBlockColorType.Shuffle ();
		currentBlockColorType = listBlockColorType [0];
		//clear data
		finger.SetActive (false);
		hintList.Clear ();
		hintCount = 0;
		isDraging = false;
		// xoa nhung block cu
		if (listBlocks != null) {
			foreach (OneLineBlock bl in listBlocks) {
				//Destroy (bl.gameObject);
				SimplePool.Despawn (bl.gameObject);
			}
			listBlocks.Clear ();
		} else {
			listBlocks = new List<OneLineBlock> ();
		}
		// load data leve moi 
		string levelData = GameManager.LoadLevel (GameManager.currentPackName, GameManager.currentLevel);
		if (levelData == "") {
			GW_GAME_PLAY.instance.Invoke ("SelectPack", 0.2F);
		}
		string[] dataChars = levelData.Split (new char[]{ ',' });
		// board width
		int w = int.Parse (dataChars [0]);
		// board height
		int h = int.Parse (dataChars [1]);
		// tao cac block va set data
		for (int x = 0; x < h; x++) {
			for (int y = 0; y < w; y++) {
				Vector3 pos = new Vector3 (-(float)w / 2 + 0.5F + y, (float)h / 2 - 0.5F - x);
				//GameObject blObj = (GameObject)Instantiate (blockPrefap, pos, Quaternion.Euler (0, 0, 0));
				GameObject blObj = SimplePool.Spawn (blockPrefap, pos, Quaternion.identity);
				OneLineBlock bl = blObj.GetComponent<OneLineBlock> ();
				bl.id = x * w + y;
				bl.name = "block_" + x + "_" + y;
				listBlocks.Add (bl);
				bl.isWall = true;
				bl.isFiled = false;
				bl.nextBlock = null;
			}
		}

		// đọc và set thứ tự , tr thự các block theo data 
		for (int i = 2; i < dataChars.Length; i++) {
			
			OneLineBlock bl = listBlocks [int.Parse (dataChars [i])];
			bl.isWall = false;
			hintList.Add (bl);
			if (i == 2) {
				bl.isFiled = true;
				currentBlock = bl;
				bl.startPosShowAni.tween.Restart (true);
				bl.startBlockDot.SetActive (true);
			}
		}
		//resize camera 
		CalcCamera (w, h);
		// hien thong tin cua level len UI
		GW_GAME_PLAY.instance.InitInfo ();
		// hien finger 
		if (GameManager.currentLevel == 1 && GameManager.currentPackType == PackType.Beginner) {
			ShowFinger ();
		}
		GameManager.gameState = GameState.PLAYING;
	}

	void CalcCamera (int col, int row)
	{
		// resize camera theo do lon cua board 
		if (col >= row) {
			// fit chiều ngang
			Camera.main.orthographicSize = col * Camera.main.pixelHeight / (Camera.main.pixelWidth * 2) + 1F;
		} else {
			// fit chiều dọc
			Camera.main.orthographicSize = row;
		}
	}

	// tạo board dạng level editor
	public 	void CreateLevelEditorBoard (int w, int h)
	{
		if (listBlocks != null) {
			foreach (OneLineBlock bl in listBlocks) {
				//Destroy (bl.gameObject);
				SimplePool.Despawn (bl.gameObject);
			}
			listBlocks.Clear ();
		} else {
			listBlocks = new List<OneLineBlock> ();
		}
		listBlockColorType.Shuffle ();
		currentBlockColorType = listBlockColorType [0];
		if (levelEditListBlockID != null) {
			levelEditListBlockID.Clear ();
		} else {
			levelEditListBlockID = new List<int> ();
		}
		boardW = w;
		boardH = h;
		for (int x = 0; x < h; x++) {
			for (int y = 0; y < w; y++) {
				Vector3 pos = new Vector3 (-(float)w / 2 + 0.5F + y, (float)h / 2 - 0.5F - x);
				//GameObject blObj = (GameObject)Instantiate (blockPrefap, pos, Quaternion.Euler (0, 0, 0));
				GameObject blObj = SimplePool.Spawn (blockPrefap, pos, Quaternion.identity);
				OneLineBlock bl = blObj.GetComponent<OneLineBlock> ();
				bl.nextBlock = null;
				bl.isFiled = false;
				bl.id = x * w + y;
				bl.name = "block_" + x + "_" + y;
				listBlocks.Add (bl);
				bl.isWall = false;
			}
		}
		CalcCamera (boardW, boardH);

	}

	public void BuildLevel (string text)
	{
		// tạo file level trong chế độ editor
		#if UNITY_EDITOR
		string fileName = "Level_" + GameManager.currentLevel + ".txt";
		string directory = "/Resources/Data/" + GameManager.currentGameName + "/" + GameManager.currentPackName + "/";
		if (!AssetDatabase.IsValidFolder ("Assets/Resources/LevelData")) {
			AssetDatabase.CreateFolder ("Assets/Resources", "LevelData");
			File.WriteAllText (Application.dataPath + directory + fileName, text);
			AssetDatabase.Refresh ();
		} else {
			File.WriteAllText (Application.dataPath + directory + fileName, text);
			AssetDatabase.Refresh ();
		}
		#endif
	}

	void OnEnable ()
	{		
		if (isLevelEditor) {	
			//Debug.Log ("Tao editor");
			CreateLevelEditorBoard (boardW, boardH);
			GameManager.gameState = GameState.PLAYING;
		} else {
			//Debug.Log ("start game");
			StartNewGame ();
		}
	}
	//list lưu thứ tự các block để tạo file level
	public List<int> levelEditListBlockID;

	void Update ()
	{
		if (Input.GetMouseButtonUp (0)) {
			isDraging = false;
			// tạo file level nếu đang ở chế độ level editor
			if (isLevelEditor) {
				if (levelEditListBlockID.Count > 0) {
					string t = boardW + "," + boardH;
					for (int i = 0; i < levelEditListBlockID.Count; i++) {
						t += "," + levelEditListBlockID [i];
					}
					BuildLevel (t);
				}
			}

		}
	}

	public void CheckWin ()
	{
		bool isWin = true;
		for (int i = 0; i < listBlocks.Count; i++) {
			if (listBlocks [i].isWall == false && listBlocks [i].isFiled == false) {
				return;
			}
		}
		if (finger.activeInHierarchy) {
			HideFinger ();
		}
		Debug.Log ("Chien Thang");
		GameManager.gameState = GameState.WIN;
		AudioManager.instance.PlaySound (AudioClipType.LevelSuccess);
		// hiệu ứng xoay các wall
		float time = 0;
		for (int i = 0; i < listBlocks.Count; i++) {
			if (listBlocks [i].isWall) {
				listBlocks [i].ani.tween.SetDelay (0.01F * i);
				listBlocks [i].ani.tween.Restart (true);
				time += 0.05F * i;
			}

		}
		// hiện popup win
		Invoke ("ShowWinPopup", 1);
        ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });

    }

	void ShowWinPopup ()
	{
		GW_GAME_PLAY.instance.popupWin.gameObject.SetActive (true);
	}

	int hintCount;
	[Space (10)]
	[Tooltip ("Số ô được fill sau 1 lần bấm hint")]
	public int hintStep = 3;

	public void Hint ()
	{
		// clear trạng thái các block về nofill 
		for (int i = 0; i < listBlocks.Count; i++) {
			if (listBlocks [i].isWall == false) {
				listBlocks [i].isFiled = false;
				listBlocks [i].nextBlock = null;
			}
		}
		hintList [0].isFiled = true;
		// fill các block 
		for (int i = 0; i < (hintCount + 1) * hintStep; i++) {
			if (i < hintList.Count - 1) {
				hintList [i].nextBlock = hintList [i + 1];
				hintList [i + 1].isFiled = true;
				currentBlock = hintList [i + 1];
			}
		}
		hintCount++;
		CheckWin ();
	}

	void HideFinger ()
	{
		finger.transform.DOKill ();
		finger.SetActive (false);
	}

	void ShowFinger ()
	{
		// tạo path di chuyển cho finger 
		List<Vector3> path = new List<Vector3> ();
		for (int i = 0; i < hintList.Count; i++) {
			path.Add (hintList [i].transform.position);
		}
		//active và di chuyển finger theo path
		finger.SetActive (true);
		finger.transform.position = path [0];
		finger.transform.DOKill ();
		finger.transform.DOPath (path.ToArray (), 4, PathType.Linear, PathMode.Full3D).SetEase (Ease.Linear).SetDelay (0.5F).SetLoops (-1, LoopType.Restart);
	}
}
