using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class selectionScript : MonoBehaviour
{

    public GameObject selectedSoldier;
    public GameObject infoButton;
    public GameObject digButton;
    public GameObject groupButton;
    public GameObject groupMoveButton;
    public GameObject groupAttackButton;
    public GameObject fragButton;
    public GameObject gasButton;
    public GameObject infoScreen;
    public GameObject attackButton;
    public GameObject retreatButton;
    public GameObject spotlightButton;
    public GameObject gatlingButton;
	public GameObject bombButton;
	public GameObject lightButton;
    public List<GameObject> soldierGroup;

    public GameObject rangeArtillery;
    public GameObject rangeInfantry;
    public GameObject rangeGrenade;
    public GameObject rangeGatling;

    bool buttonReleased;

    controller c;

    public enum phase
    {
        none,
        movingSoldier,
        fire,
        dig,
        artillery,
        look,
        info,
        attack,
        group,
        frag,
        gas,
		bombing,
		lighting,
        groupMove,
        groupAttack
    }

    public phase selectionPhase;

    // Use this for initialization
    void Start()
    {
        infoScreen = GameObject.FindGameObjectWithTag("infoScreen");
        infoButton = GameObject.FindGameObjectWithTag("buttonInfo");
        digButton = GameObject.FindGameObjectWithTag("buttonDig");
        groupButton = GameObject.FindGameObjectWithTag("buttonGroup");
        groupMoveButton = GameObject.FindGameObjectWithTag("buttonGroupMove");
        groupAttackButton = GameObject.FindGameObjectWithTag("buttonGroupAttack");
        fragButton = GameObject.FindGameObjectWithTag("buttonFrag");
        gasButton = GameObject.FindGameObjectWithTag("buttonGas");
        attackButton = GameObject.FindGameObjectWithTag("buttonAttack");
        retreatButton = GameObject.FindGameObjectWithTag("buttonRetreat");
        spotlightButton = GameObject.FindGameObjectWithTag("buttonSpotlight");
		gatlingButton = GameObject.FindGameObjectWithTag("buttonGatling");
		bombButton = GameObject.FindGameObjectWithTag("buttonBomb");
		lightButton = GameObject.FindGameObjectWithTag("buttonLight");
        soldierGroup = new List<GameObject>();
        selectionPhase = phase.none;
        buttonReleased = false;
        rangeArtillery = GameObject.FindGameObjectWithTag("rangeIndicatorArtillery");
        rangeInfantry = GameObject.FindGameObjectWithTag("rangeIndicatorInfantry");
        rangeGrenade = GameObject.FindGameObjectWithTag("rangeIndicatorGrenade");
        rangeGatling = GameObject.FindGameObjectWithTag("rangeIndicatorGatling");

        c = Camera.main.gameObject.GetComponent<controller>();
    }

    // Update is called once per frame
    void Update()
    {
		if (c.lights > 0) {
			lightButton.SetActive (true);
			lightButton.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = c.lights.ToString();
		} else {
			lightButton.SetActive (false);
		}
		if (c.bombs > 0) {
			bombButton.SetActive (true);
			bombButton.transform.GetChild (0).GetChild (0).GetComponent<Text> ().text = c.bombs.ToString();
		} else {
			bombButton.SetActive (false);
		}
        if (selectionPhase == phase.group || selectionPhase == phase.groupMove || selectionPhase == phase.groupAttack)
        {
            transform.position = new Vector3(-100, -100, -9);
            infoButton.SetActive(false);
            digButton.SetActive(false);
            fragButton.SetActive(false);
            attackButton.SetActive(false);
            retreatButton.SetActive(false);
            spotlightButton.SetActive(false);
            gatlingButton.SetActive(false);
            gasButton.SetActive(false);
            groupButton.SetActive(true);
            groupMoveButton.SetActive(true);
            groupAttackButton.SetActive(true);
        }
        else
        {
            groupMoveButton.SetActive(false);
            groupAttackButton.SetActive(false);
            if (selectedSoldier == null)
            {
                transform.position = new Vector3(-100, -100, -9);
                infoButton.SetActive(false);
                digButton.SetActive(false);
                fragButton.SetActive(false);
                gasButton.SetActive(false);
                groupButton.SetActive(true);
                attackButton.SetActive(false);
                retreatButton.SetActive(false);
                spotlightButton.SetActive(false);
                gatlingButton.SetActive(false);
            }
            else
            {
                transform.position = new Vector3(selectedSoldier.transform.position.x, selectedSoldier.transform.position.y, -9);
                infoButton.SetActive(true);
                groupButton.SetActive(false);
                if(selectedSoldier.GetComponent<soldierScript>().fragCount > 0)
                {
                    fragButton.SetActive(true);
                    fragButton.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = ""+selectedSoldier.GetComponent<soldierScript>().fragCount;
                }
                else
                {
                    fragButton.SetActive(false);
                }
                if (selectedSoldier.GetComponent<soldierScript>().gasCount > 0)
                {
                    gasButton.SetActive(true);
                    gasButton.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "" + selectedSoldier.GetComponent<soldierScript>().gasCount;
                }
                else
                {
                    gasButton.SetActive(false);
                }
                if (selectedSoldier.GetComponent<soldierScript>().spotlightCount > 0)
                {
                    spotlightButton.SetActive(true);
                    spotlightButton.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "" + selectedSoldier.GetComponent<soldierScript>().spotlightCount;
                }
                else
                {
                    spotlightButton.SetActive(false);
                }
                if (selectedSoldier.GetComponent<soldierScript>().gatlingCount > 0)
                {
                    gatlingButton.SetActive(true);
                    gatlingButton.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = "" + selectedSoldier.GetComponent<soldierScript>().gatlingCount;
                }
                else
                {
                    gatlingButton.SetActive(false);
                }
                rangeGrenade.SetActive(selectionPhase == phase.frag || selectionPhase == phase.gas);
                rangeGatling.SetActive(selectedSoldier.GetComponent<soldierScript>().isEngineerInGatling());
                switch (selectedSoldier.GetComponent<soldierScript>().soldierType)
                {
                    case soldierScript.type.infantry:
                        digButton.SetActive(false);
                        rangeArtillery.SetActive(false);
                        rangeInfantry.SetActive(selectionPhase != phase.frag && selectionPhase != phase.gas);
                        attackButton.SetActive(true);
                        retreatButton.SetActive(true);
                        break;
                    case soldierScript.type.engineer:
                        digButton.SetActive(true);
                        rangeArtillery.SetActive(false);
                        rangeInfantry.SetActive(false);
                        attackButton.SetActive(false);
                        retreatButton.SetActive(false);
                        break;
                    case soldierScript.type.artillery:
                        digButton.SetActive(false);
                        rangeArtillery.SetActive(selectionPhase != phase.frag && selectionPhase != phase.gas);
                        rangeInfantry.SetActive(false);
                        attackButton.SetActive(true);
                        retreatButton.SetActive(true);
                        break;
                }
            }
        }

        if(attackButton.activeSelf)
        {
            if (selectedSoldier.GetComponent<soldierScript>().attacking)
            {
                attackButton.GetComponent<Image>().color = new Color(0, 255, 0,120);
                retreatButton.GetComponent<Image>().color = new Color(255, 255, 255, 120);
            }
            else
            {
                attackButton.GetComponent<Image>().color = new Color(255, 255, 255, 120);
                retreatButton.GetComponent<Image>().color = new Color(0, 255, 0, 120);
            }
        }
    }

    public void select(Vector2 pos)
    {
        switch (selectionPhase)
        {
            case phase.none:
                {
                    GameObject closestSoldier = null;
                    float closestDist = Camera.main.orthographicSize / 5;
                    List<GameObject> soldiers = Camera.main.gameObject.GetComponent<controller>().soldiers;
                    for (int i = 0; i < soldiers.Count; i++)
                    {
                        float dist = Vector2.Distance(pos, soldiers[i].transform.position);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestSoldier = soldiers[i];
                        }
                    }
                    
                    selectedSoldier = closestSoldier;
                    break;
                }
            case phase.dig:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    selectedSoldier.GetComponent<soldierScript>().moveTo(new Vector2((int)pos.x,(int)pos.y));
                    selectedSoldier.GetComponent<soldierScript>().digging = true;
                    selectionPhase = phase.none;
                }
                break;
            case phase.info:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    infoScreen.GetComponent<infoScript>().hide();
                    selectionPhase = phase.none;
                }
                break;
            case phase.frag:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    if (Vector2.Distance(pos, selectedSoldier.transform.position) < 19/*experiment*/* 0.6f)
                    {
                        c.launchGrenade(selectedSoldier, pos, false);
                    }
                    selectionPhase = phase.none;
                }
                break;
            case phase.gas:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    if (Vector2.Distance(pos, selectedSoldier.transform.position) < 19/*experiment*/* 0.6f)
                    {
                        c.launchGrenade(selectedSoldier, pos, true);
                    }
                    selectionPhase = phase.none;
                }
                break;
            case phase.lighting:
				if(buttonReleased==false)
				{
					buttonReleased=true;
				}
				else
				{
					c.lightingAt(pos.y);
					selectionPhase=phase.none;
				}
				break;
			case phase.bombing:
				if(buttonReleased==false)
				{
					buttonReleased=true;
				}
				else
				{
					c.bombAt(pos);
					selectionPhase=phase.none;
				}
				break;
            case phase.group:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    List<GameObject> soldiers = Camera.main.gameObject.GetComponent<controller>().soldiers;
                    float minDistance = 1000000;
                    GameObject closest = null;
                    for (int i = 0; i < soldiers.Count; i++)
                    {
                        Vector2 soldierPos = soldiers[i].transform.position;
                        float dx = soldierPos.x - pos.x;
                        float dy = soldierPos.y - pos.y;
                        float dist = Mathf.Sqrt(dx * dx + dy * dy);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            closest = soldiers[i];
                        }
                    }

                    if(soldierGroup.Contains(closest))
                    {
                        closest.transform.GetChild(4).gameObject.SetActive(false);
                        soldierGroup.Remove(closest);
                    }
                    else
                    {
                        closest.transform.GetChild(4).gameObject.SetActive(true);
                        soldierGroup.Add(closest);
                    }
                }
                break;
            case phase.groupMove:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    groupMove(pos);
                    foreach (GameObject s in soldierGroup)
                    {
                        s.GetComponent<soldierScript>().attacking = false;
                    }
                    clearGroup();
                    selectionPhase = phase.none;
                }
                break;
            case phase.groupAttack:
                if (buttonReleased == false)
                {
                    buttonReleased = true;
                }
                else
                {
                    groupMove(pos);
                    foreach (GameObject s in soldierGroup)
                    {
                        s.GetComponent<soldierScript>().attacking = true;
                    }
                    clearGroup();
                    selectionPhase = phase.none;
                }
                break;
        }
    }

    public void soldierOverwatch()
    {
        soldierScript s = selectedSoldier.GetComponent<soldierScript>();
        s.nextAction = soldierScript.action.none;
        s.path = new List<Vector2>();
    }

    public void soldierRepair()
    {
        soldierScript s = selectedSoldier.GetComponent<soldierScript>();
        s.nextAction = soldierScript.action.none;
        s.path = new List<Vector2>();
    }

    public void setPhase(string s)
    {
        phase p = phase.none;
        switch(s)
        {
            case "dig":
                p = phase.dig;
                break;
            case "info":
                p = phase.info;
                break;
            case "group":
                p = phase.group;
                break;
            case "frag":
                p = phase.frag;
                break;
            case "gas":
                p = phase.gas;
                break;
            case "bombing":
                p = phase.bombing;
                break;
           case "lighting":
                p = phase.lighting;
                break;
            case "groupMove":
                p = phase.groupMove;
                break;
            case "groupAttack":
                p = phase.groupAttack;
                break;
        }
        setPhase(p);
    }
    public void setPhase(phase p)
    {
    	buttonReleased=false;
        if (p==phase.info)
        {
            infoScreen.GetComponent<infoScript>().display(selectedSoldier);
            buttonReleased = false;
        }
        if(p==phase.group)
        {
            buttonReleased = false;
            clearGroup();
            if(selectionPhase==phase.group)
            {
                p = phase.none;
            }
        }
        selectionPhase = p;
    }

    public void groupMove(Vector2 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        bool trench = false;
        if(c.terrain[x,y])
        {
            trench = true;
        }
        List<Vector2> positions = new List<Vector2>();
        bool[,] used = new bool[c.xDim, c.yDim];
        for(int s=0; positions.Count<soldierGroup.Count;s++)
        {
            for(int i=-s;i<=s;i++)
            {
                for(int i2=-s;i2<=s;i2++)
                {
                    if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim)
                    {
                        if (!trench || c.terrain[x+i,y+i2])
                        {
                            if(!used[x+i,y+i2])
                            {
                                used[x + i, y + i2] = true;
                                positions.Add(new Vector2(x + i, y + i2));
                            }
                        }
                    }
                }
            }
        }
        for(int i=0;i<soldierGroup.Count;i++)
        {
            Vector2 chosen = positions[Random.Range(0, positions.Count)];
            positions.Remove(chosen);
            soldierGroup[i].GetComponent<soldierScript>().moveTo(chosen);
        }
    }

    void clearGroup()
    {
        foreach(GameObject s in soldierGroup)
        {
            s.transform.GetChild(4).gameObject.SetActive(false);
        }
        soldierGroup = new List<GameObject>();
    }

    public void soliderAttack()
    {
        selectedSoldier.GetComponent<soldierScript>().attacking = true;
    }

    public void soldierRetreat()
    {
        selectedSoldier.GetComponent<soldierScript>().attacking = false;
    }
    public void placeSpotlight()
    {
        selectedSoldier.GetComponent<soldierScript>().placeSpotlight();
    }
    public void placeGatling()
    {
        selectedSoldier.GetComponent<soldierScript>().placeGatling();
    }
}
