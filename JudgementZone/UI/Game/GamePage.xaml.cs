using System;
using System.Linq;
using System.Threading.Tasks;
using JudgementZone.Models;
using JudgementZone.Services;
using Realms;
using Xamarin.Forms;

namespace JudgementZone.UI
{

    public enum E_GamePageState
    {
        LoaderPresented = 1,
        QuestionPresented,
        QuestionStatsPresented,
        GameStatsPresented
    }

    public partial class GamePage : ContentPage
    {

        public E_GamePageState PageState { get; private set; } = E_GamePageState.LoaderPresented;

        public IDisposable RealmGameStateListenerToken { get; private set; }

        #region Constructor

        public GamePage()
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(async () =>
            {
                QV_DisableAnswerSubmission(); //HACK
                await GameLoaderView.StartContinuousFadeLoaderAsync("Waiting for Other Players...");
            });
        }

        #endregion

        #region Page Lifecycle

        protected override void OnAppearing()
        {
            SetupRealmSubscriptions();
            SetUpUISubscriptions();
        }

        protected override void OnDisappearing()
        {
            ReleaseRealmSubscriptions();
            ReleaseUISubscriptions();
        }

        #endregion

        #region UI Root Game Page Helper Methods

        private async Task GP_AnimateTransitionToPageState(E_GamePageState newPageState, string loadMessage = "Loading...", E_LogoColor startColor = E_LogoColor.Random)
        {
            if (newPageState == PageState)
                return;

            PageState = newPageState;

            // Lock UI
            IsEnabled = false;

            if (GameLoaderView.AnimationIsRunning("FadeIn") && newPageState != E_GamePageState.LoaderPresented)
            {
                GameLoaderView.AbortAnimation("FadeIn");
            }

            if (GameQuestionView.AnimationIsRunning("FadeIn") && newPageState != E_GamePageState.QuestionPresented)
            {
                GameQuestionView.AbortAnimation("FadeIn");
            }

            if (GameQuestionStatsView.AnimationIsRunning("FadeIn") && newPageState != E_GamePageState.QuestionStatsPresented)
            {
                GameQuestionStatsView.AbortAnimation("FadeIn");
            }

            if (!GameLoaderView.AnimationIsRunning("FadeOut") && GameLoaderView.Opacity > 0.0 && newPageState != E_GamePageState.LoaderPresented)
            {
                GameLoaderView.Animate("FadeOut",
                                       (percent) =>
                                       {
                                           GameLoaderView.Opacity = Math.Abs(percent - 1.0);
                                       },
                                       16, 250, Easing.CubicInOut,
                                       (double percent, bool canceled) =>
                                       {
                                           if (!canceled)
                                           {
                                               GameLoaderView.StopContinuousFadeLoader();
                                               GameLoaderView.IsVisible = false;
                                               GameLoaderView.IsEnabled = false;
                                           }
                                       });
            }

            if (!GameQuestionView.AnimationIsRunning("FadeOut") && GameQuestionView.Opacity > 0.0 && newPageState != E_GamePageState.QuestionPresented)
            {
                GameQuestionView.Animate("FadeOut",
                                         (percent) =>
                                         {
                                             GameQuestionView.Opacity = Math.Abs(percent - 1.0);
                                         }, 16, 250, Easing.CubicInOut,
                                         (double percent, bool canceled) =>
                                         {
                                             if (!canceled)
                                             {
                                                 GameQuestionView.IsVisible = false;
                                                 GameQuestionView.IsEnabled = false;
                                             }
                                         });
            }

            if (!GameQuestionStatsView.AnimationIsRunning("FadeOut") && GameQuestionStatsView.Opacity > 0.0 && newPageState != E_GamePageState.QuestionStatsPresented)
            {
                GameQuestionStatsView.Animate("FadeOut",
                                              (percent) =>
                                              {
                                                  GameQuestionStatsView.Opacity = Math.Abs(percent - 1.0);
                                              }, 16, 250, Easing.CubicInOut,
                                              (double percent, bool canceled) =>
                                              {
                                                  if (!canceled)
                                                  {
                                                      GameQuestionStatsView.IsVisible = false;
                                                      GameQuestionStatsView.IsEnabled = false;
                                                  }
                                              });
            }

            switch (newPageState)
            {
                case E_GamePageState.LoaderPresented:
                    if (GameLoaderView.AnimationIsRunning("FadeOut"))
                    {
                        GameLoaderView.AbortAnimation("FadeOut");
                    }
                    if (!GameLoaderView.AnimationIsRunning("FadeIn"))
                    {
                        GameLoaderView.IsVisible = true;
                        GameLoaderView.StartContinuousFadeLoaderAsync(loadMessage, startColor);
                        GameLoaderView.Animate("FadeIn",
                                               (percent) =>
                                               {
                                                   GameLoaderView.Opacity = percent;
                                               },
                                               16, 250, Easing.CubicInOut,
                                               (double percent, bool canceled) =>
                                               {
                                                   if (canceled)
                                                   {
                                                       GameLoaderView.StopContinuousFadeLoader();
                                                       GameLoaderView.IsVisible = false;
                                                   }
                                                   else
                                                   {
                                                       GameLoaderView.IsEnabled = true;
                                                   }
                                               });
                    }
                    break;
                case E_GamePageState.QuestionPresented:
                    if (GameQuestionView.AnimationIsRunning("FadeOut"))
                    {
                        GameQuestionView.AbortAnimation("FadeOut");
                    }
                    if (!GameQuestionView.AnimationIsRunning("FadeIn"))
                    {
                        GameQuestionView.IsVisible = true;
                        GameQuestionView.Animate("FadeIn",
                                                 (percent) =>
                                                 {
                                                     GameQuestionView.Opacity = percent;
                                                 },
                                                 16, 250, Easing.CubicInOut,
                                                 (double percent, bool canceled) =>
                                                 {
                                                     if (canceled)
                                                     {
                                                         GameQuestionView.IsVisible = false;
                                                     }
                                                     else
                                                     {
                                                         GameQuestionView.IsEnabled = true;
                                                         MainAbsoluteLayout.RaiseChild(GameQuestionView);
                                                     }

                                                 });
                    }
                    break;
                case E_GamePageState.QuestionStatsPresented:
                    if (GameQuestionStatsView.AnimationIsRunning("FadeOut"))
                    {
                        GameQuestionStatsView.AbortAnimation("FadeOut");
                    }
                    if (!GameQuestionStatsView.AnimationIsRunning("FadeIn"))
                    {
                        GameQuestionStatsView.IsVisible = true;
                        GameQuestionStatsView.Animate("FadeIn",
                                                      (percent) =>
                                                      {
                                                          GameQuestionStatsView.Opacity = percent;
                                                      },
                                                      16, 250, Easing.CubicInOut,
                                                      (double percent, bool canceled) =>
                                                      {
                                                          if (canceled)
                                                          {
                                                              GameQuestionStatsView.IsVisible = false;
                                                          }
                                                          else
                                                          {
                                                              GameQuestionStatsView.IsEnabled = true;
                                                              MainAbsoluteLayout.RaiseChild(GameQuestionStatsView);
                                                          }
                                                      });
                    }
                    break;
            }

            // Unlock UI
            IsEnabled = true;

        }

