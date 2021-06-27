using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarketUygulamasi
{
    public partial class UrunForm : Form
    {
        private readonly SqlConnection con;
        public Urun Urun { get; private set; }
        public UrunForm(SqlConnection connection)
        {
            con = connection;
            InitializeComponent();
            KategorileriYükle();
            txtUrunAd.Focus();
        }

        public UrunForm(SqlConnection connection, Urun urun) : this(connection)
        {
            Urun = urun;
            txtId.Text = urun.Id.ToString();
            txtUrunAd.Text = urun.UrunAd;
            nudBirimFiyat.Value = urun.BirimFiyat;
            nudStokAdet.Value = urun.StokAdet;
            cboKategori.SelectedValue = urun.KategoriId;
            pboResim.Image = urun.ResmiGetir();
        }
        private void KategorileriYükle()
        {
            var kategoriler = new List<Kategori>();
            var cmd = new SqlCommand("SELECT Id, KategoriAd FROM Kategoriler ORDER BY KategoriAd", con);
            var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                kategoriler.Add(new Kategori()
                {
                    Id = (int)dr[0],
                    KategoriAd = (string)dr[1]
                });
            }
            dr.Close();
            cboKategori.DataSource = kategoriler;
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAd.Text.Trim();
            if (urunAd == string.Empty)
            {
                MessageBox.Show("Ürün adı belirtmediniz.");
                return;
            }
            if (Urun == null)
                Urun = new Urun();

            Urun.UrunAd = urunAd;
            Urun.KategoriId = (int)cboKategori.SelectedValue;
            Urun.BirimFiyat = nudBirimFiyat.Value;
            Urun.StokAdet = (int)nudStokAdet.Value;
            Urun.Resim = ImageToByteArray(pboResim.Image);
            if (Urun.Id == 0)
                UrunEkle(Urun);
            else
                UrunGuncelle(Urun);
            DialogResult = DialogResult.OK;
        }

        private void UrunGuncelle(Urun urun)
        {
            var cmd = new SqlCommand(
                "UPDATE Urunler " +
                "SET KategoriId = @p1, UrunAd = @p2, BirimFiyat = @p3, StokAdet = @p4, Resim = @p6 " +
                "WHERE Id = @p5", con);
            cmd.Parameters.AddWithValue("@p1", urun.KategoriId);
            cmd.Parameters.AddWithValue("@p2", urun.UrunAd);
            cmd.Parameters.AddWithValue("@p3", urun.BirimFiyat);
            cmd.Parameters.AddWithValue("@p4", urun.StokAdet);
            cmd.Parameters.AddWithValue("@p5", urun.Id);
            if (urun.Resim == null)
            {
                cmd.Parameters.Add("@p6", SqlDbType.VarBinary).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.AddWithValue("@p6", urun.Resim);
            }
            cmd.ExecuteNonQuery();
        }

        private void UrunEkle(Urun urun)
        {
            var cmd = new SqlCommand(
                     "INSERT INTO Urunler(KategoriId, UrunAd, BirimFiyat, StokAdet, Resim) " +
                      "VALUES(@p1, @p2, @p3, @p4, @p5);" +
                      "SELECT SCOPE_IDENTITY();", con);
            cmd.Parameters.AddWithValue("@p1", urun.KategoriId);
            cmd.Parameters.AddWithValue("@p2", urun.UrunAd);
            cmd.Parameters.AddWithValue("@p3", urun.BirimFiyat);
            cmd.Parameters.AddWithValue("@p4", urun.StokAdet);
            //cmd.Parameters.AddWithValue("@p5", urun.Resim);
            if (urun.Resim == null)
            {
                cmd.Parameters.Add("@p5", SqlDbType.VarBinary).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.AddWithValue("@p5", urun.Resim);
            }
            urun.Id = (int)(decimal)cmd.ExecuteScalar();
            Urun = urun;
        }

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;
            return (byte[])new ImageConverter().ConvertTo(image, typeof(byte[]));
        }

        private void btnResimSec_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdResim.ShowDialog();

            if (dr == DialogResult.OK)
            {
                pboResim.Image = Image.FromFile(ofdResim.FileName);
            }
        }

        private void btnResimSil_Click(object sender, EventArgs e)
        {
            pboResim.Image = null;
        }
    }
}
