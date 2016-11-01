using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour {

    public bool[,] terrain;
    public GameObject[,] trenches;

    public bool[,] enemyTerrain;
    public bool[,] playerTerrain;

    public GameObject trench;

    public List<GameObject> soldiers;
    public List<GameObject> enemies;
    public GameObject soldier;
    public GameObject enemy;

    public GameObject fog;
    public float[,] visibility;
    public float[,] lastVisibility;

    public float[,] enemyVisibility;

    public float dragFactor=0.15f;

    float touchMoved;
    int maxTouches;
    
    bool mouseMoved;
    float mouseLastX;
    float mouseLastY;
    Vector2 mouseClickStart;

    public GameObject selectionIcon;

    Vector2 dragVelocity;
    float scaleVelocity;

    public int dayTime;
    int dayLength;
    public int day;
    public int nextResupplyTime;
    public int nextEventTime;

    public GameObject restartMenu;

    public GameObject reinforcementMenu;

    public float visibilityProportion;

    public GameObject alertText;

    public GameObject worldCanvas;

    public bool DebugFogOn;

    public AIPhase enemyAIPhase;

    public int enemyAggression;

    public int xDim;
    public int yDim;

    public GameObject grenade;

	public GameObject artilleryEffect;

	public GameObject lightingMarker;
	public GameObject bombingMarker;

	public int lights;
	public int bombs;

	public int fragCount;
	public int gasCount;

    public GameObject gasImage;
    public int[,] mustardGas;
    public float[,] realMustardGas;

    public enum AIPhase
    {
        hold,
        advance,
        retreat,
        attack,
    }

    public int reinforcementPoints;
    public enum eventType
    {
        none,
        plane,
        capture
    }
    public GameObject eventClock;
    public eventType currentEvent;
    public int eventTime;
    public int eventMaxTime;
    public GameObject planeProp;
    public GameObject planeWoundedSoldierProp;
    public GameObject takeFrameProp;

    public GameObject selectedMarker;
    public GameObject selectedAttacker;

    public GameObject selectedPointer;

	// Use this for initialization
	void Start () {
        xDim = 100;
        yDim = 100;
		lights = 1;
		bombs = 1;
        transform.position = new Vector3(xDim / 2 - 0.5f, yDim / 2 - 0.5f, -10);
        GetComponent<Camera>().orthographicSize=(xDim+yDim)*0.15f;
        initTerrain();

        soldiers = new List<GameObject>();

        soldiers.Add((GameObject)Instantiate(soldier, new Vector3(38, 27, -2), Quaternion.identity));
        soldiers.Add((GameObject)Instantiate(soldier, new Vector3(55, 27, -2), Quaternion.identity));
        soldiers.Add((GameObject)Instantiate(soldier, new Vector3(34, 23, -2), Quaternion.identity));
        soldiers[1].GetComponent<soldierScript>().soldierType = soldierScript.type.engineer;
        soldiers[2].GetComponent<soldierScript>().soldierType = soldierScript.type.artillery;

        enemies = new List<GameObject>();

        enemies.Add ((GameObject)Instantiate(enemy, new Vector3(33, 75, -2), Quaternion.identity));
        enemies.Add((GameObject)Instantiate(enemy, new Vector3(36, 75, -2), Quaternion.identity));
        enemies.Add((GameObject)Instantiate(enemy, new Vector3(70, 80, -2), Quaternion.identity));
        enemies.Add((GameObject)Instantiate(enemy, new Vector3(66, 75, -2), Quaternion.identity));
        enemies[2].GetComponent<enemyScript>().enemyType = soldierScript.type.engineer;
        enemies[3].GetComponent<enemyScript>().enemyType = soldierScript.type.artillery;

        dragVelocity = new Vector2(0, 0);
        scaleVelocity = 0;

        touchMoved = 0;
        maxTouches = 0;
        
        mouseMoved = false;
        mouseLastX = 0;
        mouseLastY = 0;

        day = 0;
        dayLength = 2400;
        dayTime = 0;
        nextResupplyTime = 4;
        nextEventTime = Random.Range(1, 4);
        restartMenu = GameObject.FindGameObjectWithTag("restartMenu");

        reinforcementMenu = GameObject.FindGameObjectWithTag("reinforcementText");

        visibilityProportion = 1;

        selectionIcon = GameObject.FindGameObjectWithTag("selection");

        fog = GameObject.FindGameObjectWithTag("fog");
        Color32[] cs = new Color32[9 * xDim * yDim];
        for (int i = 0; i < 3 * xDim; i++)
        {
            for (int i2 = 0; i2 < 3 * yDim; i2++)
            {
                cs[i2 * 3 * xDim + i] = new Color(0.392f, 0.38f, 0.353f);
            }
        }
        fog.GetComponent<SpriteRenderer>().sprite.texture.SetPixels32(cs);
        fog.GetComponent<SpriteRenderer>().sprite.texture.Apply();


        visibility = new float[3*xDim, 3*yDim];
        lastVisibility = new float[3 * xDim, 3 * yDim];
        for(int i=0;i< 3 * xDim; i++)
        {
            for(int i2=0;i2< 3 * yDim; i2++)
            {
                visibility[i, i2] = 1;
                lastVisibility[i, i2] = 100;
            }
        }

        enemyVisibility = new float[3 * xDim, 3 * yDim];
        for (int i = 0; i < 3 * xDim; i++)
        {
            for (int i2 = 0; i2 < 3 * yDim; i2++)
            {
                enemyVisibility[i, i2] = 1;
            }
        }

        alertText = GameObject.FindGameObjectWithTag("alertText");

        worldCanvas = GameObject.FindGameObjectWithTag("worldCanvas");

        checkTrenches();
        
        enemyAIPhase = AIPhase.hold;

        reinforcementPoints = 2;
        currentEvent = eventType.none;
        eventClock = GameObject.FindGameObjectWithTag("eventClock");

        gasImage = GameObject.FindGameObjectWithTag("gasImage");
        mustardGas = new int[xDim * 2, yDim * 2];
        realMustardGas = new float[xDim * 2, yDim * 2];
        for(int i=0;i<xDim*2;i++)
        {
            for(int i2=0;i2<yDim*2;i2++)
            {
                mustardGas[i, i2] = -1;
                realMustardGas[i, i2] = -1;
            }
        }
        //Debug stuff
        DebugFogOn = true;
    }
	
	// Update is called once per frame
	void Update () {
        checkTrenches();
        inputUpdate();
        cameraUpdate();

        visibilityProportion = 1;
        if(dayTime>=1200 && dayTime<1500)
        {
            visibilityProportion = 1.0f - (dayTime - 1200) / 750.0f;
        }
        if(dayTime>=1500 && dayTime<2100)
        {
            visibilityProportion = 0.6f;
        }
        if(dayTime>=2100)
        {
            visibilityProportion = 0.6f + (dayTime - 2100) / 750.0f;
        }
        fog.GetComponent<SpriteRenderer>().color = new Color(visibilityProportion, visibilityProportion, visibilityProportion, 1);
        if(!DebugFogOn)
        {
            fog.GetComponent<SpriteRenderer>().color = new Color(visibilityProportion, visibilityProportion, visibilityProportion, 0.5f);
        }
    }

    void FixedUpdate()
    {
        if (soldiers.Count > 0)
        {
            dayTime++;
            restartMenu.SetActive(false);
        }
        else
        {
            restartMenu.SetActive(true);
        }
        if(dayTime==dayLength)
        {
            newDay();
        }

        updateMustardGas();

        updateVisibility();

        enemyAIStep();

        eventTime++;

        if(eventTime==eventMaxTime)
        {
            endEvent();
        }

        if(currentEvent!=eventType.none)
        {
            eventClock.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, (-360.0f * eventTime) / eventMaxTime);
        }
    }

    public void initTerrain()
    {
        terrain = new bool[xDim, yDim];
        playerTerrain = new bool[xDim, yDim];
        enemyTerrain = new bool[xDim, yDim];
        trenches = new GameObject[xDim, yDim];

        for (int i = 35; i < 65; i++)
        {
            terrain[i, 27] = true;
            playerTerrain[i, 27] = true;
        }
        for (int i = 14; i < 28; i++)
        {
            terrain[34, i] = true;
            terrain[65, i] = true;
            playerTerrain[34, i] = true;
            playerTerrain[65, i] = true;
        }

        for (int i = 30; i < 70; i++)
        {
            terrain[i, 75] = true;
            enemyTerrain[i, 75] = true;
        }
        for (int i = 75; i < 85; i++)
        {
            terrain[29, i] = true;
            terrain[70, i] = true;
            enemyTerrain[29, i] = true;
            enemyTerrain[70, i] = true;
        }

        bool[,] randomTrenches = new bool[xDim, yDim];

        int trenchY = Random.Range(68, 72);
        while(trenchY>30)
        {
            int y = trenchY;
            int x = 50;
            int endX = 50;
            if(Random.Range(0,100)<50)
            {
                x += Random.Range(19, 29);
                endX -= Random.Range(19, 29);
            }
            else
            {
                x -= Random.Range(19, 29);
                endX += Random.Range(19, 29);
            }
            int xItt = (int)Mathf.Sign(endX - x);
            int yItt = 0;

            while(x!=endX)
            {
                int c = 0;
                for(int i=-1;i<=1;i++)
                {
                    for(int i2=-1;i2<=1;i2++)
                    {
                        if(terrain[x+i,y+i2])
                        {
                            c++;
                        }
                    }
                }
                if (c < 3 && Random.Range(0,100)<97)
                {
                    terrain[x, y] = true;
                    randomTrenches[x, y] = true;
                }
                if (yItt == 0)
                {
                    if (Random.Range(0, 100) <12)
                    {
                        if (Random.Range(0, 100) < 50)
                        {
                            yItt = 1;
                        }
                        else
                        {
                            yItt = -1;
                        }
                    }
                }
                else
                {
                    if (Random.Range(0, 100) < 50)
                    {
                        yItt = 0;
                    }
                }

                if(yItt==0)
                {
                    x += xItt;
                }
                else
                {
                    y += yItt;
                }
            }

            trenchY -= Random.Range(5, 8);
        }

        checkTrenches();
        for(int i=0;i<xDim;i++)
        {
            for(int i2=0;i2<yDim;i2++)
            {
                if(randomTrenches[i,i2] && Random.Range(0,100)<20)
                {
                    trenches[i, i2].GetComponent<trenchScript>().health = 3;
                    for(int i3=-1;i3<=1;i3++)
                    {
                        for(int i4=-1;i4<=1;i4++)
                        {
                            if(randomTrenches[i+i3,i2+i4])
                            {
                                trenches[i+i3, i2+i4].GetComponent<trenchScript>().health = 2;
                            }
                        }
                    }
                }
            }
        }
        for (int i = 0; i < xDim; i++)
        {
            for (int i2 = 0; i2 < yDim; i2++)
            {
                if (randomTrenches[i, i2])
                {
                    if (trenches[i, i2].GetComponent<trenchScript>().health == 4)
                    {
                        trenches[i, i2].GetComponent<trenchScript>().health = 3;
                    }
                }
            }
        }
    }

    public void checkTrenches()
    {
        for (int i = 0; i < xDim; i++)
        {
            for (int i2 = 0; i2 < yDim; i2++)
            {
                if (terrain[i, i2])
                {
                    if (trenches[i, i2] == null)
                    {
                        trenches[i, i2] = (GameObject)Instantiate(trench, new Vector3(i, i2, -1), Quaternion.identity);
                    }
                }
                else
                {
                    if (trenches[i, i2] != null)
                    {
                        Destroy(trenches[i, i2]);
                        trenches[i, i2] = null;
                    }
                }
            }
        }
    }

    void inputUpdate()
    {
        switch(Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                mobileInputUpdate();
                break;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                desktopInputUpdate();
                break;
        }
    }
    void mobileInputUpdate()
    {
        if(Input.touchCount==0)
        {
            maxTouches = 0;
        }
        else
        {
            maxTouches = Mathf.Max(maxTouches, Input.touchCount);
        }
        switch (Input.touchCount)
        {
            case 1:
                switch (Input.touches[0].phase)
                {
                    case TouchPhase.Began:
                        touchMoved = 0;
                        break;
                    case TouchPhase.Moved:
                        dragVelocity = Input.touches[0].deltaPosition;
                        touchMoved += Mathf.Sqrt(Vector2.SqrMagnitude(Input.touches[0].deltaPosition));
                        break;
                    case TouchPhase.Ended:
                        if(touchMoved<10 && maxTouches==1)
                        {
                            selectionIcon.GetComponent<selectionScript>().select(Camera.main.ScreenToWorldPoint(Input.touches[0].position));
                        }
                        break;
                }
                
                break;
            case 2:
                if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved)
                {
                    float dist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                    float oldDist = Vector2.Distance(new Vector2(Input.touches[0].position.x - Input.touches[0].deltaPosition.x, Input.touches[0].position.y - Input.touches[0].deltaPosition.y), new Vector2(Input.touches[1].position.x - Input.touches[1].deltaPosition.x, Input.touches[1].position.y - Input.touches[1].deltaPosition.y));
                    float deltaDist = oldDist - dist;
                    scaleVelocity = deltaDist* 2 * dragFactor;
                }
                break;
        }
    }
    void desktopInputUpdate()
    {
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            mouseClickStart = mousePos;

            float minPointerDist = 2;
            float minMarkerDist = 2;
            float minAttackerDist = 2;
            Vector2 worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePos);
            foreach (GameObject s in soldiers)
            {
                float pointerDist = Vector2.Distance(worldPos, s.transform.GetChild(5).position);
                float markerDist = Vector2.Distance(worldPos, s.transform.position);
                float attackerDist = Vector2.Distance(worldPos, s.transform.GetChild(7).position);
                if (pointerDist< minPointerDist)
                {
                    minPointerDist = pointerDist;
                    selectedPointer = s.transform.GetChild(5).gameObject;
                }
                if(markerDist<minMarkerDist)
                {
                    minMarkerDist = markerDist;
                    selectedMarker = s.transform.GetChild(6).gameObject;
                }
                if(attackerDist<minAttackerDist)
                {
                    minAttackerDist = attackerDist;
                    selectedAttacker = s.transform.GetChild(7).gameObject;
                }
            }
            if(minMarkerDist>=minPointerDist || minMarkerDist>=minAttackerDist)
            {
                selectedMarker = null;
            }
            if(minPointerDist>=minMarkerDist || minPointerDist>=minAttackerDist)
            {
                selectedPointer = null;
            }
            if(minAttackerDist>=minMarkerDist || minAttackerDist>=minPointerDist)
            {
                selectedAttacker = null;
            }

        }
        float newX = mousePos.x;
        float newY = mousePos.y;

        if(Input.GetMouseButton(0) && (newX!=mouseLastX || newY!=mouseLastY))
        {
            if (selectedPointer == null && selectedMarker==null && selectedAttacker ==null)
            {
                dragVelocity = new Vector2(newX - mouseLastX, newY - mouseLastY);
            }
            else
            {
                if (selectedPointer != null)
                {
                    Vector2 worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePos);
                    Vector2 soldierPos = selectedPointer.transform.parent.position;
                    float a = Mathf.Atan2(worldPos.y - soldierPos.y, worldPos.x - soldierPos.x);
                    selectedPointer.transform.localPosition = new Vector2(4 * Mathf.Cos(a), 4 * Mathf.Sin(a));
                }
                if(selectedMarker!=null)
                {
                    Vector2 worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePos);
                    selectedMarker.transform.parent.gameObject.GetComponent<soldierScript>().markerPos=new Vector2((int)(worldPos.x+0.5f), (int)(worldPos.y + 0.5f));
                    while(selectedMarker.transform.childCount>0)
                    {
                        GameObject point = selectedMarker.transform.GetChild(0).gameObject;
                        point.transform.parent = null;
                        Destroy(point);
                    }
                }
                if(selectedAttacker!=null)
                {
                    soldierScript s = selectedAttacker.transform.parent.gameObject.GetComponent<soldierScript>();
                    Vector2 worldPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePos);
                    if (s.soldierType == soldierScript.type.infantry)
                    {
                        Vector2 soldierPos = selectedAttacker.transform.parent.position;
                        float a = Mathf.Atan2(worldPos.y - soldierPos.y, worldPos.x - soldierPos.x);
                        float lowestADiff = 2 * Mathf.PI;
                        List<GameObject> aimable = s.getEnemiesInRange();
                        if (aimable.Count > 0)
                        {
                            GameObject closestEnemy=aimable[0];
                            foreach (GameObject e in aimable)
                            {
                                float enemyA = Mathf.Atan2(e.transform.position.y - soldierPos.y, e.transform.position.x - soldierPos.x);
                                float aDiff = Mathf.Abs(a - enemyA);
                                if (aDiff > Mathf.PI)
                                {
                                    aDiff = (2 * Mathf.PI) - aDiff;
                                }
                                if (aDiff < lowestADiff)
                                {
                                    lowestADiff = aDiff;
                                    closestEnemy = e;
                                }
                            }
                            s.targetedEnemy = closestEnemy;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Vector2.Distance(mouseClickStart, mousePos) < 10)
            {
                selectionIcon.GetComponent<selectionScript>().select((Vector2)Camera.main.ScreenToWorldPoint(mousePos) + new Vector2(0.5f, 0.5f));
            }
            selectedPointer = null;
            if(selectedMarker!=null)
            {
                soldierScript s = selectedMarker.transform.parent.gameObject.GetComponent<soldierScript>();
                s.moveTo(selectedMarker.transform.position);
                s.nextAction = soldierScript.action.none;
                selectedMarker = null;
            }
        }
        mouseLastX = newX;
        mouseLastY = newY;
        scaleVelocity = -20*Input.GetAxis("Mouse ScrollWheel");
    }
    void cameraUpdate()
    {
        float scale = Camera.main.orthographicSize / 10;
        dragVelocity = new Vector2(Mathf.Clamp(dragVelocity.x, -25, 25), Mathf.Clamp(dragVelocity.y, -25, 25));
        transform.position = new Vector3(transform.position.x + (-dragFactor * scale * dragVelocity.x), transform.position.y + (-dragFactor * scale * dragVelocity.y), -10);

        scaleVelocity = Mathf.Clamp(scaleVelocity, -10, 10);
        Camera.main.orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize + scaleVelocity, 8, 48);

        float dragDecay = Mathf.Pow(0.01f, Time.deltaTime);
        dragVelocity = new Vector2(dragVelocity.x * dragDecay, dragVelocity.y * dragDecay);
        scaleVelocity *= dragDecay;
    }
    public void outOfRangeAlert()
    {
        alertText.GetComponent<alertScript>().outOfRange();
    }
    public void DebugFogToggle()
    {
        DebugFogOn = !DebugFogOn;
    }
    public void DebugSetPhaseRetreat()
    {
        enemyAIPhase = AIPhase.retreat;
    }
    public void DebugSetPhaseAdvance()
    {
        enemyAIPhase = AIPhase.advance;
    }
    public void DebugSetPhaseHold()
    {
        enemyAIPhase = AIPhase.hold;
    }
    public void DebugSetPhaseAttack()
    {
        enemyAIPhase = AIPhase.attack;
    }
    public void DebugToggleEnemyTime()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject e = enemies[i];
            enemyScript eScript = e.GetComponent<enemyScript>();
            if (eScript.moveTime == 10)
            {
                eScript.moveTime = 2;
                eScript.digTime = 12;
            }
            else
            {
                eScript.moveTime = 10;
                eScript.digTime = 60;
            }
        }
    }

    public void updateVisibility()
    {
        bool[,] newPlayerTerrain = new bool[xDim, yDim];
        bool[,] newEnemyTerrain = new bool[xDim, yDim];
        for (int i=0;i<soldiers.Count;i++)
        {
            soldierScript s = soldiers[i].GetComponent<soldierScript>();
            int x = (int)s.realPosition.x;
            int y = (int)s.realPosition.y;
            for(int i2=-3;i2<=3;i2++)
            {
                for(int i3=-3;i3<=3;i3++)
                {
                    if(x+i2>=0 && x+i2<xDim && y+i3>=0 && y+i3<yDim)
                    {
                        if(terrain[x+i2,y+i3])
                        {
                            newPlayerTerrain[x + i2, y + i3] = true;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyScript e = enemies[i].GetComponent<enemyScript>();
            int x = (int)e.realPosition.x;
            int y = (int)e.realPosition.y;
            for (int i2 = -3; i2 <= 3; i2++)
            {
                for (int i3 = -3; i3 <= 3; i3++)
                {
                    if (x + i2 >= 0 && x + i2 < xDim && y + i3 >= 0 && y + i3 < yDim)
                    {
                        if (terrain[x + i2, y + i3])
                        {
                            newEnemyTerrain[x + i2, y + i3] = true;
                        }
                    }
                }
            }
        }
        for(int i=0;i<xDim;i++)
        {
            for(int i2=0;i2<yDim;i2++)
            {
                if(newPlayerTerrain[i,i2])
                {
                    playerTerrain[i, i2] = true;
                    if (!newEnemyTerrain[i,i2])
                    {
                        enemyTerrain[i, i2] = false;
                    }
                }
                if (newEnemyTerrain[i, i2])
                {
                    enemyTerrain[i, i2] = true;
                    if (!newPlayerTerrain[i, i2])
                    {
                        playerTerrain[i, i2] = false;
                    }
                }
            }
        }

        bool[,] squaresVisible = new bool[xDim,yDim];
        for (int i = 0; i < xDim; i++)
        {
            for (int i2 = 0; i2 < yDim; i2++)
            {
                if (playerTerrain[i, i2])
                {
                    for (int i3 = -2; i3 <= 2; i3++)
                    {
                        for (int i4 = -2; i4 <= 2; i4++)
                        {
                            int x = i + i3;
                            int y = i2 + i4;
                            if (x >= 0 && x < xDim && y >= 0 && y < yDim)
                            {
                                squaresVisible[x, y] = true;
                            }
                        }
                    }
                }
            }
        }

        bool[,] visiblePixels = new bool[3* xDim, 3* yDim];
        for (int i = 0; i < xDim; i++)
        {
            for (int i2 = 0; i2 < yDim; i2++)
            {
                if (squaresVisible[i, i2])
                {
                    for (int i3 = 0; i3 < 3; i3++)
                    {
                        for (int i4 = 0; i4 < 3; i4++)
                        {
                            visiblePixels[3 * i + i3, 3 * i2 + i4] = true;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < soldiers.Count; i++)
        {
            soldierScript s = soldiers[i].GetComponent<soldierScript>();
            int x = 1 + (int)(3 * soldiers[i].transform.position.x);
            int y = 1 + (int)(3 * soldiers[i].transform.position.y);
            for (int i2 = -24; i2 <= 24; i2++)
            {
                for (int i3 = -24; i3 <= 24; i3++)
                {
                    if (Mathf.Sqrt(i2 * i2 + i3 * i3) <= 24 * visibilityProportion)
                    {
                        if (x + i2 >= 0 && x + i2 < 3* xDim && y + i3 >= 0 && y + i3 < 3* yDim)
                        {
                            visiblePixels[x + i2, y + i3] = true;
                        }
                    }
                }
            }
            int x1 = 1 + (int)(s.transform.GetChild(5).position.x * 3);
            int y1 = 1 + (int)(s.transform.GetChild(5).position.y * 3);
            int xDir = (int)Mathf.Sign(x1 - x);
            int yDir = (int)Mathf.Sign(y1 - y);
            float a = Mathf.Atan2(y - y1, x - x1);
            int size = 36;
            if(s.GetComponent<soldierScript>().soldierType==soldierScript.type.engineer)
            {
                size = 54;
            }
            for (int i2 = -size; i2 <= size; i2++)
            {
                for (int i3 = -size; i3 <= size; i3++)
                {
                    if (y + i2 >= 0 && y + i2 < 3*yDim && x + i3 >= 0 && x + i3 < 3*xDim && Mathf.Sqrt(i2 * i2 + i3 * i3) <= size * visibilityProportion)
                    {
                        float newA = Mathf.Atan2(y - (y + i2), x - (x + i3));
                        if (Mathf.Abs(newA - a) < 0.7f || Mathf.Abs((newA - 2 * Mathf.PI) - a) < 0.7f || Mathf.Abs((newA + 2 * Mathf.PI) - a) < 0.7f)
                        {
                            visiblePixels[x + i3, y + i2] = true;
                        }
                    }
                }
            }
        }

        GameObject[] spotlights = GameObject.FindGameObjectsWithTag("spotlight");
        foreach (GameObject s in spotlights)
        {
			if (s.GetComponent<spotlightScript> ().player) {
				float a = s.GetComponent<spotlightScript> ().a;
				int x = 1 + (int)(s.transform.position.x * 3);
				int y = 1 + (int)(s.transform.position.y * 3);
				for (int i = -100; i <= 100; i++) {
					for (int i2 = -100; i2 <= 100; i2++) {
						if (x + i >= 0 && x + i < 3 * xDim && y + i2 >= 0 && y + i2 < 3 * yDim) {
							if (Mathf.Abs ((Mathf.Atan2 (i2, i) - Mathf.PI / 2) - a * (Mathf.PI / 180)) < 0.3f && Mathf.Sqrt (i * i + i2 * i2) <= 100) {
								visiblePixels [x + i, y + i2] = true;
							}
						}
					}
				}
			}
        }

		GameObject[] lights = GameObject.FindGameObjectsWithTag ("lighting");
		foreach (GameObject l in lights) {
			l.transform.position += new Vector3 (0.1f, 0,0);
			int x = 1 + (int)(l.transform.position.x * 3);
			int y = 1 + (int)(l.transform.position.y * 3);
			for (int i = -50; i <= 50; i++) {
				for (int i2 = -25; i2 <= 25; i2++) {
					if (x+i>=0 && x+i<3*xDim && y+i2>=0 && y+i2<3*yDim && Mathf.Sqrt (i * i/2.0f+ i2 * i2) <= 25) {
						visiblePixels [x + i, y + i2] = true;
					}
				}
			}
			if (l.transform.position.x >= xDim - 10) {
				Destroy (l);
			}
		}

		GameObject[] bombs = GameObject.FindGameObjectsWithTag ("bombing");
		foreach (GameObject b in bombs) {
			int x = 1 + (int)(b.transform.position.x * 3);
			int y = 1 + (int)(b.transform.position.y * 3);
			for (int i = -30; i <= 30; i++) {
				for (int i2 = -15; i2 <= 15; i2++) {
					if (x+i>=0 && x+i<3*xDim && y+i2>=0 && y+i2<3*yDim && Mathf.Sqrt (i * i/2.0f+ i2 * i2) <= 15) {
						visiblePixels [x + i, y + i2] = true;
					}
				}
			}
		}

        for (int i = 0; i < 3*xDim; i++)
        {
            for (int i2 = 0; i2 < 3* yDim; i2++)
            {
                if (visiblePixels[i, i2])
                {
                    visibility[i, i2] = Mathf.Max(0, visibility[i, i2] - 0.055f);
                }
                else
                {
                    visibility[i, i2] = Mathf.Min(1, visibility[i, i2] + 0.055f);
                }
            }
        }


        squaresVisible = new bool[xDim, yDim];
        for (int i = 0; i < xDim; i++)
        {
            for (int i2 = 0; i2 < yDim; i2++)
            {
                if (enemyTerrain[i, i2])
                {
                    for (int i3 = -2; i3 <= 2; i3++)
                    {
                        for (int i4 = -2; i4 <= 2; i4++)
                        {
                            int x = i + i3;
                            int y = i2 + i4;
                            if (x >= 0 && x < xDim && y >= 0 && y < yDim)
                            {
                                squaresVisible[x, y] = true;
                            }
                        }
                    }
                }
            }
        }

        visiblePixels = new bool[3* xDim, 3* yDim];
        for (int i = 0; i < xDim; i++)
        {
            for (int i2 = 0; i2 < yDim; i2++)
            {
                if (squaresVisible[i, i2])
                {
                    for (int i3 = 0; i3 < 3; i3++)
                    {
                        for (int i4 = 0; i4 < 3; i4++)
                        {
                            visiblePixels[3 * i + i3, 3 * i2 + i4] = true;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyScript s = enemies[i].GetComponent<enemyScript>();
            int x = 1 + (int)(3 * enemies[i].transform.position.x);
            int y = 1 + (int)(3 * enemies[i].transform.position.y);
            for (int i2 = -24; i2 <= 24; i2++)
            {
                for (int i3 = -24; i3 <= 24; i3++)
                {
                    if (Mathf.Sqrt(i2 * i2 + i3 * i3) <= 24 * visibilityProportion)
                    {
                        if (x + i2 >= 0 && x + i2 < 3* xDim && y + i3 >= 0 && y + i3 < 3* yDim)
                        {
                            visiblePixels[x + i2, y + i3] = true;
                        }
                    }
                }
            }
			int size = 36;
			if(s.enemyType==soldierScript.type.engineer)
			{
				size = 54;
			}
			for (int i2 = -size; i2 <= size; i2++)
			{
				for (int i3 = -size; i3 <= size; i3++)
				{
					if (y + i2 >= 0 && y + i2 < 3*yDim && x + i3 >= 0 && x + i3 < 3*xDim && Mathf.Sqrt(i2 * i2 + i3 * i3) <= size * visibilityProportion)
					{
						float newA = Mathf.Atan2(y - (y + i2), x - (x + i3));
						if (Mathf.Abs(newA - Mathf.PI/2) < 0.7f || Mathf.Abs((newA - 2 * Mathf.PI) - Mathf.PI/2) < 0.7f || Mathf.Abs((newA + 2 * Mathf.PI) - Mathf.PI/2) < 0.7f)
						{
							visiblePixels[x + i3, y + i2] = true;
						}
					}
				}
			}
        }
			
		foreach (GameObject s in spotlights)
		{
			if (!s.GetComponent<spotlightScript> ().player) {
				float a = s.GetComponent<spotlightScript> ().a-180;
				int x = 1 + (int)(s.transform.position.x * 3);
				int y = 1 + (int)(s.transform.position.y * 3);
				for (int i = -100; i <= 100; i++) {
					for (int i2 = -100; i2 <= 100; i2++) {
						if (x + i >= 0 && x + i < 3 * xDim && y + i2 >= 0 && y + i2 < 3 * yDim) {
							if (Mathf.Abs ((Mathf.Atan2 (i2, i) - Mathf.PI / 2) - a * (Mathf.PI / 180)) < 0.3f && Mathf.Sqrt (i * i + i2 * i2) <= 100) {
								visiblePixels [x + i, y + i2] = true;
							}
						}
					}
				}
			}
		}

        for (int i = 0; i < 3* xDim; i++)
        {
            for (int i2 = 0; i2 < 3* yDim; i2++)
            {
                if (visiblePixels[i, i2])
                {
                    enemyVisibility[i, i2] = Mathf.Max(0, enemyVisibility[i, i2] - 0.055f);
                }
                else
                {
                    enemyVisibility[i, i2] = Mathf.Min(1, enemyVisibility[i, i2] + 0.055f);
                }
            }
        }

		Texture2D t = fog.GetComponent<SpriteRenderer>().sprite.texture;
        Color32[] cs = t.GetPixels32();
        for (int i = 0; i < 3 * xDim; i++)
        {
            for (int i2 = 0; i2 < 3 * yDim; i2++)
            {
				if (visibility[i, i2] != lastVisibility[i, i2])
                {
                    lastVisibility[i, i2] = visibility[i, i2];
                    cs[i2 * 3*xDim + i] = new Color(0.392f, 0.38f, 0.353f, visibility[i, i2]);
                }
            }
        }
        t.SetPixels32(cs);
        t.Apply();
    }
    public void enemyAIStep()
    {
        if(Random.Range(0,100)<3)
        {
            enemyAggression += enemies.Count - soldiers.Count;
        }

        if(enemyAggression<-20)
        {
            enemyAIPhase = AIPhase.retreat;
        }
        if (enemyAggression >= -20 && enemyAggression<30)
        {
            enemyAIPhase = AIPhase.hold;
        }
        if (enemyAggression >=30 && enemyAggression<70)
        {
            enemyAIPhase = AIPhase.advance;
        }
        if (enemyAggression >= 70)
        {
            enemyAIPhase = AIPhase.attack;
        }


        List<int> enemyYs = new List<int>();
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyYs.Add((int)enemies[i].GetComponent<enemyScript>().realPosition.y);
        }
        float averageY = 0;
        foreach(int i in enemyYs)
        {
            averageY += i;
        }

        float lowestY = 1000000;
        for (int i=0;i<soldiers.Count;i++)
        {
            if(soldiers[i].transform.position.y<lowestY)
            {
                lowestY = soldiers[i].transform.position.y;
            }
        }
        
        averageY /= enemyYs.Count;
        for (int i=0;i<enemies.Count;i++)
        {
            int r = Random.Range(0, 100);

            GameObject e = enemies[i];
            enemyScript eScript = e.GetComponent<enemyScript>();

            if (r<1)
            {
                switch(Random.Range(0,4))
                {
                    case 0:
                        eScript.enemyPhase = enemyScript.phase.advance;
                        break;
                    case 1:
                        eScript.enemyPhase = enemyScript.phase.retreat;
                        break;
                    case 2:
                        eScript.enemyPhase = enemyScript.phase.hold;
                        break;
                    case 3:
                        eScript.enemyPhase = enemyScript.phase.attack;
                        break;
                }
            }
            else if(r<7)
            {
                float yDiff = averageY - eScript.realPosition.y;
                switch (enemyAIPhase)
                {
                    case AIPhase.hold:
                        {
                            if (yDiff < -5)
                            {
                                eScript.enemyPhase = enemyScript.phase.advance;
                            }
                            else if (yDiff > 5)
                            {
                                eScript.enemyPhase = enemyScript.phase.retreat;
                            }
                            else
                            {
                                eScript.enemyPhase = enemyScript.phase.hold;
                            }
                            break;
                        }
                    case AIPhase.advance:
                        {
                            if (eScript.enemyType == soldierScript.type.engineer)
                            {
                                eScript.enemyPhase = enemyScript.phase.advance;
                            }
                            else
                            {
                                if (yDiff < 0)
                                {
                                    eScript.enemyPhase = enemyScript.phase.advance;
                                }
                                else if (yDiff > 8)
                                {
                                    eScript.enemyPhase = enemyScript.phase.retreat;
                                }
                                else
                                {
                                    eScript.enemyPhase = enemyScript.phase.hold;
                                }
                            }
                            break;
                        }
                    case AIPhase.retreat:
                        {
                            if (yDiff < -8)
                            {
                                eScript.enemyPhase = enemyScript.phase.advance;
                            }
                            else if (yDiff > 0)
                            {
                                eScript.enemyPhase = enemyScript.phase.retreat;
                            }
                            else
                            {
                                eScript.enemyPhase = enemyScript.phase.hold;
                            }
                            break;
                        }
                    case AIPhase.attack:
                        {
                            if (eScript.enemyType == soldierScript.type.engineer)
                            {
                                eScript.enemyPhase = enemyScript.phase.advance;
                            }
                            else
                            {
                                eScript.enemyPhase = enemyScript.phase.attack;
                            }
                            break;
                        }
                }
            }

            if(e.transform.position.y<=lowestY && (eScript.enemyPhase==enemyScript.phase.attack || eScript.enemyPhase == enemyScript.phase.advance))
            {
                eScript.enemyPhase = enemyScript.phase.hold;
            }
        }
    }

    public void enemyDamaged(int d)
    {
        enemyAggression -= 8;
    }

    public void enemyDied()
    {
        enemyAggression -= 32;
    }

    public void soldierDamaged(int d)
    {
        enemyAggression += 6;
    }

    public void soldierDied()
    {
        enemyAggression += 24;
    }

    public void newDay()
    {
        dayTime = 0;
        day++;
        reinforcementPoints+=2;
        enemyAggression = Mathf.Max(enemyAggression, -21);

		foreach (GameObject e in enemies) {
			e.GetComponent<enemyScript> ().reinforcementConsumables ();
		}

        int enemyNum = 1;
        if(Random.Range(0,100)<50)
        {
            enemyNum = 2;
        }
        for (int i = 0; i < enemyNum; i++)
        {
			GameObject newEnemy = (GameObject)Instantiate(enemy, new Vector2(Random.Range(xDim/5,4*(xDim/5)), yDim-1), Quaternion.identity);
            enemyScript newScript = newEnemy.GetComponent<enemyScript>();
            switch (Random.Range(0, 3))
            {
                case 0:
                    newScript.enemyType = soldierScript.type.infantry;
                    break;
                case 1:
                    newScript.enemyType = soldierScript.type.artillery;
                    break;
                case 2:
                    newScript.enemyType = soldierScript.type.engineer;
                    break;
            }
            newScript.enemyPhase = enemyScript.phase.advance;
            enemies.Add(newEnemy);
        }

        reinforcementMenu.GetComponent<reinforcementMenuScript>().activate();
        if (day == 1)
        {
            newEvent(eventType.plane);
        }
        if(day==3)
        {
            newEvent(eventType.capture);
        }
    }
    public void createSoldier(Vector2 pos, soldierScript.type soldierType)
    {
        GameObject s = (GameObject)Instantiate(soldier, pos, Quaternion.identity);
        s.GetComponent<soldierScript>().soldierType = soldierType;
        soldiers.Add(s);
    }
    public void newEvent(eventType e)
    {
        currentEvent = e;
        switch(e)
        {
            case eventType.plane:
                {
                    eventTime = 0;
                    eventMaxTime = 4400;
                    float playerY = 0;
                    foreach (GameObject s in soldiers)
                    {
                        playerY += s.transform.position.y;
                    }
                    playerY /= soldiers.Count;
                    float enemyY = 0;
                    foreach (GameObject s in enemies)
                    {
                        enemyY += s.transform.position.y;
                    }
                    enemyY /= enemies.Count;

                    float x = Random.Range(30.0f, 70);
                    float y = Random.Range((enemyY + playerY) / 2 - 7, (enemyY + playerY) / 2 + 7);
                    Instantiate(planeWoundedSoldierProp, new Vector3(x, y, -2), Quaternion.identity);
                    Instantiate(planeProp, new Vector3(x - 3, y + 3, -2), Quaternion.Euler(0, 0, Random.Range(-20.0f, 20)));
                    break;
                }
            case eventType.capture:
                {
                    eventTime = 0;
                    eventMaxTime = 3600;
                    float enemyY = 0;
                    foreach (GameObject s in enemies)
                    {
                        enemyY += s.transform.position.y;
                    }
                    enemyY /= enemies.Count;

                    float x = Random.Range(30.0f, 70);
                    float y = Random.Range(enemyY - 15, enemyY-5);
                    Instantiate(takeFrameProp, new Vector3(x, y, -4), Quaternion.identity);
                    break;
                }
        }
        eventClock.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        eventClock.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
    public void endEvent()
    {
        currentEvent = eventType.none;
        eventMaxTime = 0;

        eventClock.SetActive(false);
        eventClock.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        eventClock.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        GameObject[] props = GameObject.FindGameObjectsWithTag("eventProp");
        foreach (GameObject p in props)
        {
            Destroy(p);
        }
    }
    public void completeEvent()
    {
        reinforcementPoints += 3;
        switch(currentEvent)
        {
            case eventType.plane:
                {
                    GameObject[] eventProps = GameObject.FindGameObjectsWithTag("eventProp");
                    foreach (GameObject p in eventProps)
                    {
                        if (p.transform.childCount > 0)
                        {
                            int x = (int)Mathf.Round(p.transform.position.x);
                            int y = (int)Mathf.Round(p.transform.position.y);
                            createSoldier(new Vector2(x, y), soldierScript.type.infantry);
                        }
                    }
                    break;
                }
        }
        currentEvent = eventType.none;
        eventMaxTime = 0;

        eventClock.SetActive(false);
        eventClock.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        eventClock.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        GameObject[] props = GameObject.FindGameObjectsWithTag("eventProp");
        foreach(GameObject p in props)
        {
            Destroy(p);
        }
    }
    public void restartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void launchGrenade(GameObject s, Vector2 pos, bool isGas)
    {
		s.GetComponent<soldierScript> ().gainExp (20);
        if(isGas)
        {
            s.GetComponent<soldierScript>().gasCount--;
        }
        else
        {
            s.GetComponent<soldierScript>().fragCount--;
        }
        GameObject newG = (GameObject)GameObject.Instantiate(grenade, s.transform.position, Quaternion.identity);
        newG.GetComponent<grenadeScript>().init(isGas, pos);
    }
	public void launchEnemyGrenade(GameObject e, Vector2 pos, bool isGas)
	{
		if(isGas)
		{
			e.GetComponent<enemyScript>().gasCount--;
		}
		else
		{
			e.GetComponent<enemyScript>().fragCount--;
		}
		GameObject newG = (GameObject)GameObject.Instantiate(grenade, e.transform.position, Quaternion.identity);
		newG.GetComponent<grenadeScript>().init(isGas, pos);
	}
    public void updateMustardGas()
    {
        int[,] newMustardGas = new int[xDim * 2, yDim * 2];
        for(int i=0;i<xDim*2;i++)
        {
            for(int i2=0;i2<yDim*2;i2++)
            {
                for (int i3 = 0; i3 < mustardGas[i, i2]; i3++)
                {
                    if (Random.Range(0, 200) > 0)
                    {
                        int x = i + Random.Range(-1, 2);
                        int y = i2 + Random.Range(-1, 2);
                        if (x >= 0 && x < xDim * 2 && y >= 0 && y < yDim * 2)
                        {
                            if (terrain[i / 2, i2 / 2])
                            {
                                if (terrain[x / 2, y / 2])
                                {
                                    if (Random.Range(0, 100) < 50)
                                    {
                                        newMustardGas[x, y]++;
                                    }
                                    else
                                    {
                                        newMustardGas[i, i2]++;
                                    }
                                }
                                else
                                {
                                    if (Random.Range(0, 100) < (mustardGas[i, i2] - 12)/2)
                                    {
                                        newMustardGas[x, y]++;
                                    }
                                    else
                                    {
                                        newMustardGas[i, i2]++;
                                    }
                                }
                            }
                            else
                            {
                                if (Random.Range(0, 100) < mustardGas[i, i2]/2)
                                {
                                    newMustardGas[x,y]++;
                                }
                                else
                                {
                                    newMustardGas[i, i2]++;
                                }
                            }
                        }
                    }
                }
            }
        }

        Texture2D t = gasImage.GetComponent<SpriteRenderer>().sprite.texture;
        Color32[] cs = t.GetPixels32();
        for (int i = 0; i < 2 * xDim; i++)
        {
            for (int i2 = 0; i2 < 2 * yDim; i2++)
            {
                if (newMustardGas[i, i2] != mustardGas[i, i2])
                {
                    mustardGas[i, i2] = newMustardGas[i, i2];
                }
            }
        }
        for (int i = 0; i < 2 * xDim; i++)
        {
            for (int i2 = 0; i2 < 2 * yDim; i2++)
            {
                float newVal = 0.9f * realMustardGas[i, i2] + 0.1f * mustardGas[i,i2];
                if(newVal<0.1)
                {
                    newVal = 0;
                }
                if(realMustardGas[i,i2]!=newVal)
                {
                    realMustardGas[i, i2] = newVal;
                    if (realMustardGas[i, i2] > 0)
                    {
                        cs[i2 * 2 * xDim + i] = new Color(0.835f, 0.835f, 0.404f, 0.5f + realMustardGas[i, i2] / 160.0f);
                    }
                    else
                    {
                        cs[i2 * 2 * xDim + i] = new Color(0.835f, 0.835f, 0.404f, 0);
                    }
                }
            }
        }
        t.SetPixels32(cs);
        t.Apply();
    }

	public void lightingAt(float y)
	{
		Instantiate(lightingMarker,new Vector2(10,y),Quaternion.identity);
		lights--;
	}
	public void bombAt(Vector2 pos)
	{
		Instantiate(bombingMarker,new Vector2(pos.x+12.5f,pos.y),Quaternion.identity);
		bombs--;
	}
	public void bomb(Vector2 pos)
	{
		foreach(GameObject e in enemies)
		{
			if (Vector2.Distance (e.transform.position, pos) < 3) {
				e.GetComponent<enemyScript> ().health-=30;
			}
		}
		createArtilleryEffect (pos);
	}

	public void createArtilleryEffect(Vector2 pos)
	{
		GameObject.Instantiate (artilleryEffect, new Vector3 (pos.x, pos.y, -4), Quaternion.identity);
	}
}
