using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private string player1, player2;
    private int p1Wins, p2Wins;

    public string lastActions;
    public AI IA;
    public Gif_Code enemyCode;
    public Gif_Code heroeCode;
    public Text p1Text, p2Text, result, p1TotalWins, p2TotalWins;

    public void Start()
    {
        p1Wins = 0;
        p2Wins = 0;
        lastActions = "";
        enemyCode = GameObject.Find("Enemy").GetComponent<Gif_Code>();
        heroeCode = GameObject.Find("Heroe").GetComponent<Gif_Code>();


        if (enemyCode == null)
        {
            Debug.Log("no se ha encontrado enemigo");
        }
    }

    public string getPlayer1()
    {
        return player1;
    }

    public string getPlayer2()
    {
        return player2;
    }

    public void setPlayer1(string p1)
    {
        player1 = p1;
    }

    public void setPlayer2(string p2)
    {
        player2 = p2;
    }

    public void Winner()
    {
        if (player1 == player2)
        {
            result.text = "DRAW!";
            StartCoroutine(enemyCode.BlinkColor(new Color(255, 255, 255, 200), true));
        }
        else if ((player1 == "R" && player2 == "S") ||
                 (player1 == "P" && player2 == "R") ||
                 (player1 == "S" && player2 == "P"))
        {
            result.text = "Winner Winner Chicken Dinner!";
            StartCoroutine(enemyCode.BlinkColor(new Color32(255,0,0,255), false));
            
            p1Wins++;
            p1TotalWins.text = "Wins:" + p1Wins;
        }
        else if ((player1 == "R" && player2 == "P") ||
                 (player1 == "P" && player2 == "S") ||
                 (player1 == "S" && player2 == "R"))
        {
            result.text = "Loser!";
            StartCoroutine(heroeCode.BlinkColor(new Color32(255, 0, 0, 255), false));
            p2Wins++;
            p2TotalWins.text = "Wins:" + p2Wins;
        }
        else Debug.Log("Error, invalid move");
        CalculateString();
        Debug.Log(lastActions);

    }

    public void CalculateString()
    {
        if (lastActions.Length < 3)
        {
            lastActions = lastActions + player1;
        }
        else if (lastActions.Length == 3)
        {
            lastActions = lastActions.Substring(1, 2) + player1;
        }
    }

    public void ButtonAtack(string selection)
    {
        player1 = selection;
        p1Text.text = selection;
        IA.Play();
    }
}
