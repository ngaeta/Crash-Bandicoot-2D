using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;

namespace CrashBandicoot
{
    static class AudioManager
    {
        private static Dictionary<string, AudioClip> audioClips;

        private static AudioSource defaultAudioBackground;
        private static AudioSource currAudioBackground;

        private static List<AudioSource3D> audioSources3D;
        private static List<AudioSource> audioSourcesToDispose;

        private static AudioClip clipBackground;
        private static AudioClip defaultClipBackground;

        public static void InitClips()
        {
            audioClips = new Dictionary<string, AudioClip>();
        }

        public static void InitPlayScene()
        {
            defaultAudioBackground = new AudioSource();
            defaultAudioBackground.Volume = 0.85f;

            currAudioBackground = new AudioSource();
            currAudioBackground.Volume = 0.85f;

            audioSourcesToDispose = new List<AudioSource>();
            audioSources3D = new List<AudioSource3D>();
        }

        public static void Load()
        {
            AddAudioClip("defaultClipBackground", "Assets/Audio/BackgroundTheme.ogg");
            AddAudioClip("clipAkuAkuInvincibility", "Assets/Audio/Invincible Aku Aku.ogg");
            AddAudioClip("jungleBackground", "Assets/Audio/jungleBackground.ogg");

            AddAudioClip("grassFootStep", "Assets/Audio/grassFootStep.wav");
            AddAudioClip("woodenFootStep", "Assets/Audio/woodenFootStep.wav");
            AddAudioClip("metalFootStep", "Assets/Audio/metalFootStep.wav");
            AddAudioClip("metal2FootStep", "Assets/Audio/metal2FootStep.wav");

            AddAudioClip("crateBreak", "Assets/Audio/CrateBreak.wav");
            AddAudioClip("crashSpin", "Assets/Audio/CrashSpin.wav");
            AddAudioClip("crashHop", "Assets/Audio/CrashHop.wav");
            AddAudioClip("crashSlide", "Assets/Audio/CrashSlide.wav");
            AddAudioClip("crashWoah", "Assets/Audio/CrashWoah.wav");
            AddAudioClip("applePicked", "Assets/Audio/ApplePicked.wav");
            AddAudioClip("crystalPicked", "Assets/Audio/CrystalPicked.wav");

            AddAudioClip("bleep", "Assets/Audio/Bleep.wav");
            AddAudioClip("extraLifePicked", "Assets/Audio/ExtraLifePicked.wav");
            AddAudioClip("tntCountdown", "Assets/Audio/TntCountdown.wav");
            AddAudioClip("tntExplosion", "Assets/Audio/TntExplosion.wav");
            AddAudioClip("akuAkuPicked", "Assets/Audio/AkuAkuPicked.wav");
            AddAudioClip("nitroExplosion", "Assets/Audio/NitroExplosion.wav");
            AddAudioClip("crateBounce", "Assets/Audio/CrateBounce.wav");
            AddAudioClip("spinHit", "Assets/Audio/SpinHit.wav");
            AddAudioClip("wumpaHit", "Assets/Audio/WumpaHit.wav");
            AddAudioClip("crateTriggered", "Assets/Audio/CrateTriggered.wav");
            AddAudioClip("ironCrateHit", "Assets/Audio/IronCrateHit.wav");
            AddAudioClip("akuLevelDown", "Assets/Audio/AkuLevelDown.wav");
            AddAudioClip("jumpEnemyDead", "Assets/Audio/JumpEnemyDead.wav");
            AddAudioClip("crateTriggerHit", "Assets/Audio/CrateTrigger.wav");
            AddAudioClip("deathAngel", "Assets/Audio/DeathAngel.wav");
            AddAudioClip("fallingDeath", "Assets/Audio/FallingDeath.wav");
            AddAudioClip("waterSplash", "Assets/Audio/WaterSplash.wav");
            AddAudioClip("burned", "Assets/Audio/Burned.wav");
            AddAudioClip("checkpointHitted", "Assets/Audio/CheckpointHit.wav");
            AddAudioClip("smashed", "Assets/Audio/smashed.wav");


            //enemies
            AddAudioClip("birdFly", "Assets/Audio/Enemies/birdFly.wav");
            AddAudioClip("birdAttack", "Assets/Audio/Enemies/birdAttack.wav");
            AddAudioClip("lizard", "Assets/Audio/Enemies/lizard.wav");
            AddAudioClip("clipBurn", "Assets/Audio/Enemies/Burned.wav");

            AddAudioClip("gecko", "Assets/Audio/Enemies/gecko.wav");
            AddAudioClip("blowGun", "Assets/Audio/Enemies/blowGun.wav");

            //trap
            AddAudioClip("stoneWheelRotation", "Assets/Audio/Trap/stoneWheelRotation.wav");
            AddAudioClip("stoneHit", "Assets/Audio/Trap/stoneHit.wav");
            AddAudioClip("branchUp", "Assets/Audio/Trap/branchUp.wav");
            AddAudioClip("branchDown", "Assets/Audio/Trap/branchDown.wav");
            AddAudioClip("fireBurn", "Assets/Audio/Trap/FireBurn.wav");
        }

