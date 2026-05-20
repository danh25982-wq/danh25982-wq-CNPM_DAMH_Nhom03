using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM_DeMo.Models
{
    public class BorrowDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BorrowTicketId { get; set; }
        public virtual BorrowTicket BorrowTicket { get; set; }

        [Required]
        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        public DateTime? NgayTraThucTe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienPhatQuaHan { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienPhatHuHai { get; set; } = 0;

        public ReturnCondition? TinhTrangKhiTra { get; set; }

        [Required]
        public bool DaTra { get; set; } = false;
    }
}