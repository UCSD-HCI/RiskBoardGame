using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RiskSpace
{
    public enum GameState
    {
        Invalid = 0,

        ChooseCountry,

        //TODO: use cards
        AddArmy,

        AttackChooseSource,
        AttackPickArmy,
        AttackChooseDest,
        AttackWaitDice,
        AttackAnimation,

        //TODO: rearrange
        //TODO: pick cards
        
    }

    public class StateManager
    {
        private PlayerManager playerManager;

        public GameState State { get; private set; }
        public int ActivePlayerId { get; private set; }
        public  int RoundCount { get; private set; }
        public int AvailabeNewArmy { get; private set; }

        public StateManager(PlayerManager playerManager)
        {
            State = GameState.ChooseCountry;
            ActivePlayerId = 1;
            RoundCount = 0;
            this.playerManager = playerManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>If it's already the last player, return false; otherwise return true </returns>
        private bool nextPlayer()
        {
            if (ActivePlayerId == PlayerManager.PlayerNum)
            {
                ActivePlayerId = 1;
                return false;
            }
            else
            {
                ActivePlayerId++;
                return true;
            }
        }

        public void Next()
        {
            switch (State)
            {
                case GameState.ChooseCountry:
                    nextPlayer();
                    break;

                case GameState.AddArmy:
                    AvailabeNewArmy--;
                    if (AvailabeNewArmy == 0)
                    {
                        State = GameState.AttackChooseSource;
                    }
                    break;

                case GameState.AttackPickArmy:
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            refreshPlayers();
        }

        public void Finish()
        {
            switch (State)
            {
                case GameState.ChooseCountry:
                    ActivePlayerId = 1;
                    State = GameState.AddArmy;
                    refreshAvailabeNewArmy();
                    break;

                case GameState.AttackChooseSource:
                    State = GameState.AttackPickArmy;
                    break;

                case GameState.AttackPickArmy:
                    State = GameState.AttackChooseDest;
                    break;

                case GameState.AttackWaitDice:
                    State = GameState.AttackAnimation;
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            refreshPlayers();
        }

        public void Cancel()
        {
            switch (State)
            {
                case GameState.AttackPickArmy:
                    State = GameState.AttackChooseSource;
                    break;

                case GameState.AttackChooseDest:
                    State = GameState.AttackChooseSource;
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            refreshPlayers();
        }

        public void RoundPass()
        {
            Debug.Assert(State == GameState.AttackChooseSource);
            State = GameState.AddArmy;
            if (!nextPlayer())
            {
                RoundCount++;
            }
        }

        private void refreshPlayers()
        {
            playerManager.Update();
        }

        private void refreshAvailabeNewArmy()
        {
            AvailabeNewArmy = Math.Max(3, (int)(playerManager.GetPlayer(ActivePlayerId)).CountryNum / 3);
        }
    }
}
