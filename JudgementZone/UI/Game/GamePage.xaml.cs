using System;
using System.Threading.Tasks;
using JudgementZone.Models;
using JudgementZone.Services;
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
            QuestionReceived(null);
            MessagingCenter.Subscribe<S_GameConnector>(this, "questionReceived", QuestionReceived);
            MessagingCenter.Subscribe<S_GameConnector, int>(this, "answerSubmitted", AnswerSubmitted);
            MessagingCenter.Subscribe<S_GameConnector>(this, "enableAnswerSubmission", EnableAnswerSubmission);
            MessagingCenter.Subscribe<S_GameConnector, M_AnswerStats>(this, "questionStatsReceived", DisplayQuestionStats);
        }

        private void ReleaseSignalRSubscriptions()
        {
            MessagingCenter.Unsubscribe<S_GameConnector>(this, "questionReceived");
            MessagingCenter.Unsubscribe<S_GameConnector, int>(this, "answerSubmitted");
            MessagingCenter.Unsubscribe<S_GameConnector>(this, "enableAnswerSubmission");
            MessagingCenter.Unsubscribe<S_GameConnector, M_AnswerStats>(this, "questionStatsReceived");
        }

        private void QuestionReceived(S_GameConnector sender)
        {
            var localGameData = S_LocalGameData.Instance;
            if (localGameData.FocusedPlayer == null || localGameData.FocusedQuestion == null)
            {
                Console.WriteLine("ERROR FOCUSED PLAYER OR QUESTION NOT FOUND");
                return;
            }
            if (localGameData.MyPlayer == null)
            {
                Console.WriteLine("ERROR WITH MY PLAYER");
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                GameQuestionView.Opacity = 0.0;
                GameQuestionView.IsVisible = true;
                QV_SetFocusedQuestionAndFocusedPlayer(localGameData.FocusedQuestion, localGameData.FocusedPlayer, localGameData.MyPlayer);
                await GP_AnimateTransitionToPageState(E_GamePageState.QuestionPresented);
            });
        }

        private void EnableAnswerSubmission(S_GameConnector sender)
        {
            QV_EnableAnswerSubmission();
        }

        private void AnswerSubmitted(S_GameConnector sender, int answerKey)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await GP_AnimateTransitionToPageState(E_GamePageState.LoaderPresented, "Waiting for Judgement...", (E_LogoColor)answerKey);
            });
        }

        private void DisplayQuestionStats(S_GameConnector sender, M_AnswerStats answerStats)
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                GameQuestionStatsView.Opacity = 0.0;
				GameQuestionStatsView.IsVisible = true;
                GameQuestionStatsView.DisplayStats(answerStats);
                await Task.Delay(500);
                await GP_AnimateTransitionToPageState(E_GamePageState.QuestionStatsPresented);
            });
        }

        #endregion

	}
}