        #endregion

        #region UI Sub-View Helper Methods

        private void GTIB_Hide(bool animated = true)
        {
            if (!animated)
            {
                GameTurnIndicatorBackground.Opacity = 0.0;
                GameTurnIndicatorBackground.IsVisible = false;
                return;
            }

            if (GameTurnIndicatorBackground.AnimationIsRunning("FadeIn"))
            {
                GameTurnIndicatorBackground.AbortAnimation("FadeIn");
            }
            if (!GameTurnIndicatorBackground.AnimationIsRunning("FadeOut"))
            {
                GameTurnIndicatorBackground.IsVisible = true;
                GameTurnIndicatorBackground.Animate("FadeOut",
                                                    (percent) =>
                                                    {
                                                        GameTurnIndicatorBackground.Opacity = Math.Abs(percent - 1.0);
                                                    },
                                                    16, 500, Easing.CubicInOut,
                                                    (double percent, bool canceled) =>
                                                    {
                                                        if (canceled)
                                                        {
                                                            GameTurnIndicatorBackground.Opacity = 1.0;
                                                            GameTurnIndicatorBackground.IsVisible = true;
                                                        }
                                                        else
                                                        {
                                                            GameTurnIndicatorBackground.Opacity = 0.0;
                                                            GameTurnIndicatorBackground.IsVisible = false;
                                                        }
                                                    });
            }
        }

        private void GTIB_Present(bool animated = true)
        {
            if (!animated)
            {
                GameTurnIndicatorBackground.Opacity = 1.0;
                GameTurnIndicatorBackground.IsVisible = true;
                return;
            }

            if (GameTurnIndicatorBackground.AnimationIsRunning("FadeOut"))
            {
                GameTurnIndicatorBackground.AbortAnimation("FadeOut");
            }
            if (!GameTurnIndicatorBackground.AnimationIsRunning("FadeIn"))
            {
                GameTurnIndicatorBackground.IsVisible = true;
                GameTurnIndicatorBackground.Animate("FadeIn",
                                                    (percent) =>
                                                    {
                                                        GameTurnIndicatorBackground.Opacity = percent;
                                                    },
                                                    16, 500, Easing.CubicInOut,
                                                    (double percent, bool canceled) =>
                                                    {
                                                        if (canceled)
                                                        {
                                                            GameTurnIndicatorBackground.Opacity = 0.0;
                                                            GameTurnIndicatorBackground.IsVisible = false;
                                                        }
                                                        else
                                                        {
                                                            GameTurnIndicatorBackground.Opacity = 1.0;
                                                            GameTurnIndicatorBackground.IsVisible = true;
                                                        }
                                                    });
            }
        }

