using UnityEngine;
using CommonTools;

namespace QwertyGarden
{
    public class SoundManager : Singleton<SoundManager>
    {
        AudioSource m_audioSource;
        public AudioClip SFXMarbleMarble;
        public AudioClip SFXMarbleSlot;
        public AudioClip SFXMarbleInSlot;
        public AudioClip SFXScoring;
        public AudioClip SFXScoringTotal;
        public AudioClip SFXMoney;
        public AudioClip SFXWheelSpin;
        public AudioClip SFXButtonOK;
        public AudioClip SFXButtonCancel;
        public AudioClip SFXGameOver;
        public AudioClip SFXWinRound;
        public AudioClip SFXWinGame;
        public AudioClip SFXGateOpen;

        bool m_sfxExists;

        MetaData metaData;

        //Awake is always called before any Start functions
        protected override void Awake()
        {
            base.Awake();

            if (SFXMarbleMarble == null)
                m_sfxExists = false;
            else
                m_sfxExists = true;

            m_audioSource = GetComponent<AudioSource>();
        }

        public void Init(MetaData metaData)
        {
            this.metaData = metaData;
        }

        public void PlaySFXMarbleMarble()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXMarbleMarble);
        }
    }
}