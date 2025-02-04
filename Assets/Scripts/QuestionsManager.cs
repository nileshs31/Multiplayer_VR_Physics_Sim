using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System;

public class QuestionManager : NetworkBehaviour
{
    public List<Question> questions; // Add questions via the Unity Inspector
    private int currentQuestionIndex = 0, currentScore = 0;
    public GameObject QuestionsUI, spectatorViewDetailsButton;
    [SerializeField]
    TextMeshProUGUI questionText, option1, option2, score, progress, detailedReportText;
    private float questionStartTime;
    public Button option1bu, option2bu;

    // NetworkVariables to store time taken and selected answer
    private NetworkList<double> timeTakenList = new NetworkList<double>(new List<double> {}, NetworkVariableReadPermission.Everyone);
    public NetworkList<FixedString64Bytes> OptionsSelected = new NetworkList<FixedString64Bytes>(new List<FixedString64Bytes> {}, NetworkVariableReadPermission.Everyone);

    public static QuestionManager Instance; // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void DisplayNextQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {

            questionStartTime = Time.time;

            var option1Colors = option1bu.colors;
            option1Colors.normalColor = Color.white;
            option1bu.colors = option1Colors;
            option2bu.colors = option1Colors;


            option1bu.enabled = true;
            option2bu.enabled = true;
            Question currentQuestion = questions[currentQuestionIndex];
            questionText.text = currentQuestion.questionText;
            option1.text = currentQuestion.optionA;
            option2.text = currentQuestion.optionB;
            progress.text = currentQuestionIndex + 1 + "/" + questions.Count;
        }
        else
        {
            //quiz ends
            questionText.text = "Final Score - " + currentScore + "/" + questions.Count;
            option1bu.gameObject.SetActive(false);
            option2bu.gameObject.SetActive(false);
            spectatorViewDetailsButton.SetActive(true);

        }
    }

    public void SubmitAnswer(bool isOptionASelected)
    {
        float timeTaken = Mathf.Round((Time.time - questionStartTime) * 100f) / 100f;
        string selectedAnswer = isOptionASelected ? option1.text : option2.text;

        OnOptionSelectedServerRpc(isOptionASelected, timeTaken, selectedAnswer);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnOptionSelectedServerRpc(bool isOptionASelected, float timeTaken, string selectedAnswer)
    {
        timeTakenList.Add(timeTaken);
        OptionsSelected.Add(selectedAnswer);
        OnOptionSelectedClientRpc(isOptionASelected);
    }


    [ClientRpc(RequireOwnership = false)]
    public void OnOptionSelectedClientRpc(bool isOptionASelected)
    {

        detailedReportText.text += "\nQ " + (OptionsSelected.Count) + " - Answer Selected: " + OptionsSelected[OptionsSelected.Count - 1] + " - Time Taken: " + timeTakenList[OptionsSelected.Count - 1];

        option1bu.enabled = false;
        option2bu.enabled = false;
        if (currentQuestionIndex < questions.Count)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            bool isCorrect = isOptionASelected == currentQuestion.isOptionACorrect;

            UpdateUIForAnswerClientRPC(isOptionASelected, isCorrect);

            DisplayNextQuestionClientRpc();
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void OnTwoJoinedServerRpc()
    {
        OnTwoJoinedClientRpc();
    }

    public void TurnQuestionUI()
    {
        QuestionsUI.SetActive(true);
        QuestionManager.Instance.DisplayNextQuestion();
        score.text = "Score - " + currentScore;
    }

    [ClientRpc(RequireOwnership = false)]
    public void OnTwoJoinedClientRpc()
    {
        QuestionsUI.SetActive(true);
        QuestionManager.Instance.DisplayNextQuestion();
        score.text = "Score - " + currentScore;
    }

    [ClientRpc(RequireOwnership = false)]
    private void DisplayNextQuestionClientRpc()
    {
        currentQuestionIndex++;
        Invoke("DisplayNextQuestion",1.25f);
    }

    [ClientRpc(RequireOwnership = false)]
    public void UpdateUIForAnswerClientRPC(bool isOptionASelected, bool isCorrect)
    {
        var option1Colors = option1bu.colors;
        var option2Colors = option2bu.colors;

        if (isCorrect)
        {
            if (isOptionASelected)
            {
                option1Colors.normalColor = Color.green;
                option2Colors.normalColor = Color.red;
            }
            else
            {
                option2Colors.normalColor = Color.green;
                option1Colors.normalColor = Color.red;
            }
            currentScore++;
        }
        else
        {
            if (isOptionASelected)
            {
                option1Colors.normalColor = Color.red;
                option2Colors.normalColor = Color.green;
            }
            else
            {
                option2Colors.normalColor = Color.red;
                option1Colors.normalColor = Color.green;
            }
        }

        option1bu.colors = option1Colors;
        option2bu.colors = option2Colors;

        score.text = "Score - " + currentScore;
    }

}