# # DemoCloud — Talep ve Onay Yönetim Sistemi

DemoCloud müşterilerinin sistem üzerinden talep oluşturabildiği, yetkili ve yönetici rollerinin bu talepleri inceleyip onaylayabildiği/reddedebildiği, rol bazlı bir Talep ve Onay Yönetim Sistemi.

## Proje Mimarisi

Sistem iki bağımsız katmandan oluşur:
- **Backend:** ASP.NET Core 8 Web API — sadece JSON döndüren, bağımsız bir REST API
- **Frontend:** Saf HTML/CSS/JavaScript — API'ye HTTP istekleriyle bağlanan, backend'den tamamen ayrık bir istemci

Bu ayrım bilinçli bir tercih: API bağımsız olduğu için ileride aynı backend'e bir mobil veya masaüstü uygulama da bağlanabilir.

## Kullanılan Teknolojiler

- **Backend:** ASP.NET Core 8, Entity Framework Core 8, MSSQL Server
- **Kimlik Doğrulama:** JWT (JSON Web Token), BCrypt (şifre hash'leme)
- **Veritabanı:** Trigger, Stored Procedure, View
- **Frontend:** HTML5, CSS3 (özgün tasarım sistemi), Vanilla JavaScript, Chart.js
- **Dokümantasyon:** Swagger / OpenAPI
- **Konteynerleştirme:** Docker

## Roller ve Yetkiler

| Rol | Yetkiler |
|---|---|
| **Müşteri** | Talep oluşturur, sadece kendi firmasının taleplerini görür |
| **Yetkili** | Tüm talepleri görür, onaylar/reddeder, not ekler |
| **Yönetici** | Yetkili'nin tüm yetkileri + dashboard, KPI raporları |

## Veritabanı Şeması

8 tablo, foreign key ilişkileriyle normalize edilmiştir:

- `Roller` — sistem rolleri
- `Musteriler` — firma bilgileri
- `Kullanicilar` — sistem kullanıcıları
- `TalepTipleri` — talep kategorileri (VPN, Sunucu, Hosting, Lisans vb.)
- `TalepDurumlari` — talep durumları (Beklemede, Onaylandı, Reddedildi vb.)
- `Talepler` — ana talep kaydı
- `TalepNotlari` — taleplere eklenen notlar
- `TalepGecmisi` — denetim kaydı (trigger ile otomatik oluşturulur)

## Güvenlik

- Şifreler **BCrypt** ile hash'lenerek saklanır (salt + bilerek yavaş algoritma)
- Kimlik doğrulama **JWT** ile sağlanır; her istek, imzalı token ile doğrulanır
- **Rol bazlı yetkilendirme** (`[Authorize(Roles=...)]`) ile her endpoint sadece yetkili rollere açık
- **Veri izolasyonu**: Bir müşteri sadece kendi firmasının verilerine erişebilir — bu, `MusteriId`'nin istemciden değil, JWT token'dan alınmasıyla garanti altına alınmıştır

## Veritabanı Seviyesinde İleri Teknikler

- **Trigger** (`TR_Talepler_DurumDegisti`): Bir talep onaylandığında/reddedildiğinde `TalepGecmisi` tablosuna otomatik kayıt atar — bu sayede API dışından yapılan bir güncelleme olsa bile denetim izi garantilenir.
- **Stored Procedure**: `SP_AylikTalepSayisi` ve `SP_EnAktifMusteriler` — dashboard raporlama sorguları için.
- **View**: `VW_TalepDetay` — raporlama sorgularında tekrar eden JOIN'leri tek bir görünümde toplar.

## API Endpoint'leri

### Auth
- `POST /api/Auth/login` — giriş, JWT token döner

### Talep
- `POST /api/Talep` — talep oluştur (Müşteri)
- `GET /api/Talep` — talepleri listele (role göre filtreli)
- `GET /api/Talep/{id}` — talep detayı
- `PUT /api/Talep/{id}/onayla` — talebi onayla (Yetkili/Yönetici)
- `PUT /api/Talep/{id}/reddet` — talebi reddet (Yetkili/Yönetici)
- `POST /api/Talep/{id}/not-ekle` — talebe not ekle
- `GET /api/Talep/{id}/notlar` — talep notlarını listele

### Dashboard
- `GET /api/Dashboard/musteri` — müşteri istatistikleri
- `GET /api/Dashboard/yetkili` — yetkili istatistikleri
- `GET /api/Dashboard/yonetici` — yönetici KPI'ları
- `GET /api/Dashboard/aylik-talep` — aylık talep grafiği verisi
- `GET /api/Dashboard/aktif-musteriler` — en aktif müşteriler
- `GET /api/Dashboard/talep-detay-view` — View tabanlı detaylı rapor

## Kurulum

1. Repoyu klonla
2. `appsettings.json`'daki `ConnectionStrings:DefaultConnection`'ı kendi MSSQL ortamına göre ayarla
3. SQL script'lerini çalıştırarak veritabanını ve tabloları oluştur
4. Visual Studio'da projeyi aç, `F5` ile çalıştır
5. Swagger arayüzü otomatik açılır (`https://localhost:7045/swagger`)
6. `Frontend` klasöründeki `login.html`'i tarayıcıda açarak arayüzü kullan

### Demo Kullanıcılar
| Email | Şifre | Rol |
|---|---|---|
| musteri@democloud.com | 123456 | Müşteri |
| yetkili@democloud.com | 123456 | Yetkili |
| yonetici@democloud.com | 123456 | Yönetici |

## Ekran Görüntüleri
# login ekranı
<img width="1917" height="970" alt="image" src="https://github.com/user-attachments/assets/64c798bb-5756-4258-982f-2bfa7fde6866" />
# müşteri ekranı
<img width="1916" height="862" alt="image" src="https://github.com/user-attachments/assets/e553edb8-b177-4341-b129-5f85cb1ac984" />
# yetkili ekranı
<img width="1917" height="857" alt="image" src="https://github.com/user-attachments/assets/056bac07-f83c-45c9-8762-9f403ef70c73" />
# yönetici ekranı 
<img width="1917" height="845" alt="image" src="https://github.com/user-attachments/assets/4bd7d949-ca9c-4044-9e33-f3c7e39ffd92" />

## Geliştirici

Buse Yüsra — Sakarya Üniversitesi, Yazılım Mühendisliği — DemoCloud Stajı
