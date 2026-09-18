using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    public GameObject playerOneText;
    public GameObject playerTwoText;

    private bool hasWinner = false;

    // Start is called before the first frame update
    void Start()
    {
        playerOneText.GetComponent<Text>().text = "";
        playerTwoText.GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void OnCollisionEnter(Collision collision)
    {
        if (hasWinner) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.tag == "Player")
        {
            PlayerController2 player = collision.gameObject.GetComponent<PlayerController2>();
            if (player != null)
            {
                hasWinner = true;
                int ID = player.ID;
                if (ID == 1)
                {
                    SetWinText(playerOneText, "1P WIN");
                    playerOneText.SetActive(true);
                }
                else if (ID == 2)
                {
                    SetWinText(playerTwoText, "2P WIN");
                    playerTwoText.SetActive(true);
                }
            }
        }
    }

    private void SetWinText(GameObject textObject, string winText)
    {
        if (textObject == null) return;

        // Try TextMeshPro first
        var tmp = textObject.GetComponent<TMPro.TMP_Text>();
        if (tmp != null)
        {
            tmp.text = winText;
            return;
        }

        // Try standard UI Text
        var uiText = textObject.GetComponent<UnityEngine.UI.Text>();
        if (uiText != null)
        {
            uiText.text = winText;
            return;
        }
    }
}
