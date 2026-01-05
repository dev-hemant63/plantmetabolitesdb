using System.ComponentModel.DataAnnotations;

namespace TandemDB.Models
{
    public class M03_BiologicalActivity
    {
        [Key]
        public int ID { get; set; }
        public int M01ID { get; set; }
        public string BiologicalActivity { get; set; }
        public bool IsActive { get; set; }

        public virtual PlantMetabolites PlantMetabolites { get; set; }
    }
}