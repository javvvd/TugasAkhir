using UnityEngine;
using TMPro;
using Yarn.Unity; // Wajib dipanggil agar bisa berkomunikasi dengan Yarn

public class UangManager : MonoBehaviour
{
    public TextMeshProUGUI teksUang;
    private int saldo = 5000000; // Uang awal Raka

    void Start()
    {
        UpdateLayar();
    }

    // [YarnCommand] membuat fungsi ini bisa dipanggil langsung dari teks naskah Yarn!
    [YarnCommand("potong_uang")]
    public void PotongUang(int jumlahPotongan)
    {
        saldo = saldo - jumlahPotongan;
        UpdateLayar();
    }

    void UpdateLayar()
    {
        // Menampilkan angka dengan format ribuan (titik)
        teksUang.text = saldo.ToString("N0");
    }
}