        private static bool AddAudioClip(string key, string audioClipName)
        {
            if (!audioClips.ContainsKey(key))
            {
                audioClips.Add(key, new AudioClip(audioClipName));
                return true;
            }
            return false;
        }

        public static void SetNewAudioBackground(string name)
        {
            AudioClip clip = GetAudioClip(name);

            if (clip != null)
            {
                defaultAudioBackground.Pause();
                clipBackground = clip;
                clipBackground.Rewind();
            }
        }

        public static void SetDefaultClipBackground(string name)
        {
            AudioClip clip = GetAudioClip(name);

            if (clip != null)
            {
                defaultClipBackground = clip;
            }
        }

        public static void Update()
        {
            //if (ambienceClipBackground != null)
            //{
            //    ambienceAudioBackground.Stream(ambienceClipBackground, Game.DeltaTime);
            //}

            if (clipBackground != null)
            {
                currAudioBackground.Stream(clipBackground, Game.DeltaTime, false);

                if (!currAudioBackground.IsPlaying)
                {
                    currAudioBackground.Stop();
                    clipBackground = null;
                    defaultAudioBackground.Resume();
                }
            }
            else if (defaultClipBackground != null)
            {
                defaultAudioBackground.Stream(defaultClipBackground, Game.DeltaTime);
            }

            for (int i = 0; i < audioSources3D.Count; i++)
            {
                audioSources3D[i].Update();
            }

            for (int i = 0; i < audioSourcesToDispose.Count; i++)
            {
                if (audioSourcesToDispose[i] == null)
                {
                    audioSourcesToDispose.Remove(audioSourcesToDispose[i]);
                    i--;
                }

                else if (!audioSourcesToDispose[i].IsPlaying)
                {
                    audioSourcesToDispose[i].Dispose();
                    audioSourcesToDispose.Remove(audioSourcesToDispose[i]);
                    i--;
                }
            }
        }

        public static AudioClip GetAudioClip(string audioClipName)
        {
            if (audioClips.ContainsKey(audioClipName))
            {
                return audioClips[audioClipName];
            }

            return null;
        }

        public static void AddAudioSource3D(AudioSource3D source3D)
        {
            audioSources3D.Add(source3D);
        }

        public static void DisposeAudioSource(AudioSource audioSource)
        {
            audioSourcesToDispose.Add(audioSource);
        }


        public static void DestroyAudioSource()
        {
            audioSources3D.Clear();
            audioSourcesToDispose.Clear();

            audioSources3D = null;
            audioSourcesToDispose = null;
            defaultAudioBackground = null;
            currAudioBackground = null;

            clipBackground = null;
            defaultClipBackground = null;
        }
    }
}
