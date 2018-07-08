using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class TriggerSpeakCoco : Trigger
    {
        float countDownStopGame;
        bool startCountDown;

        AudioSource source;
        AudioClip clip;

        public TriggerSpeakCoco(Rect rect) : base(rect)
        {
            countDownStopGame = 5f;

            clip = AudioManager.GetAudioClip("crystalPicked");
        }

        public override void OnTriggerEnter(GameObject collider)
        {
            base.OnTriggerEnter(collider);
            Player player = PlayScene.Player;

            if (player.IsGrounded)
            {
                if (GameManager.CrystalOfLevel != null)
                {
                    source = new AudioSource();
                    source.Play(clip);

                    GameManager.CrystalOfLevel.IsActive = true;
                    GameManager.CrystalOfLevel.Position = Rect.Position + new Vector2(Rect.HalfWidth, -Rect.HalfHeight * 2);
                    GuiManager.ActiveCrystal(false);

                    player.ChangeState(Player.State.Idle);
                    player.GetSprite().FlipX = false;
                    startCountDown = true;
                    player.StopInput = true;
                    player.Velocity = Vector2.Zero;
                }
            }
        }

        //public void Update()
        //{
        //    if (startCountDown)
        //    {
        //        if (countDownStopGame <= 0)
        //        {
        //            Game.CurrScene.NextScene = Game.CurrScene.PreviousScene;
        //            Game.CurrScene.IsPlaying = false;
        //        }
        //        else
        //            countDownStopGame -= Game.DeltaTime;
        //    }
        //}
    }
}
