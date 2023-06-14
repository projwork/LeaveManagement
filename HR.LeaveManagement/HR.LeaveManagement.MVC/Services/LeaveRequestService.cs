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
            AddBearerToken();
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
                AddBearerToken();
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
            AddBearerToken();
            var leaveRequest = await _client.LeaveRequestsGETAsync(id);
            return new LeaveRequestVM()
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate.DateTime,
                EndDate = leaveRequest.EndDate.DateTime,
                DateRequested = leaveRequest.DateRequested.DateTime,
                DateActioned = leaveRequest.DateActioned?.DateTime,
                Approved = leaveRequest.Approved,
                Cancelled = leaveRequest.Cancelled,
                RequestComments = leaveRequest.RequestComments,
                LeaveTypeId = leaveRequest.LeaveTypeId,
                LeaveType = new LeaveTypeVM()
                {
                    Id = leaveRequest.LeaveType.Id,
                    Name = leaveRequest.LeaveType.Name,
                    DefaultDays = leaveRequest.LeaveType.DefaultDays
                },
                Employee = new EmployeeVM()
                {
                    Id = leaveRequest.Employee.Id,
                    Email = leaveRequest.Employee.Email,
                    Firstname = leaveRequest.Employee.Firstname,
                    Lastname = leaveRequest.Employee.Lastname
                }
            };
        }

        public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
        {
            AddBearerToken();
            var leaveRequests = await _client.LeaveRequestsAllAsync(isLoggedInUser: false);

            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequests.Count(),
                ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
                PendingRequests = leaveRequests.Count(q => q.Approved == null),
                RejectedRequests = leaveRequests.Count(q => q.Approved == false),
                LeaveRequests = new List<LeaveRequestVM>()
            };

            foreach (var leaveRequestListDto in leaveRequests)
            {
                var leaveRequest = new LeaveRequestVM()
                {
                    Id = leaveRequestListDto.Id,
                    DateRequested = leaveRequestListDto.DateRequested.DateTime,
                    Approved = leaveRequestListDto.Approved,
                    StartDate = leaveRequestListDto.StartDate.DateTime,
                    EndDate = leaveRequestListDto.EndDate.DateTime,
                    LeaveType = new LeaveTypeVM()
                    {
                        Id = leaveRequestListDto.LeaveType.Id,
                        Name = leaveRequestListDto.LeaveType.Name,
                        DefaultDays = leaveRequestListDto.LeaveType.DefaultDays
                    },
                    Employee = new EmployeeVM()
                    {
                        Id = leaveRequestListDto.Employee.Id,
                        Email = leaveRequestListDto.Employee.Email,
                        Firstname = leaveRequestListDto.Employee.Firstname,
                        Lastname = leaveRequestListDto.Employee.Lastname
                    }
                };
                model.LeaveRequests.Add(leaveRequest);
            }
            return model;
        }

        public async Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests()
        {
            AddBearerToken();
            var leaveRequests = await _client.LeaveRequestsAllAsync(isLoggedInUser: true);
            var allocations = await _client.LeaveAllocationsAllAsync(isLoggedInUser: true);
            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = new List<LeaveAllocationVM>(),
                LeaveRequests = new List<LeaveRequestVM>()
            };

            foreach (var leaveAllocationDto in allocations)
            {
                var leaveAllocation = new LeaveAllocationVM()
                {
                    Id = leaveAllocationDto.Id,
                    LeaveTypeId = leaveAllocationDto.LeaveTypeId,
                    LeaveType = new LeaveTypeVM()
                    {
                        Id = leaveAllocationDto.LeaveType.Id,
                        Name = leaveAllocationDto.LeaveType.Name,
                        DefaultDays = leaveAllocationDto.LeaveType.DefaultDays,
                    },
                    NumberOfDays = leaveAllocationDto.NumberOfDays,
                    Period = leaveAllocationDto.Period
                };
                model.LeaveAllocations.Add(leaveAllocation);
            }

            foreach (var leaveRequestListDto in leaveRequests)
            {
                var leaveRequestVm = new LeaveRequestVM()
                {
                    Id = leaveRequestListDto.Id,
                    DateRequested = leaveRequestListDto.DateRequested.DateTime,
                    Approved = leaveRequestListDto.Approved,
                    StartDate = leaveRequestListDto.StartDate.DateTime,
                    EndDate = leaveRequestListDto.EndDate.DateTime,
                    LeaveType = new LeaveTypeVM()
                    {
                        Id = leaveRequestListDto.LeaveType.Id,
                        Name = leaveRequestListDto.LeaveType.Name,
                        DefaultDays = leaveRequestListDto.LeaveType.DefaultDays
                    },
                    Employee = new EmployeeVM()
                    {
                        Id = leaveRequestListDto.Employee.Id,
                        Email = leaveRequestListDto.Employee.Email,
                        Firstname = leaveRequestListDto.Employee.Firstname,
                        Lastname = leaveRequestListDto.Employee.Lastname
                    }
                };
                model.LeaveRequests.Add(leaveRequestVm);
            }

            return model;
        }
    }
}
