using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Pacman.Direction;
using static Pacman.PacmanSettings;
using Random = UnityEngine.Random;

namespace Pacman
{
    public enum EnemyType//性格タイプ
    {
        Akabee,
        Pinky,
        Aosuke,
        Guzuta
    }

    public class Enemy : Mover, IEnemy
    {
        public Directions cantOneWayDirections = Directions.Up; //一方通行の方向
        internal bool isKilledByPacman = false;//パックマンに殺された？

        protected override void Update()
        {
            if (respawnWait) return;//リスポーン待ちなら処理をスキップする
            base.Update();
        }

        public override void Initialize()//初期化
        {
            cantOneWayDirections = Directions.Up;
            SetNormalColor();
            if (routine != null) StopCoroutine(routine);
            routine = null;
            respawnWait = false;
            isKilledByPacman = false;
            isFirstMoveAfterPowerPacman = false;
            isFirstMoveAfterBreak = false;
            moveTarget = EnemyStartPos;
            transform.localPosition = EnemyStartPos;
            base.Initialize();
            gameObject.SetActive(false);
        }

        internal PlayerMove PlayerMove;

        //方向反転フラグ
        private bool isFirstMoveAfterPowerPacman = false;
        private bool isFirstMoveAfterBreak = false;

        public void OnBreakModeChanged(bool arg0)//休息時のコールバック
        {
            if (arg0)
            {
                isFirstMoveAfterBreak = false;
            }
        }

        //パックマンの状態変化時にトリガー
        public void OnPacmanStateChanged(bool isPowerPacman, bool isAddTime)
        {
            if (respawnWait) return;
            if (!isPowerPacman)
            {
                SetNormalColor();
                cantOneWayDirections = Directions.Up;
                return;
            }

            cantOneWayDirections = Directions.Down;
            isKilledByPacman = false;
            spriteRenderer.color = IzikeColor;
            if (!isAddTime)
            {
                isFirstMoveAfterPowerPacman = false;
            }
        }

        Spurt SpurtType//スパート
        {
            get
            {
                if (enemyType == EnemyType.Akabee)
                {
                    int remaining = envManager.totalCookie - PlayerMove.cookieCount;
                    if (remaining <= 10)
                    {
                        return Spurt.Level2;
                    }

                    if (remaining <= 20)
                    {
                        return Spurt.Level1;
                    }
                }

                return Spurt.None;
            }
        }

        enum Spurt
        {
            None,
            Level1,
            Level2
        }

        protected override void SpeedCheck()//スピード設定
        {
            speed = CalcSpeed(-1);

            switch (SpurtType)
            {
                case Spurt.Level1:
                    speed = CalcSpeed(0);
                    break;
                case Spurt.Level2:
                    speed = CalcSpeed(1);
                    break;
            }

            if (PlayerMove.isPowerPacman && !isKilledByPacman)
            {
                speed = CalcSpeed(-6);
            }

            if ((CellPosition.y == LeftWarpPoint.y || CellPosition.y == RightWarpPoint.y) &&
                (CellPosition.x - LeftWarpPoint.x <= 2 || RightWarpPoint.x - CellPosition.x <= 2))
            {
                speed = CalcSpeed(-8);
            }
        }

        void SetNormalColor() //色設定
        {
            switch (enemyType)
            {
                case EnemyType.Akabee:
                    spriteRenderer.color = AkabeeColor;
                    break;
                case EnemyType.Pinky:
                    spriteRenderer.color = PinkyColor;
                    break;
                case EnemyType.Aosuke:
                    spriteRenderer.color = AosukeColor;
                    break;
                case EnemyType.Guzuta:
                    spriteRenderer.color = GuzutaColor;
                    break;
            }
        }

