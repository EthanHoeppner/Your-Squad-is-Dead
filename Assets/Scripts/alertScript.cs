using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class alertScript : MonoBehaviour {

    float life;

	// Use this for initialization
	void Start () {
        life = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FixedUpdate()
    {
        life -= 0.05f;
        if (life > 0)
        {
            GetComponent<Text>().color = new Color(0, 0, 0, Mathf.Min(1,life/2));
        }
        else
        {
            GetComponent<Text>().color = new Color(0, 0, 0, 0);
        }
    }

    public void outOfRange()
    {
        GetComponent<Text>().text = "Out of Range";
        life = 2.5f;
    }
}
