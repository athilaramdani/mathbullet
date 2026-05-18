# Math Bullet 🎮📐

Math Bullet adalah game *top-down shooter* 2D yang memadukan aksi tembak-tembakan dengan tantangan matematika dasar. 

Dalam game ini, pemain harus bertahan hidup dari gelombang zombie yang terus berdatangan sambil memecahkan soal matematika sederhana untuk mendapatkan peluru.

## 🎯 Fitur Utama

*   **Top-down Shooter Gameplay:** Bergerak menggunakan tombol WASD/Panah dan arahkan tembakan menggunakan kursor mouse (klik kiri).
*   **Sistem Peluru Matematika:** Peluru tidak tak terbatas! Ambil kotak "Math Ammo" yang muncul secara acak, dan jawab soal matematika (Penjumlahan, Pengurangan, atau Perkalian).
    *   ✅ **Jawaban Benar:** +20 Peluru
    *   ❌ **Jawaban Salah:** -5 Peluru
*   **Difficulty Scaling:** Semakin tinggi skor kamu, semakin banyak, cepat, dan sering zombie muncul.
*   **Power-ups:**
    *   ❤️ **Heal:** Menambah 1 nyawa (Maksimal 5).
    *   ❄️ **Freeze:** Membekukan semua zombie di layar selama 3 detik.
    *   🛡️ **Shield:** Memberikan tameng kebal selama 5 detik, menabrak zombie akan memantulkannya.
    *   ⚡ **Speed Boost:** Meningkatkan kecepatan lari pemain secara drastis selama 5 detik.

## 🛠️ Instalasi & Setup (Unity)

Game ini dibangun menggunakan Unity (2D Core). Untuk membuka dan mengedit proyek ini:

1. Clone repositori ini ke komputer lokal kamu.
2. Buka proyek ini menggunakan **Unity Editor** (Pastikan versi Unity mendukung paket TextMeshPro).
3. Setelah terbuka, jalankan menu **One-Click Setup** agar *scene* dan referensi otomatis terkonfigurasi dengan benar:
   * Klik menu di bagian atas Unity: `MathBullet` > `▶ SETUP GAME (One Click)`
4. Tekan **Play**!

## 🎧 Audio & Suara
*Proyek ini menggunakan beberapa aset suara. Pastikan file suara diletakkan dalam direktori `Assets/Audio/`:*
*   `ost1.wav` (Background Music)
*   `shoot.wav` (SFX Menembak)
*   `benar.wav` (SFX Jawaban Benar)
*   `freezesound.wav` (SFX Powerup Freeze)
*   `zombiedead1.wav` - `zombiedead4.wav` (SFX Zombie Mati)

## 🎮 Kontrol Permainan
*   **W, A, S, D** / **Panah**: Bergerak
*   **Mouse**: Mengarahkan karakter / Membidik senjata
*   **Klik Kiri**: Menembak
*   **Keyboard Angka/Numpad**: Digunakan untuk menjawab soal matematika. Tekan "Jawab" di layar atau "Enter" (jika dikonfigurasi) saat memasukkan jawaban.
