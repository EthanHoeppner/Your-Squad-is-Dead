using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class reinforcementMenuScript : MonoBehaviour {
    
    public bool active;
    controller c;

    public enum reinforcementType
    {
        none,
        infantry,
        artillery,
        engineer,
        frag,
        gas,
        spotlight,
        gatling,
        heal,
		bomb,
		light
    }

    public reinforcementType[] types;
    public int[] costs;
    public Sprite infantrySprite;
    public Sprite artillerySprite;
    public Sprite engineerSprite;
    public Sprite fragSprite;
    public Sprite gasSprite;
    public Sprite spotlightSprite;
    public Sprite gatlingSprite;
    public Sprite healSprite;
	public Sprite bombSprite;
	public Sprite lightSprite;

    // Use this for initialization
    void Start () {
        active = false;
        deactivate();
        types = new reinforcementType[6];
        costs = new int[6];

        c = Camera.main.gameObject.GetComponent<controller>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void activate()
    {
        GetComponent<Text>().text = "Select Reinforcements\n"+c.reinforcementPoints+" Points Left";

        bool[] useTypes = new bool[10];
        for (int i2 = 0; i2 < 10; i2++)
        {
            useTypes[i2] = true;
        }
        int useCount = 10;
        while(useCount>6)
        {
            int ind = Random.Range(0,10);
            if(useTypes[ind])
            {
                useTypes[ind] = false;
                useCount--;
            }
        }


        int num = 0;
        for(int i=0;i<10;i++)
        {
            if(useTypes[i])
            {
                types[num] = (reinforcementType)(1 + i);
                num++;
            }
        }
        for(int i=0;i<6;i++)
        {
            switch(types[i])
            {
            case reinforcementType.infantry:
            case reinforcementType.artillery:
            case reinforcementType.engineer:
			case reinforcementType.gatling:
			case reinforcementType.bomb:
                costs[i] = 2;
                break;
            case reinforcementType.frag:
            case reinforcementType.gas:
            case reinforcementType.heal:
            case reinforcementType.spotlight:
			case reinforcementType.light:
                costs[i] = 1;
                break;
            }
        }


        for (int i=0;i<6;i++)
        {
            GameObject button = transform.GetChild(i).gameObject;
            switch (types[i])
            {
            case reinforcementType.none:
                button.SetActive(false);
                break;
            case reinforcementType.infantry:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = infantrySprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Infantry ("+costs[i]+")";
                break;
            case reinforcementType.artillery:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = artillerySprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Artillery (" + costs[i] + ")";
                break;
            case reinforcementType.engineer:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = engineerSprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Engineer (" + costs[i] + ")";
                break;
            case reinforcementType.frag:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = fragSprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Frag Grenade (" + costs[i] + ")";
                break;
            case reinforcementType.gas:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = gasSprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Gas Grenade (" + costs[i] + ")";
                break;
            case reinforcementType.gatling:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = gatlingSprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Gatling Gun (" + costs[i] + ")";
                break;
            case reinforcementType.spotlight:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = spotlightSprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Spotlight (" + costs[i] + ")";
                break;
            case reinforcementType.heal:
                button.SetActive(true);
                button.GetComponent<Image>().sprite = healSprite;
                button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Heal (" + costs[i] + ")";
                break;
			case reinforcementType.bomb:
				button.SetActive(true);
				button.GetComponent<Image>().sprite = bombSprite;
				button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Bombing (" + costs[i] + ")";
				break;
			case reinforcementType.light:
				button.SetActive(true);
				button.GetComponent<Image>().sprite = lightSprite;
				button.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Illuminate (" + costs[i] + ")";
				break;
            }
        }
        transform.GetChild(6).gameObject.SetActive(true);
    }
    public void deactivate()
    {
        GetComponent<Text>().text = "";
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false);
        transform.GetChild(6).gameObject.SetActive(false);
    }

    public void select(int index)
    {
        if (c.reinforcementPoints >= costs[index])
        {
            c.reinforcementPoints -= costs[index];
            transform.GetChild(index).gameObject.SetActive(false);
            switch (types[index])
            {
            case reinforcementType.infantry:
                c.createSoldier(new Vector2(Random.Range(c.xDim / 5, 4 * (c.xDim / 5)), 0), soldierScript.type.infantry);
                break;
            case reinforcementType.artillery:
                c.createSoldier(new Vector2(Random.Range(c.xDim / 5, 4 * (c.xDim / 5)), 0), soldierScript.type.artillery);
                break;
            case reinforcementType.engineer:
                c.createSoldier(new Vector2(Random.Range(c.xDim / 5, 4 * (c.xDim / 5)), 0), soldierScript.type.engineer);
                break;
            case reinforcementType.frag:
                foreach(GameObject s in c.soldiers)
                {
                    s.GetComponent<soldierScript>().fragCount++;
                }
                break;
            case reinforcementType.gas:
                foreach (GameObject s in c.soldiers)
                {
                    s.GetComponent<soldierScript>().gasCount++;
                }
                break;
            case reinforcementType.gatling:
                foreach (GameObject s in c.soldiers)
                {
                    if (s.GetComponent<soldierScript>().soldierType == soldierScript.type.engineer)
                    {
                        s.GetComponent<soldierScript>().gatlingCount++;
                    }
                }
                break;
            case reinforcementType.spotlight:
                foreach (GameObject s in c.soldiers)
                {
                    if (s.GetComponent<soldierScript>().soldierType == soldierScript.type.engineer)
                    {
                        s.GetComponent<soldierScript>().spotlightCount++;
                    }
                }
                break;
            case reinforcementType.heal:
                foreach (GameObject s in c.soldiers)
                {
                    s.GetComponent<soldierScript>().health = s.GetComponent<soldierScript>().maxHealth;
                }
                break;
			case reinforcementType.bomb:
				c.bombs++;
				break;
			case reinforcementType.light:
				c.lights++;
				break;
            }
            GetComponent<Text>().text = "Select Reinforcements\n" + c.reinforcementPoints + " Points Left";
        }
    }
}
