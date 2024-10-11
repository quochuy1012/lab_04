using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLSinhVien.Model;

namespace QLSinhVien
{
    public partial class Form1 : Form
    {
        private readonly StudentContextDB context = new StudentContextDB();
        public Form1()
        {
            InitializeComponent();
        }

        private void FalcultyLoad(List<Faculty> faculty)
        {
            cbb_Nganh.DataSource = faculty;
            cbb_Nganh.DisplayMember = "FacultyName";
            cbb_Nganh.ValueMember = "FacultyID";
        }

        private void StudentLoad(List<Student> students)
        {
            dtgv_ThongTin.Rows.Clear();

            foreach (var item in students)
            {
                int index = dtgv_ThongTin.Rows.Add();

                dtgv_ThongTin.Rows[index].Cells[0].Value = item.StudentID;
                dtgv_ThongTin.Rows[index].Cells[1].Value = item.FullName;
                dtgv_ThongTin.Rows[index].Cells[3].Value = item.AverageScore;
                dtgv_ThongTin.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
            }
        }


        private void ResetControl()
        {
            txb_MSSV.Clear();
            txb_HoTen.Clear();
            cbb_Nganh.SelectedIndex = 1;
            txb_Diem.Clear();
            dtgv_ThongTin.ClearSelection();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var faculty = context.Faculties.ToList();
            FalcultyLoad(faculty);

            var student = context.Students.ToList();
            StudentLoad(student);

            ResetControl();

        }

        private void ThongBaoTrong()
        {
            MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool KiemTraRangBuoc()
        {
            if (String.IsNullOrWhiteSpace(txb_MSSV.Text))
            {
                ThongBaoTrong();
                txb_MSSV.Focus();
                return false;
            }

            if (String.IsNullOrWhiteSpace(txb_HoTen.Text))
            {
                ThongBaoTrong();
                txb_HoTen.Focus();
                return false;
            }

            if (String.IsNullOrWhiteSpace(txb_Diem.Text))
            {
                ThongBaoTrong();
                txb_Diem.Focus();
                return false;
            }

            return true;

        }


        private void dtgv_ThongTin_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dtgv_ThongTin.Rows[e.RowIndex];

                txb_MSSV.Text = row.Cells[0].Value.ToString();

                txb_HoTen.Text = row.Cells[1].Value.ToString();

                cbb_Nganh.Text = row.Cells[2].Value.ToString();

                txb_Diem.Text = row.Cells[3].Value.ToString();

            }

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraRangBuoc()) { return; }

            String maSV = txb_MSSV.Text;

            var timSV = context.Students.Find(maSV);

            //Hiển thị lỗi khi nhân viên đã tốn tại
            if (timSV != null)
            {
                MessageBox.Show("Mã sinh viên đã tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; //Thoát luôn hàm, không thực hiện chức năng thêm
            }

            var student = new Student
            {
                StudentID = txb_MSSV.Text,
                FullName = txb_HoTen.Text,                    
                AverageScore = float.Parse(txb_Diem.Text),
                FacultyID = (cbb_Nganh.SelectedItem as Faculty).FacultyID
            };
                
             // Thêm vào cơ sở dữ liệu
             context.Students.Add(student);
             context.SaveChanges();
                
             // Thông báo thành công
             MessageBox.Show("Thêm mới dữ liệu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
             // Cập nhật lại danh sách nhân viên và reset controls
             StudentLoad(context.Students.ToList());
             ResetControl();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (!KiemTraRangBuoc()) return;

                String maSV = txb_MSSV.Text;

                var timSV = context.Students.Find(maSV);

                if (timSV != null)
                {

                    timSV.StudentID = txb_MSSV.Text;
                    timSV.FullName = txb_HoTen.Text;
                    timSV.FacultyID = (cbb_Nganh.SelectedItem as Faculty).FacultyID;
                    timSV.AverageScore = float.Parse(txb_Diem.Text);
                    
                }

                context.SaveChanges ();

                MessageBox.Show("Cập nhật dữ liệu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                StudentLoad(context.Students.ToList());
                ResetControl();
            }
            catch
            {
                MessageBox.Show("Không tìm thấy MSSV cần sửa", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!KiemTraRangBuoc()) return;

                String maSV = txb_MSSV.Text;

                var timSV = context.Students.Find(maSV);

                if (timSV != null)
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa dữ liệu này không?", "Xác nhận xóa",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        // Thực hiện xóa dữ liệu 
                        context.Students.Remove(timSV);
                        MessageBox.Show("Dữ liệu đã được xóa thành công!");
                    }
                    else
                    {
                        // Không làm gì cả nếu người dùng chọn No
                        MessageBox.Show("Hành động xóa đã bị hủy bỏ.");
                        return;
                    }
                }

                context.SaveChanges();                

                StudentLoad(context.Students.ToList());
                ResetControl();
            }
            catch
            {
                MessageBox.Show("Không tìm thấy MSSV cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
