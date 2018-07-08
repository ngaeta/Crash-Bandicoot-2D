using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashBandicoot
{
    class Animation : IActivable, IUpdatable, ICloneable, IDestroyable
    {
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        float frameDelay;
        float counter;
        int startX;
        int startY;

        int currentFrameIndex;
        int currentFrame
        {
            get { return currentFrameIndex; }
            set
            {
                currentFrameIndex = value;
                OffsetX = startX + (currentFrameIndex % cols) * FrameWidth;
                OffsetY = startY + (currentFrameIndex / cols) * FrameHeight;
            }
        }

        int rows;
        int cols;

        int numFrames;

        int loopAtFrame;

        Dictionary<int, float> waitFrame;

        bool loop;
        public bool IsPlaying { get; protected set; }
        public bool IsRestarting { get; protected set; }
        public int CurrFrame { get { return currentFrameIndex; } }
        public float Speed { get { return frameDelay; } set { frameDelay = value; } }
        public bool IsActive { get; set; }


        public Animation(int fwidth, int fheight, int cols = 1, int rows = 1, float fps = 1f, bool loop = true, int startX = 0, int startY = 0)
        {
            FrameWidth = fwidth;
            FrameHeight = fheight;
            this.loop = loop;
            this.rows = rows;
            this.cols = cols;
            this.startX = startX;
            this.startY = startY;
            frameDelay = 1 / fps;
            numFrames = rows * cols;
            IsActive = true;
            currentFrame = 0;  //al
            loopAtFrame = -1;
            IsRestarting = false;
            GameManager.Destoyables.Add(this);
        }
        protected virtual void OnAnimationEnds()
        {
            if (loop)
            {
                currentFrame = 0;
                IsRestarting = true;
            }
            else
            {
                IsActive = false;
                IsPlaying = false;
                IsRestarting = false;
            }
        }
        public void Update()
        {
            if (IsActive)
            {
                IsRestarting = false;
                IsPlaying = true;
                counter += Game.DeltaTime;

                if (waitFrame != null && waitFrame.Count > 0)
                {
                    if (waitFrame.ContainsKey(currentFrame))
                    {
                        if (counter > waitFrame[currentFrame])
                        {
                            counter = 0;
                            waitFrame.Remove(currentFrame);
                        }
                    }
                }

                else if (counter >= frameDelay)
                {
                    counter = 0;
                    ++currentFrame;

                    if (currentFrame == loopAtFrame)
                    {
                        currentFrame--;
                    }

                    else if (currentFrame == numFrames)
                    {
                        OnAnimationEnds();
                    }
                }
            }
        }

        public void Play()
        {
            IsPlaying = true;
            IsActive = true;
        }

        public void LoopAtFrame(int frame)
        {
            loopAtFrame = frame;
            IsActive = true;
        }

        public void Reset()
        {
            currentFrame = 0;
            IsActive = true;
        }

        public void NextFrame()
        {
            currentFrame++;
        }

        public void WaitForNextFrame(int frameToWait, float timeToWait)
        {
            if (waitFrame == null)
                waitFrame = new Dictionary<int, float>();

            waitFrame.Add(frameToWait, timeToWait);
        }

        public object Clone()
        {
            return MemberwiseClone(); //crea copia superficiale della classe
        }

        public void Destroy()
        {
            if (waitFrame != null)
            {
                waitFrame.Clear();
                waitFrame = null;
            }
        }
    }
}
