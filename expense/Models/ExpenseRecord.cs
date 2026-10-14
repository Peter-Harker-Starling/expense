using expense.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace expense.Models
{
    public class ExpenseRecord
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Item { get; set; } = null!;
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [ValidateNever]
        public string UserId { get; set; } = null!;
        [ValidateNever]
        public ApplicationUser User { get; set; } = null!;
    }
}
