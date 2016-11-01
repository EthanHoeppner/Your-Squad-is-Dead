using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class grenadeScript : MonoBehaviour {

    public Sprite frag;
    public Sprite gas;

    public float life;

    public bool isGas;
    float vX;
    float vY;
    float vA;

    controller c;

	public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
        life = 3;
        c = Camera.main.gameObject.GetComponent<controller>();
	}

    public void init(bool isGas, Vector2 target)
    {
        this.isGas = isGas;
        if(isGas)
        {
            GetComponent<SpriteRenderer>().sprite = gas;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = frag;
        }
        vX = (target.x - transform.position.x) / 3;
        vY = (target.y - transform.position.y) / 3;
        if(Random.Range(0,100)<50)
        {
            vA = Random.Range(120, 180);
        }
        else
        {
            vA = Random.Range(-180, -120);
        }
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
	
	// Update is called once per frame
	void Update () {
        life -= Time.deltaTime;
        transform.position += new Vector3(vX * Time.deltaTime, vY * Time.deltaTime,0);
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + vA * Time.deltaTime);
        if(life<0)
        {
            Destroy(gameObject);
            if(isGas)
            {
                for(int i=-6;i<=8;i++)
                {
                    for(int i2=-6;i2<=8;i2++)
                    {
                        int x = (int)(2*transform.position.x) + i;
                        int y = (int)(2*transform.position.y) + i2;
                        if(x>=0 && x<c.xDim*2 && y>=0 && y<c.yDim*2 && Mathf.Sqrt((i-1)*(i-1)+(i2-1)*(i2-1))<=7)
                        {
                            c.mustardGas[x, y] = 160;
                        }
                    }
                }
            }
            else
            {
				((GameObject)Instantiate (explosionEffect, new Vector3 (transform.position.x, transform.position.y,-4), Quaternion.identity)).GetComponent<artilleryExplosionEffectScript> ().size = 4;
                List<GameObject> soldiers = c.soldiers;
                for (int i = 0; i < soldiers.Count; i++)
                {
                    if (Vector2.Distance(soldiers[i].transform.position, transform.position) < 4)
                    {
                        soldiers[i].GetComponent<soldierScript>().getAttacked(40);
                    }
                }
                List<GameObject> enemies = c.enemies;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (Vector2.Distance(enemies[i].transform.position, transform.position) < 4)
                    {
                        enemies[i].GetComponent<enemyScript>().getAttacked(40);
                    }
                }
            }
        }
	}
    void FixedUpdate()
    {
        vA *= 0.99f;
    }
}
