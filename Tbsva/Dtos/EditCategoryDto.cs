using WebShopping.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShopping.Dtos
{
    public enum ActionStatus
    {
        Update,
        Insert,
        Delete
    }

    public class EditCategoryDto
    {
        [Required]
        [RegularExpression("[1-9][0-9]*", ErrorMessage = "主類別{0}必須為正整數")]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "主類別{0}長度不能超過100")]
        public string Name { get; set; }

        [Required]       
        [EnumDataType(typeof(OpenStatus))]
        public OpenStatus Enable { get; set; }

        public List<EditSubCategoryDto> SubCategories { get; set; } = new List<EditSubCategoryDto>();
    }

    public class EditFirstCategoryDto
    {
        [Required]
        [RegularExpression("[1-9][0-9]*", ErrorMessage = "主類別{0}必須為正整數")]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "主類別{0}長度不能超過100")]
        public string Name { get; set; }
    
        public List<EditSubCategoryDto> SubCategories { get; set; } = new List<EditSubCategoryDto>();
    }

    public class EditSubCategoryDto
    {
        [Required]
        [RegularExpression("[1-9][0-9]*", ErrorMessage = "次類別{0}必須為正整數")]
        public int Id { get; set; }

        //[Required]
        //[RegularExpression("[1-9]*", ErrorMessage = "次類別{0}必須為正整數")]
        //public int Parent_Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "主類別{0}長度不能超過100")]
        public string Name { get; set; }

        [Required]      
        [EnumDataType(typeof(OpenStatus))]
        public OpenStatus Enable { get; set; }

        [Required]      
        [EnumDataType(typeof(ActionStatus))]
        public ActionStatus EditAction { get; set; }
    }
}