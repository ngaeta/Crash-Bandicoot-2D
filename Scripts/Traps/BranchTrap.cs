using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class BranchTrap : Trap, IWalkable, ICheckpointLoadable
    {
        enum AnimationType { Down, Up }

        static float timeIdleOnGround = 1.5f;

        private AnimationType currAnim;
        private Dictionary<Tuple<AnimationType, int>, Rect> collidersAtFrame;
        private int indexGetOff;
        private float currTimeAction;
        private List<float> randomTimesGetOff;

        private AudioSource3D audioSource3D;
        private AudioClip clipUp;

        public bool CanWalkable { get; set; }

        public AudioClip AudioFootStep { get { return null; } }

        public BranchTrap(Vector2 spritePosition, string spriteSheetName = "branchTrap", DrawManager.Layer layer = DrawManager.Layer.Middleground) : base(spritePosition, spriteSheetName, layer)
        {
            CanWalkable = true;
            CreateColliders();
         
            List<float> randomNumbers = new List<float>
            {
                1.5f,
                2f,
                3f,
                4f,
            };

            randomTimesGetOff = new List<float>();

            while (randomNumbers.Count > 0)
            {
                float randomNumber = randomNumbers[RandomGenerator.GetRandom(0, randomNumbers.Count)];
                randomTimesGetOff.Add(randomNumber);

                randomNumbers.Remove(randomNumber);
            }

            indexGetOff = 0;
            playerAnimDie = Player.State.DeathMashed;

            audioSource3D = new AudioSource3D(this);

            clipUp = AudioManager.GetAudioClip("branchUp");

            animations[(int)AnimationType.Down].LoopAtFrame(4);
            animations[(int)AnimationType.Up].LoopAtFrame(4);
            Animation.IsActive = true;
            ChangeAnim(AnimationType.Down);
        }

        public override void Update()
        {
            base.Update();

            if (Animation.CurrFrame == 3)
            {
                if (currTimeAction <= 0)
                {
                    if (currAnim == AnimationType.Down)
                    {
                        ChangeAnim(AnimationType.Up);
                        indexGetOff = ++indexGetOff % randomTimesGetOff.Count;
                        currTimeAction = randomTimesGetOff[indexGetOff];
                    }
                    else
                    {
                        ChangeAnim(AnimationType.Down);
                        currTimeAction = timeIdleOnGround;
                    }

                }
                else
                    currTimeAction -= Game.DeltaTime;
            }
            else
                ChangeCollider();
        }

        protected override void OnCollisionWithPlayer(Player p, Collision collisionInfo)
        {
            if (currAnim == AnimationType.Down || (currAnim == AnimationType.Up && Animation.CurrFrame == 0))
            {
                float deltaX = collisionInfo.Delta.X;
                float deltaY = collisionInfo.Delta.Y;

                if (deltaX > deltaY)
                {
                    //Console.WriteLine(deltaY);
                    if (Position.Y + Height/2 > p.Position.Y - Height  && p.IsGrounded)
                    {
                        base.OnCollisionWithPlayer(p, collisionInfo);
                    }
                }
            }
        }

        private void ChangeCollider()
        {
            Rect newCollider = collidersAtFrame[new Tuple<AnimationType, int>(currAnim, Animation.CurrFrame)];
            RigidBody.SetBoundingBox(newCollider);
        }

        private void CreateColliders()
        {
            collidersAtFrame = new Dictionary<Tuple<AnimationType, int>, Rect>();

            //collidersDown
            Rect rect = new Rect(new Vector2(0, -Height / 4), null, Width, Height / 4);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Down, 0), rect);

            rect = new Rect(new Vector2(0, -Height / 4), null, Width, Height / 2);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Down, 1), rect);

            rect = new Rect(new Vector2(0, -Height / 4), null, Width, Height);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Down, 2), rect);

            rect = new Rect(new Vector2(0, 0), null, Width, Height);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Down, 3), rect);

            //collidersUp
            rect = new Rect(new Vector2(0, 0), null, Width, Height);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Up, 0), rect);

            rect = new Rect(new Vector2(0, -Height / 4), null, Width, Height);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Up, 1), rect);

            rect = new Rect(new Vector2(0, -Height / 4), null, Width, Height / 2);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Up, 2), rect);

            rect = new Rect(new Vector2(0, -Height / 4), null, Width, Height / 4);
            collidersAtFrame.Add(new Tuple<AnimationType, int>(AnimationType.Up, 3), rect);
        }

        private void ChangeAnim(AnimationType type)
        {
            currAnim = type;
            Animation = animations[(int)currAnim];
            Animation.Reset();

            PlayAudio(clipUp);
        }

        private void PlayAudio(AudioClip clip)
        {
            audioSource3D.PlayAudio(clip);
        }

        public void OnCheckpointLoad(CheckpointCrate checkpoint)
        {
            if(audioSource3D.IsPlaying)
            {
                audioSource3D.StopAudio();
            }
        }
    }
}