        public void OnDead() //しにました
        {
            isKilledByPacman = true;
            isFirstMoveAfterPowerPacman = false;
            moveTarget = EnemyStartPos;
            transform.localPosition = EnemyStartPos + ConvertDirToCell(Directions.Down, 0.64f) * 2;
            SetNormalColor();
            routine = Respawn();
            cantOneWayDirections = Directions.Up;
            StartCoroutine(routine);
        }

        private IEnumerator routine;

        private bool respawnWait;

        IEnumerator Respawn()//リスポーン(3秒待つ)
        {
            respawnWait = true;
            yield return wait3s;
            Go();
            respawnWait = false;
        }

        public void Go() //ゴー!
        {
            SetNormalColor();
            transform.localPosition = EnemyStartPos;
        }

        public override void NextReceive()
        {
            SetNextDirection(Select());
            base.NextReceive();
        }

        public EnemyType enemyType = EnemyType.Akabee;

        private Directions Select() //経路選択(フローチャート参照)
        {
            var canMoveDirs = CheckMovable().ToList();
            if (enemyManager.isBreak && SpurtType == Spurt.None) //休息時
            {
                var diff = EnemyManager.breakPoint[enemyType] - CellPosition;

                if (diff.x < 0 && canMoveDirs.ExistDirection(Directions.Left))
                {
                    return Directions.Left;
                }
                else if (diff.x > 0 && canMoveDirs.ExistDirection(Directions.Right))
                {
                    return Directions.Right;
                }

                if (diff.y < 0 && canMoveDirs.ExistDirection(Directions.Down))
                {
                    return Directions.Down;
                }
                else if (diff.y > 0 && canMoveDirs.ExistDirection(Directions.Up))
                {
                    return Directions.Up;
                }
            }
            else //攻撃時
            {
                switch (enemyType)
                {
                    case EnemyType.Akabee: //アカベエ(常にパックマン？を追いかける)
                        var diff = PlayerMove.CellPosition - CellPosition;

                        if (diff.x < 0 && canMoveDirs.ExistDirection(Directions.Left))
                        {
                            return Directions.Left;
                        }
                        else if (diff.x > 0 && canMoveDirs.ExistDirection(Directions.Right))
                        {
                            return Directions.Right;
                        }

                        if (diff.y < 0 && canMoveDirs.ExistDirection(Directions.Down))
                        {
                            return Directions.Down;
                        }
                        else if (diff.y > 0 && canMoveDirs.ExistDirection(Directions.Up))
                        {
                            return Directions.Up;
                        }

                        break;
                    case EnemyType.Pinky: //ピンキー (パックマンの向いている方向*3マス先に移動)
                        if (canMoveDirs.Exists(directions1 => directions1 == PlayerMove.nowDirection))
                        {
                            Vector3Int? targetCell = null;
                            for (int i = 3; i > 0; i--)
                            {
                                var t = tileMap.GetTile(PlayerMove.CellPosition +
                                                        ConvertDirToCell(PlayerMove.nowDirection) * (i));
                                if (t != cantMoveTile)
                                {
                                    targetCell = PlayerMove.CellPosition +
                                                 ConvertDirToCell(PlayerMove.nowDirection) * (i);
                                    break;
                                }
                            }

                            if (targetCell != null)
                            {
                                var diff3 = (Vector3Int)targetCell - CellPosition;

                                if (diff3.x < 0 && canMoveDirs.ExistDirection(Directions.Left))
                                {
                                    return Directions.Left;
                                }
                                else if (diff3.x > 0 && canMoveDirs.ExistDirection(Directions.Right))
                                {
                                    return Directions.Right;
                                }

                                if (diff3.y < 0 && canMoveDirs.ExistDirection(Directions.Down))
                                {
                                    return Directions.Down;
                                }
                                else if (diff3.y > 0 && canMoveDirs.ExistDirection(Directions.Up))
                                {
                                    return Directions.Up;
                                }
                            }

                            return PlayerMove.nowDirection;
                        }

                        var diff2 = PlayerMove.CellPosition - CellPosition;
                        if (PlayerMove.nowDirection == Directions.Up ||
                            PlayerMove.nowDirection == Directions.Down)
                        {
                            if (diff2.x < 0 && canMoveDirs.ExistDirection(Directions.Left))
                            {
                                return Directions.Left;
                            }
                            else if (diff2.x > 0 && canMoveDirs.ExistDirection(Directions.Right))
                            {
                                return Directions.Right;
                            }
                        }

                        if (PlayerMove.nowDirection == Directions.Left ||
                            PlayerMove.nowDirection == Directions.Right)
                        {
                            if (diff2.y < 0 && canMoveDirs.ExistDirection(Directions.Down))
                            {
                                return Directions.Down;
                            }
                            else if (diff2.y > 0 && canMoveDirs.ExistDirection(Directions.Up))
                            {
                                return Directions.Up;
                            }
                        }

                        break;
                    case EnemyType.Aosuke: //アオスケ(アカベエの点対称を目指す)
                        var diff4 = (enemyManager.akabee.CellPosition - PlayerMove.CellPosition) * -1 -
                                    CellPosition;

                        if (diff4.x < 0 && canMoveDirs.ExistDirection(Directions.Left))
                        {
                            return Directions.Left;
                        }
                        else if (diff4.x > 0 && canMoveDirs.ExistDirection(Directions.Right))
                        {
                            return Directions.Right;
                        }

                        if (diff4.y < 0 && canMoveDirs.ExistDirection(Directions.Down))
                        {
                            return Directions.Down;
                        }
                        else if (diff4.y > 0 && canMoveDirs.ExistDirection(Directions.Up))
                        {
                            return Directions.Up;
                        }

                        break;
                    case EnemyType.Guzuta: //グズタ(ランダムに動く)
                        if (Vector3.Distance(transform.position, PlayerMove.transform.position) >= 8.125f)
                        {
                            //遠すぎるとアカベエの挙動をとる
                            goto case EnemyType.Akabee;
                        }
                        else
                        {
                            return canMoveDirs[Random.Range(0, canMoveDirs.Count)];
                        }
                }
            }

            //詰んだとき用
            return canMoveDirs[Random.Range(0, canMoveDirs.Count)];
        }

