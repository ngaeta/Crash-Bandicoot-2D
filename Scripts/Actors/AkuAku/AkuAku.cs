using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Fast2D;
using Aiv.Audio;

namespace CrashBandicoot
{
    class AkuAku : GameObject, ICheckpointLoadable
    {
        enum Level { FIRST, SECOND, THIRD }

        public const string CLIP_INVINCIBILITY = "clipAkuAkuInvincibility";
        public const float TIME_INVULNERABILITY = 20f;

        private StateMachine machine;
        private Texture[] textures;
        private Level currLevel;
        private Player owner;
        private AudioSource audioSourceOnPicked;
        private AudioClip clipOnPicked;
        private AudioClip clipLevelDown;

        public bool IsInvincible { get { return currLevel == Level.THIRD; } }

        public AkuAku(Vector2 spritePosition, Player owner, DrawManager.Layer drawLayer = DrawManager.Layer.Foreground) : base(spritePosition, "akuFirstLevel", drawLayer)
        {
            sprite.scale = new Vector2(0.9f, 0.9f);
            IsActive = false;
            CreateTextures();
            currLevel = Level.FIRST;

            this.owner = owner;
            clipOnPicked = AudioManager.GetAudioClip("akuAkuPicked");
            clipLevelDown = AudioManager.GetAudioClip("akuLevelDown");

            AkuAkuState.Player = owner;
            AkuAkuState.Offset = new Vector2(-30f, -20f);

            machine = new StateMachine(this);
            machine.RegisterState((int)AkuState.Idle, new IdleAkuAkuState());
            machine.RegisterState((int)AkuState.Follow, new FollowAkuAkuState());
            machine.RegisterState((int)AkuState.Attack, new AttackAkuAkuState());

            machine.Switch((int)AkuState.Idle);
        }

        public override void Update()
        {
            if (IsActive)
            {
                base.Update();
                machine.Run();
            }
        }

        public bool LevelUp()
        {
            if (!IsActive)
            {
                Position = owner.Position;
                IsActive = true;
                currLevel = 0;

                PlayAudio(clipOnPicked);
                texture = textures[(int)currLevel];
            }
            else if (currLevel != Level.THIRD)
            {
                currLevel++;

                if (currLevel == Level.THIRD)
                {
                    AudioManager.SetNewAudioBackground(CLIP_INVINCIBILITY);
                    machine.Switch((int)Level.THIRD);
                    return true;
                }
                else
                {
                    PlayAudio(clipOnPicked);
                }

                texture = textures[(int)currLevel];
            }

            return false;
        }

        public void LevelDown()
        {
            currLevel--;

            if (currLevel < 0)
                IsActive = false;
            else
                texture = textures[(int)currLevel];

            OnHit();
        }

        public void OnHit()
        {
            PlayAudio(clipLevelDown);
        }

        public void ResetLevel()
        {
            OnHit();
            IsActive = false;
        }

        private void CreateTextures()
        {
            int numLevel = 3;
            textures = new Texture[numLevel];

            for (int i = 0; i < numLevel; i++)
            {
                switch ((Level)i)
                {
                    case Level.FIRST:
                        textures[i] = GfxManager.GetSpritesheet("akuFirstLevel").Item1;
                        break;
                    case Level.SECOND:
                        textures[i] = GfxManager.GetSpritesheet("akuSecondLevel").Item1;
                        break;
                    case Level.THIRD:
                        textures[i] = GfxManager.GetSpritesheet("akuSecondLevel").Item1;
                        break;
                }
            }
        }

        public void EndInvulnerability()
        {
            currLevel--;
            machine.Switch((int)AkuState.Follow);
        }

        private void PlayAudio(AudioClip clip)
        {
            if(audioSourceOnPicked == null)
                audioSourceOnPicked = new AudioSource();

            audioSourceOnPicked.Play(clip);
            AudioManager.DisposeAudioSource(audioSourceOnPicked);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            machine = null;
            textures = null;
            owner = null;
            audioSourceOnPicked = null;
            clipOnPicked = null;
            clipLevelDown = null;
        }

        public void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if(audioSourceOnPicked != null)
            {
                if(audioSourceOnPicked.IsPlaying)
                    audioSourceOnPicked.Stop();

                audioSourceOnPicked.Dispose();
                audioSourceOnPicked = null;
            }
        }
    }
}
