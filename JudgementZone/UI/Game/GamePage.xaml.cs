using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using JudgementZone.Models;
using JudgementZone.Services;
using Realms;
using Xamarin.Forms;
using JudgementZone.Interfaces;

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

        public DateTimeOffset GamePageCreationDate { get; set; } = DateTimeOffset.UtcNow;

        #region Constructor

        public GamePage()
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(async () =>
            {
                QV_DisableAnswerSubmission(); //HACK
                GameLoaderView.LoaderMessage = "Waiting for other players...";
                await GameLoaderView.StartContinuousFadeLoaderAsync();
            });
        }

        #endregion

        #region Page Lifecycle

        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
				SetupRealmSubscriptions();
				SetupUISubscriptions();
            });
        }

        protected override void OnDisappearing()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
				ReleaseRealmSubscriptions();
				ReleaseUISubscriptions();
            });
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

            // Create full list of displayed views
            List<I_PresentableGameView> gameViewsToHide = new List<I_PresentableGameView>
            {
                GameLoaderView,
                GameQuestionView,
                GameQuestionStatsView,
                GameStatsView
            };

            // Get view to present/remove from views to hide
            I_PresentableGameView viewToPresent;
            switch(newPageState)
			{
				case E_GamePageState.LoaderPresented:
                    GameLoaderView.FirstColor = startColor;
                    GameLoaderView.LoaderMessage = loadMessage;
					viewToPresent = GameLoaderView;
                    gameViewsToHide.Remove(GameLoaderView);
					break;
                case E_GamePageState.QuestionPresented:
                    viewToPresent = GameQuestionView;
                    gameViewsToHide.Remove(GameQuestionView);
					MainAbsoluteLayout.RaiseChild(GameQuestionView);
                    break;
                case E_GamePageState.QuestionStatsPresented:
                    viewToPresent = GameQuestionStatsView;
                    gameViewsToHide.Remove(GameQuestionStatsView);
                    MainAbsoluteLayout.RaiseChild(GameQuestionStatsView);
                    break;
                case E_GamePageState.GameStatsPresented:
                    viewToPresent = GameStatsView;
                    gameViewsToHide.Remove(GameStatsView);
                    MainAbsoluteLayout.RaiseChild(GameStatsView);
                    break;
				default:
					GameLoaderView.FirstColor = startColor;
					GameLoaderView.LoaderMessage = loadMessage;
                    viewToPresent = GameLoaderView;
                    gameViewsToHide.Remove(GameLoaderView);
                    break;
            }

            // Hide all views not meant to be displayed
            foreach (var gameView in gameViewsToHide)
            {
                gameView.Hide();
            }

            // Present view
            viewToPresent.Present();

            // Unlock UI
            IsEnabled = true;

            foreach (var view in MainAbsoluteLayout.Children)
            {
                Console.WriteLine($"{view.GetType()}");
            }

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

                if (focusedPlayer.PlayerId == myPlayer.PlayerId && GameTurnIndicatorBackground.Opacity < 1.0 && gameState.ClientViewCode != 4)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var animated = changes != null;
                        GTIB_Present(animated);
                    });
                }
                else if ((focusedPlayer.PlayerId != myPlayer.PlayerId || gameState.ClientViewCode == 4) && GameTurnIndicatorBackground.Opacity > 0.0)
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
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            GameStatsView.UpdateView(gameState.PlayerGameStatsList.ToList(), gameState.PlayerList.ToList());
							await GP_AnimateTransitionToPageState(E_GamePageState.GameStatsPresented);
                        });
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

        private void SetupUISubscriptions()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
				MessagingCenter.Unsubscribe<QuestionView, int>(this, "AnswerSelected");
				MessagingCenter.Unsubscribe<GameStatsView, int>(this, "EndGameButtonPressed");
				MessagingCenter.Subscribe<QuestionView, int>(this, "AnswerSelected", AnswerSelected);
				MessagingCenter.Subscribe<GameStatsView>(this, "EndGameButtonPressed", EndGameButtonPressed);
            });
        }

        private void ReleaseUISubscriptions()
        {
			Device.BeginInvokeOnMainThread(() =>
			{
                MessagingCenter.Unsubscribe<QuestionView, int>(this, "AnswerSelected");
                MessagingCenter.Unsubscribe<GameStatsView, int>(this, "EndGameButtonPressed");
            });
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

        private void EndGameButtonPressed(GameStatsView sender)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
                try
                {
                    Console.WriteLine($"GAME PAGE CREATED: {GamePageCreationDate}");

                    if (PageState == E_GamePageState.GameStatsPresented)
					{
						ReleaseUISubscriptions();
						ReleaseRealmSubscriptions();

						var gameStateRealm = Realm.GetInstance("GameState.Realm");
						gameStateRealm.Write(() =>
						{
							gameStateRealm.RemoveAll();
						});

                        await Navigation.PopModalAsync();
					}
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ex caught: {ex.Message}");
                    throw ex;
                }
			});
		}

        #endregion

    }
}
