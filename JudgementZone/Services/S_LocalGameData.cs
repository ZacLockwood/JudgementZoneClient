using System;
using System.Collections.Generic;
using JudgementZone.Models;

namespace JudgementZone.Obsolete
{
    public class S_LocalGameData
    {

        #region Singleton Instance

        private static volatile S_LocalGameData instance;
        private static object syncRoot = new Object();
        public static S_LocalGameData Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new S_LocalGameData();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Instance Properties

        private M_Player _myPlayer;
        public M_Player MyPlayer
        {
            get
            {
                return _myPlayer;
            }
            set
            {
                if (_playersInGame != null)
                {
                    if (_playersInGame.Contains(_myPlayer))
                    {
                        _playersInGame.Remove(_myPlayer);
                    }

                    _playersInGame.Insert(0, value);
                }

                _myPlayer = value;
            }
        }

        private List<M_Player> _playersInGame;
        public List<M_Player> PlayersInGame
        {
            get
            {
                if (_playersInGame == null)
                {
                    _playersInGame = new List<M_Player>();
                }

                return _playersInGame;
            }
        }

        private List<M_Player> _allPlayers;
        public List<M_Player> AllPlayers
        {
            get
            {
                if (_allPlayers == null)
                {
                    _allPlayers = new List<M_Player>();
                    if (_myPlayer != null)
                    {
                        _allPlayers.Add(MyPlayer);
                    }
                }
                else
                {
                    // Should be unnecessary
                    if (_myPlayer != null && !_allPlayers.Contains(_myPlayer))
                    {
                        _allPlayers.Insert(0, _myPlayer);
                    }
                }
                return _allPlayers;
            }
        }

		private M_QuestionCard _focusedQuestion;
		public M_QuestionCard FocusedQuestion
		{
			get
			{
				return _focusedQuestion;
			}
			set
			{
				_focusedQuestion = value;
			}
		}

        private M_Player _focusedPlayer;
		public M_Player FocusedPlayer
		{
			get
			{
                return _focusedPlayer;
			}
			set
			{
                _focusedPlayer = value;
			}
		}

        public string GameKey { get; set; }

        #endregion

        #region Private Singleton Constructor

        private S_LocalGameData()
        {
        }

        #endregion

    }
}
