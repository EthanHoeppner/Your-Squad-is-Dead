using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyScript : MonoBehaviour {

    public int health;
    public int maxHealth;
    public int fireDamage;
    public int fireTime;
    public int fireAccuracy;
    public int artilleryTime;
    public int artilleryDamage;
	public float artillerySpread;
	public int gatlingTime;
	public int gatlingDamage;
	public int gatlingAccuracy;
    public int digTime;
    public int repairTime;
    public int actionDelay;
    public int moveTime;

    public Vector2 movementDirection;

    public soldierScript.type enemyType;

    public Vector2 realPosition;
    public GameObject healthbarBackground;
    public GameObject healthbarForeground;

    public phase enemyPhase;

	public int fragCount;
	public int gasCount;

	public int spotlightCount;
	public int gatlingCount;

	public GameObject spotlight;
	public GameObject gatling;

    controller c;

    public enum phase
    {
        attack,
        advance,
        retreat,
        hold
    }

	// Use this for initialization
	void Start () {
        actionDelay = 0;
        moveTime = 10;
        fireTime = 40;
        realPosition = transform.position;
        maxHealth = 50;
        fireDamage = 20;
        artilleryDamage = 30;
		artillerySpread = 3.8f;
        artilleryTime = 200;
		gatlingAccuracy = 70;
		gatlingTime = 5;
		gatlingDamage = 10;
        digTime = 60;
        repairTime = 15;
        fireAccuracy = 75;

		fragCount = 0;
		gasCount = 0;
		spotlightCount = 0;
		gatlingCount = 0;
		reinforcementConsumables ();

        health = maxHealth;
        movementDirection = new Vector2(0, 0);
        GameObject[] background = GameObject.FindGameObjectsWithTag("healthbarBackground");
        for(int i=0;i<background.Length;i++)
        {
            if(background[i].transform.parent==transform)
            {
                healthbarBackground = background[i];
                break;
            }
        }

        GameObject[] foreground = GameObject.FindGameObjectsWithTag("healthbarForeground");
        for (int i = 0; i < foreground.Length; i++)
        {
            if (foreground[i].transform.parent == transform)
            {
                healthbarForeground = foreground[i];
                break;
            }
        }

        enemyPhase = phase.hold;

        c = Camera.main.gameObject.GetComponent<controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health == maxHealth)
        {
            healthbarBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            healthbarForeground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            healthbarBackground.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            healthbarForeground.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            healthbarForeground.transform.localPosition = new Vector3((((float)health) / maxHealth) * 0.5f-0.5f,-0.4f,-2);
            healthbarForeground.transform.localScale = new Vector3((((float)health) / maxHealth) * 64, 8, 1);
        }
    }

    public void FixedUpdate()
    {
        int gx = (int)(transform.position.x * 2);
        int gy = (int)(transform.position.y * 2);
		actionDelay--;
		if (c.realMustardGas [gx, gy] > 0 || c.realMustardGas [gx + 1, gy] > 0 || c.realMustardGas [gx, gy + 1] > 0 || c.realMustardGas [gx + 1, gy + 1] > 0) {
			health -= 1;
			if (health <= 0) {
				Destroy (gameObject);
				c.enemies.Remove (gameObject);
			}
			if (actionDelay <= 0) {
				realPosition = new Vector2 (realPosition.x, realPosition.y + 1);
				actionDelay = moveTime;
			}
		} else {
			if (actionDelay <= 0) {
				switch (enemyType) {
				case soldierScript.type.infantry:
					infantryAI ();
					break;
				case soldierScript.type.artillery:
					artilleryAI ();
					break;
				case soldierScript.type.engineer:
					engineerAI ();
					break;
				}
				if (actionDelay == 0 && !isEngineerInGatling ()) {
					move ();
				}
			}
		}
        float distanceX = realPosition.x - transform.position.x;
        float distanceY = realPosition.y - transform.position.y;
        transform.position = new Vector3(transform.position.x + Mathf.Sign(distanceX) * Mathf.Min((1.0f / moveTime), Mathf.Abs(distanceX)), transform.position.y + Mathf.Sign(distanceY) * Mathf.Min((1.0f / moveTime), Mathf.Abs(distanceY)), -2);
    }

    public bool getAttacked(int damage)
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if (c.terrain[x, y])
        {
            if (Random.Range(0, 100) < 50)
            {
                health -= damage;
                c.enemyDamaged(damage);
                if (health <= 0)
                {
                    c.enemies.Remove(gameObject);
                    Destroy(gameObject);
                    c.enemyDied();
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
            c.enemyDamaged(damage);
            if (health <= 0)
            {
                c.enemies.Remove(gameObject);
                Destroy(gameObject);
                c.enemyDied();
                return true;
            }
            return false;
        }
    }

    public void fire(GameObject soldier)
    {
        if (Random.Range(0, 100) < fireAccuracy)
        {
            soldierScript s = soldier.GetComponent<soldierScript>();
            s.getAttacked(fireDamage);
        }
        int x = (int)soldier.transform.position.x;
        int y = (int)soldier.transform.position.y;
        if (x>= 0 && x< c.xDim && y >= 0 && y < c.yDim && c.terrain[x, y])
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
        actionDelay = fireTime;
    }
	public void shootGatling(GameObject soldier)
	{
		if (Random.Range(0, 100) < gatlingAccuracy)
		{
			soldierScript s = soldier.GetComponent<soldierScript>();
			s.getAttacked(gatlingDamage);
		}
		int x = (int)soldier.transform.position.x;
		int y = (int)soldier.transform.position.y;
		if (x>= 0 && x< c.xDim && y >= 0 && y < c.yDim && c.terrain[x, y])
		{
			c.trenches[x, y].GetComponent<trenchScript>().damage(1);
		}
		actionDelay = gatlingTime;
	}
    public void artillery(Vector2 position)
    {
        actionDelay = artilleryTime;
		Vector2 actualTarget = position;
		while(true)
		{
			Vector2 off=new Vector2(Random.Range(-artillerySpread,artillerySpread),Random.Range(-artillerySpread,artillerySpread));
			if(off.sqrMagnitude<=artillerySpread*artillerySpread)
			{
				actualTarget+=off;
				break;
			}
		}
        List<GameObject> soldiers = c.soldiers;
        for (int i = 0; i < soldiers.Count; i++)
        {
            soldierScript s = soldiers[i].GetComponent<soldierScript>();
            float dx = actualTarget.x - s.transform.position.x;
			float dy = actualTarget.y - s.transform.position.y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            if (dist <= 3)
            {
                if (s.getAttacked(artilleryDamage))
                {
                    i--;
                }
            }
        }
        int x = (int)actualTarget.x;
		int y = (int)actualTarget.y;
        for (int i = -3; i <= 3; i++)
        {
            for (int i2 = -3; i2 <= 3; i2++)
            {
                if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim && c.terrain[x + i, y + i2])
                {
                    int d = 1;
                    if (Mathf.Abs(i) < 2 && Mathf.Abs(i2) < 2)
                    {
                        d = 2;
                    }
                    c.trenches[x + i, y + i2].GetComponent<trenchScript>().damage(d);
                }
            }
        }
		c.createArtilleryEffect (actualTarget);
    }

    public void wanderInTrenches()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        int mX = (int)movementDirection.x;
        int mY = (int)movementDirection.y;
        bool[,] terrain = c.terrain;

		if((mX==0 && mY==0) || !(x+mX>=0 && x+mX<c.xDim && y+mY>=0 && y+mY<c.yDim) || !terrain[x+mX,y+mY] || Random.Range(0,100)<10)
        {
            List<Vector2> possibleMovements = new List<Vector2>();
            if(x+1<c.xDim && terrain[x+1,y])
            {
                possibleMovements.Add(new Vector2(1, 0));
            }
            if (x-1>=0 && terrain[x - 1, y])
            {
                possibleMovements.Add(new Vector2(-1, 0));
            }
            if (y+1<c.yDim && terrain[x, y+1])
            {
                possibleMovements.Add(new Vector2(0, 1));
            }
            if (y-1>=0 && terrain[x, y-1])
            {
                possibleMovements.Add(new Vector2(0, -1));
            }
            
            if (possibleMovements.Count > 0)
            {
                movementDirection = possibleMovements[Random.Range(0, possibleMovements.Count)];
            }
        }
    }
    public void gotoNearestTrench()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        for(int r=0;r<c.xDim && r<c.yDim; r++)
        {
            List<Vector2> directions = new List<Vector2>();
            for(int i=-r;i<=r;i++)
            {
                for(int i2=-r;i2<=r;i2++)
                {
                    if(x+i>=0 && x+i<c.xDim && y+i2>=0 && y+i2<c.yDim && c.terrain[x+i,y+i2])
                    {
                        int xDir = i==0?1:(int)Mathf.Sign(i);
                        int yDir = i2 == 0 ? 1 : (int)Mathf.Sign(i2);
                        directions.Add(new Vector2(xDir,yDir));
                    }
                }
            }
            if(directions.Count>0)
            {
                movementDirection = directions[Random.Range(0, directions.Count)];
                break;
            }
        }
    }
    public void pathfindDownInTrenches()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        for(int r=1;true;r++)
        {
            List<Vector2> trenchPositions = new List<Vector2>();
            for(int i=-r;i<=r;i++)
            {
                for(int i2=-r;i2<=-1;i2++)
                {
                    if(x+i>=0 && x+i<c.xDim && y+i2>=0 && y+i2<c.yDim && c.terrain[x+i,y+i2])
                    {
                        trenchPositions.Add(new Vector2(x + i, y + i2));
                    }
                }
            }
            if(trenchPositions.Count>0)
            {
                Vector2 chosenPos = trenchPositions[Random.Range(0, trenchPositions.Count)];
                int chosenX = (int)chosenPos.x;
                int chosenY = (int)chosenPos.y;
                if(r==1)
                {
                    int xDir = chosenX == x ? 0 : (int)Mathf.Sign(chosenX - x);
                    movementDirection = new Vector2(xDir, -1);
                    break;
                }
                else
                {

                    int xDir = chosenX == x ? 0 : (int)Mathf.Sign(chosenX - x);
                    if(xDir==0)
                    {
                        movementDirection = new Vector2(0, -1);
                        break;
                    }
                    else
                    {

                        if(x+xDir<c.xDim && x+xDir>=0 && c.terrain[x+xDir,y])
                        {
                            movementDirection = new Vector2(xDir, 0);
                            break;
                        }
                        else
                        {
                            movementDirection = new Vector2(xDir, -1);
                            break;
                        }
                    }
                }
            }
            else
            {
                if(y-r<=0)
                {
                    movementDirection = new Vector2(0, 0);
                    break;
                }
            }
        }
    }
    public void pathfindUpInTrenches()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        for (int r = 1; true; r++)
        {
            List<Vector2> trenchPositions = new List<Vector2>();
            for (int i = -r; i <= r; i++)
            {
                for (int i2 = 1; i2 <= r; i2++)
                {
                    if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim && c.terrain[x + i, y + i2])
                    {
                        trenchPositions.Add(new Vector2(x + i, y + i2));
                    }
                }
            }
            if (trenchPositions.Count > 0)
            {
                Vector2 chosenPos = trenchPositions[Random.Range(0, trenchPositions.Count)];
                int chosenX = (int)chosenPos.x;
                int chosenY = (int)chosenPos.y;
                if (r == 1)
                {
                    int xDir = chosenX == x ? 0 : (int)Mathf.Sign(chosenX - x);
                    movementDirection = new Vector2(xDir, 1);
                    break;
                }
                else
                {

                    int xDir = chosenX == x ? 0 : (int)Mathf.Sign(chosenX - x);
                    if (xDir == 0)
                    {
                        movementDirection = new Vector2(0, 1);
                        break;
                    }
                    else
                    {

                        if (x + xDir < c.xDim && x + xDir >= 0 && c.terrain[x + xDir, y])
                        {
                            movementDirection = new Vector2(xDir, 0);
                            break;
                        }
                        else
                        {
                            movementDirection = new Vector2(xDir, 1);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (y + r >= c.yDim)
                {
                    movementDirection = new Vector2(0, 0);
                    break;
                }
            }
        }
    }
    public void move()
    {
        realPosition += movementDirection;
        actionDelay = moveTime;
    }

    public void infantryAI()
    {
		List<GameObject> inRange=getEnemiesInGrenadeRange();
		if((fragCount>0 || gasCount>0) &&inRange.Count>0 && Random.Range(0,100)<1)
		{
			if(fragCount>0)
			{
				c.launchEnemyGrenade(gameObject,inRange[Random.Range(0,inRange.Count)].transform.position,false);
			}
			else
			{
				c.launchEnemyGrenade(gameObject,inRange[Random.Range(0,inRange.Count)].transform.position,true);
			}
		}
		else
		{
	        List<GameObject> soldiers = c.soldiers;
	        List<GameObject> nearbySoldiers = new List<GameObject>();
	        for (int i = 0; i < soldiers.Count; i++)
	        {
	            float dx = transform.position.x - soldiers[i].transform.position.x;
	            float dy = transform.position.y - soldiers[i].transform.position.y;
	            if (Mathf.Sqrt(dx * dx + dy * dy) < 13/*experiment*/*0.6f)
	            {
	                int soldierX = 1 + (int)(3 * soldiers[i].transform.position.x);
	                int soldierY = 1 + (int)(3 * soldiers[i].transform.position.y);
	                int vis = 0;
	                for (int i2 = -1; i2 <= 1 && vis<4; i2++)
	                {
	                    for (int i3 = -1; i3 <= 1 && vis<4; i3++)
	                    {
	                        if (soldierX + i2 >= 0 && soldierX + i2 < 3*c.xDim && soldierY + i3 >= 0 && soldierY + i3 < 3*c.yDim && c.enemyVisibility[soldierX + i2, soldierY + i3] != 1)
	                        {
	                            vis++;
	                        }
	                    }
	                }
	                if (vis>=4)
	                {
	                    nearbySoldiers.Add(soldiers[i]);
	                }
	            }
	        }
	        if (nearbySoldiers.Count > 0 && enemyPhase != phase.retreat)
	        {
	            fire(nearbySoldiers[Random.Range(0, nearbySoldiers.Count)]);
	        }
	        else
	        {
	            switch(enemyPhase)
	            {
	                case phase.hold:
	                    if (c.terrain[(int)realPosition.x, (int)realPosition.y])
	                    {
	                        wanderInTrenches();
	                    }
	                    else
	                    {
	                        gotoNearestTrench();
	                    }
	                    break;
	                case phase.advance:
	                    pathfindDownInTrenches();
	                    break;
	                case phase.attack:
	                    realPosition = new Vector2(realPosition.x + Random.Range(-1, 2), realPosition.y - 1);
	                    actionDelay = moveTime;
	                    break;
	                case phase.retreat:
	                    pathfindUpInTrenches();
	                    break;
	            }
	        }
        }
    }
    public void artilleryAI()
    {
    	List<GameObject> inRange=getEnemiesInGrenadeRange();
    	if((fragCount>0 || gasCount>0) &&inRange.Count>0 && Random.Range(0,100)<1)
    	{
			if(fragCount>0)
			{
				c.launchEnemyGrenade(gameObject,inRange[Random.Range(0,inRange.Count)].transform.position,false);
			}
			else
			{
				c.launchEnemyGrenade(gameObject,inRange[Random.Range(0,inRange.Count)].transform.position,true);
			}
		}
		else
		{
	        List<GameObject> soldiers = c.soldiers;
	        List<GameObject> nearbySoldiers = new List<GameObject>();
	        for (int i = 0; i < soldiers.Count; i++)
	        {
	            float dx = transform.position.x - soldiers[i].transform.position.x;
	            float dy = transform.position.y - soldiers[i].transform.position.y;
	            if (Mathf.Sqrt(dx * dx + dy * dy) < 19/**/*0.6f)
	            {
	                int soldierX = 1 + (int)(3 * soldiers[i].transform.position.x);
	                int soldierY = 1 + (int)(3 * soldiers[i].transform.position.y);
	                int vis = 0;
	                for (int i2 = -1; i2 <= 1 && vis<4; i2++)
	                {
	                    for (int i3 = -1; i3 <= 1 && vis<4; i3++)
	                    {
	                        if (soldierX + i2 >= 0 && soldierX + i2 < 3*c.xDim && soldierY + i3 >= 0 && soldierY + i3 < 3*c.yDim && c.enemyVisibility[soldierX + i2, soldierY + i3] != 1)
	                        {
	                            vis++;
	                        }
	                    }
	                }
	                if (vis>=4)
	                {
	                    nearbySoldiers.Add(soldiers[i]);
	                }
	            }
	        }
	        if (nearbySoldiers.Count > 0 && enemyPhase!=phase.retreat)
	        {
	            artillery(nearbySoldiers[Random.Range(0, nearbySoldiers.Count)].transform.position);
	        }
	        else
	        {
	            switch (enemyPhase)
	            {
	                case phase.hold:
	                    if (c.terrain[(int)realPosition.x, (int)realPosition.y])
	                    {
	                        wanderInTrenches();
	                    }
	                    else
	                    {
	                        gotoNearestTrench();
	                    }
	                    break;
	                case phase.advance:
	                    pathfindDownInTrenches();
	                    break;
	                case phase.attack:
	                    realPosition = new Vector2(realPosition.x + Random.Range(-1, 2), realPosition.y - 1);
	                    actionDelay = moveTime;
	                    break;
	                case phase.retreat:
	                    pathfindUpInTrenches();
	                    break;
	            }
	        }
        }
    }
    public void engineerAI()
    {
		if(isEngineerInGatling())
		{
			List<GameObject> enemies=getEnemiesInRange();
			GameObject e=enemies[Random.Range(0,enemies.Count)];
			shootGatling(e);
			GameObject g=getGatling();
			g.transform.rotation=Quaternion.Euler(0,0,(180/Mathf.PI)*Mathf.Atan2(g.transform.position.y-e.transform.position.y,g.transform.position.x-e.transform.position.x)+90);
		}
		else
		{
	    	List<GameObject> inRange=getEnemiesInGrenadeRange();
	    	if((fragCount>0 || gasCount>0) &&inRange.Count>0 && Random.Range(0,100)<40)
	    	{
				if(fragCount>0)
				{
					c.launchEnemyGrenade(gameObject,inRange[Random.Range(0,inRange.Count)].transform.position,false);
				}
				else
				{
					c.launchEnemyGrenade(gameObject,inRange[Random.Range(0,inRange.Count)].transform.position,true);
				}
			}
			else
			{
				if((spotlightCount>0 || gatlingCount>0) && Random.Range(0,100)<10)
				{
					if(gatlingCount>0)
					{
						placeGatling();
					}
					else
					{
						placeSpotlight();
					}
				}
				else
				{
			        switch (enemyPhase)
			        {
			            case phase.hold:
			                {
			                    int x = (int)realPosition.x;
			                    int y = (int)realPosition.y;
			                    if (c.terrain[x, y] && c.trenches[x, y].GetComponent<trenchScript>().health < 4)
			                    {
			                        c.trenches[x, y].GetComponent<trenchScript>().repair();
			                        actionDelay = repairTime;
			                    }
			                    else
			                    {
			                        List<GameObject> repairable = new List<GameObject>();
			                        for (int i = -1; i <= 1; i++)
			                        {
			                            for (int i2 = -1; i2 <= 1; i2++)
			                            {
			                                if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim)
			                                {
			                                    if (c.terrain[x + i, y + i2] && c.trenches[x + i, y + i2].GetComponent<trenchScript>().health < 4)
			                                    {
			                                        repairable.Add(c.trenches[x + i, y + i2]);
			                                    }
			                                }
			                            }
			                        }
			                        if (repairable.Count > 0)
			                        {
			                            GameObject chosen = repairable[Random.Range(0, repairable.Count)];
			                            realPosition = chosen.transform.position;
			                            actionDelay = moveTime;
			                        }
			                        else
			                        {
			                            if ((x - 1 >= 0 && !c.terrain[x - 1, y]) || (x + 1 < c.xDim && !c.terrain[x + 1, y]))
			                            {
			                                int nearbyTrenches = 0;
			                                for (int i = -8; i <= 8; i++)
			                                {
			                                    for (int i2 = -3; i2 <= 3; i2++)
			                                    {
			                                        if (x + i >= 0 && x + i < c.xDim && y + i2 >= 0 && y + i2 < c.yDim && c.terrain[x + i, y + i2])
			                                        {
			                                            nearbyTrenches++;
			                                        }
			                                    }
			                                }
			                                if (nearbyTrenches < 14)
			                                {
			                                    if (x - 1 >= 0 && !c.terrain[x - 1, y] && x + 1 < c.xDim && !c.terrain[x + 1, y])
			                                    {
			                                        if (Random.Range(0, 100) < 50)
			                                        {
			                                            c.terrain[x - 1, y] = true;
			                                            realPosition = new Vector2(x - 1, y);
			                                        }
			                                        else
			                                        {
			                                            c.terrain[x + 1, y] = true;
			                                            realPosition = new Vector2(x + 1, y);
			                                        }
			                                    }
			                                    else if (x - 1 >= 0 && !c.terrain[x - 1, y])
			                                    {
			                                        c.enemyTerrain[x - 1, y] = true;
			                                        realPosition = new Vector2(x - 1, y);
			                                    }
			                                    else
			                                    {
			                                        c.terrain[x + 1, y] = true;
			                                        realPosition = new Vector2(x + 1, y);
			                                    }
			                                    actionDelay = digTime;
			                                }
			                            }
			                            if (c.terrain[(int)realPosition.x, (int)realPosition.y])
			                            {
			                                wanderInTrenches();
			                            }
			                            else
			                            {
			                                gotoNearestTrench();
			                            }
			                        }
			                    }
			                    break;
			                }
			            case phase.advance:
			            case phase.attack:
			                {
			                    int x = (int)realPosition.x;
			                    int y = (int)realPosition.y;
			                    if (c.terrain[x, y - 1])
			                    {
			                        realPosition = new Vector2(x, y-1);
			                        actionDelay = moveTime;
			                    }
			                    else
			                    {
			                        c.terrain[x, y - 1] = true;
			                        realPosition = new Vector2(x, y-1);
			                        actionDelay = digTime;
			                    }
			                    break;
			                }
			            case phase.retreat:
			                pathfindUpInTrenches();
			                break;
			        }
		        }
	        }
        }
    }

	public void placeSpotlight()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if (!c.terrain[x, y - 1])
        {
            ((GameObject)Instantiate(spotlight, new Vector3(x, y - 1, -2), Quaternion.identity)).GetComponent<spotlightScript>().player=false;
            spotlightCount--;
        }
    }
    public void placeGatling()
    {
        int x = (int)realPosition.x;
        int y = (int)realPosition.y;
        if (!c.terrain[x, y - 1])
        {
            ((GameObject)Instantiate(gatling, new Vector3(x, y - 1, -2), Quaternion.identity)).GetComponent<gatlingScript>().player=false;;
            gatlingCount--;
        }
    }

	public bool isEngineerInGatling()
    {
        if(enemyType==soldierScript.type.engineer)
        {
            GameObject[] gatlings = GameObject.FindGameObjectsWithTag("gatling");
            int x = (int)transform.position.x;
            int y = (int)transform.position.y; 
            foreach (GameObject g in gatlings)
            {
				if(!g.GetComponent<gatlingScript>().player)
				{
	                int gx = (int)g.transform.position.x;
	                int gy = (int)g.transform.position.y;
	                if(x==gx && y-1==gy)
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
        if(enemyType==soldierScript.type.engineer)
        {
            GameObject[] gatlings = GameObject.FindGameObjectsWithTag("gatling");
            int x = (int)transform.position.x;
            int y = (int)transform.position.y;
            foreach (GameObject g in gatlings)
            {
	            if(!g.GetComponent<gatlingScript>().player)
				{
	                int gx = (int)g.transform.position.x;
	                int gy = (int)g.transform.position.y;
	                if (x == gx && y - 1 == gy)
	                {
	                    return g;
	                }
                }
            }
        }
        return null;
    }

	public List<GameObject> getEnemiesInGrenadeRange()
	{
		List<GameObject> inRange=new List<GameObject>();
		foreach(GameObject e in c.soldiers)
		{
			if(Vector2.Distance(transform.position,e.transform.position)<19*0.6f && enemyInRange(e))
			{
				inRange.Add(e);
			}
		}
		return inRange;
	}

	public bool enemyInRange(GameObject enemy)
    {
        float range = 0;
        if (enemyType ==soldierScript.type.infantry)
        {
            range = 13/*experiment*/* 0.6f;
        }
        if (enemyType == soldierScript.type.artillery)
        {
            range = 19/*experiment*/* 0.6f;
        }
        if(enemyType==soldierScript.type.engineer)
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
                    if (enemyX + i2 >= 0 && enemyX + i2 < 3 * c.xDim && enemyY + i3 >= 0 && enemyY + i3 < 3 * c.yDim && c.enemyVisibility[enemyX + i2, enemyY + i3] != 1)
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
        foreach (GameObject e in c.soldiers)
        {
            if(enemyInRange(e))
            {
                enemies.Add(e);
            }
        }
        return enemies;
    }

	public void reinforcementConsumables()
	{
		if(enemyType==soldierScript.type.engineer)
		{
			fragCount = 0;
			if (Random.Range (0, 100) < 30) {
				fragCount++;
			}
			gasCount = 0;
			if (Random.Range (0, 100) < 30) {
				gasCount++;
			}
			spotlightCount = 0;
			if (Random.Range (0, 100) < 30) {
				spotlightCount++;
			}
			gatlingCount = 0;
			if (Random.Range (0, 100) < 30) {
				gatlingCount++;
			}
		}
		else
		{
			fragCount = 0;
			if (Random.Range (0, 100) < 30) {
				fragCount++;
			}
			gasCount = 0;
			if (Random.Range (0, 100) < 30) {
				gasCount++;
			}fragCount = 0;
			if (Random.Range (0, 100) < 30) {
				fragCount++;
			}
			gasCount = 0;
			if (Random.Range (0, 100) < 30) {
				gasCount++;
			}
		}
	}
}
