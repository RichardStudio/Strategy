using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviourPunCallbacks
{
    public float baseHP;
    public HealthBar healthBar;
    [HideInInspector] public GameObject[] Units;
    [HideInInspector] public GameObject[] OtherUnits;
    public TMP_Text GoldText;
    public Timer Timer;
    public GameObject GameInfo;
    [HideInInspector] public Transform[] spawnPoints;
    public GameObject otherMainBuilding;
    public TMP_Text endGameText;
    public GameObject endGamePanel;
    public Button BuyWarrior;
    public Button BuySpear;
    public Button BuyKnight;

    private float gold;
    private Information info;
    private float bonusGold = 3;

    private void Awake()
    {
        GetSpawnTransform();

        healthBar.SetSliderMaxValue(baseHP);

        info = GameInfo.GetComponent<Information>();
        gold = 30;
        GoldText.text = gold.ToString();
        Units = info.GetUnitsArr();
    }

    private void Start()
    {
        OtherUnits = info.GetUnitsArr();
        StartCoroutine(Income(bonusGold));
    }



    public void GetSpawnTransform()
    {
        spawnPoints = new Transform[3];
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    public void RecieveDamage(float Damage)
    {
        baseHP -= Damage;
        healthBar.SetSliderValue(baseHP);
        if (baseHP <= 0)
        {
            baseHP = 0;
            healthBar.SetSliderValue(baseHP);
            photonView.RPC("EndGame", RpcTarget.All, transform.tag);
        }
    }

    public void AddGold(float addGold)
    {
        gold += addGold;
        GoldText.text = gold.ToString();
    }

    private IEnumerator Income(float addGold)
    {
        while (!info.endGame)
        {
            yield return new WaitForSeconds(5);
            AddGold(addGold);
        }
    }

    public void Spawn(int id)
    {
        Debug.Log(id + " айди");
        Debug.Log(Units.Length + " длинна юнитов");
        Debug.Log(spawnPoints.Length + " длинна спавнов");
        GameObject unit = PhotonNetwork.Instantiate(Units[id].name, spawnPoints[id].position, Quaternion.identity);
        if (transform.tag == "LeftBase")
        {
            unit.tag = "Left";
        }
        else
        {
            unit.tag = "Right";
        }
    }

    [PunRPC]
    public void EndGame(string tag)
    {
        info.endGame = true;
        if (tag == "RightBase")
        {
            endGameText.text = PhotonNetwork.PlayerList[0].NickName + " win";
        }
        else
        {
            endGameText.text = PhotonNetwork.PlayerList[1].NickName + " win";
        }
        endGamePanel.SetActive(true);
        healthBar.gameObject.SetActive(false);
    }

    public void OnBuyWarriorClicked()
    {
        if (gold >= 10)
        {
            Spawn(0);
            gold -= 10;
            GoldText.text = gold.ToString();
        }
    }
    public void OnBuySpearClicked()
    {
        if (gold >= 20)
        {
            Spawn(1);
            gold -= 20;
            GoldText.text = gold.ToString();
        }
    }
    public void OnBuyKnightClicked()
    {
        if (gold >= 50)
        {
            Spawn(2);
            gold -= 50;
            GoldText.text = gold.ToString();
        }
    }

}
