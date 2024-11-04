using AutoMapper;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Models.DTOs;

namespace ReimbursementTrackingApplication.Mapper
{
    public class UserProfile:Profile
    {
        public UserProfile() {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
