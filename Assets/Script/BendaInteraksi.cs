using UnityEngine;
using Yarn.Unity;

public class BendaInteraktif : MonoBehaviour
{
    public string namaNodeYarn;
    private Collider areaKlik;

    void Start()
    {
        areaKlik = GetComponent<Collider>();
    }

    [YarnCommand("matikan_interaksi")]
    public void MatikanInteraksi()
    {
        if (areaKlik != null) areaKlik.enabled = false;
    }

    [YarnCommand("aktifkan_interaksi")]
    public void AktifkanInteraksi()
    {
        if (areaKlik != null) areaKlik.enabled = true;
    }

    // FUNGSI BARU: Mengganti target node dari dalam Yarn
    [YarnCommand("ubah_node")]
    public void UbahNode(string nodeBaru)
    {
        namaNodeYarn = nodeBaru;
    }
}