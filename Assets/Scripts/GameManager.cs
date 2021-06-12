using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform gameOverMenu;
    [SerializeField] private Transform player;
    [SerializeField] private Transform pointsLabel;
    [SerializeField] private Transform timerLabel;
    [SerializeField] private float startTime = 30f;

    public static GameManager instance;
    private int points = 0;
    private float timeRemaining;
    private bool timerRunning = true;

    private Text pointsText;
    private Text timerText;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            pointsText = pointsLabel.GetComponent<Text>();
            timerText = timerLabel.GetComponent<Text>();
            timeRemaining = startTime;
        } else {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (timerRunning) {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time remaining: " + Mathf.FloorToInt(timeRemaining % 60);

            if (timeRemaining <= 0) {
                timerRunning = false;
                timerText.text = "Time remaining: 0";
                
                gameOverMenu.gameObject.SetActive(true);
                player.GetComponent<PlayerController>().DisablePlayer();

                // play buzzer?
            }
        }
    }

    public void AddPoints(int _points)
    {
        points += _points;
        pointsText.text = "Matches: " + points;
        // play sound for match
    }
}
