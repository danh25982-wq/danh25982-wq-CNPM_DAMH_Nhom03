using System;
using System.ComponentModel.DataAnnotations;

namespace CNPM_DeMo.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(255)]
        public string MatKhauMaHoa { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        public SystemRole VaiTro { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}