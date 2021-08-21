using System;
using System.Collections;
using static Pacman.PacmanSettings;
using Unity.MLAgents;
using Unity.MLAgents.Sensors.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Pacman.Direction;

namespace Pacman
{
    public interface IEnemy
    {
        /// <summary>
        /// 死んだとき
        /// </summary>
        void OnDead();
    }

    /// <summary>
    /// 一マスづつ動くキャラのためのクラス 抽象クラス
    /// </summary>
    public abstract class Mover : MonoBehaviour
    {
        public TileBase cantMoveTile;
        [NonSerialized] public Directions nowDirection = Directions.Nothing;

        [NonSerialized] public Directions nextDirection = Directions.Nothing;
        public Tilemap tileMap;
        public Vector3Int CellPosition => tileMap.LocalToCell(transform.localPosition);

        protected SpriteRenderer spriteRenderer;
        //[NonSerialized]public bool isMoving = false;
        //[Tooltip("一マスの単位(Pixel)")] private float unit = 0.64f;


       [NonSerialized] public float speed;

        internal EnvManager envManager;
        internal EnemyManager enemyManager;

        public virtual void Initialize()
        {
            nowDirection = Directions.Nothing;
            nextDirection = Directions.Nothing;
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            envManager = transform.root.GetComponent<EnvManager>();
            enemyManager = transform.root.GetComponent<EnemyManager>();
        }

        public void SetNextDirection(Directions next)
        {
            if (next != nowDirection || nowDirection.IsNothing())
                nextDirection = next;
        }






        protected Vector3 moveTarget = PacmanSettings.AgentStartPos;

        public virtual void NextReceive()
        {
            if (!nextDirection.IsNothing()) //外部入力があったとき
            {
                if (tileMap.GetTile(CellPosition + ConvertDirToCell(nextDirection)) != cantMoveTile) //移動可能
                {
                    moveTarget = tileMap.CellToLocal(CellPosition + ConvertDirToCell(nextDirection)) +
                                 new Vector3(tileMap.cellSize.x / 2, tileMap.cellSize.y / 2, 0);
                    nowDirection = nextDirection;
                    nextDirection = Directions.Nothing;
                    return;
                }
            }

            if (!nowDirection.IsNothing())
            {
                if (tileMap.GetTile(CellPosition + ConvertDirToCell(nowDirection)) != cantMoveTile) //移動可能
                {
                    moveTarget = tileMap.CellToLocal(CellPosition + ConvertDirToCell(nowDirection)) +
                                 new Vector3(tileMap.cellSize.x / 2, tileMap.cellSize.y / 2, 0);

                    return;
                }
            }
        }

        protected virtual void Update()
        {


            if (transform.localPosition == moveTarget)
            {
                if (CellPosition == PacmanSettings.LeftWarpPoint && nowDirection == Directions.Left)
                {
                    transform.localPosition = tileMap.CellToLocal(PacmanSettings.RightWarpPoint) +
                                              new Vector3(tileMap.cellSize.x / 2, tileMap.cellSize.y / 2, 0);
                }
                else if (CellPosition == PacmanSettings.RightWarpPoint && nowDirection == Directions.Right)
                {
                    transform.localPosition = tileMap.CellToLocal(PacmanSettings.LeftWarpPoint) +
                                              new Vector3(tileMap.cellSize.x / 2, tileMap.cellSize.y / 2, 0);
                }

                NextReceive();
            }


            if (transform.localPosition != moveTarget)
            {
                Move();
            }
        }

        protected void Move()
        {
            SpeedCheck();
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, moveTarget, speed * Time.deltaTime);
        }

        protected virtual void SpeedCheck()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsMovable(Directions dir)
        {
            return tileMap.GetTile(CellPosition + ConvertDirToCell(dir)) != cantMoveTile;
        }
    }


    public enum ObjectKind
    {
        Wall, //壁
        Nothing, //なにもない
        Player, //プレイヤー
        Enemy, //敵
        Cookie, //クッキー
        PowerCookie //すごいクッキ―
    }
}
