using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {
	GameObject canvas;
	Player player;
	//load the popup data
	TextAsset data;
	//init json object
	JSONObject popupData;
	Dictionary<string, string> popups;

	GameObject popupObject;

	public void LoadPopups() {
		data = Resources.Load("common/popups") as TextAsset;
		popupData = new JSONObject(data.text);
		popups = new Dictionary<string, string>();
		for (int i = 0; i < popupData.list[0].Count; i++) {
			JSONObject popup = popupData.list[0][i];
			popups.Add(popup[0].str, popup[1].str);
		}
	}

	public void InitUI() {
		LoadPopups();

		//pick the correct canvas
		canvas = GameObject.Find("ButtonCanvas");
		player = Global.Instance.battleManager.player;
		popupObject = canvas.transform.Find("Popup").gameObject;
		popupObject.SetActive(false);
		//for each soldier
		for (int i = 0; i < player.soldierObjects.Length; i++) {
			//load the prefab
			GameObject portrait = Instantiate(Resources.Load("prefabs/ui/PortraitButton")) as GameObject;
			//add it as a child of the correct UI item
			portrait.transform.SetParent(canvas.gameObject.transform.Find("PortraitBar"), false);
			//load the unit's portrait
			portrait.transform.Find("PortraitImage").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("graphics/" + player.soldierStats[i].portrait);
			//load the unit's name
			portrait.transform.Find("Name").gameObject.GetComponent<Text>().text = player.soldierObjects[i].name;
			portrait.name = player.soldierObjects[i].name;
			//load the unit's movement points
			portrait.transform.Find("InfoPanel/MovementText").gameObject.GetComponent<Text>().text = player.soldierStats[i].baseMovementPoints.ToString();
			//copy i into new var
			int s = i;
			//add event listener
			portrait.GetComponent<Button>().onClick.AddListener(delegate{Global.Instance.battleManager.SoldierSelected(s);});
		}

		GameObject endTurn = canvas.transform.Find("InfoBar/EndTurnButton").gameObject;
		endTurn.GetComponent<Button>().onClick.AddListener(delegate{Global.Instance.battleManager.EndTurn();});


		AddOrderButton("shoot");
		AddOrderButton("run");
		AddOrderButton("overwatch");

		GameObject panel = GameObject.Find("ButtonCanvas/InfoBar");
		AddPopup(panel.transform.Find("Stats/Strength").gameObject, "strength");
		AddPopup(panel.transform.Find("Stats/Toughness").gameObject, "toughness");
		AddPopup(panel.transform.Find("Stats/Initiative").gameObject, "initiative");
		AddPopup(panel.transform.Find("Stats/WS").gameObject, "weaponSkill");
		AddPopup(panel.transform.Find("Stats/BS").gameObject, "ballisticSkill");

		Unit.OnPointsChanged += UpdatePoints;
	}

	private void AddPopup(GameObject item, string popup) {
		EventTrigger trigger = item.GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate{MouseOver(popup);});
		trigger.triggers.Add(entry);

		EventTrigger.Entry entry1 = new EventTrigger.Entry();
		entry1.eventID = EventTriggerType.PointerExit;
		entry1.callback.AddListener(delegate{MouseOut(popup);});
		trigger.triggers.Add(entry1);
	}

	private void AddOrderButton(string order) {
		GameObject button = Instantiate(Resources.Load("prefabs/ui/OrderButton")) as GameObject;
		button.transform.SetParent(canvas.gameObject.transform.Find("OrderBar"), false);
		button.transform.Find("PortraitImage").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("graphics/ui/" + order);
		button.GetComponent<Button>().onClick.AddListener(delegate{Global.Instance.battleManager.OrderSelected(order);});
		
		EventTrigger trigger = button.GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate{MouseOver(order);});
		trigger.triggers.Add(entry);
		

		EventTrigger.Entry entry1 = new EventTrigger.Entry();
		entry1.eventID = EventTriggerType.PointerExit;
		entry1.callback.AddListener(delegate{MouseOut(order);});
		trigger.triggers.Add(entry1);
		//button.GetComponent<Button>().OnMouseEnter.AddListener(delegate{RollOver(order);});
	}

	public void MouseOver(string order) {
		popupObject.transform.GetChild(0).GetComponent<Text>().text = popups[order];
		popupObject.SetActive(true);
	}

	public void MouseOut(string order) {
		popupObject.SetActive(false);
	}

	public void UpdatePoints(string soldier, int points, string type) {
		Transform portrait = canvas.transform.Find("PortraitBar/" + soldier);
		Text textComponent = portrait.transform.Find("InfoPanel/" + type + "Text").gameObject.GetComponent<Text>();
		textComponent.text = points.ToString();

		GameObject.Find("ButtonCanvas/InfoBar/" + type + "Text").gameObject.GetComponent<Text>().text = points.ToString();

		Color newColor;
		if (points <= 0) {
			newColor = new Color(1f, 1f, 1f, 0.25f);
		} else {
			newColor = new Color(1f, 1f, 1f, 1f);
		}
		textComponent.color = newColor;
		portrait.transform.Find("InfoPanel/" + type + "Icon").gameObject.GetComponent<Image>().color = newColor;
	}

	public void HighlightSoldier(int selectedSoldier) {
		Transform portrait;
		Color color;
		Transform outliner = canvas.transform.Find("PortraitBar");
		for (int i = 0; i < outliner.childCount; i++) {
			portrait = outliner.GetChild(i);
			color = new Color(1f, 1f, 1f, 0.25f);
			portrait.GetComponent<Image>().color = color;
		}
		portrait = outliner.GetChild(selectedSoldier);
		color = new Color(1f, 1f, 1f, 1f);
		portrait.GetComponent<Image>().color = color;

		GameObject panel = GameObject.Find("ButtonCanvas/InfoBar");
		panel.transform.Find("PortraitImage").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("graphics/" + player.soldierStats[selectedSoldier].portrait);
		panel.transform.Find("NameText").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].myName;
		panel.transform.Find("MovementText").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].currentMovementPoints.ToString();
		panel.transform.Find("ActionText").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].currentActionPoints.ToString();
		panel.transform.Find("Stats/Strength/Text").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].strength.ToString();
		panel.transform.Find("Stats/Toughness/Text").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].toughness.ToString();
		panel.transform.Find("Stats/Initiative/Text").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].initiative.ToString();
		panel.transform.Find("Stats/WS/Text").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].weaponSkill.ToString();
		panel.transform.Find("Stats/BS/Text").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].ballisticSkill.ToString();
		
		panel.transform.Find("WeaponStats/WeaponImage").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("graphics/weapons/" + player.soldierStats[selectedSoldier].weapon1);
		panel.transform.Find("WeaponStats/WeaponName").gameObject.GetComponent<Text>().text = player.soldierStats[selectedSoldier].weapon1;
	}
}