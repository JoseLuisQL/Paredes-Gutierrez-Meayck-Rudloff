using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(RigidbodyFirstPersonController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public AudioClip jumpSound;
    public AudioClip landSound;
    
    private RigidbodyFirstPersonController m_RigidBodyFPC;
    private AudioSource m_AudioSource;
    private bool m_PreviouslyGrounded;
    private float m_StepCycle;
    private float m_NextStep;
    
    [SerializeField] private float m_StepInterval = 5f;

    private void Start()
    {
        m_RigidBodyFPC = GetComponent<RigidbodyFirstPersonController>();
        m_AudioSource = GetComponent<AudioSource>();
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_PreviouslyGrounded = true;
    }

    private void Update()
    {
        if (!m_PreviouslyGrounded && m_RigidBodyFPC.Grounded)
        {
            PlayLandingSound();
        }

        if (!m_RigidBodyFPC.Grounded && m_PreviouslyGrounded && m_RigidBodyFPC.Jumping)
        {
            PlayJumpSound();
        }

        m_PreviouslyGrounded = m_RigidBodyFPC.Grounded;
    }

    private void FixedUpdate()
    {
        ProgressStepCycle(m_RigidBodyFPC.Velocity.magnitude);
    }

    private void ProgressStepCycle(float speed)
    {
        if (m_RigidBodyFPC.Velocity.sqrMagnitude > 0 && m_RigidBodyFPC.Grounded)
        {
            m_StepCycle += (speed + (1f)) * Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }

    private void PlayFootStepAudio()
    {
        if (!m_RigidBodyFPC.Grounded || footstepSounds == null || footstepSounds.Length == 0)
        {
            return;
        }
        int n = Random.Range(1, footstepSounds.Length);
        m_AudioSource.clip = footstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        footstepSounds[n] = footstepSounds[0];
        footstepSounds[0] = m_AudioSource.clip;
    }

    private void PlayLandingSound()
    {
        if (landSound != null) {
            m_AudioSource.clip = landSound;
            m_AudioSource.Play();
        }
        m_NextStep = m_StepCycle + .5f;
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null) {
            m_AudioSource.clip = jumpSound;
            m_AudioSource.Play();
        }
    }
}
