using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CNPM_DeMo.Models
{
    public class BorrowTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string MaPhieu { get; set; }

        [Required]
        public int ReaderCardId { get; set; }
        public virtual ReaderCard ReaderCard { get; set; }

        [Required]
        public DateTime NgayMuon { get; set; }

        [Required]
        public DateTime NgayPhaiTra { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        // Một phiếu mượn chứa nhiều cuốn sách
        public virtual ICollection<BorrowDetail> BorrowDetails { get; set; }
    }
}