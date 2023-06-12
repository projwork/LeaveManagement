using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveTypeService : BaseHttpService, ILeaveTypeService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IClient _httpclient;

        public LeaveTypeService(IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._httpclient = httpclient;
        }

        public async Task<Response<int>> CreateLeaveType(CreateLeaveTypeVM leaveType)
        {
            try
            {
                var response = new Response<int>();
                CreateLeaveTypeDto createLeaveType = new CreateLeaveTypeDto()
                {
                    Name = leaveType.Name,
                    DefaultDays = leaveType.DefaultDays
                };

                AddBearerToken();
                var apiResponse = await _client.LeaveTypesPOSTAsync(createLeaveType);
                if (apiResponse.Success)
                {
                    response.Data = apiResponse.Id;
                    response.Success = true;
                }
                else
                {
                    foreach (var error in apiResponse.Errors)
                    {
                        response.ValidationErrors += error + Environment.NewLine;
                    }
                }
                return response;
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

        public async Task<Response<int>> DeleteLeaveType(int id)
        {
            try
            {
                AddBearerToken();
                await _client.LeaveTypesDELETEAsync(id);
                return new Response<int>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

        public async Task<LeaveTypeVM> GetLeaveTypeDetails(int id)
        {
            AddBearerToken();
            var leaveType = await _client.LeaveTypesGETAsync(id);
            return new LeaveTypeVM()
            {
                Name = leaveType.Name,
                DefaultDays = leaveType.DefaultDays
            };
        }

        public async Task<List<LeaveTypeVM>> GetLeaveTypes()
        {
            AddBearerToken();
            var leaveTypes = await _client.LeaveTypesAllAsync();
            var leaveTypesVM = new List<LeaveTypeVM>();

            foreach (var leaveTypeDto in leaveTypes)
            {
                var leaveTypeVM = new LeaveTypeVM()
                {
                    Id = leaveTypeDto.Id,
                    Name = leaveTypeDto.Name,
                    DefaultDays = leaveTypeDto.DefaultDays
                };
                leaveTypesVM.Add(leaveTypeVM);
            }
            return leaveTypesVM;
        }

        public async Task<Response<int>> UpdateLeaveType(int id, LeaveTypeVM leaveType)
        {
            try
            {
                LeaveTypeDto leaveTypeDto = new LeaveTypeDto()
                {
                    Name = leaveType.Name,
                    DefaultDays = leaveType.DefaultDays
                };

                AddBearerToken();
                await _client.LeaveTypesPUTAsync(id.ToString(), leaveTypeDto);
                return new Response<int>() { Success = true };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

    }
}
