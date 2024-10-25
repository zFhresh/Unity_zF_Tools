### Unity zF Tools
İçindekiler


- **Texture Combine** : 4 farklı siyah beyaz texture'u RGBA kanalları arasında combine eden bir araç kullanıcının tercihine göre işlemi CPU Veya GPU (Compute shader) üzerinden yapabilir.
- **Dosya oluşturucu** : Gerekli klasörleri otomatik olarak oluşturan ve isteğe bağlı olarak konfigüre edebileceğimiz bir araçtır. Dosya düzeninizi kaydedebilir veya hazır düzenlerden birini kullanabilirsiniz.
- **Mesafe hesaplayıcı** : Seçili objelerin merkez noktalarına olan uzaklıkları belirten bir araçtır. (Ctrl + T) kısayolu ile açılıp kapatılabilir.
- **Mesh Analizi** : Referans olarak verilen objenin üçgen (tris) miktarını sayar ve optimizasyon gerekip gerekmediğini belirler. <br>Sınırlar şu şekildedir:<br><br>
      Güvenli tris miktarı: ≤ 10,000; optimizasyon gerekmez <br>
  Dikkat edilmesi gereken tris miktarı: 10,000 < tris < 50,000; optimizasyon gerekebilir <br>
  Tehlikeli ve optimize edilmesi gereken tris miktarı: ≥ 50,000; optimize edilmesi gereklidir
