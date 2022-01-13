using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeasingProject
{
    public partial class FormDuzenle : Form
    {
        private FF_LeasingSozlesme _LeasingModel;
        private string _AcilmaYontemi;
        KOZAEntities db;
        List<string> paraBirimi = new List<string>() { "TL", "USD", "EUR" };

        public FormDuzenle()
        {
            InitializeComponent();
        }
        public FormDuzenle(FF_LeasingSozlesme leasingModel, string acilmaYontemi)
        {
            _LeasingModel = leasingModel;
            _AcilmaYontemi = acilmaYontemi;
            InitializeComponent();
        }

        private void FormDuzenle_Load(object sender, EventArgs e)
        {
            if (_LeasingModel != null)
            {

                if (_AcilmaYontemi == "DoubleClick")
                {
                    SozlesmeDetay(_LeasingModel);
                    this.Text = "Lease Detail";
                    groupSozlesme.Enabled = false;
                    bar1.Visible = false;
                    gridView1.Columns["VadeTutar"].OptionsColumn.ReadOnly = true;
                }
                else if (_AcilmaYontemi == "RightClick")
                {
                    SozlesmeDetay(_LeasingModel);

                    this.Text = "Lease Detail";

                    lookUpParaBirim.Properties.DataSource = paraBirimi;

                    db = new KOZAEntities();
                    var TDR_Ad = (from tedarikci in db.tyTDR_Tedarikci
                                  where tedarikci.TDR_FlagAktif == 1
                                  orderby tedarikci.TDR_AdTedarikci ascending
                                  select tedarikci.TDR_AdTedarikci).ToList();
                    lookUpTDA.Properties.DataSource = TDR_Ad;
                }
            }
            else
            {
                lookUpParaBirim.Properties.DataSource = paraBirimi;
                lookUpParaBirim.EditValue = "";

                db = new KOZAEntities();
                var TDR_Ad = (from tedarikci in db.tyTDR_Tedarikci
                              where tedarikci.TDR_FlagAktif == 1
                              orderby tedarikci.TDR_AdTedarikci ascending
                              select tedarikci.TDR_AdTedarikci).ToList();
                lookUpTDA.Properties.DataSource = TDR_Ad;

                this.Text = "New Lease";

            }

        }

        private void SozlesmeDetay(FF_LeasingSozlesme modelSozlesmeId)
        {
            db = new KOZAEntities();

            var vadeDetay = (from vade in db.FF_LeasingVade
                             where vade.SozlesmeId == modelSozlesmeId.SozlesmeId
                             select vade).ToList();

            detayGridControl.DataSource = vadeDetay;

            textId.EditValue = modelSozlesmeId.SozlesmeId;
            dateTarih.EditValue = modelSozlesmeId.SozlesmeTarihi;
            lookUpTDA.Properties.NullText = modelSozlesmeId.TedarikciAd;
            lookUpTDA.EditValue = modelSozlesmeId.TedarikciAd;
            lookUpParaBirim.EditValue = modelSozlesmeId.ParaBirimi;
            lookUpParaBirim.Properties.NullText = modelSozlesmeId.ParaBirimi;
            textTDK.EditValue = modelSozlesmeId.TedarikciKod;
            textSozlesmeNo.EditValue = modelSozlesmeId.SozlesmeNo;
            textTutar.EditValue = modelSozlesmeId.SozlesmeTutar;
            textVade.EditValue = modelSozlesmeId.Vade;
            memoAciklama.EditValue = modelSozlesmeId.Aciklama;
            textAy.EditValue = modelSozlesmeId.VadeAraligi;

            gridView1.Columns["SozlesmeId"].Visible = false;
            gridView1.Columns["FF_LeasingSozlesme"].Visible = false;

            RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
            edit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            edit.Mask.EditMask = "n2";
            edit.Mask.UseMaskAsDisplayFormat = true;
            gridView1.Columns["VadeTutar"].ColumnEdit = edit;

            gridView1.BestFitColumns();
            gridView1.Columns["SozlesmeVade"].Width = 60;
            gridView1.Columns["VadeTarihi"].Width = 50;
            gridView1.Columns["ParaBirimi"].Width = 40;
            gridView1.Columns["VadeTutar"].OptionsColumn.ReadOnly = false;
            gridView1.Columns["SozlesmeVade"].OptionsColumn.ReadOnly = true;
            gridView1.Columns["VadeTarihi"].OptionsColumn.ReadOnly = true;
            gridView1.Columns["ParaBirimi"].OptionsColumn.ReadOnly = true;
            SummaryHesapla();

        }

        private void SummaryHesapla()
        {
            gridView1.Columns["VadeTutar"].Summary.Clear();
            GridColumnSummaryItem item1 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, DevExpress.Data.SummaryMode.AllRows, "VadeTutar", "Toplam Tutar: {0:n2}");
            gridView1.Columns["VadeTutar"].Summary.Add(item1);

        }
        private void SozlesmeKaydet()
        {
            db = new KOZAEntities();
            FF_LeasingSozlesme yeniSozlesme = new FF_LeasingSozlesme();

            if (IsNullOrEmpty() == true)
            {
                string mesaj = "Lütfen boş alan bırakmayınız.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(mesaj, "Eksik Bilgi", buttons, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {

                }
            }
            else
            {
                var sozlesmeKayit = db.FF_LeasingSozlesme.Find(textId.EditValue);
                if (sozlesmeKayit != null)
                {
                    if (sozlesmeKayit.TedarikciAd != (string)lookUpTDA.EditValue || sozlesmeKayit.SozlesmeNo != (string)textSozlesmeNo.EditValue
                            || sozlesmeKayit.Aciklama != (string)memoAciklama.EditValue || sozlesmeKayit.SozlesmeTarihi != (DateTime)dateTarih.EditValue
                            || sozlesmeKayit.SozlesmeTutar != Convert.ToDouble(textTutar.EditValue) || sozlesmeKayit.ParaBirimi != (string)lookUpParaBirim.EditValue
                            || sozlesmeKayit.Vade != Convert.ToInt32(textVade.EditValue) || sozlesmeKayit.VadeAraligi != Convert.ToInt32(textAy.EditValue))
                    {
                        SozlesmeGuncelle(sozlesmeKayit);
                    }
                    else if (sozlesmeKayit.SozlesmeTarihi == (DateTime)dateTarih.EditValue && sozlesmeKayit.SozlesmeTutar == Convert.ToDouble(textTutar.EditValue) && sozlesmeKayit.ParaBirimi == (string)lookUpParaBirim.EditValue
                        && sozlesmeKayit.Vade == Convert.ToInt32(textVade.EditValue) && sozlesmeKayit.VadeAraligi == Convert.ToInt32(textAy.EditValue))
                    {
                        VadeKayitKontrol();
                    }
                }
                else
                {
                    yeniSozlesme.SozlesmeTarihi = (DateTime)dateTarih.EditValue;
                    yeniSozlesme.TedarikciKod = (string)textTDK.EditValue;
                    yeniSozlesme.TedarikciAd = (string)lookUpTDA.EditValue;
                    yeniSozlesme.SozlesmeNo = (string)textSozlesmeNo.EditValue;
                    yeniSozlesme.SozlesmeTutar = Convert.ToDouble(textTutar.EditValue);
                    yeniSozlesme.ParaBirimi = (string)lookUpParaBirim.EditValue;
                    yeniSozlesme.Vade = Convert.ToInt32(textVade.EditValue);
                    yeniSozlesme.KayitTarihi = DateTime.Now;
                    yeniSozlesme.Aciklama = (string)memoAciklama.EditValue;
                    yeniSozlesme.VadeAraligi = Convert.ToInt32(textAy.EditValue);
                    db.FF_LeasingSozlesme.Add(yeniSozlesme);
                    db.SaveChanges();

                    VadeHesapla(yeniSozlesme);
                    SozlesmeDetay(yeniSozlesme);
                }
            }
        }

        private void VadeHesapla(FF_LeasingSozlesme yeniSozlesme)
        {
            FF_LeasingVade yeniVade;
            for (int i = 1; i <= yeniSozlesme.Vade; i++)
            {
                yeniVade = new FF_LeasingVade();
                yeniVade.SozlesmeId = yeniSozlesme.SozlesmeId;
                yeniVade.SozlesmeVade = i;
                yeniVade.FF_LeasingSozlesme = yeniSozlesme;
                yeniVade.VadeTutar = yeniSozlesme.SozlesmeTutar / Convert.ToDouble(yeniSozlesme.Vade);
                yeniVade.VadeTarihi = yeniSozlesme.SozlesmeTarihi.AddMonths(yeniSozlesme.VadeAraligi * i);
                yeniVade.ParaBirimi = yeniSozlesme.ParaBirimi;
                db.FF_LeasingVade.Add(yeniVade);
            }
            db.SaveChanges();

        }
        private void VadeGuncelle(FF_LeasingSozlesme yeniSozlesme)
        {
            FF_LeasingVade yeniVade;
            for (int i = 1; i <= yeniSozlesme.Vade; i++)
            {
                double VadeTutarCellValue = Convert.ToDouble(gridView1.GetRowCellValue(i - 1, "VadeTutar"));

                yeniVade = new FF_LeasingVade();
                yeniVade.SozlesmeId = yeniSozlesme.SozlesmeId;
                yeniVade.SozlesmeVade = i;
                yeniVade.FF_LeasingSozlesme = yeniSozlesme;
                yeniVade.VadeTutar = VadeTutarCellValue;
                yeniVade.VadeTarihi = yeniSozlesme.SozlesmeTarihi.AddMonths(yeniSozlesme.VadeAraligi * i);
                yeniVade.ParaBirimi = yeniSozlesme.ParaBirimi;
                db.FF_LeasingVade.Add(yeniVade);
            }
            db.SaveChanges();

        }

        private void SozlesmeGuncelle(FF_LeasingSozlesme eskiKayit)
        {
            db = new KOZAEntities();
            if (IsNullOrEmpty() == true)
            {
                string mesaj = "Lütfen boş alan bırakmayınız.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(mesaj, "Eksik Bilgi", buttons, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {

                }
            }
            else
            {
                var yeniKayit = db.FF_LeasingSozlesme.Find(eskiKayit.SozlesmeId);
                yeniKayit.SozlesmeId = Convert.ToInt32(textId.EditValue);
                yeniKayit.SozlesmeTarihi = (DateTime)dateTarih.EditValue;
                yeniKayit.TedarikciKod = (string)textTDK.EditValue;
                yeniKayit.TedarikciAd = (string)lookUpTDA.EditValue;
                yeniKayit.SozlesmeNo = (string)textSozlesmeNo.EditValue;
                yeniKayit.SozlesmeTutar = Convert.ToDouble(textTutar.EditValue);
                yeniKayit.ParaBirimi = (string)lookUpParaBirim.EditValue;
                yeniKayit.Vade = Convert.ToInt32(textVade.EditValue);
                yeniKayit.KayitTarihi = DateTime.Now;
                yeniKayit.Aciklama = (string)memoAciklama.EditValue;
                yeniKayit.VadeAraligi = Convert.ToInt32(textAy.EditValue);

                var vadeDetay = (from vade in db.FF_LeasingVade
                                 where vade.SozlesmeId == eskiKayit.SozlesmeId
                                 select vade).ToList();

                foreach (var vade in vadeDetay)
                {
                    db.FF_LeasingVade.Remove(vade);
                }

                VadeGuncelle(yeniKayit);

                db.SaveChanges();
                SozlesmeDetay(yeniKayit);

            }
        }

        private void LookUpTDK_EditValueChanged(object sender, EventArgs e)
        {
            db = new KOZAEntities();
            var TDR_Kod = (from tedarikci in db.tyTDR_Tedarikci
                           where tedarikci.TDR_AdTedarikci == (string)lookUpTDA.EditValue
                           select tedarikci.TDR_KodTederikci).FirstOrDefault();
            textTDK.EditValue = TDR_Kod;
        }

        private void FormDuzenle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_LeasingModel != null)
            {
                db = new KOZAEntities();
                double toplam = Convert.ToDouble(gridView1.Columns["VadeTutar"].SummaryItem.SummaryValue);
                double yuvarlanmisToplam = Math.Round(toplam, 2);
                var selectedRow = GetRowLeasingVade();

                if (selectedRow != null)
                {
                    var SozlesmeBilgisi = (from s in db.FF_LeasingSozlesme
                                           where s.SozlesmeId == selectedRow.SozlesmeId
                                           select s).FirstOrDefault();

                    if (SozlesmeBilgisi.SozlesmeTutar != yuvarlanmisToplam)
                    {
                        string mesaj = "Sözleşme tutarı toplam tutar ile eşit değil!";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        DialogResult result = MessageBox.Show(mesaj, "Tutar Eksik", buttons, MessageBoxIcon.Warning);
                        if (result == DialogResult.OK)
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                if (textId.EditValue == null && dateTarih.EditValue == null && textTDK.EditValue == null && lookUpTDA.EditValue == "" && textSozlesmeNo.EditValue == null && Convert.ToInt32(textTutar.EditValue) == 0
                                && lookUpParaBirim.EditValue == "" && textVade.EditValue == null && textAy.EditValue == null)
                {
                    e.Cancel = false;

                }
                else
                {
                    if (textId.EditValue == null)
                    {
                        string mesaj = "Sözleşmeyi kaydetmediniz. Kaydetmek ister misiniz?";
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result = MessageBox.Show(mesaj, "Kayıt Gerçekleşmedi", buttons, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SozlesmeKaydet();

                            e.Cancel = true;
                        }
                        else
                        {
                            e.Cancel = false;
                        }
                    }
                }
            }
        }

        private void LargeButtonYeniKayit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            textId.EditValue = null;
            dateTarih.EditValue = null;
            lookUpTDA.EditValue = "";
            lookUpTDA.Properties.NullText = "";
            lookUpParaBirim.EditValue = "";
            textTDK.EditValue = null;
            textSozlesmeNo.EditValue = null;
            textTutar.EditValue = 0;
            textVade.EditValue = null;
            memoAciklama.EditValue = "";
            textAy.EditValue = null;
            detayGridControl.DataSource = null;
            _LeasingModel = null;
        }

        private void LargeButtonKaydet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SozlesmeKaydet();
        }

        private bool IsNullOrEmpty()
        {
            if (dateTarih.EditValue == null || textTDK.EditValue == null || lookUpTDA.EditValue == null || textSozlesmeNo.EditValue == null || textTutar.EditValue == null
                || lookUpParaBirim.EditValue == null || textVade.EditValue == null || textAy.EditValue == null || dateTarih.Text == "" || textTDK.Text == "" || lookUpTDA.Text == ""
                || textSozlesmeNo.Text == "" || textTutar.Text == "" || lookUpParaBirim.Text == "" || textVade.Text == "" || textAy.Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            db = new KOZAEntities();
            var selectedRow = GetRowLeasingVade();

            if (selectedRow != null && selectedRow.VadeTutar != null)
            {
                var yeniVade = (from v in db.FF_LeasingVade
                                where v.SozlesmeId == selectedRow.SozlesmeId && v.SozlesmeVade == selectedRow.SozlesmeVade
                                select v).FirstOrDefault();
                yeniVade.SozlesmeId = selectedRow.SozlesmeId;
                yeniVade.SozlesmeVade = selectedRow.SozlesmeVade;
                yeniVade.VadeTarihi = selectedRow.VadeTarihi;
                yeniVade.VadeTutar = selectedRow.VadeTutar;
                yeniVade.ParaBirimi = selectedRow.ParaBirimi;

                db.SaveChanges();

                var SozlesmeBilgisi = (from s in db.FF_LeasingSozlesme
                                       where s.SozlesmeId == selectedRow.SozlesmeId
                                       select s).FirstOrDefault();

                double oncekiler = 0;
                var suankiVade = selectedRow.SozlesmeVade;
                if (suankiVade > 1)
                {
                    for (int i = 1; i < suankiVade; i++)
                    {
                        var onceki = (from v in db.FF_LeasingVade
                                      where v.SozlesmeId == selectedRow.SozlesmeId && v.SozlesmeVade == suankiVade - i
                                      select v.VadeTutar).FirstOrDefault();
                        oncekiler += onceki;
                    }
                }

                var kalan = SozlesmeBilgisi.SozlesmeTutar - yeniVade.VadeTutar - oncekiler;

                for (int i = 1; i <= SozlesmeBilgisi.Vade - selectedRow.SozlesmeVade; i++)
                {
                    if (selectedRow.SozlesmeVade != SozlesmeBilgisi.Vade) //sonuncu vade değil ise
                    {
                        var yeniTutar = (from v in db.FF_LeasingVade
                                         where v.SozlesmeId == selectedRow.SozlesmeId && v.SozlesmeVade == selectedRow.SozlesmeVade + i
                                         select v).FirstOrDefault();
                        var hesaplananVade = kalan / (SozlesmeBilgisi.Vade - selectedRow.SozlesmeVade);
                        if (hesaplananVade < 0)
                        {
                            yeniTutar.VadeTutar = 0;
                        }
                        else
                        {
                            yeniTutar.VadeTutar = Convert.ToDouble(hesaplananVade);
                        }
                        db.SaveChanges();
                    }
                }
                SozlesmeDetay(SozlesmeBilgisi);
            }
            else
            {
                string mesaj = "Lütfen boş alan bırakmayınız.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result = MessageBox.Show(mesaj, "Eksik Bilgi", buttons, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {

                }
            }
        }

        private FF_LeasingVade GetRowLeasingVade()
        {
            int row = gridView1.GetSelectedRows()[0];

            if (row >= 0)
            {
                return (gridView1.GetRow(gridView1.GetSelectedRows()[0]) as FF_LeasingVade);
            }
            else
            {
                return null;
            }
        }

        private void VadeKayitKontrol()
        {
            db = new KOZAEntities();
            double toplam = Convert.ToDouble(gridView1.Columns["VadeTutar"].SummaryItem.SummaryValue);
            double yuvarlanmisToplam = Math.Round(toplam, 2);
            var selectedRow = GetRowLeasingVade();

            if (selectedRow != null)
            {
                var SozlesmeBilgisi = (from s in db.FF_LeasingSozlesme
                                       where s.SozlesmeId == selectedRow.SozlesmeId
                                       select s).FirstOrDefault();

                if (SozlesmeBilgisi.SozlesmeTutar != yuvarlanmisToplam)
                {
                    string mesaj = "Sözleşme tutarı toplam tutar ile eşit değil!";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result = MessageBox.Show(mesaj, "Tutar Eksik", buttons, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {

                    }
                }
                else
                {
                    string mesaj = "Güncelleme başarılı.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result = MessageBox.Show(mesaj, "Başarılı", buttons, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {

                    }
                }

            }
        }

        private void FormDuzenle_LocationChanged(object sender, EventArgs e)
        {
            ////left possition
            //if (this.Left <= 0)
            //{
            //    this.Location = new Point(0, this.Location.Y);
            //    if (this.Top < 0)
            //    {
            //        this.Top = 0;
            //    }
            //    else if (this.Top > ((this.MdiParent.ClientRectangle.Height - 50) - this.Height))
            //    {
            //        this.Top = (this.MdiParent.ClientRectangle.Height - 30) - this.Height;
            //    }
            //}
            ////right
            //else if (this.Right >= this.MdiParent.ClientRectangle.Width)
            //{
            //    this.Left = (this.MdiParent.Width - 20) - this.Width;
            //}
            ////top
            //else if (this.Top < 0)
            //{
            //    this.Top = 0;
            //}
            ////bottom
            //else if (this.Top > ((this.MdiParent.ClientRectangle.Height - 50) - this.Height))
            //{
            //    this.Top = (this.MdiParent.ClientRectangle.Height - 30) - this.Height;

            //}
        }
    }
}
