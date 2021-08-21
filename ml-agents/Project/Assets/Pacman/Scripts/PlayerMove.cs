using System;
using System.Linq;
using System.Security.Cryptography;
using static Pacman.PacmanSettings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = System.Random;

namespace Pacman
{
    /// <summary>
    /// 動く管理のみ
    /// speedなど
    /// </summary>
    public class PlayerMove : Mover
    {
        public override void Initialize()
        {
            score = 0;
            cookieCell = null;
            cookieCount = 0;
            noEatTime = 0;
            isPowerPacman = false;
            powerStart = -1;
            enemyKillCount = 0;
            moveTarget = AgentStartPos;
            transform.localPosition = AgentStartPos;
            spriteRenderer.color=Color.green;
            base.Initialize();
        }
        private int highScore = 0;
        public Text highScoreText;
        public int Score
        {
            get => score;
            set
            {
                highScore = score > highScore ? score : highScore;
                highScoreText.text = $"ハイスコア:{highScore}";
                score = value;
            }
        }
        [NonSerialized] private int score = 0;
        internal PlayerAgent agent;
        (Vector3Int, float)? cookieCell;
        public int cookieCount = 0;

        internal bool isPowerPacman = false;
        private float powerStart = -1;
        private static readonly float powerStateSecond = 6;
        internal int enemyKillCount = 0;


        void GameClear() => agent.GameClear();

        void GameOver() => agent.GameOver();

        private void Awake()
        {
            OnPacmanStateChanged += OnPacmanStateChange;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.gameObject.tag)
            {
                case "Cookie":
                    Eating(other.transform.localPosition, false);
                    Score += 10;
                    cookieCount++;
                    Destroy(other.gameObject);
                    agent.EatCookie(false);
                    if (cookieCount >= envManager.totalCookie) GameClear();
                    break;
                case "PowerCookie":
                    Eating(other.transform.localPosition, true);
                    Score += 50;
                    cookieCount++;
                    if (isPowerPacman)
                    {
                        //増加
                        powerStart = Time.timeSinceLevelLoad;
                        OnPacmanStateChanged.Invoke(true, true);
                    }
                    else
                    {
                        powerStart = Time.timeSinceLevelLoad;
                        isPowerPacman = true;
                        OnPacmanStateChanged.Invoke(true, false);
                    }

                    Destroy(other.gameObject);
                    agent.EatCookie(true);

                    if (cookieCount >= envManager.totalCookie) GameClear();
                    break;
                case "Enemy":
                    Enemy enemy= enemyManager.enemies.First(enemy => enemy.gameObject == other.gameObject);

                    if (isPowerPacman&&!enemy.isKilledByPacman)
                    {
                        enemyKillCount++;
                        // 2^n * 100
                        Score += 100 * (int)Math.Pow(2, enemyKillCount);


                        enemyManager.Kill(other.gameObject);
                        agent.KilledMonster();
                    }
                    else
                    {
                        GameOver();
                    }


                    break;
            }
        }


        private static readonly float PowerSpeed = CalcSpeed(2);

        private void Eating(Vector3 pos, bool isPower)
        {
            noEatTime = 0;
            float setSpeed;
            if (isPowerPacman)
            {
                setSpeed = CalcSpeed(1);
                if (isPower)
                {
                    setSpeed = CalcSpeed(-1);
                }
            }
            else
            {
                setSpeed = CalcSpeed(-1);
                if (isPower)
                {
                    setSpeed = CalcSpeed(-3);
                }
            }

            cookieCell = (tileMap.LocalToCell(pos), setSpeed);
        }


        /// <summary>
        /// 変速
        /// </summary>
        protected override void SpeedCheck()
        {
            if (cookieCell != null)
            {
                if (cookieCell.Value.Item1 == CellPosition)
                {
                    speed = cookieCell.Value.Item2;
                }
                else
                {
                    cookieCell = null;
                    speed = isPowerPacman ? PowerSpeed : defaultSpeed;
                }
            }
            else
            {
                speed = isPowerPacman ? PowerSpeed : defaultSpeed;
            }
        }

        public UnityAction<bool, bool> OnPacmanStateChanged;

        void OnPacmanStateChange(bool isPower, bool _)
        {
            spriteRenderer.color = isPower ? Color.yellow : Color.green;
        }

        public float noEatTime;
        protected override void Update()
        {
            noEatTime += Time.deltaTime;
            if (powerStart != -1 && powerStart + powerStateSecond <= Time.timeSinceLevelLoad)
            {
                powerStart = -1;
                isPowerPacman = false;
                OnPacmanStateChanged.Invoke(false, false);
                enemyKillCount = 0;
            }

            base.Update();
            if (moveTarget==transform.localPosition)
            {
                //agent.DontMove();
            }
        }
    }
}
