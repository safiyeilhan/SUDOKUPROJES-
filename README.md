
# SUDOKU PROJESİ

Bu depo, Windows Forms tabanlı bir Sudoku uygulaması içerir. Proje .NET 10 üzerine geliştirilmiştir ve temel amaç Sudoku çözme/oynama işlevselliğini sağlayan bir masaüstü uygulamasıdır.

Önemli Not: Proje Windows Forms (WinForms) kullanır; uygulamayı Visual Studio veya uygun bir .NET masaüstü ortamında çalıştırmanız önerilir.

Gereksinimler

- .NET 10 SDK
- Microsoft Visual Studio 2026 (önerilir) veya eşdeğer bir IDE

Hızlı Başlangıç

1) Depoyu klonlayın:
   git clone https://github.com/safiyeilhan/SUDOKUPROJES-.git

2) Çözümü Visual Studio ile açın:
   SUDOKUPROJESİ.slnx dosyasını Solution Explorer ile açın ve başlatma projesini "SUDOKUPROJESİ" (Windows Forms) olarak seçin.

3) Komut satırından çalıştırma (dotnet CLI):
   dotnet build SUDOKUPROJESİ.slnx
   dotnet run --project SUDOKUPROJESİ.csproj

Proje Yapısı (ana dosyalar)

- SUDOKUPROJESİ.csproj  - Proje dosyası (başlatma projesi)
- Program.cs            - Uygulama giriş noktası (Application.Run(new Form1()))
- Form1.cs              - Ana formun kod-behind dosyası
- Form1.Designer.cs     - Form'un tasarım tanımı
- Form1.resx            - Form kaynakları

Çalıştırma

- Visual Studio kullanıyorsanız: Çözümü açın, proje olarak "SUDOKUPROJESİ" seçin ve F5 ile çalıştırın.
- Komut satırı ile: dotnet run --project SUDOKUPROJESİ.csproj

Testler

Bu depo içinde özel bir test projesi bulunmayabilir. Test projeleriniz varsa aşağıdaki komutla çalıştırabilirsiniz:
   dotnet test

Sürekli Entegrasyon (CI)

Bir örnek GitHub Actions iş akışı .github/workflows/dotnet.yml dosyasında bulunmaktadır. Bu iş akışı push ve pull request olaylarında projeyi restore, build ve test eder (varsa testleri çalıştırır).

Katkıda Bulunma

- Pull request ve issue'lar kabul edilir.
- Değişiklik yapmadan önce bir issue açıp ne yapmak istediğinizi kısaca açıklarsanız iyi olur.

Lisans

Bu projede MIT lisansı kullanılmıştır. Detaylar için LICENSE dosyasına bakın.

İletişim

Proje sahibi: safiyeilhan (GitHub)
