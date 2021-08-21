using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    /// <summary>
    /// 方向
    /// </summary>
    public static class Direction
    {
        public static bool IsNothing(this Directions dir) => dir == Directions.Nothing;


        public static Vector3Int ConvertDirToCell(Directions dir)
        {
            switch (dir)
            {
                case Directions.Up:
                    return Vector3Int.up;
                case Directions.Down:
                    return Vector3Int.down;
                case Directions.Left:
                    return Vector3Int.left;
                case Directions.Right:
                    return Vector3Int.right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Nothingは禁止です");
            }
        }

        public static Vector3 ConvertDirToCell(Directions dir, float unit = 1)
        {
            if (unit <= 0) throw new ArgumentOutOfRangeException(nameof(unit));
            switch (dir)
            {
                case Directions.Up:
                    return Vector3.up * unit;
                case Directions.Down:
                    return Vector3.down * unit;
                case Directions.Left:
                    return Vector3.left * unit;
                case Directions.Right:
                    return Vector3.right * unit;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Nothingは禁止です");
            }
        }

        /// <summary>
        /// 逆
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Directions InverseDirection(Directions dir)
        {
            switch (dir)
            {
                case Directions.Up:
                    return Directions.Down;
                case Directions.Down:
                    return Directions.Up;
                case Directions.Left:
                    return Directions.Right;
                case Directions.Right:
                    return Directions.Left;
                default:
                    //エラー:変換不可
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Nothingは禁止です");
            }
        }


        public static bool ExistDirection(this List<Directions> directionsList, Directions dir)
        {
            return directionsList.Exists(directions => directions == dir);
        }

        //方向
        public enum Directions
        {
            Nothing = -1,
            Up,
            Down,
            Left,
            Right
        }
    }
}
