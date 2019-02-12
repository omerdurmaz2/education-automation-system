﻿using System;
using System.Data;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
namespace sp
{
    public partial class SinavProgrami : Tasarim
    {
        #region Yapıcı Metod ve Form_Load

        public SinavProgrami()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 0, 0)); // border radius  
            yToolStripMenuItem.Visible = false; // normal boyuta getir butonu kapalı
                                                //this.WindowState = FormWindowState.Maximized;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            Height = Screen.PrimaryScreen.WorkingArea.Height;
            baslikhizala();
            DonemBelirle();
        }
        private void SinavProgrami_Load(object sender, EventArgs e)
        {
            //if (Login.Session)
            //{
            //Filtrelerin Basıldığı Yer
            OgretimGorevlileriListele();
            FiltreTarihBas();
            FiltreSaatBas();
            FiltreOgretimGorevlisiBas();
            FiltreBolumKoduAdıBas();
            //-----

            //    Listele();
            //}
            //else
            //{
            //    this.BeginInvoke(new MethodInvoker(this.Close));// formu zorla kapatma yolu
            //}

        }
        #endregion

        #region Tasarım İçin Yapılmış Değişiklikler
        #region Köşelerin Yuvarlanması

        //Köşelerin Yuvarlanması 
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
                int nLeftRect,     // x-coordinate of upper-left corner
                int nTopRect,      // y-coordinate of upper-left corner
                int nRightRect,    // x-coordinate of lower-right corner
                int nBottomRect,   // y-coordinate of lower-right corner
                int nWidthEllipse, // height of ellipse
                int nHeightEllipse // width of ellipse
            );



        #endregion

        #endregion

        #region Dışarıda Tanımlananlar
        MySqlDataReader dr; // sorgu methodu için tablo okumaya yarayan class
        VeritabaniIslemler islemler = new VeritabaniIslemler(); // veritabanı classına giderek yapmak istediğimiz işleme göre kolaylıklar sağlıyor

        DataTable dt = new DataTable(); // veritabanından getirilen tabloların geçici olarak tutulduğu yer

        string komut = "";  //veritabanı komutlarının tutulduğu yer
        string mesaj = "";  //Hata ve bildirim durumlarında mesajların kaydedildiği yer
        string donem = "";  //dönemim belirlenip içine atıldığı değişken


        #endregion

        #region Filtre İşlemleri
        #region Tarihin Basıldığı Yer
        public void FiltreTarihBas()
        {
            try
            {
                cmbfiltretarih.Items.Clear();
                DateTime tarih = DateTime.Now;
                komut = "select * from sinavtarihleri order by tarih asc;";
                dr = islemler.Oku(komut);
                while (dr.Read())
                {
                    tarih = Convert.ToDateTime(dr.GetString("tarih"));
                    cmbfiltretarih.Items.Add(tarih);
                }
                islemler.Kapat();
            }
            catch (Exception err)
            {
                MessageBox.Show("Filtre Tarihler Listelenirken Hata! Hata Kodu:" + err);
            }

        }
        #endregion

        #region Saatin Basıldığı Yer
        public void FiltreSaatBas()
        {
            try
            {
                cmbfiltresaat.Items.Clear();
                komut = "select saat from sinavsaatleri order by saat asc";
                dr = islemler.Oku(komut);
                while (dr.Read())
                {
                        cmbfiltresaat.Items.Add(dr.GetString("saat"));
                }
                islemler.Kapat();
            }
            catch (Exception err)
            {
                MessageBox.Show("Filtre Saatler Listelenirken Hata! Hata Kodu:" + err);
            }
        }
        #endregion

        #region Öğretim Görevlilerinin Basıldığı Yer
        public void FiltreOgretimGorevlisiBas()
        {
            try
            {
                cmbfiltreogretimgorevlisi.Items.Clear();
                komut = "select * from ogretimelemani";
                dr = islemler.Oku(komut);
                while (dr.Read())
                {
                    cmbfiltreogretimgorevlisi.Items.Add(dr.GetString("unvan")+" "+ dr.GetString("Ad_Soyad"));
                }
                islemler.Kapat();
            }
            catch (Exception err)
            {
                MessageBox.Show("Filtre Öğretim Görevlisi Listelenirken Hata! Hata Kodu:" + err);
            }
        }
        #endregion

        #region Bölüm Kodunun ve Adının Basıldığı Yer
        public void FiltreBolumKoduAdıBas()
        {
            try
            {
                cmbfiltrebolumid.Items.Clear();
                cmbfiltrebolumadi.Items.Clear();
                cmbfiltrebolumkodu.Items.Clear();

                komut = "select * from bolumler";
                dr = islemler.Oku(komut);
                while (dr.Read())
                {
                    cmbfiltrebolumadi.Items.Add(dr.GetString("bolum_adi"));
                    cmbfiltrebolumkodu.Items.Add(dr.GetString("bolum_kodu"));
                    cmbfiltrebolumid.Items.Add(dr.GetString("id"));
                }
                islemler.Kapat();
            }
            catch (Exception err)
            {
                MessageBox.Show("Filtre Öğretim Görevlisi Listelenirken Hata! Hata Kodu:" + err);
            }
        }

        //bölüm değiştiğinde id si de değişecek
        private void cmbfiltrebolumadi_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbfiltrebolumid.SelectedIndex = cmbfiltrebolumadi.SelectedIndex;
        }

        #endregion

        #region Filtrelerin Temizlendiği Yer
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cmbfiltrebolumadi.SelectedIndex = -1;
            cmbfiltrebolumadi.Text = "Bölüm Adı:";

            cmbfiltrebolumkodu.SelectedIndex = -1;
            cmbfiltrebolumkodu.Text = "Bölüm Kodu:";

            cmbfiltreogretimsekli.SelectedIndex = -1;
            cmbfiltreogretimsekli.Text = "Öğretim Şekli";

            cmbfiltreogretimgorevlisi.SelectedIndex = -1;
            cmbfiltreogretimgorevlisi.Text = "Öğretim Görevlisi:";

            cmbfiltretarih.SelectedIndex = -1;
            cmbfiltretarih.Text = "Tarih:";

            cmbfiltresaat.SelectedIndex = -1;
            cmbfiltresaat.Text = "Saat";




        }
        #endregion

        #endregion

        #region Öğretim Elemanları Listelendiği Yer
        public void OgretimGorevlileriListele()
        {
            komut = "select unvan as 'Ünvanı', Ad_Soyad as 'Ad Soyad',Kendi_Sinav_Sayisi as 'Kendi Sınav Sayısı',Gozetmenlik_Sayisi as 'Gözetmenlik Sayısı' from ogretimelemani";
            dataGridView2.Rows.Clear();
            if (islemler.Al(komut) != null)
            {
                dataGridView2.DataSource = islemler.Al(komut);
            }
        }
        #endregion

        #region Dönem Belirleme
        //Bu sayede kullanıcı güz dönemi girdiğine güz dönemindeki veriler listelenecek ve kaydedilecektir. Aynı şekilde bahar dönemi de              
        public void DonemBelirle()
        {
            DateTime donemtarihi = new DateTime(DateTime.Now.Year, 1, 30);
            if (DateTime.Now < donemtarihi) donem = "guz";
            else donem = "bahar";

        }
        #endregion

    }
}