using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using leashApi.Models;
using leashed.Controllers.Resources;

namespace leashed.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {

            // Domain to API Resource
            CreateMap<ParkItem,ParkItemResource>();
            CreateMap<UserData, UserResource>()
                .ForMember(ur => ur.Park, opt => opt.MapFrom(u => u.Park.Id));
            CreateMap<UserData, int>()
                .ConstructUsing(u => u.Id);
            CreateMap<Dog,DogResource>();

            //API Resource to Domain
            CreateMap<DogResource,Dog>();
            CreateMap<ParkItemResource,ParkItem>();
            

        }
        
    }
}