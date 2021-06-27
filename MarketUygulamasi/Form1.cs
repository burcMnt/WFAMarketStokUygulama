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
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("server=.; database=KuzeyYeli1; uid=sa; pwd=123");
        List<Kategori> kategoriler;
        List<Urun> urunler;

        public Form1()
        {
            con.Open();
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false; // otomatik sütun oluşturmayı durdurur.
            KategorileriListele();
        }

        private void KategorileriListele()
        {
            kategoriler = new List<Kategori>();
            SqlCommand cmd = new SqlCommand("SELECT Id, KategoriAd, UstKategoriId FROM Kategoriler ORDER BY KategoriAd", con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                kategoriler.Add(new Kategori()
                {
                    Id = (int)dr[0],
                    KategoriAd = (string)dr[1],
                    UstKategoriId = dr[2] is DBNull ? null as int? : (int)dr[2]
                });
            }
            dr.Close();
            tviKategoriler.Nodes.Clear();
            KategorileriDugumOlarakEkle(tviKategoriler.Nodes, null);
        }

        /// <summary>
        /// Bu metot verilen düğüm koleksiyonuna üst kategori id'sine ait olan tüm alt kategorileri
        /// düğüm olarak (TreeNode) ekler ve aynı işlemi her bir eklenen node'un -varsa- alt 
        /// kategorileri içinde recursive olarak yapar.
        /// </summary>
        /// <param name="nodes">İçerisine düğümlerin ekleneceği düğüm koleksiyonu</param>
        /// <param name="ustKategoriId">Eklenecek kategorilerin üst kategorisinin id'si</param>

        private void KategorileriDugumOlarakEkle(TreeNodeCollection nodes, int? ustKategoriId)
        {
            foreach (Kategori kategori in kategoriler.Where(x => x.UstKategoriId == ustKategoriId))
            {
                TreeNode node = new TreeNode(kategori.KategoriAd);
                node.Tag = kategori;
                KategorileriDugumOlarakEkle(node.Nodes, kategori.Id);
                nodes.Add(node);
            }
        }
        private void UrunleriListele()
        {
            if (tviKategoriler.SelectedNode == null)
            {
                dgvUrunler.DataSource = null;
                return;
            }

            Kategori kategori = (Kategori)tviKategoriler.SelectedNode.Tag;
            int kategoriId = kategori.Id;
            urunler = new List<Urun>();
            var cmd = new SqlCommand("SELECT Id, KategoriId, UrunAd, BirimFiyat, StokAdet, Resim FROM Urunler WHERE KategoriId= @p1", con);
            cmd.Parameters.AddWithValue("@p1", kategoriId);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                urunler.Add(new Urun
                {
                    Id = (int)dr[0],
                    KategoriId = (int)dr[1],
                    UrunAd = (string)dr[2],
                    BirimFiyat = (decimal)dr[3],
                    StokAdet = (int)dr[4],
                    Resim = dr[5] is DBNull ? null : (byte[])dr[5]
                });
            }
            dr.Close();
            dgvUrunler.DataSource = urunler;
        }

        private void tviKategoriler_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UrunleriListele();
            btnKategoriEkle.Enabled = btnKategoriDuzenle.Enabled = btnKategoriSil.Enabled = tviKategoriler.SelectedNode != null;
        }

        private void dgvUrunler_SelectionChanged(object sender, EventArgs e)
        {
            btnUrunDuzenle.Enabled = btnUrunSil.Enabled = dgvUrunler.SelectedRows.Count != 0;
        }

        private void btnKategoriSil_Click(object sender, EventArgs e)
        {
            if (tviKategoriler.SelectedNode == null) return;
            DialogResult dr = MessageBox.Show("Seçili kategori ve altındaki tüm ürünler silinecektir. Onaylıyor musunuz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                Kategori kategori = (Kategori)tviKategoriler.SelectedNode.Tag;
                var cmd = new SqlCommand("DELETE FROM Kategoriler WHERE Id= @p1", con);
                cmd.Parameters.AddWithValue("@p1", kategori.Id);
                cmd.ExecuteNonQuery();
                KategorileriListele();
            }

        }

        private void btnUrunSil_Click(object sender, EventArgs e)
        {
            if (dgvUrunler.SelectedRows.Count == null) return;
            DialogResult dr = MessageBox.Show("Seçili ürün silinecektir. Onaylıyor musunuz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                Urun urun = (Urun)dgvUrunler.SelectedRows[0].DataBoundItem;
                var cmd = new SqlCommand("DELETE FROM Urunler WHERE Id=@p1", con);
                cmd.Parameters.AddWithValue("@p1", urun.Id);
                cmd.ExecuteNonQuery();
                KategorileriListele();
                KategoriyiSec(urun.KategoriId);
            }
        }

        private void KategoriyiSec(int kategoriId)
        {
            foreach (TreeNode node in tviKategoriler.Descendants())
            {
                Kategori kategori = (Kategori)node.Tag;
                if (kategori.Id == kategoriId)
                {
                    tviKategoriler.SelectedNode = node;
                    return;
                }
            }
        }

        private void btnKategoriEkle_Click(object sender, EventArgs e)
        {
            KategoriForm kForm = new KategoriForm(con);
            DialogResult dr = kForm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                KategorileriListele();
                KategoriyiSec(kForm.SonEklenenId);
            }
        }

        private void btnKategoriDuzenle_Click(object sender, EventArgs e)
        {
            if (tviKategoriler.SelectedNode == null) return;
            Kategori kategori = (Kategori)tviKategoriler.SelectedNode.Tag;
            KategoriForm kForm = new KategoriForm(con, kategori);
            DialogResult dr = kForm.ShowDialog();
            if (dr==DialogResult.OK)
            {
                KategorileriListele();
                KategoriyiSec(kategori.Id);
            }
        }

        private void dgvUrunler_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && dgvUrunler.SelectedRows.Count >0)
            {
                btnUrunSil.PerformClick();
            }
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            UrunForm uForm = new UrunForm(con);
            DialogResult dr = uForm.ShowDialog();
            if (dr==DialogResult.OK)
            {
                KategorileriListele();
                UrunSec(uForm.Urun);
            }
        }

        private void UrunSec(Urun urun)
        {
            KategoriyiSec(urun.KategoriId);

            dgvUrunler.ClearSelection();
            for (int i = 0; i < dgvUrunler.Rows.Count; i++)
            {
                DataGridViewRow row = dgvUrunler.Rows[i];
                Urun u = (Urun)row.DataBoundItem;

                if (urun.Id == u.Id)
                {
                    row.Selected = true;
                }
            }
        }

        private void btnUrunDuzenle_Click(object sender, EventArgs e)
        {
            if (dgvUrunler.SelectedRows.Count == 0) return;
            Urun urun = (Urun)dgvUrunler.SelectedRows[0].DataBoundItem;
            UrunForm uForm = new UrunForm(con, urun);
            DialogResult dr = uForm.ShowDialog();

            if (dr == DialogResult.OK)
            {
                KategorileriListele();
                UrunSec(urun);
            }
        }
    }
}
