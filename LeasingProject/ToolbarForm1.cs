using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.SqlClient;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.About;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Extensions;

namespace LeasingProject
{
    public partial class ToolbarForm1 : DevExpress.XtraEditors.XtraForm
    {
        public ToolbarForm1()
        {
            InitializeComponent();
        }

        private FormDuzenle formDuzenle;
        private void ButtonNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFormDuzenle();
        }
        private void FormDuzenle_FormClosed(object sender, FormClosedEventArgs e)
        {
            bar1.Visible = true;
            formDuzenle = null;
            SozlesmeListele();
            gridView1.BestFitColumns();
        }

        KOZAEntities db;
        private void ToolbarForm1_Load(object sender, EventArgs e)
        {
            SozlesmeListele();
            gridView1.BestFitColumns();
        }

        private void OpenFormDuzenle(FF_LeasingSozlesme selectedModel = null, string acilmaYontemi = null)
        {
            if (formDuzenle == null)
            {
                foreach (Control ctl in this.Controls)
                {
                    if ((ctl) is MdiClient)
                    {
                        ctl.BackColor = Color.White;
                    }
                }
                bar1.Visible = false;
                if (selectedModel == null && acilmaYontemi == null)
                {
                    formDuzenle = new FormDuzenle();
                    formDuzenle.FormClosed += new FormClosedEventHandler(FormDuzenle_FormClosed);
                    formDuzenle.MdiParent = this;
                    gridControl1.Visible = false;
                    formDuzenle.Show();
                }
                else
                {
                    formDuzenle = new FormDuzenle(selectedModel, acilmaYontemi);
                    formDuzenle.FormClosed += new FormClosedEventHandler(FormDuzenle_FormClosed);
                    formDuzenle.MdiParent = this;
                    gridControl1.Visible = false;
                    formDuzenle.StartPosition = FormStartPosition.CenterScreen;
                    formDuzenle.Show();
                }
            }
        }

        private void GridControl1_DoubleClick(object sender, EventArgs e)
        {
            var selectedModel = GetRowLeasingSozlesme();
            OpenFormDuzenle(selectedModel, "DoubleClick");
        }
        private FF_LeasingSozlesme GetRowLeasingSozlesme()
        {
            int row = gridView1.GetSelectedRows()[0];

            if (row >= 0)
            {
                return (gridView1.GetRow(gridView1.GetSelectedRows()[0]) as FF_LeasingSozlesme);
            }
            else
            {
                return null;
            }
        }

        public void SozlesmeListele()
        {
            gridControl1.Visible = true;
            using (db = new KOZAEntities())
            {
                try
                {
                    if (db.Database.Exists())
                    {
                        var SozlesmeListesi = db.FF_LeasingSozlesme.ToList();
                        gridControl1.DataSource = SozlesmeListesi;

                        gridView1.Columns["SozlesmeId"].Visible = false;
                        gridView1.Columns["FF_LeasingVade"].Visible = false;

                        RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
                        edit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                        edit.Mask.EditMask = "n2";
                        edit.Mask.UseMaskAsDisplayFormat = true;
                        gridView1.Columns["SozlesmeTutar"].ColumnEdit = edit;
                    }
                    else
                    {
                        string mesaj = "Veritabanına Bağlantı Sağlanamadı!";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        DialogResult result = MessageBox.Show(mesaj, "Bağlantı Hatası", buttons, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SozlesmeSil(FF_LeasingSozlesme silinecekKayit)
        {
            db = new KOZAEntities();
            if (silinecekKayit != null)
            {
                var sil = db.FF_LeasingSozlesme.Find(silinecekKayit.SozlesmeId);
                db.FF_LeasingSozlesme.Remove(sil);

                var silinecekVade = (from v in db.FF_LeasingVade
                                     where v.SozlesmeId == silinecekKayit.SozlesmeId
                                     select v).ToList();

                foreach (var vade in silinecekVade)
                {
                    db.FF_LeasingVade.Remove(vade);
                }

                db.SaveChanges();
                SozlesmeListele();
            }
            else
            {

            }
        }

        private void GridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            else
            {
                var row = gridView1.FocusedRowHandle;
                var focusRowView = gridView1.GetFocusedRow();
                if (focusRowView == null)
                {
                    return;
                }
                else
                {
                    if (row >= 0)
                    {
                        popupMenu1.ShowPopup(barManager1, new Point(MousePosition.X, MousePosition.Y));
                    }
                    else
                    {
                        popupMenu1.HidePopup();
                    }
                }
            }
        }

        private void PopupButtonDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = GetRowLeasingSozlesme();
            if (selected != null)
            {
                SozlesmeSil(selected);
            }
        }

        private void PopupButtonNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFormDuzenle();
        }

        private void PopupButtonEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedModel = GetRowLeasingSozlesme();
            OpenFormDuzenle(selectedModel, "RightClick");
        }

        private void ToolbarForm1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.CenterToScreen();
            }
            if (formDuzenle != null)
            {
                formDuzenle.Location = new Point(((this.ClientSize.Width - formDuzenle.Width)) / 2, (this.ClientSize.Height - formDuzenle.Height) / 2);
            }
        }
    }
}