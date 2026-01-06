/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;
using CommonTools;

namespace QwertyGarden
{
    public class MusicManager : Singleton<MusicManager>
    {
        public AudioClip MusicClip;

        AudioSource m_audioSource;
        MetaData metaData;

        public void Init(MetaData metaData)
        {
            this.metaData = metaData;
            m_audioSource = GetComponent<AudioSource>();
            m_audioSource.loop = true;
            Mute();
        }

        public void Mute()
        {
            m_audioSource.mute = !metaData.Music;
        }

        // // clipIdnex -1 means random clip
        public void PlayMusic()
        {
            if (MusicClip != null)
            {
                m_audioSource.clip = MusicClip;
                m_audioSource.Play();
            }
        }
    }
}