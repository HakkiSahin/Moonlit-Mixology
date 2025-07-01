using UnityEngine;
using UnityEngine.Splines;
public class SplineMove : MonoBehaviour
{
    // Hareketi kontrol edeceğimiz SplineAnimate bileşeni
    private SplineAnimate splineAnimate;

    [Tooltip("Nesnenin spline üzerindeki hareket hızı.")]
    [SerializeField] private float speed = 0.1f;

    void Awake()
    {
        // Scriptin eklendiği nesnedeki SplineAnimate bileşenini bul ve referans al
        splineAnimate = GetComponent<SplineAnimate>();

        // Otomatik oynatmanın kapalı olduğundan emin olmak için animasyonu duraklat
        splineAnimate.Pause();
    }

    void Update()
    {
        // Eğer farenin sol tuşuna basılı tutuluyorsa...
        if (Input.GetMouseButton(0))
        {
            // Mevcut animasyon zamanını al (0 ile 1 arasında bir değerdir)
            float currentTime = splineAnimate.NormalizedTime;

            // Zamanı hız ve Time.deltaTime (kare hızından bağımsız hareket) ile artır
            currentTime += speed * Time.deltaTime;

            // Değerin 1'i geçmesini veya 0'ın altına inmesini engelle (Clamp01 metodu bunu yapar)
            splineAnimate.NormalizedTime = Mathf.Clamp01(currentTime);
        }
    }

    // Ekstra: Yolu sıfırlamak için bir fonksiyon
    public void ResetPath()
    {
        splineAnimate.Restart(false); // Animasyonu durdurarak başa al
    }
}
