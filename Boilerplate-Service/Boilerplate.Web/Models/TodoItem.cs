using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Web.Models
{
    /// <summary>
    /// Todo item class
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Identity
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// is complete
        /// </summary>
        [DefaultValue(false)]
        public bool IsComplete { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [StringLength(100)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Created date
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; } = DateTime.Now;

        /// <summary>
        /// Updated date
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? Updated { get; set; } = null;

        /// <summary>
        /// Deleted date
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? Deleted { get; set; } = null;

        public string? Secret { get; set; }
    }
}
