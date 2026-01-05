using System.ComponentModel.DataAnnotations;

namespace TandemDB.Models
{
    public class M06_CompuondClass
    {
        [Key]
        public int ID { get; set; }
        public int M01ID { get; set; }
        public string CompuondClass { get; set; }
        public bool IsActive { get; set; }

        public virtual PlantMetabolites PlantMetabolites { get; set; }
    }
}