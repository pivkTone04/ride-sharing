using AutoMapper;
using RideSharing.Models;
using RideSharing.ViewModels;

namespace RideSharing.ViewModels
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VehicleCreateViewModel, Vehicle>();
            CreateMap<Vehicle, VehicleEditViewModel>();
            CreateMap<VehicleEditViewModel, Vehicle>();

            CreateMap<RideCreateViewModel, Ride>();
            CreateMap<Ride, RideEditViewModel>();
            CreateMap<RideEditViewModel, Ride>();


            CreateMap<RideRequestCreateViewModel, RideRequest>();
            CreateMap<RideRequest, RideRequestEditViewModel>();
            CreateMap<RideRequestEditViewModel, RideRequest>();
        }
    }
}
