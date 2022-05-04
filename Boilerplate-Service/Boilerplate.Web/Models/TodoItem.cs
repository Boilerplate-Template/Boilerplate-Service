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
    }
}
