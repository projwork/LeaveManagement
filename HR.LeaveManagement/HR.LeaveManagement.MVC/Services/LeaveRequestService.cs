using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveRequestService : BaseHttpService, ILeaveRequestService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IClient _httpclient;

        public LeaveRequestService(IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._httpclient = httpclient;
        }

        public async Task ApproveLeaveRequest(int id, bool approved)
        {
            try
            {
                var request = new ChangeLeaveRequestApprovalDto { Approved = approved, Id = id };
                await _client.ChangeapprovalAsync(id, request);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Response<int>> CreateLeaveRequest(CreateLeaveRequestVM leaveRequest)
        {
            try
            {
                var response = new Response<int>();
                CreateLeaveRequestDto createLeaveRequest = new CreateLeaveRequestDto()
                {
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    LeaveTypeId = leaveRequest.LeaveTypeId,
                    RequestComments = leaveRequest.RequestComments
                };

                var apiResponse = await _client.LeaveRequestsPOSTAsync(createLeaveRequest);
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

        public Task DeleteLeaveRequest(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<LeaveRequestVM> GetLeaveRequest(int id)
        {
            var leaveRequest = await _client.LeaveRequestsGETAsync(id);
            return new LeaveRequestVM()
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate.UtcDateTime,
                EndDate = leaveRequest.EndDate.UtcDateTime,
            };
        }
    }
}
