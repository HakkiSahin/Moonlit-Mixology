using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;

public class SplineTest : MonoBehaviour
{
    // Ana spline (bu spline uzatılacak)
    public SplineContainer splineA;

    // Ana spline'a eklenecek olan ikinci spline
    public SplineContainer splineB;

    void Update()
    {
        // 'J' tuşuna basıldığında birleştirme (Join) işlemini yapalım
        if (Input.GetKeyDown(KeyCode.J))
        {
            StitchSplines();
        }
    }

    public void StitchSplines()
    {
        if (splineA == null || splineB == null || splineA.Spline == null || splineB.Spline == null)
        {
            Debug.LogError("Her iki spline da atanmış olmalı!");
            return;
        }

        if (splineA.Spline.Count == 0 || splineB.Spline.Count == 0)
        {
            Debug.LogError("Spline'lardan biri boş, birleştirilemez!");
            return;
        }

        // --- 1. DÜNYA UZAYINDA HİZALAMA İÇİN GEREKLİ KAYDIRMA MİKTARINI HESAPLA ---

        // SplineA'nın son noktasının dünya pozisyonunu al
        BezierKnot lastKnotA = splineA.Spline.Last();
        float3 lastKnotA_WorldPos = splineA.transform.TransformPoint(lastKnotA.Position);

        // SplineB'nin ilk noktasının dünya pozisyonunu al
        BezierKnot firstKnotB = splineB.Spline.First();
        float3 firstKnotB_WorldPos = splineB.transform.TransformPoint(firstKnotB.Position);

        // İki nokta arasındaki fark (kaydırma vektörü)
        float3 positionOffset = lastKnotA_WorldPos - firstKnotB_WorldPos;


        // --- 2. SPLINEB'NİN NOKTALARINI DÖNÜŞTÜREREK SPLINEA'YA EKLE ---

        // SplineB'nin ilk noktası hariç tüm noktalarını dön
        // Skip(1) metodu ilk elemanı atlamamızı sağlar
        foreach (var knotToCopy in splineB.Spline.Knots.Skip(1))
        {
            var newKnot = knotToCopy;

            // Kopyalanacak noktanın mevcut dünya pozisyonunu hesapla
            float3 currentWorldPos = splineB.transform.TransformPoint(knotToCopy.Position);

            // Bu pozisyonu, hesaplanan ofset miktarı kadar kaydırarak yeni dünya pozisyonunu bul
            float3 newWorldPos = currentWorldPos + positionOffset;

            // Yeni dünya pozisyonunu, SplineA'nın lokal uzayına çevir ve knot'a ata
            newKnot.Position = splineA.transform.InverseTransformPoint(newWorldPos);

            // Teğetlerin (eğim kollarının) yönünü de doğru şekilde dönüştür
            newKnot.TangentIn =
                splineA.transform.InverseTransformDirection(splineB.transform.TransformDirection(knotToCopy.TangentIn));
            newKnot.TangentOut =
                splineA.transform.InverseTransformDirection(
                    splineB.transform.TransformDirection(knotToCopy.TangentOut));

            splineA.Spline.Add(newKnot, TangentMode.Broken); // Teğetleri biz ayarladığımız için şimdilik 'Broken'
        }

        // --- 3. BİRLEŞME NOKTASINI PÜRÜZSÜZLEŞTİR (SİHİRLİ DOKUNUŞ) ---

        // SplineA'nın orijinal son noktasının indeksini bul
        // Yeni noktaları eklemeden önceki son elemanın indeksi
        int stitchIndex = splineA.Spline.Count - splineB.Spline.Count;

        // Bu noktadaki teğet modunu 'AutoSmooth' yap.
        // Bu, Unity'nin birleşme noktasındaki eğimi otomatik olarak pürüzsüzleştirmesini sağlar.
        if (stitchIndex >= 0 && stitchIndex < splineA.Spline.Count)
        {
            splineA.Spline.SetTangentMode(stitchIndex, TangentMode.AutoSmooth);
        }

        Debug.Log("Spline'lar başarıyla birleştirildi ve pürüzsüzleştirildi!");
    }
}