        /// <summary>
        /// 敵キャラは真逆の方向に方向転換しないので可能な限り除外する
        /// ※スーパーパックマン状態変化後の初回 or 休息時をのぞく
        /// </summary>
        /// <returns>移動可能な向き</returns>
        private Directions[] CheckMovable()
        {
            var dirList = Enum.GetValues(typeof(Directions)).Cast<Directions>()
                .Where(dir => !dir.IsNothing())
                .Where(IsMovable).ToList();
            if (dirList.Count > 1 && nowDirection != Directions.Nothing)
            {
                if (dirList.Exists(direction => direction == InverseDirection(nowDirection)))
                {
                    if (PlayerMove.isPowerPacman && !isFirstMoveAfterPowerPacman)
                    {
                        dirList.RemoveAll(directions => directions != InverseDirection(nowDirection));
                        isFirstMoveAfterPowerPacman = true;
                    }
                    else if (SpurtType==Spurt.None&&enemyManager.isBreak && !isFirstMoveAfterBreak)
                    {
                        dirList.RemoveAll(directions => directions != InverseDirection(nowDirection));
                        isFirstMoveAfterBreak = true;

                    }
                    else
                    {
                        dirList.Remove(InverseDirection(nowDirection));
                    }
                }
            }

            return dirList.ToArray();
        }

        public override bool IsMovable(Directions dir)//移動可能向き
        {
            var canMove = base.IsMovable(dir);

            if (canMove && cantOneWayDirections == dir) //一方通行の方向と行く方向がおなじなら
            {
                var isOneWay = OneWayPoints.ToList().Exists(i => CellPosition + ConvertDirToCell(dir) == i);
                if (isOneWay)
                {
                    canMove = false;
                }
            }

            return canMove;
        }
    }
}
