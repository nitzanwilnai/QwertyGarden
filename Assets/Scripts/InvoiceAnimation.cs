using UnityEngine;

namespace QwertyGarden
{
    public class InvoiceAnimation : MonoBehaviour
    {
        public GameObject[] PrefabLines;
        [HideInInspector] public GameObject[] InvoiceLines;
        [HideInInspector] public bool[] ShowInvoiceLIne;
        public int ItemLineIndexStart;

        public float AnimTime;
        float m_animTime;
        int m_lineIndex;
        bool m_playAnimation = false;

        public void Init(Balance balance)
        {
            InvoiceLines = new GameObject[PrefabLines.Length + balance.NumFlowers];
            ShowInvoiceLIne = new bool[PrefabLines.Length + balance.NumFlowers];
            for (int i = 0; i < ItemLineIndexStart; i++)
                InvoiceLines[i] = PrefabLines[i];
            for (int i = ItemLineIndexStart; i < PrefabLines.Length; i++)
                InvoiceLines[i + balance.NumFlowers] = PrefabLines[i];

            for (int i = 0; i < ShowInvoiceLIne.Length; i++)
                ShowInvoiceLIne[i] = true;
        }

        public void StartAnimation()
        {
            m_playAnimation = true;
            m_animTime = 0.0f;
            m_lineIndex = 0;
            for (int i = 0; i < InvoiceLines.Length; i++)
                InvoiceLines[i].SetActive(false);
        }

        void Update()
        {
            if (m_playAnimation)
            {
                m_animTime += Time.deltaTime;
                if (m_animTime >= AnimTime)
                {
                    if (ShowInvoiceLIne[m_lineIndex])
                    {
                        m_animTime -= AnimTime;
                        InvoiceLines[m_lineIndex].SetActive(true);
                    }
                    m_lineIndex++;
                    if (m_lineIndex >= InvoiceLines.Length)
                    {
                        SoundManager.Instance.PlaySFXReceiptTotal();
                        m_playAnimation = false;
                    }
                    else
                        SoundManager.Instance.PlaySFXReceiptLine();
                }
            }
        }
    }
}
