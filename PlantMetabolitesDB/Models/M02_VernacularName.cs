using System.ComponentModel.DataAnnotations;

namespace TandemDB.Models
{
    public class M02_VernacularName
    {
        [Key]
        public int ID { get; set; }
        public int M01ID { get; set; }
        public string VernacularName { get; set; }
        public bool IsActive { get; set; }

        public virtual PlantMetabolites PlantMetabolites { get; set; }
    }
}