using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Pacman.Direction;

namespace Pacman
{

    public class PlayerAgent : Agent
    {

        private PlayerMove playerMove;


        //初期化
        public override void OnEpisodeBegin()
        {

            playerMove.Initialize();
            playerMove.envManager.Init();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);//3
            foreach (var enemy in playerMove.enemyManager.enemies)//4*4=16
            {
                sensor.AddObservation(enemy.transform.localPosition);
                //V3
                sensor.AddOneHotObservation((int)enemy.nowDirection,4);
            }

            sensor.AddObservation(playerMove.isPowerPacman);//1

            sensor.AddObservation(playerMove.cookieCount);//1
            //22
            //V3
            sensor.AddOneHotObservation((int)playerMove.nowDirection,4);//
//V4
            var floats = new[]
            {
                playerMove.IsMovable(Directions.Up)?1.0f:0.0f,
                playerMove.IsMovable(Directions.Down)?1.0f:0.0f,
                playerMove.IsMovable(Directions.Left)?1.0f:0.0f,
                playerMove.IsMovable(Directions.Right)?1.0f:0.0f
            };
            sensor.AddObservation(floats);

        }

        private void Start()
        {

            playerMove = GetComponent<PlayerMove>();
            playerMove.agent = this;
        }

        public void GameClear()
        {
            AddReward(10);
            EndEpisode();
        }

        public void GameOver()
        {
            //V5 -0.5 to
            AddReward(-1f);
            EndEpisode();
        }

        //V5
        public void EatCookie(bool isPower) => AddReward(isPower ? 0.1f : 0.08f);

        public void KilledMonster() => AddReward(playerMove.enemyKillCount * 0.3f);

        //途中から追加
        public void DontMove() => AddReward(-0.01f);



        //ここまで
        public override void OnActionReceived(ActionBuffers actions)
        {
            Directions dir = (Directions)actions.DiscreteActions[0];
            if (dir != Directions.Nothing)
            {
                playerMove.SetNextDirection(dir);
            }
            //V5
            //AddReward(0.01f);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = (int)Directions.Nothing;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                discreteActionsOut[0] = (int)Directions.Up;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                discreteActionsOut[0] = (int)Directions.Down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                discreteActionsOut[0] = (int)Directions.Left;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                discreteActionsOut[0] = (int)Directions.Right;
            }
        }
    }
}
