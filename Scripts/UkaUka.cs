using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class UkaUka : GameObject
    {
        public enum AnimationType { Show, Talk, Idle }

        public AnimationType CurrAnim { get; private set; }

        private AudioClip clipGameOver;
        private AudioClip clipShow;

        private AudioSource audioSource;

        public UkaUka(Vector2 spritePosition, string spriteSheetName = "ukaUka", DrawManager.Layer drawLayer = DrawManager.Layer.Playground) : base(spritePosition, spriteSheetName, drawLayer)
        {
            sprite.scale = new Vector2(2.5f);
            audioSource = new AudioSource();
            clipGameOver = new AudioClip("Assets/Audio/gameOver.wav");
            clipShow = new AudioClip("Assets/Audio/ukaUkaShowed.wav");

            Show();
        }

        public void Show()
        {
            Animation = animations[(int)AnimationType.Show];
            Animation.Reset();

            audioSource.Play(clipShow);

            CurrAnim = AnimationType.Show;
        }

        public void Talk()
        {
            Animation = animations[(int)AnimationType.Talk];
            CurrAnim = AnimationType.Talk;

            audioSource.Play(clipGameOver);
        }

        public override void Update()
        {
            base.Update();

            if(CurrAnim == AnimationType.Talk && !audioSource.IsPlaying)
            {
                StopAnim();
            }
        }

        private void StopAnim()
        {
            Animation = animations[(int)AnimationType.Idle];
            CurrAnim = AnimationType.Idle;

            audioSource = null;
            clipGameOver = clipShow = null;
        }
    }
}
