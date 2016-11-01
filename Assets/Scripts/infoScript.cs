using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class infoScript : MonoBehaviour {

    public GameObject TitleText;
	public GameObject RankText;
    public GameObject HealthText;
    public GameObject DamageText;
	public GameObject AccuracyText;
	public GameObject ExpText;

	// Use this for initialization
	void Start () {
		TitleText = transform.GetChild (0).gameObject;
		RankText = transform.GetChild (1).gameObject;
		HealthText = transform.GetChild (2).gameObject;
		DamageText = transform.GetChild (3).gameObject;
		AccuracyText = transform.GetChild (4).gameObject;
		ExpText = transform.GetChild (5).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void display(GameObject soldier)
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
        TitleText.GetComponent<Text>().color = new Color(0, 0, 0, 1);
		RankText.GetComponent<Text> ().color = new Color (0, 0, 0, 1);
		ExpText.GetComponent<Text> ().color = new Color (0, 0, 0, 1);
        HealthText.GetComponent<Text>().color = new Color(0, 0, 0, 1);
        DamageText.GetComponent<Text>().color = new Color(0, 0, 0, 1);
        AccuracyText.GetComponent<Text>().color = new Color(0, 0, 0, 1);
        soldierScript s = soldier.GetComponent<soldierScript>();
        HealthText.GetComponent<Text>().text = "Health: " + s.health + "/" + s.maxHealth;
        switch (s.soldierType)
        {
            case soldierScript.type.infantry:
                TitleText.GetComponent<Text>().text = "Infantry";
                DamageText.GetComponent<Text>().text = "Damage: "+s.fireDamage;
                AccuracyText.GetComponent<Text>().text = "Accuracy: " + s.fireAccuracy;
                break;
            case soldierScript.type.artillery:
                TitleText.GetComponent<Text>().text = "Artillery";
                DamageText.GetComponent<Text>().text = "Damage: " + s.artilleryDamage;
                AccuracyText.GetComponent<Text>().text = "";
                break;
            case soldierScript.type.engineer:
                TitleText.GetComponent<Text>().text = "Engineer";
                DamageText.GetComponent<Text>().text = "";
                AccuracyText.GetComponent<Text>().text = "";
                break;
        }
		switch (s.soldierRank) {
		case soldierScript.rank.priv:
			RankText.GetComponent<Text> ().text = "Private";
			ExpText.GetComponent<Text> ().text = "Exp: " + s.exp + "/"+s.maxExp;
			break;
		case soldierScript.rank.lieutenant:
			RankText.GetComponent<Text>().text = "Lieutenant";
			ExpText.GetComponent<Text> ().text = "Exp: " + s.exp + "/"+s.maxExp;
			break;
		case soldierScript.rank.sergeant:
			RankText.GetComponent<Text>().text = "Sergeant";
			ExpText.GetComponent<Text> ().text = "";
			break;
		}
    }

    public void hide()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
		TitleText.GetComponent<Text>().color = new Color(0, 0, 0, 0);
		RankText.GetComponent<Text>().color = new Color(0, 0, 0, 0);
		ExpText.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        HealthText.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        DamageText.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        AccuracyText.GetComponent<Text>().color = new Color(0, 0, 0, 0);
    }
}
