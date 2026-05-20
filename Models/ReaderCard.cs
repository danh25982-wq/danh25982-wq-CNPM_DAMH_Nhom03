using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CNPM_DeMo.Models
{
    public class ReaderCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string MaThe { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(255)]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public DateTime NgayCap { get; set; }

        [Required]
        public DateTime NgayHetHan { get; set; }

        // Một độc giả có thể có nhiều phiếu mượn
        public virtual ICollection<BorrowTicket> BorrowTickets { get; set; }
    }
}