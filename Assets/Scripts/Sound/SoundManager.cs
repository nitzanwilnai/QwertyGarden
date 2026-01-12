using UnityEngine;
using CommonTools;

namespace QwertyGarden
{
    public class SoundManager : Singleton<SoundManager>
    {
        AudioSource m_audioSource;
        public AudioClip SFXMoney;
        public AudioClip SFXFlowerCollected;
        public AudioClip SFXButtonCancel;
        public AudioClip SFXPrestige;
        public AudioClip SFXReceiptLine;
        public AudioClip SFXReceiptTotal;
        public AudioClip SFXWordComplete;
        public AudioClip SFXKeyClick;
        public AudioClip SFXKeyError;
        public AudioClip SFXHintBubble;
        
        bool m_sfxExists;

        MetaData metaData;

        //Awake is always called before any Start functions
        protected override void Awake()
        {
            base.Awake();

            if (SFXMoney == null)
                m_sfxExists = false;
            else
                m_sfxExists = true;

            m_audioSource = GetComponent<AudioSource>();
        }

        public void Init(MetaData metaData)
        {
            this.metaData = metaData;
        }

        public void PlaySFXMoney()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXMoney);
        }

        public void PlaySFXFlowerCollected()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXFlowerCollected);
        }

        public void PlaySFXFButtonCancel()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXButtonCancel);
        }

        public void PlaySFXFPrestige()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXPrestige);
        }

        public void PlaySFXReceiptLine()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXReceiptLine);
        }

        public void PlaySFXReceiptTotal()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXReceiptTotal);
        }

        public void PlaySFXWordComplete()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXWordComplete);
        }

        public void PlaySFXKeyClick()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXKeyClick);
        }

        public void PlaySFXKeyError()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXKeyError);
        }

        public void PlaySFXHitBubble()
        {
            if (m_sfxExists && metaData.SFX)
                m_audioSource.PlayOneShot(SFXHintBubble);
        }
    }
}