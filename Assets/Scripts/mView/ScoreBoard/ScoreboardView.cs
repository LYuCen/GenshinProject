using System;
using System.Collections.Generic;
using Connect;
using Connect.DTO;
using mEvent;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView.ScoreBoard
{
    public class ScoreboardView : MonoBehaviour, IController
    {
        [SerializeField] private TextMeshProUGUI blueTeamScore;
        [SerializeField] private TextMeshProUGUI redTeamScore;
        [SerializeField] private TextMeshProUGUI endScore;
        [SerializeField] private TextMeshProUGUI time;

        [SerializeField] private GameObject scoreboard;
        [SerializeField] private Transform blueTeamScoreContent;
        [SerializeField] private Transform redTeamScoreContent;

        [SerializeField] private Image blueImg;
        [SerializeField] private Image redImg;

        private float _remainTime;

        private void Start()
        {
            Listener.OnScoreboardReceived += UpdateScoreboard;
            this.RegisterEvent<ShowScoreboardEvent>(_ =>
            {
                scoreboard.SetActive(!scoreboard.activeSelf);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void UpdateScoreboard(ScoreboardDto dto)
        {
            for (int i = 0; i < redTeamScoreContent.childCount; i++)
            {
                redTeamScoreContent.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < blueTeamScoreContent.childCount; i++)
            {
                blueTeamScoreContent.GetChild(i).gameObject.SetActive(false);
            }

            int blueIdx = 0;
            int redIdx = 0;

            foreach (var p in dto.players)
            {
                var content = p.team == 0 ? blueTeamScoreContent : redTeamScoreContent;
                int idx = p.team == 0 ? blueIdx++ : redIdx++;
                if (idx >= content.childCount) continue;
                content.GetChild(idx).GetComponent<PlayerScoreboard>().Init(p.name, p.kill, p.death);
            }


            blueTeamScore.text = dto.blueScore.ToString();
            redTeamScore.text = dto.redScore.ToString();
            _remainTime = dto.remainTime;
            endScore.text = dto.killValue.ToString();
            redImg.fillAmount = 1.0f * dto.redScore / dto.killValue + 0.01f;
            blueImg.fillAmount = 1.0f * dto.blueScore / dto.killValue + 0.01f;
        }

        private void Update()
        {
            if (_remainTime > 0)
            {
                _remainTime -= Time.deltaTime;
                if (_remainTime < 0) _remainTime = 0;
            }

            int mm = (int)_remainTime / 60;
            int ss = (int)_remainTime % 60;
            time.text = $"{mm:00}:{ss:00}";
        }

        private void OnDestroy()
        {
            Listener.OnScoreboardReceived -= UpdateScoreboard;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}