using UnityEngine;

namespace Pacman
{
    public class PacmanSettings
    {
        //Local
        public static Vector3 AgentStartPos = new Vector3(0.32f, -3.52f, 0);
        public static Vector3 EnemyStartPos = new Vector3(0.32f, 1.6f, 0);

        public static readonly Vector3Int LeftWarpPoint = new Vector3Int(-10, 0, 0);
        public static readonly Vector3Int RightWarpPoint = new Vector3Int(10, 0, 0);

        public static readonly Vector3Int[] OneWayPoints = {
            new Vector3Int(-1,3,0),
            new Vector3Int(1,3,0),
            new Vector3Int(-1,-5,0),
            new Vector3Int(1,-5,0),
        };

        public static readonly float defaultSpeed = 4; //ベースの速さ
        public static readonly float speedUnit = 0.25f; //スピード１段階の違い

        /// <summary>
        /// パフォーマンスのために
        /// </summary>
        public static WaitForSeconds wait3s = new WaitForSeconds(3);

        public static readonly Color AkabeeColor = new Color(0.98f, 0.2f, 0.2f);
        public static readonly Color PinkyColor = new Color(1f, 0.38f, 0.8f);
        public static readonly Color AosukeColor = new Color(0f, 0.7f, 1f);
        public static readonly Color GuzutaColor = new Color(1f, 0.69f, 0f);

        public static readonly Color IzikeColor = new Color(0.2f, 0.09f, 0.3f);


        /// <summary>
        ///Offsetは
        /// https://docs.google.com/spreadsheets/d/1kFZdvRlqa_7FbavgDjMkMr5rYhl4SJAqtV1mx_1ntZg/edit?usp=sharing
        /// 参照
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float CalcSpeed(int offset) => defaultSpeed + speedUnit * offset;
    }
}