        private void QV_EnableAnswerSubmission()
        {
            GameQuestionView.EnableAnswerControls();
        }

        private void QV_DisableAnswerSubmission()
        {
            GameQuestionView.DisableAnswerControls();
        }

        #endregion

        #region Subscription Setup Methods

        private void SetupRealmSubscriptions()
        {
            var gameStateRealm = Realm.GetInstance("GameState.Realm");
            RealmGameStateListenerToken = gameStateRealm.All<M_Client_GameState>().SubscribeForNotifications((sender, changes, errors) =>
            {
                var gameState = sender.FirstOrDefault();
                if (gameState == null)
                {
                    return;
                }

                var myPlayer = Realm.GetInstance("MyPlayerData.Realm").All<M_Player>().FirstOrDefault();
                var focusedPlayer = gameState.PlayerList.First(p => p.PlayerId == gameState.FocusedPlayerId);

                if (focusedPlayer.PlayerId == myPlayer.PlayerId && GameTurnIndicatorBackground.Opacity < 1.0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var animated = changes != null;
                        GTIB_Present(animated);
                    });
                }
                else if (focusedPlayer.PlayerId != myPlayer.PlayerId && GameTurnIndicatorBackground.Opacity > 0.0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var animated = changes != null;
                        GTIB_Hide(animated);
                    });
                }

                switch (gameState.ClientViewCode)
                {
                    case 1:
                        // WAITING FOR GAME START
                        break;
                    case 2:
                        // DISPLAY QUESTION
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            // if new round, await new round animation
                            // then continue...

                            // Update Question View
                            var focusedQuestion = Realm.GetInstance("QuestionDeck.Realm").Find<M_QuestionCard>(gameState.CurrentQuestionId);
                            var questionNum = gameState.CurrentQuestionNum;
                            var maxQuestionNum = gameState.MaxQuestionNum;
                            var roundNum = gameState.CurrentRoundNum;
                            var maxRoundNum = gameState.MaxRoundNum;
                            GameQuestionView.UpdateView(focusedQuestion, myPlayer, focusedPlayer, questionNum, maxQuestionNum, roundNum, maxRoundNum);

                            // Present Question View
                            await GP_AnimateTransitionToPageState(E_GamePageState.QuestionPresented);

                            // Enable Answer Submission
                            if (gameState.CanSubmitAnswer)
                            {
                                GameQuestionView.EnableAnswerControls();
                            }
                            else
                            {
                                GameQuestionView.DisableAnswerControls();
                            }
                        });
                        break;
                    case 3:
                        // DISPLAY QUESTION STATS
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            var stats = gameState.QuestionStats;
                            var focusedQuestion = Realm.GetInstance("QuestionDeck.Realm").Find<M_QuestionCard>(gameState.CurrentQuestionId);
							var questionNum = gameState.CurrentQuestionNum;
							var maxQuestionNum = gameState.MaxQuestionNum;
							var roundNum = gameState.CurrentRoundNum;
							var maxRoundNum = gameState.MaxRoundNum;

                            GameQuestionStatsView.Opacity = 0.0;
                            GameQuestionStatsView.IsVisible = true;
                            GameQuestionStatsView.DisplayStats(stats, focusedQuestion, myPlayer, focusedPlayer, questionNum, maxQuestionNum, roundNum, maxRoundNum);

                            await Task.Delay(500);
                            await GP_AnimateTransitionToPageState(E_GamePageState.QuestionStatsPresented);
                        });
                        break;
                    case 4:
                        // DISPLAY GAME STATS
                        break;
                }
            });
        }

        private void ReleaseRealmSubscriptions()
        {
            if (RealmGameStateListenerToken != null)
            {
                RealmGameStateListenerToken.Dispose();
                RealmGameStateListenerToken = null;
            }
        }

        private void SetUpUISubscriptions()
        {
            MessagingCenter.Subscribe<QuestionView, int>(this, "AnswerSelected", AnswerSelected);
        }

        private void ReleaseUISubscriptions()
        {
            MessagingCenter.Unsubscribe<QuestionView, int>(this, "AnswerSelected");
        }

		#endregion

		#region UI Subscription Responder Methods

		private void AnswerSelected(QuestionView sender, int answerId)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				var gameStateRealm = Realm.GetInstance("GameState.Realm");
				var gameState = gameStateRealm.All<M_Client_GameState>().FirstOrDefault();
				if (gameState != null)
				{
					await GP_AnimateTransitionToPageState(E_GamePageState.LoaderPresented, "Waiting for Judgement...", (E_LogoColor)answerId);
					await S_GameConnector.Connector.SendAnswerSubmission(answerId, gameState.GameKey);
				}
				else
				{
					sender.EnableAnswerControls();
				}
			});
		}

        #endregion

    }
}
