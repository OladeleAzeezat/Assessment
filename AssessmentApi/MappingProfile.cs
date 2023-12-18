using AssessmentApi.Models;
using AssessmentApi.TableDTO;
using AutoMapper;

namespace AssessmentApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap < Employee, EmployeeDto > ();
            CreateMap < Item, ItemDto > ();
        }
    }
}
