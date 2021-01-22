using UnityEngine;
using System.Collections;
using InitScriptName;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;
	public GraphicRaycaster raycaster;
	public GameObject MenuPlay;
	public GameObject MenuPause;
	public GameObject MenuWin;
	public GameObject MenuFailed;
	public GameObject MenuCurrencyShop;
	public GameObject PrePlayBanner;
	public GameObject PreFailedBanner;
	public GameObject PreWinBanner;
	public GameObject RewardPopup;
	public GameObject MenuLifeShop;
	public GameObject MenuBoostShop;
	public GameObject MenuSettings;
	public GameObject MenuPurchased;
	public GameObject MenuTutorial;
	public GameObject Loading;

	public GameObject CongratulationsMenu;

	public delegate void MenuDelegate();

	public static MenuDelegate OnMenuLeadboard;

	void Awake()
	{
		Instance = this;
		Loading = GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject;
		//raycaster.enabled = false;
	}

	void OnEnable()
	{
		GameEvent.OnStatus += OnStatusChanged;
	}

	void OnDisable()
	{
		GameEvent.OnStatus -= OnStatusChanged;
	}

	void OnStatusChanged(GameState status)
	{
		//raycaster.enabled = true;
		if (status == GameState.PlayMenu) {
			MenuPlay.SetActive(true);
			if (OnMenuLeadboard != null)
				OnMenuLeadboard();
		}

		if (status == GameState.WinMenu) {
			MenuWin.SetActive(true);
			if (OnMenuLeadboard != null)
				OnMenuLeadboard();
		}
		if (status == GameState.GameOver) {
			MenuFailed.SetActive(true);
			InitScript.Instance.SpendLife(1);
			if (OnMenuLeadboard != null)
				OnMenuLeadboard();
		}

		if (status == GameState.Pause) {
			MenuPause.SetActive(true);
		}

		if (status == GameState.PreFailed) {
			PreFailedBanner.SetActive(true);
		}

		if (status == GameState.PrePlayBanner) {
			PrePlayBanner.SetActive(true);
		}

		if (status == GameState.WinBanner) {
			// PreWinBanner.SetActive (true);
		}

	}

	public void OnCloseMenuEvent()
	{
		//raycaster.enabled = false;

	}

	public void ShowCurrencyShop()
	{
		MenuCurrencyShop.SetActive(true);
	}

	public void ShowLifeShop()
	{
		MenuLifeShop.SetActive(true);
	}

	public void ShowPurchased(BoostType bType)
	{
		MenuPurchased.GetComponent<PurchasedMenu>().SetIconSprite(bType);
		MenuPurchased.SetActive(true);
	}

	public void ShowTutorial()
	{
		MenuTutorial.SetActive(true);
	}
}
