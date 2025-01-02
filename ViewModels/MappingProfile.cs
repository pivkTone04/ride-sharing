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


            CreateMap<VehicleCreateViewModel, Vehicle>();
            CreateMap<RideRequestCreateViewModel, RideRequest>();
            CreateMap<RideRequest, RideRequestDetailsViewModel>();
            CreateMap<RideRequestEditViewModel, RideRequest>();
        }
    }
}
