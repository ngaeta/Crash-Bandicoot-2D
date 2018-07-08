using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Aiv.Audio;

namespace CrashBandicoot
{
    class LizardAttackState : EnemyState
    {
        private AudioClip clipFireBurn;
        private AudioSource audioSource;

        public override void AssignStateMachine(StateMachine machine)
        {
            base.AssignStateMachine(machine);
            clipFireBurn = AudioManager.GetAudioClip("clipBurn");
        }

        public override void Enter()
        {
            base.Enter();
            ((LizardEnemy)Owner).OnAttack();

            if(audioSource == null)
            {
                audioSource = new AudioSource();
            }

            audioSource.Play(clipFireBurn);
        }

        public override void Update()
        {
            base.Update();

            float xFinishAttack = ((LizardEnemy)Owner).XFinishAttack;
            float xStartAttack = ((LizardEnemy)Owner).XStartAttack;

            float distToTarget = Owner.Position.X - xFinishAttack;

            if (Math.Abs(distToTarget) < 4f)
            {
                if (xFinishAttack == xStartAttack)
                {
                    Owner.Machine.Switch((int)Enemy.State.Patrol);
                }
                else
                {
                    ((LizardEnemy)Owner).ReturnPatrol();
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            AudioManager.DisposeAudioSource(audioSource);
            ((LizardEnemy)Owner).FinishAttack();
        }
    }
}
