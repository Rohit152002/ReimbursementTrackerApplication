using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class ExpenseCategoryProfile:Profile
    {
        public ExpenseCategoryProfile()
        {
            CreateMap<ExpenseCategory, CreateCategoryDTO>();
            CreateMap<CreateCategoryDTO, ExpenseCategory>();
            CreateMap<ExpenseCategoryDTO, ExpenseCategory>();
            CreateMap<ExpenseCategory,ExpenseCategoryDTO>();

        }
    }
}
