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
        QuestionStatsPresented
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
            SetupSignalRSubscriptions();
        }

        protected override void OnDisappearing()
        {
            ReleaseSignalRSubscriptions();
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

        private void QV_SetFocusedQuestionAndFocusedPlayer(M_QuestionCard focusedQuestion, M_Player focusedPlayer, M_Player myPlayer)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                GameQuestionView.DisplayQuestionCard(focusedQuestion);

                if (focusedPlayer.PlayerId == myPlayer.PlayerId)
                {
                    QV_EnableAnswerSubmission();
                }
                else
                {
                    QV_DisableAnswerSubmission();
                }
            });
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

        #region SignalR Responders

        private void SetupSignalRSubscriptions()
        {
            //QuestionReceived(null);

            var gameStateRealm = Realm.GetInstance("GameState.Realm");
            RealmGameStateListenerToken = gameStateRealm.All<M_ClientGameState>().SubscribeForNotifications((sender, changes, errors) =>
            {
                var gameState = sender.FirstOrDefault();
                if (gameState == null)
                {
                    return;
                }

                switch(gameState.ClientGameStateId)
                {
                    case 1:
                        // WAITING FOR GAME START
                        break;
                    case 2:
						// DISPLAY QUESTION
						Device.BeginInvokeOnMainThread(async () =>
						{
							GameQuestionView.Opacity = 0.0;
							GameQuestionView.IsVisible = true;

                            var focusedQuestion = Realm.GetInstance("QuestionDeck.Realm").Find<M_QuestionCard>(gameState.FocusedQuestionId);
                            var focusedPlayer = gameState.PlayerList.First(p => p.PlayerId == gameState.FocusedPlayerId);
                            var myPlayer = Realm.GetInstance("MyPlayerData.Realm").All<M_Player>().First();

                            QV_SetFocusedQuestionAndFocusedPlayer(focusedQuestion, focusedPlayer, myPlayer);
							
                            await GP_AnimateTransitionToPageState(E_GamePageState.QuestionPresented);
						});
                        break;
                    case 3:
						// DISPLAY QUESTION STATS
						Device.BeginInvokeOnMainThread(async () =>
						{
							GameQuestionStatsView.Opacity = 0.0;
							GameQuestionStatsView.IsVisible = true;
                            GameQuestionStatsView.DisplayStats(gameState);
							await Task.Delay(500);
							await GP_AnimateTransitionToPageState(E_GamePageState.QuestionStatsPresented);
						});
                        break;
                    case 4:
                        // DISPLAY GAME STATS
                        break;
                }
            });

            // HACK
            MessagingCenter.Subscribe<S_GameConnector, int>(this, "answerSubmitted", AnswerSubmitted);
        }

        private void ReleaseSignalRSubscriptions()
        {
            MessagingCenter.Unsubscribe<S_GameConnector, int>(this, "answerSubmitted");
        }

        private void EnableAnswerSubmission(S_GameConnector sender)
        {
            QV_EnableAnswerSubmission();
        }

        private void AnswerSubmitted(S_GameConnector sender, int answerId)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await GP_AnimateTransitionToPageState(E_GamePageState.LoaderPresented, "Waiting for Judgement...", (E_LogoColor)answerId);
            });
        }

        #endregion

	}
}
