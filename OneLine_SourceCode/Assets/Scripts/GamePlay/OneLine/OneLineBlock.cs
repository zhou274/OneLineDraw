using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OneLineBlock : MonoBehaviour
{
	// vòng tròn trắng hiển thị trên block đầu tiên
	public GameObject startBlockDot;
	// tween
	public DOTweenAnimation ani, startPosShowAni;
	//
	public SpriteRenderer wallRender;
	bool _isWall;

	public int id;

	public GameObject fillObj;
	// lineRenderer
	public LineRenderer surfaceLine, bodyLine, shadowLine, whiteLine;
	// khối liền sau của khối hiện tại (dùng để vẽ line )
	OneLineBlock _nextBlock;
	// độ lệch giữa các vùng surface,dark,shadow (dùng để vẽ line dạng khối có đổ bóng)
	Vector3 surfaceLineKC = new Vector3 (0, 0.02F, 0);
	Vector3 shadowLineKC = new Vector3 (0, -0.05F, 0);
	Vector3 bodyLineKC = new Vector3 (0, -0.02F, 0);
	// biến check đang drag hay không
	public bool isDraging = false;

	[Space (10)]
	[Header ("Color")]
	// các sprite renderer 
	public SpriteRenderer cellSurface;
	public SpriteRenderer cellDark;
	public SpriteRenderer blockSurface, blockDark, blockShadow;

	//
	public OneLineBlock nextBlock {
		get { return _nextBlock; }
		set {
			_nextBlock = value;
			// set các điểm đầu cho các lineRenderer
			surfaceLine.SetPosition (0, transform.position + surfaceLineKC);
			bodyLine.SetPosition (0, transform.position + bodyLineKC);
			shadowLine.SetPosition (0, transform.position + shadowLineKC);
			whiteLine.SetPosition (0, transform.position);
			// vẽ line nếu tồn tại obj liền sau
			if (value != null) {
				surfaceLine.enabled = true;
				bodyLine.enabled = true;
				shadowLine.enabled = true;
				whiteLine.enabled = true;
				surfaceLine.SetPosition (1, value.transform.position + surfaceLineKC);
				bodyLine.SetPosition (1, value.transform.position + bodyLineKC);
				shadowLine.SetPosition (1, value.transform.position + shadowLineKC);
				whiteLine.SetPosition (1, value.transform.position);
			} else {
				surfaceLine.enabled = false;
				bodyLine.enabled = false;
				shadowLine.enabled = false;
				whiteLine.enabled = false;
			}
		}
	}

	// set wall
	public bool isWall {
		get{ return _isWall; }
		set {
			_isWall = value;
			if (value == true) {
				wallRender.enabled = true;
			} else {
				wallRender.enabled = false;
			}
		}
	}

	bool _isFilled;

	// set fill
	public bool isFiled {
		get { 
			return _isFilled;
		}
		set { 
			_isFilled = value;
			if (value) {
				fillObj.SetActive (true);

			} else {
				fillObj.SetActive (false);
			}
		}
	}

	void Awake ()
	{
		startBlockDot.SetActive (false);
		isFiled = false;
		nextBlock = null;
		GameColorManager.instance.onColorChange += new System.Action (this.ChangeCellColor);
	}

	void Start ()
	{
		SetBlockColor ();
		ChangeCellColor ();
	}

	void OnMouseDown ()
	{
		if (GameManager.gameState != GameState.PLAYING) {
			return;
		}
		if (OneLineGameplayControl.instance.isLevelEditor) {
			// chế độ tạo level
			OneLineGameplayControl.instance.currentBlock = this;
			OneLineGameplayControl.instance.isDraging = true;
			isFiled = true;
			OneLineGameplayControl.instance.levelEditListBlockID.Add (id);

		} else {
			// chế độ chơi 
			if (OneLineGameplayControl.instance.currentBlock == this) {
				OneLineGameplayControl.instance.isDraging = true;

			}
		}
	}

	void OnDestroy ()
	{
		GameColorManager.instance.onColorChange -= this.ChangeCellColor;
	}

	void OnMouseEnter ()
	{
		if (GameManager.gameState != GameState.PLAYING) {
			return;
		}
		if (OneLineGameplayControl.instance.isDraging) {
			if (isFiled == false && isWall == false) {
				// fill block
				// chỉ tính là fill nếu block này ở cạnh block đã fill trước đó
				if (Vector3.Distance (transform.position, OneLineGameplayControl.instance.currentBlock.transform.position) <= 1) {
					OneLineGameplayControl.instance.currentBlock.nextBlock = this;
					OneLineGameplayControl.instance.currentBlock = this;

					isFiled = true;

					AudioManager.instance.PlaySound (AudioClipType.PlacePipe);
					if (OneLineGameplayControl.instance.isLevelEditor) {
						OneLineGameplayControl.instance.levelEditListBlockID.Add (id);
					} else {
						// kiểm tra thắng ngay sau khi fill
						OneLineGameplayControl.instance.CheckWin ();
					}
				}

			} else {
				// unfill block nếu user đi vòng lại 
				if (nextBlock == OneLineGameplayControl.instance.currentBlock) {
					OneLineGameplayControl.instance.currentBlock.isFiled = false;
					if (OneLineGameplayControl.instance.isLevelEditor) {
						OneLineGameplayControl.instance.levelEditListBlockID.Remove (nextBlock.id);
					}
					nextBlock = null;
					OneLineGameplayControl.instance.currentBlock = this;
					AudioManager.instance.PlaySound (AudioClipType.PlacePipe);

				}
			}
		}
	}

	void ChangeCellColor ()
	{
		Color col = GameColorManager.GetColor (GameElementColorType.GP_CELL);
		cellDark.color = GameManager.GetDarkColor (col);
		ChangeShadowColor ();
	}

	void ChangeShadowColor ()
	{
		// shadow color = dark color của background 
		Color bgColor = GameColorManager.GetColor (GameElementColorType.BACKGROUND);
		blockShadow.color = GameManager.GetDarkColor (bgColor);
		shadowLine.startColor = blockShadow.color;
		shadowLine.endColor = blockShadow.color;
	}

	void SetBlockColor ()
	{
		Color col = GameColorManager.GetColor (OneLineGameplayControl.instance.currentBlockColorType);
		Color bgColor = GameColorManager.GetColor (GameElementColorType.BACKGROUND);
		blockSurface.color = col;
		blockDark.color = GameManager.GetDarkColor (col);
		blockShadow.color = GameManager.GetDarkColor (bgColor);

		surfaceLine.startColor = col;
		surfaceLine.endColor = col;

		bodyLine.startColor = blockDark.color;
		bodyLine.endColor = blockDark.color;

		shadowLine.startColor = blockShadow.color;
		shadowLine.endColor = blockShadow.color;
	}
}

[System.Serializable]
public class CustomColor
{
	public Color surfaceColor;
	public Color darkToneColor;
	public Color shadowColor;
}