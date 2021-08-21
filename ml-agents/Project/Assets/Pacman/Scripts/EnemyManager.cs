using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using Debug = System.Diagnostics.Debug;

namespace Pacman
{
    public class EnemyManager : MonoBehaviour
    {
        public Enemy akabee;
        public Enemy pinky;
        public Enemy aosuke;
        public Enemy guzuta;
        internal List<Enemy> enemies;
        [FormerlySerializedAs("playerMover")] public PlayerMove playerMove;

        public static readonly Dictionary<EnemyType, Vector3Int> breakPoint = new Dictionary<EnemyType, Vector3Int>
        {
            { EnemyType.Akabee, new Vector3Int(8, 7, 0) },
            { EnemyType.Pinky, new Vector3Int(-8, 7, 0) },
            { EnemyType.Aosuke, new Vector3Int(8, -6, 0) },
            { EnemyType.Guzuta, new Vector3Int(-8, -6, 0) },
        };


        public void Kill(GameObject gameObject)
        {
            var killedEnemy = enemies.First(enemy => enemy.gameObject == gameObject);
            if (killedEnemy == null) throw new ArgumentException("そんな敵いないよ");
            killedEnemy.OnDead();
        }

        private void Start()
        {
            enemies = new List<Enemy>()
            {
                akabee,
                pinky,
                aosuke,
                guzuta
            };
            foreach (var enemyMover in enemies)
            {

                onBreak += enemyMover.OnBreakModeChanged;
                enemyMover.PlayerMove = playerMove;
                playerMove.OnPacmanStateChanged += enemyMover.OnPacmanStateChanged;
            }
        }

        private IEnumerator routine;

        public void Initialize()
        {
            if (routine != null) StopCoroutine(routine);
            foreach (var enemy in enemies)
            {
                enemy.Initialize();
            }

            timerForEnemy = 0;
            index = 0;

            routine = Go();
            StartCoroutine(routine);
        }

        private readonly RangeInt[] enemyState =
        {
            new RangeInt(0, 7),
            new RangeInt(27, 7),
            new RangeInt(54, 5),
            new RangeInt(79, 5),
        };

        private float timerForEnemy = 0f;
        public bool isBreak = false;

        private bool WaitForBreakEnd=false;
        private int index = 0;
        private void Update()
        {
            if (!playerMove.isPowerPacman)
            {
                timerForEnemy += Time.deltaTime;
//0
                var rangeBreak = enemyState[index];//0~7

                isBreak = rangeBreak.start <= timerForEnemy && rangeBreak.end >= timerForEnemy;//true
                if (isBreak&&!WaitForBreakEnd)
                {
                    onBreak.Invoke(true);
                    WaitForBreakEnd = true;

                }

                if (WaitForBreakEnd)
                {
                    if (!isBreak)
                    {
                        index++;
                        WaitForBreakEnd = false;
                    }
                }



            }
        }

        public UnityAction<bool> onBreak;
        private readonly int[] enemySpawnCount =
        {
            10,
            30,
            90,
        };

        IEnumerator Go()
        {
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].gameObject.SetActive(true);
                enemies[i].Go();
                playerMove.noEatTime = 0;
                if (i == enemySpawnCount.Length)
                {
                    yield break;
                }

                yield return new WaitUntil(() =>
                    playerMove.noEatTime >= 4 || playerMove.cookieCount >= enemySpawnCount[i]);
            }
        }
    }
}
