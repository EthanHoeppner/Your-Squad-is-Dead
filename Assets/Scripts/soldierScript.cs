using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class soldierScript : MonoBehaviour {

    public Vector2 realPosition;
    public List<Vector2> path;
    public Vector2 movementTarget;
    public int movementTime;
    public int fireTime;
    public int artilleryTime;
    public int digTime;
    public int repairTime;
    public bool digging;
    public int actionDelay;
    public int fireDamage;
    public int artilleryDamage;
    public int fireAccuracy;
    public float artillerySpread;
    public int fragCount;
    public int gasCount;
    public int spotlightCount;
    public int gatlingCount;
    public int gatlingAccuracy;
    public int gatlingTime;
    public int gatlingDamage;
    public type soldierType;
	public int exp;
	public int maxExp;
    public displayType soldierDisplay;
    
    public GameObject progressBackground;
    public GameObject progressForeground;
    public GameObject healthbarBackground;
    public GameObject healthbarForeground;

    public int health;
    public int maxHealth;

    public bool attacking;

    public Vector2 markerPos;

    controller c;

    public GameObject spotlight;
    public GameObject gatling;

    public enum type
    {
        infantry,
        engineer,
        artillery
    }
    public enum displayType
    {
        none,
        recuperatingFire,
        recuperatingArtillery,
        recuperatingDig,
    }
    public enum action
    {
        none,
        fire,
        artillery,
        look,
        attack,
        gatling
    }
    public enum rank
    {
        priv,
        lieutenant,
        sergeant
    }

	public rank soldierRank;

    public action nextAction;
    public GameObject targetedEnemy;
    public GameObject pathPoint;

    // Use this for initialization
    void Start () {
        realPosition = transform.position;
        path = new List<Vector2>();
        movementTime = 10;
        fireTime = 40;
        fireDamage = 20;
        fireAccuracy = 75;
        artilleryDamage = 30;
        artilleryTime = 200;
        artillerySpread = 3.8f;
        digTime = 60;
        gatlingAccuracy = 70;
        gatlingTime = 5;
        gatlingDamage = 10;
        repairTime = 15;
        actionDelay = 0;
        maxHealth = 50;
        health = maxHealth;
        fragCount = 1;
        gasCount = 1;
        spotlightCount = 1;
        gatlingCount = 1;
		exp = 0;
		maxExp = 100;

        GameObject[] background = GameObject.FindGameObjectsWithTag("playerProgressBackground");
        for (int i = 0; i < background.Length; i++)
        {
            if (background[i].transform.parent == transform)
            {
                progressBackground = background[i];
                break;
            }
        }

        GameObject[] foreground = GameObject.FindGameObjectsWithTag("playerProgressForeground");
        for (int i = 0; i < foreground.Length; i++)
        {
            if (foreground[i].transform.parent == transform)
            {
                progressForeground = foreground[i];
                break;
            }
        }

        background = GameObject.FindGameObjectsWithTag("healthbarBackground");
        for (int i = 0; i < background.Length; i++)
        {
            if (background[i].transform.parent == transform)
            {
                healthbarBackground = background[i];
                break;
            }
        }

        foreground = GameObject.FindGameObjectsWithTag("healthbarForeground");
        for (int i = 0; i < foreground.Length; i++)
        {
            if (foreground[i].transform.parent == transform)
            {
                healthbarForeground = foreground[i];
                break;
            }
        }

        nextAction = action.none;
        markerPos = transform.position;

        c = Camera.main.gameObject.GetComponent<controller>();

        attacking = true;

		soldierRank = rank.priv;
    }

    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            for (int i = 0; i < transform.GetChild(6).childCount; i++)
            {
                transform.GetChild(6).GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f,1);
            }
        }
        else
        {
            for (int i = 0; i < transform.GetChild(6).childCount; i++)
            {
                transform.GetChild(6).GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 1, 0.5f, 1);
            }
        }
        if(soldierType==type.engineer)
        {
            attacking = false;
            if(targetedEnemy!=null)
            {
                Debug.Log("YOLO");
                GameObject g = getGatling();
                g.transform.localRotation = Quaternion.Euler(0, 0, (180.0f / Mathf.PI) * Mathf.Atan2(g.transform.position.y - targetedEnemy.transform.position.y, g.transform.position.x - targetedEnemy.transform.position.x)+90);
            }
        }
        else
        {
            spotlightCount = 0;
            gatlingCount = 0;
        }
        transform.GetChild(6).position = new Vector3(markerPos.x, markerPos.y, -5);
        if(targetedEnemy==null)
        {
            transform.GetChild(7).position = new Vector3(-500, -500, -5);
        }
        else
        {
            transform.GetChild(7).position = new Vector3(targetedEnemy.transform.position.x, targetedEnemy.transform.position.y, -5);
            float x = transform.position.x;
            float y = transform.position.y;
            if(soldierType==type.engineer)
            {
                y++;
            }
            transform.GetChild(7).GetChild(0).rotation=Quaternion.Euler(0,0, (180.0f/Mathf.PI)*Mathf.Atan2(y- targetedEnemy.transform.position.y, transform.position.x- targetedEnemy.transform.position.x)-90);
            transform.GetChild(7).GetChild(0).localScale = new Vector3(1,Vector2.Distance(new Vector2(x,y),targetedEnemy.transform.position)/3, 1);
        }
        if (health == maxHealth)
        {
            healthbarBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            healthbarForeground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            healthbarBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            healthbarForeground.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            healthbarForeground.transform.localPosition = new Vector3((((float)health) / maxHealth) * 0.5f - 0.5f, -0.4f, -2);
            healthbarForeground.transform.localScale = new Vector3((((float)health) / maxHealth) * 64, 8, 1);
        }
        switch (soldierDisplay)
        {
            case displayType.none:
                {
                    progressBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    progressForeground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    break;
                }
            case displayType.recuperatingFire:
                {
                    progressBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    progressForeground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.4f, 0.4f, 1.0f);
                    progressForeground.transform.localPosition = new Vector3((((float)actionDelay) / fireTime) * 0.5f - 0.5f, 0.4f, -2);
                    progressForeground.transform.localScale = new Vector3((((float)actionDelay) / fireTime) * 64, 8, 1);
                    break;
                }
            case displayType.recuperatingArtillery:
                {
                    progressBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    progressForeground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.4f, 0.4f, 1.0f);
                    progressForeground.transform.localPosition = new Vector3((((float)actionDelay) / artilleryTime) * 0.5f - 0.5f, 0.4f, -2);
                    progressForeground.transform.localScale = new Vector3((((float)actionDelay) / artilleryTime) * 64, 8, 1);
                    break;
                }
            case displayType.recuperatingDig:
                {
                    progressBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    progressForeground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.4f, 0.4f, 1.0f);
                    progressForeground.transform.localPosition = new Vector3((((float)actionDelay) / digTime) * 0.5f - 0.5f, 0.4f, -2);
                    progressForeground.transform.localScale = new Vector3((((float)actionDelay) / digTime) * 64, 8, 1);
                    break;
                }
        }
    }
    public void FixedUpdate()
    {
        for(int i=path.Count;i<transform.GetChild(6).childCount;i++)
        {
            GameObject p = transform.GetChild(6).GetChild(transform.GetChild(6).childCount-i-1).gameObject;
            p.transform.parent = null;
            Destroy(p);
        }
        if(targetedEnemy!=null && !enemyInRange(targetedEnemy))
        {
            targetedEnemy = null;
        }
        actionDelay = Mathf.Max(actionDelay - 1, 0);
        int gx = (int)(transform.position.x * 2);
        int gy = (int)(transform.position.y * 2);
        if (c.realMustardGas[gx, gy] > 0 || c.realMustardGas[gx+1, gy] > 0 || c.realMustardGas[gx, gy+1] > 0 || c.realMustardGas[gx+1, gy+1] > 0)
        {
            health -= 1;
            if(health<=0)
            {
                Destroy(gameObject);
                c.soldiers.Remove(gameObject);
            }
        }
        if (actionDelay == 0)
        {
            switch (nextAction)
            {
                case action.none:
                    {
                        soldierDisplay = displayType.none;
                        if ((soldierType!=type.engineer && !attacking) || !overwatchAttack())
                        {
                            if (path.Count > 0)
                            {
                                realPosition += path[0];
                                path.RemoveAt(0);
                                if (digging)
                                {
                                    bool[,] terrain = c.terrain;
                                    if (!terrain[(int)realPosition.x, (int)realPosition.y])
                                    {
                                        actionDelay = digTime;
                                        soldierDisplay = displayType.recuperatingDig;
                                        terrain[(int)realPosition.x, (int)realPosition.y] = true;
                                    }
                                    else
                                    {
                                        actionDelay = movementTime;
                                    }
                                }
                                else
                                {
                                    actionDelay = movementTime;
                                }
                            }
                            else
                            {
                                if (soldierType == type.engineer && !isEngineerInGatling())
                                {
                                    int x = (int)realPosition.x;
                                    int y = (int)realPosition.y;
                                    if (c.terrain[x, y] && c.trenches[x, y].GetComponent<trenchScript>().health < 4)
                                    {
                                        c.trenches[x, y].GetComponent<trenchScript>().repair();
                                        actionDelay = repairTime;
										gainExp (2);
                                    }
                                    else
                                    {
                                        List<Vector2> possibleRepairs = new List<Vector2>();
                                        for (int i = -1; i <= 1; i++)
                                        {
                                            for (int i2 = -1; i2 <= 1; i2++)
                                            {
                                                if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim)
                                                {
                                                    if (c.terrain[x + i, y + i2] && c.trenches[x + i, y + i2].GetComponent<trenchScript>().health < 4)
                                                    {
                                                        possibleRepairs.Add(new Vector2(x + i, y + i2));
                                                    }
                                                }
                                            }
                                        }
                                        if (possibleRepairs.Count > 0)
                                        {
                                            Vector2 chosenRepair = possibleRepairs[Random.Range(0, possibleRepairs.Count)];
                                            moveTo(chosenRepair);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case action.fire:
                    {
                        fire();
                        break;
                    }
                case action.artillery:
                    {
                        artillery();
                        break;
                    }
            }
        }
        float distanceX = realPosition.x - transform.position.x;
        float distanceY = realPosition.y - transform.position.y;
        transform.position = new Vector3(transform.position.x + Mathf.Sign(distanceX) * Mathf.Min((1.0f / movementTime),Mathf.Abs(distanceX)), transform.position.y + Mathf.Sign(distanceY) * Mathf.Min((1.0f / movementTime), Mathf.Abs(distanceY)), -2);
    }
    public void moveTo(Vector2 position)
    {
        Debug.Log(position);
        markerPos = position;
        movementTarget = position;
        digging = false;
        position = new Vector2(position.x, position.y);
        int x = (int)position.x;
        int y = (int)position.y;
        path = new List<Vector2>();
        bool[,] terrain = c.terrain;
        bool[,] trench = getSoldierTrench();

        if(trench[x,y])
        {
            int[,] pathArray = new int[c.xDim,c.yDim];
            pathArray[x, y] = 1;
            while(true)
            {
                bool breakLoop = true;
                for (int i = 0; i < c.xDim; i++)
                {
                    for (int i2 = 0; i2 < c.yDim; i2++)
                    {
                        if (pathArray[i, i2]!=0)
                        {
                            for (int i3 = -1; i3 <= 1; i3++)
                            {
                                for (int i4 = -1; i4 <= 1; i4++)
                                {
                                    if (i + i3 >= 0 && i + i3 < c.xDim && i2 + i4 >= 0 && i2 + i4 < c.yDim)
                                    {
                                        if (terrain[i + i3, i2 + i4] && pathArray[i + i3, i2 + i4]==0)
                                        {
                                            pathArray[i + i3, i2 + i4] = pathArray[i,i2]+1;
                                            breakLoop = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (breakLoop)
                {
                    break;
                }
            }
            int soldierX = (int)realPosition.x;
            int soldierY = (int)realPosition.y;
            while(soldierX != x || soldierY != y)
            {
                Vector2 lowestPos = new Vector2(0, 0);
                int lowest = 10000;
                for (int i = -1; i <= 1; i++)
                {
                    for (int i2 = -1; i2 <= 1; i2++)
                    {
                        if (i + soldierX >= 0 && i + soldierX < c.xDim && i2 + soldierY >= 0 && i2 + soldierY < c.yDim)
                        {
                            if (pathArray[soldierX + i, soldierY + i2] < lowest && pathArray[soldierX+i,soldierY+i2]>0)
                            {
                                lowest = pathArray[soldierX + i, soldierY + i2];
                                lowestPos = new Vector2(i, i2);
                            }
                        }
                    }
                }
                soldierX += (int)lowestPos.x;
                soldierY += (int)lowestPos.y;
                path.Add(lowestPos);
            }
        }
        else
        {
            int closestX = x;
            int closestY = y;
            int soldierX = (int)realPosition.x;
            int soldierY = (int)realPosition.y;
            if (trench[(int)realPosition.x, (int)realPosition.y])
            {
                int closest = 1000;
                for (int i = 0; i < c.xDim; i++)
                {
                    for (int i2 = 0; i2 < c.yDim; i2++)
                    {
                        int dist = Mathf.Abs(i - x) + Mathf.Abs(i2 - y);
                        if (trench[i, i2] && dist < closest)
                        {
                            closest = dist;
                            closestX = i;
                            closestY = i2;
                        }
                    }
                }

                int[,] pathArray = new int[c.xDim, c.yDim];
                pathArray[closestX, closestY] = 1;
                while (true)
                {
                    bool breakLoop = true;
                    for (int i = 0; i < c.xDim; i++)
                    {
                        for (int i2 = 0; i2 < c.xDim; i2++)
                        {
                            if (pathArray[i, i2] != 0)
                            {
                                for (int i3 = -1; i3 <= 1; i3++)
                                {
                                    for (int i4 = -1; i4 <= 1; i4++)
                                    {
                                        if (i + i3 >= 0 && i + i3 < c.xDim && i2 + i4 >= 0 && i2 + i4 < c.yDim)
                                        {
                                            if (terrain[i + i3, i2 + i4] && pathArray[i + i3, i2 + i4] == 0)
                                            {
                                                pathArray[i + i3, i2 + i4] = pathArray[i, i2] + 1;
                                                breakLoop = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (breakLoop)
                    {
                        break;
                    }
                }
                while (soldierX != closestX || soldierY != closestY)
                {
                    Vector2 lowestPos = new Vector2(0, 0);
                    int lowest = 10000;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int i2 = -1; i2 <= 1; i2++)
                        {
                            if (i + soldierX >= 0 && i + soldierX < c.xDim && i2 + soldierY >= 0 && i2 + soldierY < c.yDim)
                            {
                                if (pathArray[soldierX + i, soldierY + i2] < lowest && pathArray[soldierX + i, soldierY + i2] > 0)
                                {
                                    lowest = pathArray[soldierX + i, soldierY + i2];
                                    lowestPos = new Vector2(i, i2);
                                }
                            }
                        }
                    }
                    soldierX += (int)lowestPos.x;
                    soldierY += (int)lowestPos.y;
                    path.Add(lowestPos);
                }
            }
            while (soldierX!=x || soldierY!=y)
            {
                Vector2 step = new Vector2(Mathf.Sign(x-soldierX),Mathf.Sign(y-soldierY));
                if(soldierX==x)
                {
                    step.x = 0;
                }
                if(soldierY==y)
                {
                    step.y = 0;
                }
                soldierX += (int)step.x;
                soldierY += (int)step.y;
                path.Add(step);
            }
        }

        while(transform.GetChild(6).childCount>0)
        {
            GameObject p = transform.GetChild(6).GetChild(0).gameObject;
            p.transform.parent = null;
            Destroy(p);
        }

        Vector2 currentPoint = new Vector2(realPosition.x, realPosition.y);
        transform.GetChild(6).position = position;
        foreach (Vector2 p in path)
        {
            GameObject point = (GameObject)GameObject.Instantiate(pathPoint, new Vector3(currentPoint.x,currentPoint.y, -1.5f), Quaternion.identity);
            point.transform.parent = transform.GetChild(6);
            currentPoint += p;
        }
    }
    public bool[,] getSoldierTrench()
    {
        bool[,] trench = new bool[c.xDim,c.yDim];
        bool[,] terrain = c.terrain;
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if(!terrain[x,y])
        {
            return trench;
        }
        trench[x, y] = true;
        while(true)
        {
            bool breakLoop = true;
            for(int i=0;i<c.xDim;i++)
            {
                for(int i2=0;i2<c.yDim;i2++)
                {
                    if(trench[i,i2])
                    {
                        for(int i3=-1;i3<=1;i3++)
                        {
                            for(int i4=-1;i4<=1;i4++)
                            {
                                if(i+i3>=0 && i+i3<c.xDim && i2+i4>=0 && i2+i4<c.yDim)
                                {
                                    if(terrain[i+i3,i2+i4] && !trench[i+i3,i2+i4])
                                    {
                                        trench[i + i3, i2 + i4] = true;
                                        breakLoop = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(breakLoop)
            {
                break;
            }
        }
        return trench;
    }
    public void attemptFire(GameObject enemy)
    {
        float dx = transform.position.x - enemy.transform.position.x;
        float dy = transform.position.y - enemy.transform.position.y;
        if (Mathf.Sqrt(dx * dx + dy * dy)<13/*experiment*/*0.6f)
        {
            if (enemyInRange(enemy))
            {
                targetedEnemy = enemy;
                nextAction = action.fire;
            }
            else
            {
                targetedEnemy = null;
                nextAction = action.none;
            }
        }
        else
        {
            c.outOfRangeAlert();
            targetedEnemy = null;
            nextAction = action.none;
        }
    }
    public void attemptArtillery(GameObject enemy)
    {
        float dx = transform.position.x - enemy.transform.position.x;
        float dy = transform.position.y - enemy.transform.position.y;
        if (Mathf.Sqrt(dx * dx + dy * dy) < 19/*experiment*/* 0.6f)
        {
            if (enemyInRange(enemy))
            {
                targetedEnemy = enemy;
                nextAction = action.fire;
            }
            else
            {
                targetedEnemy = null;
                nextAction = action.none;
            }
        }
        else
        {
            c.outOfRangeAlert();
            targetedEnemy = null;
            nextAction = action.none;
        }
    }
    public void fire()
    {
        if (targetedEnemy == null || !enemyInRange(targetedEnemy))
        {
            nextAction = action.none;
        }
        else
        {
        	gainExp(10);
            actionDelay = fireTime;
            soldierDisplay = displayType.recuperatingFire;
            if (Random.Range(0, 100) < fireAccuracy)
            {
            	enemyScript e=targetedEnemy.GetComponent<enemyScript>();
                if(e.getAttacked(fireDamage))
                {
					if(e.health<=0)
					{
						gainExp(50);
					}
				}
            }
            else
            {
                c.worldCanvas.GetComponent<worldCanvasScript>().soldierMiss(targetedEnemy.transform.position);
            }
            if (targetedEnemy.GetComponent<enemyScript>().health > 0)
            {
                attemptFire(targetedEnemy);
            }
            else
            {
                nextAction = action.none;
            }

            int x = (int)targetedEnemy.transform.position.x;
            int y = (int)targetedEnemy.transform.position.y;
            if (x >= 0 && x < c.xDim && y >= 0 && y < c.yDim && c.terrain[x, y])
            {
                c.trenches[x, y].GetComponent<trenchScript>().damage(1);
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int i2 = -1; i2 <= 1; i2++)
                {
                    if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim && c.terrain[x + i, y + i2])
                    {
                        c.trenches[x + i, y + i2].GetComponent<trenchScript>().damage(1);
                    }
                }
            }
        }
    }
    public void artillery()
    {
    	Vector2 actualTarget=targetedEnemy.transform.position;
    	while(true)
    	{
			Vector2 off=new Vector2(Random.Range(-artillerySpread,artillerySpread),Random.Range(-artillerySpread,artillerySpread));
			if(off.sqrMagnitude<=artillerySpread*artillerySpread)
			{
				actualTarget+=off;
				break;
			}
		}
		gainExp(15);
        actionDelay = artilleryTime;
        soldierDisplay = displayType.recuperatingArtillery;
        List<GameObject> enemies = c.enemies;
        for(int i=0;i<enemies.Count;i++)
        {
            enemyScript e = enemies[i].GetComponent<enemyScript>();
            float dx = actualTarget.x - e.transform.position.x;
            float dy = actualTarget.y - e.transform.position.y;
            float dist = Mathf.Sqrt(dx*dx+dy*dy);
            if(dist<=3)
            {
                if(e.getAttacked(artilleryDamage))
                {
                	gainExp(50);
                    i--;
                }
            }
        }
        int x = (int)actualTarget.x;
        int y = (int)actualTarget.y;
        for (int i=-3;i<=3;i++)
        {
            for(int i2=-3;i2<=3;i2++)
            {
                if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim && c.terrain[x + i, y + i2])
                {
                    int d = 1;
                    if(Mathf.Abs(i)<2 && Mathf.Abs(i2)<2)
                    {
                        d = 2;
                    }
                    c.trenches[x + i, y + i2].GetComponent<trenchScript>().damage(d);
                }
            }
        }
        nextAction = action.none;
        c.createArtilleryEffect(actualTarget);
    }
    void shootGatling()
    {
        if (targetedEnemy == null || !enemyInRange(targetedEnemy))
        {
            nextAction = action.none;
        }
        else
        {
        	gainExp(2);
            actionDelay = gatlingTime;
            soldierDisplay = displayType.none;
            if (Random.Range(0, 100) < gatlingAccuracy)
            {
                if(targetedEnemy.GetComponent<enemyScript>().getAttacked(gatlingDamage))
                {
					gainExp(50);
				}
            }
            else
            {
                c.worldCanvas.GetComponent<worldCanvasScript>().soldierMiss(targetedEnemy.transform.position);
            }
            int x = (int)targetedEnemy.transform.position.x;
            int y = (int)targetedEnemy.transform.position.y;
            if (targetedEnemy.GetComponent<enemyScript>().health <= 0)
            {
                targetedEnemy = null;
            }
            if (x >= 0 && x < c.xDim && y >= 0 && y < c.yDim && c.terrain[x, y])
            {
                c.trenches[x, y].GetComponent<trenchScript>().damage(1);
            }
        }
    }
    public void digTo(Vector2 position)
    {
        digging = true;
        position = new Vector2(position.x, position.y);
        int x = (int)position.x;
        int y = (int)position.y;
        path = new List<Vector2>();
        bool[,] trench = getSoldierTrench();

        int currentX = (int)realPosition.x;
        int currentY = (int)realPosition.y;
        while(currentX!=x || currentY!=y)
        {
            int stepX = 0;
            if(currentX<x)
            {
                stepX = 1;
            }
            if (currentX > x)
            {
                stepX = -1;
            }
            int stepY = 0;
            if (currentY < y)
            {
                stepY = 1;
            }
            if (currentY > y)
            {
                stepY = -1;
            }
            currentX += stepX;
            currentY += stepY;
            path.Add(new Vector2(stepX, stepY));
        }
    }
    public bool getAttacked(int damage)
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if (c.terrain[x, y])
        {
        	Debug.Log("TEATDFSA");
            if (Random.Range(0, 100) < 50)
            {
                health -= damage;
                c.soldierDamaged(damage);
                if (health <= 0)
                {
                    c.soldiers.Remove(gameObject);
                    c.soldierDied();
                    Destroy(gameObject);
                    return true;
                }
            }
            else
            {
                c.worldCanvas.GetComponent<worldCanvasScript>().unitDodge(transform.position);
            }
            return false;
        }
        else
        {
            health -= damage;
            c.soldierDamaged(damage);
            if (health <= 0)
            {
                c.soldiers.Remove(gameObject);
                c.soldierDied();
                Destroy(gameObject);
                return true;
            }
            return false;
        }
    }

    bool overwatchAttack()
    {
        List<GameObject> possibleEnemies = getEnemiesInRange();
        if (possibleEnemies.Count > 0)
        {
            targetedEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
            if (soldierType == type.infantry)
            {
                nextAction = action.fire;
                fire();
            }
            if (soldierType == type.artillery)
            {
                nextAction = action.artillery;
                artillery();
            }
            if(soldierType==type.engineer)
            {
                shootGatling();
            }
            return true;
        }
        return false;
    }
    public bool enemyInRange(GameObject enemy)
    {
        float range = 0;
        if (soldierType == type.infantry)
        {
            range = 13/*experiment*/* 0.6f;
        }
        if (soldierType == type.artillery)
        {
            range = 19/*experiment*/* 0.6f;
        }
        if(soldierType==type.engineer)
        {
            range = 0;
            if(isEngineerInGatling())
            {
                range = 17/*experiment*/*0.6f;
            }
        }
        if (Vector2.Distance(transform.position, enemy.transform.position) <= range)
        {
            int enemyX = 1 + (int)(3 * enemy.transform.position.x);
            int enemyY = 1 + (int)(3 * enemy.transform.position.y);
            int vis = 0;
            for (int i2 = -1; i2 <= 1 && vis < 4; i2++)
            {
                for (int i3 = -1; i3 <= 1 && vis < 4; i3++)
                {
                    if (enemyX + i2 >= 0 && enemyX + i2 < 3 * c.xDim && enemyY + i3 >= 0 && enemyY + i3 < 3 * c.yDim && c.visibility[enemyX + i2, enemyY + i3] != 1)
                    {
                        vis++;
                    }
                }
            }
            return vis >= 4;
        }
        return false;
    }
    public List<GameObject> getEnemiesInRange()
    {
        List<GameObject> enemies = new List<GameObject>();
        foreach (GameObject e in c.enemies)
        {
            if(enemyInRange(e))
            {
                enemies.Add(e);
            }
        }
        return enemies;
    }
    public void placeSpotlight()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if (!c.terrain[x, y + 1])
        {
            ((GameObject)Instantiate(spotlight, new Vector3(x, y + 1, -2), Quaternion.identity)).GetComponent<spotlightScript>().player=true;
            spotlightCount--;
        }
    }
    public void placeGatling()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if (!c.terrain[x, y + 1])
        {
            ((GameObject)Instantiate(gatling, new Vector3(x, y + 1, -2), Quaternion.identity)).GetComponent<gatlingScript>().player=true;
            gatlingCount--;
        }
    }
    public bool isEngineerInGatling()
    {
        if(soldierType==type.engineer && path.Count==0)
        {
            GameObject[] gatlings = GameObject.FindGameObjectsWithTag("gatling");
            int x = (int)transform.position.x;
            int y = (int)transform.position.y; 
            foreach (GameObject g in gatlings)
            {
				if(g.GetComponent<gatlingScript>().player)
				{
	                int gx = (int)g.transform.position.x;
	                int gy = (int)g.transform.position.y;
	                if(x==gx && y+1==gy)
	                {
	                    return true;
	                }
                }
            }
        }
        return false;
    }
    public GameObject getGatling()
    {
        if (soldierType == type.engineer && path.Count == 0)
        {
            GameObject[] gatlings = GameObject.FindGameObjectsWithTag("gatling");
            int x = (int)transform.position.x;
            int y = (int)transform.position.y;
            foreach (GameObject g in gatlings)
            {
	            if(g.GetComponent<gatlingScript>().player)
				{
	                int gx = (int)g.transform.position.x;
	                int gy = (int)g.transform.position.y;
	                if (x == gx && y + 1 == gy)
	                {
	                    return g;
	                }
                }
            }
        }
        return null;
    }

	public void rankUp()
	{
		c.worldCanvas.GetComponent<worldCanvasScript>().soldierRankUp(transform.position);
		switch(soldierRank)
		{
			case rank.priv:
				soldierRank=rank.lieutenant;
		        movementTime = 8;
		        fireTime = 32;
		        fireDamage = 24;
		        fireAccuracy = 80;
		        artilleryDamage = 36;
		        artilleryTime = 160;
		        artillerySpread = 3.25f;
		        digTime = 45;
		        gatlingAccuracy = 80;
		        repairTime = 12;
		        maxHealth = 60;
		        health = maxHealth;
				break;
			case rank.lieutenant:
				soldierRank=rank.sergeant;
				movementTime = 6;
				fireTime = 24;
				fireDamage = 28;
				fireAccuracy = 85;
				artilleryDamage = 42;
				artilleryTime = 120;
				artillerySpread = 2.7f;
				digTime = 30;
				gatlingAccuracy = 90;
				repairTime = 9;
				maxHealth = 70;
				health = maxHealth;
				break;
		}
	}
	public void gainExp(int e)
	{
		if(soldierRank!=rank.sergeant)
		{
			exp+=e;
			if(exp>=maxExp)
			{
				rankUp();
				exp=0;
				switch(soldierRank)
				{
					case rank.lieutenant:
						maxExp=200;
						break;
					case rank.sergeant:
						maxExp=-1;
						break;
				}
			}
		}
	}
}