using System.ComponentModel.DataAnnotations;

namespace CNPM_DeMo.Models
{
    public class SystemSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoTuoiToiThieu { get; set; } = 18;

        [Required]
        public int DoTuoiToiDa { get; set; } = 100;

        [Required]
        public int ThoiHanTheThang { get; set; } = 6;

        [Required]
        public int ThoiHanMuonNgay { get; set; } = 14;

        [Required]
        public int SoSachMuonToiDa { get; set; } = 5;

        [Required]
        public decimal DonGiaPhatNgay { get; set; } = 1000;
    }
}