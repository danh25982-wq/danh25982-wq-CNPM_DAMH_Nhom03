using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM_DeMo.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string MaSach { get; set; }

        [Required]
        [StringLength(255)]
        public string TenSach { get; set; }

        [Required]
        [StringLength(100)]
        public string TacGia { get; set; }

        [Required]
        [StringLength(100)]
        public string TheLoai { get; set; }

        [Required]
        [StringLength(100)]
        public string NhaXuatBan { get; set; }

        [Required]
        public int NamXuatBan { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTri { get; set; }

        [StringLength(50)]
        public string ViTri { get; set; }

        [Required]
        public BookStatus TrangThai { get; set; } = BookStatus.SanSang;

        // Một cuốn sách có thể nằm trong nhiều chi tiết phiếu mượn (lịch sử)
        public virtual ICollection<BorrowDetail> BorrowDetails { get; set; }
    }
}