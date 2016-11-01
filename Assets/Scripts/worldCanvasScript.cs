using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class worldCanvasScript : MonoBehaviour {

    public GameObject MissText;
    public List<GameObject> MissTexts;
    public List<float> MissTextAges;

    public GameObject DodgeText;
    public List<GameObject> DodgeTexts;
    public List<float> DodgeTextAges;

	public GameObject RankText;
	public List<GameObject> RankTexts;
	public List<float> RankTextAges;


    // Use this for initialization
    void Start () {
        MissTexts = new List<GameObject>();
        MissTextAges = new List<float>();
        DodgeTexts = new List<GameObject>();
		DodgeTextAges = new List<float>();
		RankTexts = new List<GameObject>();
		RankTextAges = new List<float>();
    }
	
	// Update is called once per frame
	void Update () {
	    for(int i=0;i<MissTexts.Count;i++)
        {
            MissTextAges[i] -= Time.deltaTime;
            if(MissTextAges[i] <=0)
            {
                GameObject m = MissTexts[i];
                MissTexts.RemoveAt(i);
                GameObject.Destroy(m);
                MissTextAges.RemoveAt(i);
                i--;
            }
            else
            {
                MissTexts[i].transform.localPosition = new Vector2(MissTexts[i].transform.localPosition.x, MissTexts[i].transform.localPosition.y + Time.deltaTime * 5);
                MissTexts[i].transform.localScale = new Vector2(MissTexts[i].transform.localScale.x - Time.deltaTime * 0.004f, MissTexts[i].transform.localScale.y - Time.deltaTime * 0.004f);
                MissTexts[i].GetComponent<Text>().color=new Color(0,0,0, MissTexts[i].GetComponent<Text>().color.a - Time.deltaTime);
            }
        }
        for (int i = 0; i < DodgeTexts.Count; i++)
        {
            DodgeTextAges[i] -= Time.deltaTime;
            if (DodgeTextAges[i] <= 0)
            {
                GameObject m = DodgeTexts[i];
                DodgeTexts.RemoveAt(i);
                GameObject.Destroy(m);
                DodgeTextAges.RemoveAt(i);
                i--;
            }
            else
            {
                DodgeTexts[i].transform.localPosition = new Vector2(DodgeTexts[i].transform.localPosition.x, DodgeTexts[i].transform.localPosition.y + Time.deltaTime * 5);
                DodgeTexts[i].transform.localScale = new Vector2(DodgeTexts[i].transform.localScale.x - Time.deltaTime * 0.004f, DodgeTexts[i].transform.localScale.y - Time.deltaTime * 0.004f);
                DodgeTexts[i].GetComponent<Text>().color = new Color(0, 0, 0, DodgeTexts[i].GetComponent<Text>().color.a - Time.deltaTime);
            }
        }
		for (int i = 0; i < RankTexts.Count; i++)
		{
			RankTextAges[i] -= Time.deltaTime;
			if (RankTextAges[i] <= 0)
			{
				GameObject m = RankTexts[i];
				RankTexts.RemoveAt(i);
				GameObject.Destroy(m);
				RankTextAges.RemoveAt(i);
				i--;
			}
			else
			{
				RankTexts[i].transform.localPosition = new Vector2(RankTexts[i].transform.localPosition.x, RankTexts[i].transform.localPosition.y + Time.deltaTime * 5);
				RankTexts[i].transform.localScale = new Vector2(RankTexts[i].transform.localScale.x - Time.deltaTime * 0.004f, RankTexts[i].transform.localScale.y - Time.deltaTime * 0.004f);
				RankTexts[i].GetComponent<Text>().color = new Color(0, 0, 0, RankTexts[i].GetComponent<Text>().color.a - Time.deltaTime);
			}
		}
    }
    public void soldierMiss(Vector2 pos)
    {
        GameObject m = (GameObject)GameObject.Instantiate(MissText,pos,Quaternion.identity);
        m.transform.SetParent(this.transform);
        MissTexts.Add(m);
        MissTextAges.Add(1);
    }
    public void unitDodge(Vector2 pos)
    {
        GameObject m = (GameObject)GameObject.Instantiate(DodgeText, pos, Quaternion.identity);
        m.transform.SetParent(this.transform);
        DodgeTexts.Add(m);
        DodgeTextAges.Add(1);
    }
	public void soldierRankUp(Vector2 pos)
	{
		GameObject m = (GameObject)GameObject.Instantiate(RankText, pos, Quaternion.identity);
		m.transform.SetParent(this.transform);
		RankTexts.Add(m);
		RankTextAges.Add(1);
	}
}
